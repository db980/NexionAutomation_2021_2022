using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nexion.Shared.Core
{
	public static class TypeExtensions
	{
		public static bool IsNumericType(this Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}
		/// <summary>
		/// return previous date
		/// </summary>
		/// <param name="date"></param>
		/// <param name="offSet">Offset days</param>
		/// <returns></returns>
		public static DateTime PreviousDate(this DateTime date, int offSet = -1)
		{
			return date.AddDays(offSet);
		}
		/// <summary>
		/// return previous date
		/// </summary>
		/// <param name="date"></param>
		/// <param name="offSet">Offset days</param>
		/// <returns></returns>
		public static DateTime NextDay(this DateTime date, int offSet = 1)
		{
			return date.AddDays(offSet);
		}

		public static List<T> GetAllConstantValues<T>(this Type type)
		{
			return type
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
				.Select(x => (T)x.GetRawConstantValue())
				.ToList();
		}
		/// <summary>
		/// Get list of all fields name in a type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static List<T> GetAllFieldsValues<T>(this Type type)
		{
			return type
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
				.Select(x => (T)x.GetRawConstantValue())
				.ToList();
		}

		/// <summary>
		/// Get value of specific property dynamically
		/// </summary>
		public static Object GetPropValue(this Object obj, String name)
		{
			foreach (String part in name.Split('.'))
			{
				if (obj == null) { return null; }

				Type type = obj.GetType();
				PropertyInfo info = type.GetProperty(part);
				if (info == null) { return null; }

				obj = info.GetValue(obj, null);
			}
			return obj;
		}
		/// <summary>
		/// Get value of specific property dynamically
		/// </summary>
		public static T GetPropValue<T>(this Object obj, String name)
		{
			Object retval = GetPropValue(obj, name);
			if (retval == null) { return default(T); }

			// throws InvalidCastException if types are incompatible
			return (T)retval;
		}


	}

	public class PropertyCopier<TParent, TChild> where TParent : class
											where TChild : class
	{
		public static void CopyFieldValue(TParent parent, TChild child, string propertyName)
		{
			var parentProperty = parent.GetType().GetProperties().FirstOrDefault(a => a.Name == propertyName);
			var childProperty = child.GetType().GetProperties().FirstOrDefault(a => a.Name == propertyName);
			if (parentProperty != null && childProperty != null)
				childProperty.SetValue(child, parentProperty.GetValue(parent));
			else
				throw new Exception($"Either of the objects don't have field name {propertyName}");
		}

		public static void Copy(TParent parent, TChild child)
		{
			var parentProperties = parent.GetType().GetProperties();
			var childProperties = child.GetType().GetProperties();

			foreach (var parentProperty in parentProperties)
			{
				foreach (var childProperty in childProperties)
				{
					if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
					{
						childProperty.SetValue(child, parentProperty.GetValue(parent));
						break;
					}
				}
			}
		}
	}
}
