using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nexion.Shared.Core
{
	public static class EnumerationExtensions
	{
		/// <summary>
		/// Inserts method that touches every item in sequence. Method is executed once per item in sequence when sequence is enumerated.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="handler"></param>
		/// <param name="itemPredicate">Optional item predicate that determines which items are touched.</param>
		/// <returns></returns>
		public static IEnumerable<T> Touch<T>(this IEnumerable<T> enumerable, Action<T> handler, Func<T, bool> itemPredicate = null)
		{
			foreach (T item in enumerable)
			{
				if (itemPredicate == null || itemPredicate(item))
					handler(item);
				yield return item;
			}
		}

		/// <summary>
		/// Inserts sample method in sequence. Method is executed for item with index 'start' and then for every X item as defined by 'every' parameter. If last item is not 
		/// hit by sampler, and extra call can be forced by setting the sampleLast parameter to true. Sampler is lazy and only called when returned enumerable is enumerated.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable">Input enumerable.</param>
		/// <param name="handler">Method to call for every sampled item. Is passed (item, zero-based index).</param>
		/// <param name="start">Zero-based index of first item to sample. Is typically zero.</param>
		/// <param name="every">Sample frequency, 1 means every single item, 10 means every 10th item, etc.</param>
		/// <param name="sampleLast">True to force sample of last item if not hit by frequency.</param>
		/// <returns>Input enumerable.</returns>
		public static IEnumerable<T> Sample<T>(this IEnumerable<T> enumerable, Action<T, int> handler, int start, int every, bool sampleLast)
		{
			if (start < 0)
				throw new ArgumentOutOfRangeException("start cannot be negative.");
			if (every < 1)
				throw new ArgumentOutOfRangeException("every must be at least 1.");
			if (handler == null)
				throw new ArgumentNullException("handler");

			int i = -1;
			int nexti = start;
			T current = default(T);

			// Start up enumeration of input sequence
			using (IEnumerator<T> e = enumerable.GetEnumerator())
				while (e.MoveNext())
				{
					if (++i == nexti)
					{
						handler(e.Current, i);
						nexti += every;
					}

					yield return current = e.Current;
				}

			// Sample last if requested and not hit by regular sampling
			if (sampleLast && i >= 0 && i != (nexti - every))
				handler(current, i);
		}

		/// <summary>
		/// Inserts sampler that makes a single call to handler method when enumeration has ended. Handler is passed last item and zero-based index of last item. Handler is not
		/// called if enumerable is empty. Handler is called after enumeration of input enumerable has ended.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="handler"></param>
		/// <returns>Input enumerable.</returns>
		public static IEnumerable<T> SampleLast<T>(this IEnumerable<T> enumerable, Action<T, int> handler)
		{
			return enumerable.Sample(handler, int.MaxValue, 1, true);
		}

		/// <summary>
		/// Inserts sampler that makes a single call to handler right before first item is returned. Handler is not called if enumerable is empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IEnumerable<T> SampleFirst<T>(this IEnumerable<T> enumerable, Action<T> handler)
		{
			return enumerable.Sample((t, i) => handler(t), 0, int.MaxValue, false);
		}

		/// <summary>
		/// Ends enumeration when token is set. Token is checked before each item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable">Input enumerable.</param>
		/// <param name="token">Token to monitor.</param>
		/// <returns></returns>
		public static IEnumerable<T> TruncateIfCancelled<T>(this IEnumerable<T> enumerable, CancellationToken token)
		{
			foreach (T item in enumerable)
			{
				if (token.IsCancellationRequested)
					yield break;

				yield return item;
			}
		}

		/// <summary>
		/// Throws OperationCancelledException if token is set. Token is checked before each item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable">Input enumerable.</param>
		/// <param name="token">Token to monitor.</param>
		/// <returns></returns>
		public static IEnumerable<T> ThrowIfCancelled<T>(this IEnumerable<T> enumerable, CancellationToken token)
		{
			foreach (T item in enumerable)
			{
				token.ThrowIfCancellationRequested();
				yield return item;
			}
		}

		/// <summary>
		/// Inserts sampler that receives a single call efter last item in input.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static IEnumerable<T> WhenEnded<T>(this IEnumerable<T> enumerable, Action handler)
		{
			foreach (T t in enumerable)
				yield return t;

			handler();
		}
		/// <summary>
		/// Inserts a greedy buffer filled by worker task such that it always is as full as possible. NOTE THAT impact is that source enumerable will be called
		/// on async thread and not on thread that calls Buffer().
		/// </summary>
		/// <remarks>Use this method to speed up any complex query with at least two costly operations, e.g. one that fetches data from source and
		/// another that transforms data.</remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="bufferSize">Max number of items to hold in buffer at any time.</param>
		/// <param name="cancellationToken">Token to be used and set if returned enumerable isn't fully enumerated.</param>
		/// <returns></returns>
		public static IEnumerable<T> Buffer<T>(this IEnumerable<T> enumerable, int bufferSize, CancellationToken cancellationToken)
		{
			if (bufferSize < 0)
				throw new ArgumentException("bufferSize must be at least 0.");
			if (bufferSize == 0)
			{
				foreach (T item in enumerable)
					yield return item;
				yield break;
			}

			// Create async queue
			using (var queue = new BlockingCollection<T>(bufferSize))
			{
				// Define producer taking from source and putting into queue
				Action producer = () =>
				{
					try
					{
						foreach (T item in enumerable)
							queue.Add(item, cancellationToken);
					}
					catch (OperationCanceledException ex)
					{
						if (ex.CancellationToken != cancellationToken)
							throw;
					}
					finally
					{
						queue.CompleteAdding();
					}
				};

				// Start producer into queue
				using (Task worker = Task.Factory.StartNew(producer, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default))
				{
					// Return items from queue as soon as they are available
					foreach (T item in queue.GetConsumingEnumerable())
						yield return item;

					// Worker will be done here but we need this to get exception rethrown if any.
					try
					{
						worker.Wait();
					}
					catch (AggregateException ex)
					{
						// Special case when canceled to throw with original cancellation token
						if (ex.InnerException is OperationCanceledException)
							throw new OperationCanceledException(ex.InnerException.Message, ex.InnerException, ((OperationCanceledException)ex.InnerException).CancellationToken);

						// If we just throw inner exception then we'll loose the real stack trace and instead get trace from here. So we try
						// to construct a new exception of same type and throw that instead.
						//throw ex.InnerException.WrapInOuter("Async enumeration of {0} in Buffer() caused exception: {1}", enumerable.GetType().GetPrettyName(), ex.InnerException.Message);
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Do action for each sequence in collection
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="act"></param>
		/// <returns></returns>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> array, Action<T> act)
		{
			foreach (var i in array)
				act(i);
			return array;
		}
		/// <summary>
		/// Do action for each sequence in collection
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="act"></param>
		/// <returns></returns>
		public static IEnumerable<T> ForEach<T>(this IEnumerable arr, Action<T> act)
		{
			return arr.Cast<T>().ForEach<T>(act);
		}
		/// <summary>
		/// Do action for each sequence in collection
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="act"></param>
		/// <returns></returns>
		public static IEnumerable<RT> ForEach<T, RT>(this IEnumerable<T> array, Func<T, RT> func)
		{
			var list = new List<RT>();
			foreach (var i in array)
			{
				var obj = func(i);
				if (obj != null)
					list.Add(obj);
			}
			return list;
		}

		/// <summary>
		/// Returns an empty enumerable if the input enumerable is null, otherwise the input enumerable is returned.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
		{
			return enumerable ?? Enumerable.Empty<T>();
		}

		/// <summary>
		/// Returns null if collection is empty, otherwise collection is returned unmodified.
		/// </summary>
		/// <typeparam name="TCollection"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static TCollection NullIfEmpty<TCollection>(this TCollection collection) where TCollection : class, ICollection
		{
			return collection.Count == 0 ? null : collection;
		}

		/// <summary>
		/// Enumerates given sequence once.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		public static void Enumerate<T>(this IEnumerable<T> enumerable)
		{
			using (IEnumerator<T> e = enumerable.GetEnumerator())
			{
				while (e.MoveNext()) ;
			}
		}

		/// <summary>
		/// Splits a sequence into pages of given page size. Enumerates the input lazy on demand. Is the opposite of SelectMany.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable">Input sequence.</param>
		/// <param name="pageSize">Max no of items on each page. Last page can have less items.</param>
		/// <returns>Pages as arrays.</returns>
		public static IEnumerable<IList<T>> SelectPages<T>(this IEnumerable<T> enumerable, int pageSize)
		{
			if (pageSize < 1)
				throw new ArgumentOutOfRangeException("pageSize must be at least 1.");

			using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
			{
				while (true)
				{
					List<T> page = new List<T>(pageSize);
					while (page.Count < pageSize && enumerator.MoveNext())
						page.Add(enumerator.Current);

					if (page.Count > 0)
					{
						// Avoid returning a lot of whitespace on last page
						page.TrimExcess();

						yield return page;
					}

					if (page.Count < pageSize)
						break;
				}
			}
		}

		/// <summary>
		/// Pagifies an input stream using variable page sizes. Given predicate is called once before an item is added to current page.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input"></param>
		/// <param name="addOneMoreToPage">Predicate that returns true to add one more to given current page or false to end current page and add item to new page.</param>
		/// <returns></returns>
		public static IEnumerable<IList<T>> SelectPages<T>(this IEnumerable<T> input, Func<IList<T>, T, bool> addOneMoreToPage)
		{
			List<T> page = new List<T>();
			foreach (T item in input)
			{
				if (addOneMoreToPage(page, item))
				{
					page.Add(item);
				}
				else
				{
					yield return page;
					page = new List<T>();
					page.Add(item);
				}
			}

			if (page.Count > 0)
				yield return page;
		}

		/// <summary>
		/// Creates new SortedList over given sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="keySelector"></param>
		/// <param name="valueSelector"></param>
		/// <param name="keyComparer"></param>
		/// <returns></returns>
		public static SortedList<TKey, TValue> ToSortedList<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector, Func<T, TValue> valueSelector, IComparer<TKey> keyComparer = null)
		{
			SortedList<TKey, TValue> list = new SortedList<TKey, TValue>(keyComparer ?? Comparer<TKey>.Default);

			foreach (T item in enumerable)
				list.Add(keySelector(item), valueSelector(item));

			return list;
		}

		/// <summary>
		/// Produces a pivot of the input sequence of sequences <paramref name="pivotSource"/>
		/// of object <typeparamref name="TSource"/>.
		/// </summary>
		/// <typeparam name="TSource">
		/// The type of object which the input sequences <paramref name="pivotSource"/>.
		/// </typeparam>
		/// <param name="pivotSource">
		/// The <paramref name="pivotSource"/> is the sequence which is transformed.
		/// The pivot transform takes each column of the input sequences
		/// <paramref name="pivotSource"/> and make a row for that column.
		/// The type of the output objects is the <typeparamref name="TSource"/> type.
		/// </param>
		/// <returns>
		/// A series of rows, one for each column of the input sequences
		/// <paramref name="pivotSource"/>.
		/// The contents of the row is all of the values from the column.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// The <seealso cref="System.ArgumentNullException"/> ArgumentNullException is
		/// thrown in two cases.
		/// <list type="number">
		/// <item>
		/// This exception is thrown if the input <paramref name="pivotSource"/>
		/// IEnumrable is null.
		/// </item>
		/// <item>
		/// This exception is thrown if first inner IEnumerable in the input
		/// <paramref name="pivotSource"/> is null.
		/// </item>
		/// </list>
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// This exception is throw in one of two cases.
		/// <list type="number">
		/// <item>
		/// If the outer IEnumerable <paramref name="pivotSource"/>
		/// contains no elements (is an empty sequence).
		/// </item>
		/// <item>
		/// If the first element in the outer sequence <paramref name="pivotSource"/>
		/// contains no elements(is an empty sequence).
		/// </item>
		/// </list>
		/// </exception>
		/// <remarks>
		/// This extension method takes the input sequence <paramref name="pivotSource"/> and
		/// converts the columns in the input to rows in the output.
		/// </remarks>
		public static IEnumerable<IEnumerable<TSource>> Pivot<TSource>(
		this IEnumerable<IEnumerable<TSource>> pivotSource)
		{
			// Validation of the input arguments, and structure of those arguments.
			if (Object.ReferenceEquals(pivotSource, null))
				throw new ArgumentNullException("pivotSource",
				"The source IEnumerable cannot be null.");
			if (pivotSource.Count() == 0)
				throw new ArgumentOutOfRangeException("pivotSource",
				"The outer IEnumerable cannot be an empty sequence");
			if (pivotSource.Any(A => Object.Equals(A, null)))
				throw new ArgumentOutOfRangeException("pivotSource",
				"None of any inner IEnumerables in pivotSource can be null");
			if (pivotSource.All(A => A.Count() == 0))
				throw new ArgumentOutOfRangeException("pivotSource",
				"All of the input inner sequences have no columns of data.");
			// Get the row lengths to check if the data needs squaring out
			int maxRowLen = pivotSource.Select(a => a.Count()).Max();
			int minRowLen = pivotSource.Select(a => a.Count()).Min();
			// Set up the input to the Pivot
			IEnumerable<IEnumerable<TSource>> squared = pivotSource;
			// If a square out is required
			if (maxRowLen != minRowLen)
				// Fill the tail of short rows with the default value for the type
				squared = pivotSource.Select(row =>
											row.Concat(
											Enumerable.Repeat(default(TSource), maxRowLen - row.Count())));
			// Perform the Pivot operation on squared out data
			var result = Enumerable.Range(0, maxRowLen)
							.Select((ColumnNumber) =>
							{
								return squared.SelectMany
								 (row => row
								 .Where((Column, ColumnPosition) =>
								 ColumnPosition == ColumnNumber)
								 );
										});
			return result;
		}

        public static Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>> Pivot<TSource, TFirstKey, TSecondKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TFirstKey> firstKeySelector, Func<TSource, TSecondKey> secondKeySelector, Func<IEnumerable<TSource>, TValue> aggregate)
        {
            var retVal = new Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>>();

            var l = source.ToLookup(firstKeySelector);
            foreach (var item in l)
            {
                var dict = new Dictionary<TSecondKey, TValue>();
                retVal.Add(item.Key, dict);
                var subdict = item.ToLookup(secondKeySelector);
                foreach (var subitem in subdict)
                {
                    dict.Add(subitem.Key, aggregate(subitem));
                }
            }

            return retVal;
        }

        public static IEnumerable<T> ConvertToLargerTimeFrame<T>(this IEnumerable<T> bars_to_convert, TimeSpan time) where T : IDataTag, ICloneable
        {
            var bars_converted = new List<T>();
            long current_tick_interval = -1;
            DateTime boundary_adjusted_time = default(DateTime);

            if (bars_to_convert.Count() == 0)
            {
                return bars_converted;
            }

            T currentReportViewModel = Activator.CreateInstance<T>();

            foreach (var bar in bars_to_convert)
            {
                var this_tick_interval = bar.DateAndTime.Ticks / time.Ticks;

                if (this_tick_interval != current_tick_interval)
                {
                    if (current_tick_interval != -1)
                    {
                        T reportViewModel = Activator.CreateInstance<T>();

                        reportViewModel = (T)currentReportViewModel.Clone();
                        reportViewModel.DateAndTime = boundary_adjusted_time;

                        bars_converted.Add(reportViewModel);
                    }
                    current_tick_interval = this_tick_interval;
                    boundary_adjusted_time = new DateTime(current_tick_interval * time.Ticks);
                    currentReportViewModel = bar;
                }
                else
                {
                    currentReportViewModel = bar;
                }
            }
            // Add the final bar
            currentReportViewModel.DateAndTime = boundary_adjusted_time;
            bars_converted.Add(currentReportViewModel);
            return bars_converted;
        }
    }

	public class DisposableEnumerable<T> : IEnumerable<T>, IDisposable
	{
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	public static class CollectionExtensions
	{

		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
		{
			var c = new ObservableCollection<T>();
			foreach (var e in coll)
				c.Add(e);
			return c;
		}
	}

}
