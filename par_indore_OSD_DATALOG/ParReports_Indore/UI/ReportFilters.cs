using Nexion.Shared.Core;
using ParReports_Indore.Logic;
using ParReports_Indore.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ParReports_Indore
{

    public partial class ReportFilters : UserControl
    {
        private System.Windows.Forms.Timer _timer;
        private BackgroundWorker backgroundWorker;
        private Action<ReportContext, CancellationToken> delegateGenerateReport;
        private DataClient _dataClient = new DataClient();
        private DateTime _startTime = DateTime.MaxValue;

        public event Action<ReportContext, CancellationToken> ExportToExcel;
        public event Action<ReportContext, CancellationToken> PrintData;
        

        private ReportContext _currentReportContext = new ReportContext();

        public ReportFilters(Action<ReportContext, CancellationToken> generateReport)
        {
            InitializeComponent();
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
            LoadDropdowns();
            delegateGenerateReport = generateReport;
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.btnShow, "Show Report");
            ToolTip1 = new System.Windows.Forms.ToolTip();
            //ToolTip1.SetToolTip(this.btnPDF, "Export to PDF");
            ToolTip1 = new System.Windows.Forms.ToolTip();
        }

        private void StartTimer()
        {
            //_startTime = DateTime.Now;
            //ShowDuration();
            _timer.Enabled = true;
        }
        private void ShowDuration()
        {
            //lblProcessingTime.Text = "Duration: " + (DateTime.Now - _startTime).TotalSeconds.ToString() + " Seconds";
        }
        private void StopTimer()
        {
            _timer.Enabled = false;
            ShowDuration();
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopTimer();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //todo: send cancellation token
            delegateGenerateReport.Invoke(_currentReportContext, CancellationToken.None);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            ShowDuration();
        }

        private void LoadDropdowns()
        {
           
            cmbReportTypes.Items.Clear();
            cmbReportTypes.Items.Add("--Select--");
            cmbReportTypes.Items.AddRange(typeof(ReportTypes).GetAllConstantValues<string>().ToArray());
            cmbReportTypes.SelectedIndex = 0;

            cmbLoad.Items.Clear();
            cmbLoad.Items.Add("--Select--");
            cmbLoad.Items.AddRange(typeof(Timeframe).GetAllConstantValues<string>().ToArray());
            cmbLoad.SelectedIndex = 0;

        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            SetValues();
            //Validate Values
            if (!ValidateParams())
            {
                return;
            }

            StartTimer();
            backgroundWorker.RunWorkerAsync();
        }

        private bool ValidateParams()
        {
            if (_currentReportContext.Criteria.ReportType == "--Select--")
            {
                return false;
            }

            if (_currentReportContext.Criteria.ReportType == "--Select--")
            {
                return false;
            }

            return true;
        }

        private void SetValues()
        {
            ReportContext rp = new ReportContext();
            rp.Criteria.Action = "Show";
            rp.Criteria.ReportDate = dtStart.Value;
            rp.Criteria.StartDateTime = dtStart.Value;
            rp.Criteria.EndDateTime = dtEnd.Value;
            rp.Criteria.ReportType = cmbReportTypes.Text;
            //rp.Criteria.Shift = cmbShift.Text;
            rp.Criteria.Timeframe = cmbLoad.Text;

            //var d = cmbLoad.SelectedItem as HotPressDBStoredProcedures.GetLoadNumberResult;
            //if (rp.Criteria.ReportType == ReportTypes.LoadWiseHWGLog)
            //{
            //    rp.Criteria.LoadInfo = new LoadInfo
            //    {
            //        LoadNumber = d.LoadNumber.Value,
            //        StartDateTime = d.StartTime.Value,
            //        EndDateTime = d.EndTime.Value
            //    };
            //}

            _currentReportContext = rp;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ExportToExcel(_currentReportContext, CancellationToken.None);
        }

        private void dtDate_ValueChanged(object sender, EventArgs e)
        {
            //if (cmbReportTypes.Text == ReportTypes.LoadWiseHWGLog) PopulateLoadNumber();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btnShow_Click(sender, e);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintData(_currentReportContext, CancellationToken.None);
        }

        private void cmbReportTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbReportTypes.Text == ReportTypes.LoadWiseHWGLog)
            //{
            //    PopulateLoadNumber();
            //    EnableDisable(true);
            //}
            //else
            //{
            //    EnableDisable(false);
            //}
        }

        private void PopulateLoadNumber()
        {
            //List<HotPressDBStoredProcedures.GetLoadNumberResult> data = _dataClient.GetLoadNumber(dtStart.Value, dtEnd.Value);

            //if (data != null && data.Count > 0)
            //{

            //    cmbLoad.DataSource = data;
            //    cmbLoad.DisplayMember = "LoadNumber";
            //    cmbLoad.SelectedIndex = 0;                
            //}
            //else
            //    cmbLoad.DataSource = null;
        }

        private void EnableDisable(bool value)
        {
            //cmbShift.Enabled = value;
            cmbLoad.Enabled = value;
        }

        private void dtEnd_ValueChanged(object sender, EventArgs e)
        {
            //if (cmbReportTypes.Text == ReportTypes.LoadWiseHWGLog) PopulateLoadNumber();
        }

        //private void cmbShift_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(cmbShift.SelectedItem.ToString()))
        //    {
        //        var shifts = _dataClient.GetShifts();
        //        if (shifts.Count > 0)
        //        {
        //            var row = shifts.FirstOrDefault(a => a.ShiftName == cmbShift.Text);
        //            if (row != null)
        //            {
        //                var shift = row.GetShiftTimings();
        //                dtStart.Value = shift.StartDateTime;
        //                dtEnd.Value = shift.EndDateTime;
        //            }
        //        }
        //    }
        //}

        private void dtStart_Leave(object sender, EventArgs e)
        {
          //  if (cmbReportTypes.Text == ReportTypes.LoadWiseHWGLog) PopulateLoadNumber();
        }

        private void dtEnd_Leave(object sender, EventArgs e)
        {
          //  if (cmbReportTypes.Text == ReportTypes.LoadWiseHWGLog) PopulateLoadNumber();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel(_currentReportContext, CancellationToken.None);
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            ExportToExcel(_currentReportContext, CancellationToken.None);
        }
    }
}
