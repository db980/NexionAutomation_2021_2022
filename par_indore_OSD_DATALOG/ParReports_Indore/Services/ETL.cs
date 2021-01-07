//using LinqToDB;
//using Rhino.Etl.Core;
//using Rhino.Etl.Core.Operations;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ParReports_Indore.Services
//{
//    public class ETLBatchService
//    {



//        public int Interval { get; set; }
//        public Task Start(Action<string> progressHandler, CancellationToken cancellationToken)
//        {
//            return Task.Run(() =>
//            {
//                // Were we already canceled?
//                cancellationToken.ThrowIfCancellationRequested();

//                while (!cancellationToken.IsCancellationRequested)
//                {

//                    ETLBatchProcess eTLBatchProcess = new ETLBatchProcess(Interval);
//                    progressHandler($"Next Batch Starting");
//                    eTLBatchProcess.Execute();
//                    progressHandler($"Batch ({eTLBatchProcess.CurrentBatch.BatchId}) {eTLBatchProcess.CurrentBatch.StartTime.Value.ToString("dd-MMM-yyyy HH:mm:ss")} - {eTLBatchProcess.CurrentBatch.EndTime.Value.ToString("dd-MMM-yyyy HH:mm:ss")} completed.");

//                    if (DateTime.Now.Subtract(eTLBatchProcess.CurrentBatch.EndTime.Value).TotalMinutes <= Interval)
//                    {
//                        Thread.Sleep(TimeSpan.FromMinutes(Interval));
//                    }
//                }

//            }, cancellationToken);

//        }
//    }

//    public class ETLBatchProcess : EtlProcess
//    {
//        public Batch CurrentBatch { get; private set; }

//        public DateTime StartTime { get; private set; }
//        public DateTime EndTime { get; private set; }
//        public int IntervalInMinutes { get; set; }

//        public ETLBatchProcess(int interval)
//        {
//            IntervalInMinutes = interval;
//        }

//        protected override void Initialize()
//        {
//            using (HotPressDB db = new HotPressDB())
//            {
//                if (db.Batches.Any())
//                {
//                    CurrentBatch = db.Batches.OrderByDescending(x => x.BatchId)
//                                              .FirstOrDefault();
//                    CurrentBatch.BatchId += 1;
//                    if (CurrentBatch.State.HasValue && CurrentBatch.State.Value)
//                    {
//                        CurrentBatch.StartTime = CurrentBatch.EndTime.Value;
//                        CurrentBatch.EndTime = CurrentBatch.StartTime.Value.AddMinutes(IntervalInMinutes);
//                    }
//                }
//                else
//                {
//                    CurrentBatch = new Batch { BatchId = 1, StartTime = new DateTime(2019, 3, 1), EndTime = new DateTime(2019, 3, 1).AddMinutes(IntervalInMinutes), BatchStart = DateTime.Now };
//                }

//                Register(new ETLDataReader(CurrentBatch)).Register(new ETLDataWriter(CurrentBatch));

//                db.GetTable<Batch>()
//                             .Insert(() => new Batch
//                             {
//                                 BatchId = CurrentBatch.BatchId,
//                                 StartTime = CurrentBatch.StartTime,
//                                 EndTime = CurrentBatch.EndTime,
//                                 State = false
//                             });
//            }
//        }

//        protected override void PostProcessing()
//        {

//            //update Batch

//            base.PostProcessing();

//            bool containsError = GetAllErrors().Any();
//            //enumerate errors and throw first one
//            //event fired after op completes never fires on error 
//            //so this is only chance I know for generic exception throwing, 
//            //otherwise they get ignored
//            foreach (var error in GetAllErrors())
//            {
//                //throw error;
//            }


//            using (HotPressDB db = new HotPressDB())
//            {
//                var shift = db.Batches
//                       .Where(a => a.BatchId == CurrentBatch.BatchId)
//                       .Set(p => p.State, !containsError)
//                       .Set(p => p.BatchEnd, DateTime.Now)
//                       .Set(p => p.Reason, containsError ? GetAllErrors().ToList()[0].Message : null)
//                       .Update();
//            }
//        }
//    }

//    public class ETLDataReader : AbstractOperation
//    {
//        private Batch _currentBatch;
//        private HotPressDB _db = new HotPressDB();

//        public ETLDataReader(Batch batch)
//        {
//            _currentBatch = batch;
//        }

//        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
//        {
//            //Get last batch information
//            IEnumerable<ReportingDataInput> datarows = _db.GetReportDataInput(_currentBatch.StartTime, _currentBatch.EndTime);

//            foreach (var row in datarows)
//            {
//                yield return Row.FromObject(row);
//            }

//        }
//    }

//    public class ETLDataWriter : AbstractOperation
//    {
//        private readonly Batch _currentBatch;
//        private readonly HotPressDB _db = new HotPressDB();

//        public ETLDataWriter(Batch batch)
//        {
//            _currentBatch = batch;
//        }

//        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
//        {
//            foreach (var row in rows)
//            {
//                var obj = row.ToObject<ReportingDataInput>();
//                _db.GetTable<ReportingData>()
//                        .Insert(() => new ReportingData
//                        {
//                            BatchId = _currentBatch.BatchId,
//                            DateAndTime = obj.DateAndTime,
//                            Date = obj.Date,
//                            PreviousDateAndtime = obj.PreviousDateAndTime,
//                            PreviousVal = obj.PreviousVal,
//                            TagIndex = obj.TagIndex,
//                            Val = obj.Val
//                        });
//                yield return row;
//            }
//        }
//    }
//}
