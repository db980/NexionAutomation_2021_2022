using Nexion.Shared.UI;
using ParReports_Indore.Logic;
using ParReports_Indore.Model;
using ParReports_Indore.UI;

//
//using NFLReports.Model;
//using NFLRepots.Model;
using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace ParReports_Indore
{
    public partial class Form1 : Form
    {
        private string AppResourcesPath = AppDomain.CurrentDomain.BaseDirectory.ToString();
        private ReportFilters reportFilters;
        private DataClient _dataclient;

        //ReportContext _lastReportParameters;
        private ReportDataGrid grid;

        //IEnumerable<WagonData> wagonDataList;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private ShowProgress progress = new ShowProgress();

        public string ReportType { get; private set; }

        public Form1()
        {
            InitializeComponent();
            InitializeMenu();
#if DEBUG
            TopMost = false;
            this.Text = "Par Reports Data Report - Debug";
#else
			TopMost = true;
			this.Text = "Par Reports Data Report - Release";
#endif

            _dataclient = new DataClient();
            //add reportfilters
            reportFilters = new ReportFilters(ProcessReport);
            reportFilters.ExportToExcel += ReportFilters_ExportToExcel;
            reportFilters.PrintData += ReportFilters_PrintData;
            reportFilters.Dock = DockStyle.Fill;
            splitContainer2.Panel1.Controls.Add(reportFilters);
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

        private void InitializeMenu()
        {
            if (Util.AppProfile.MenuList.Contains(MenuList.AuditTrail) || Util.AppProfile.MenuList.Contains(MenuList.ALL))
            {
                auditTrailToolStripMenuItem.Visible = true;
            }
            else
            {
                auditTrailToolStripMenuItem.Visible = false;
            }

            if (Util.AppProfile.MenuList.Contains(MenuList.DataReport) || Util.AppProfile.MenuList.Contains(MenuList.ALL))
            {
                tagDataToolStripMenuItem1.Visible = true;
            }
            else
            {
                tagDataToolStripMenuItem1.Visible = false;
            }
        }


        private void ReportFilters_PrintData(ReportContext obj, CancellationToken cancellation)
        {
            PrintAndExport(obj, true, cancellation);
        }

        private void ReportFilters_ExportToExcel(ReportContext reportParameter, CancellationToken cancellation)
        {
            PrintAndExport(reportParameter, false, cancellation);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TopMost)
            {
                TopMost = false;
            }

            tsTime.Text = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

#if DEBUG
#else
			timer.Start();

#endif

        }


        private async void PrintAndExport(ReportContext reportParameter, bool print, CancellationToken cancellation)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action<ReportContext, bool, CancellationToken>(PrintAndExport), reportParameter, print, cancellation);
                return;
            }

            if (print)
            {
                grid.Print();
            }
            else
            {
                try
                {


                    NexionPdfDocument pdfDocument = new NexionPdfDocument
                    {
                        FileName = $"{ConfigurationManager.AppSettings["ImportPath"]}DataReport{DateTime.Now.ToString("ddMMMyyyyHHmmss")}.pdf",
                        Header = $"{reportParameter.Criteria.ReportType} REPORT ",
                        LogoImageFileName = $"{ConfigurationManager.AppSettings["ImportPath"]}\\PARLOGO.jpg",
                        DataRowStartIndex = -1,
                        DataRowHeaderIndex = 0,
                        DataRowColIndex = 0,
                        //NbrOfDataRowsLandscape = 40,
                       // NbrOfDataRowsPortrait = 60,
                        SubHeader = $"({ reportParameter.Criteria.StartDateTime.ToString("dd-MMM-yyyy HH:mm")}-{ reportParameter.Criteria.EndDateTime.ToString("dd-MMM-yyyy HH:mm")})",
                       
                        Footer = "Printed By\t\t\t\t\t\t \tChecked By\t\t\t\t\t\t\t Verified By",
                        PageWidthOption = PdfPageWidthOption.ScaleAsWorksheet,


                    };
                    PdfSettings pdfSettings = new PdfSettings
                    {
                        FontSize = 8,
                        NbrOfColumnsToBeIncluded = grid.dataGridView1.CurrentWorksheet.PrintableRange.Cols,
                        NbrOfRowsInPage = grid.dataGridView1.CurrentWorksheet.PrintSettings.Landscape ? 40 : 60,
                        Orientation = grid.dataGridView1.CurrentWorksheet.PrintSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait,
                        Scale = Math.Round(grid.dataGridView1.CurrentWorksheet.PrintSettings.PageScaling * 100, 0),

                    };
                    frmPDFSettings frmPDFSettings = new frmPDFSettings(pdfSettings);

                    if (frmPDFSettings.ShowDialog() == DialogResult.OK)
                    {
                        pdfDocument.NbrOfDataRows = pdfSettings.NbrOfRowsInPage;
                        pdfDocument.PageOrientation = pdfSettings.Orientation == PageOrientation.Landscape ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;
                        pdfDocument.PageWidthOption = pdfSettings.WidthOption == PageWidthOption.FitToWidth ? PdfPageWidthOption.FitToWidth : PdfPageWidthOption.ScaleAsWorksheet;
                        pdfDocument.NbrOfCOlumns = pdfSettings.NbrOfColumnsToBeIncluded;
                        //pdfDocument.Scale = pdfSettings.Scale / 100;
                        showProgress("Please wait, file is generating..");
                        progress.Show();
                        await grid.ExportAsPdf(pdfDocument, showProgress, cancellation);
                    }


                }
                finally
                {
                    Thread.Sleep(1000);
                    progress.Hide();
                }
            }
        }

        private async void ProcessReport(ReportContext rp, CancellationToken cancellation)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action<ReportContext, CancellationToken>(ProcessReport), rp, cancellation);

                return;
            }
            try
            {
                //IEnumerable<ReportViewModel> data = null;
                groupBox1.Controls.Clear();
                try
                {
                    progress.Show();
                    progress.StartPosition = FormStartPosition.CenterScreen;
                    progress.TopLevel = true;
                    Int32 Timeframevalue = 1;
                    if (rp.Criteria.Timeframe == Timeframe.OneMin)
                    {
                        Timeframevalue = 1;
                    }
                   else if (rp.Criteria.Timeframe == Timeframe.FiveMins )
                    {
                        Timeframevalue = 5;
                    }
                    else if (rp.Criteria.Timeframe == Timeframe.TenMins )
                    {
                        Timeframevalue = 10;
                    }
                    else if (rp.Criteria.Timeframe == Timeframe.FifteenMins )
                    {
                        Timeframevalue = 15;
                    }
                    else if (rp.Criteria.Timeframe == Timeframe.ThirtyMins )
                    {
                        Timeframevalue = 30;
                    }
                    else if (rp.Criteria.Timeframe == Timeframe.OneHour )
                    {
                        Timeframevalue = 60;
                    }


                    System.Collections.Generic.Dictionary<DateTime, System.Collections.Generic.Dictionary<string, string>> data = await _dataclient.asyncGetLoadWiseHWGLogViewData( rp.Criteria.StartDateTime, rp.Criteria.EndDateTime, rp.Criteria.ReportType, Timeframevalue, showProgress, cancellation);
                    if (data.Any())
                    {
                        grid = new ReportDataGrid();
                        groupBox1.Controls.Add(grid);
                        grid.Dock = DockStyle.Fill;
                        await grid.AddAHULogByLoad(rp, data, cancellation, showProgress);
                    }
                    else
                    {
                        CoreMessageBox.Show("Data not available. Please change search parameter.");
                    }


                }
                catch (Exception ex)
                {
                    CoreMessageBox.Error(ex);
                }
                finally
                {
                    Thread.Sleep(1000);
                    progress.Hide();
                }

            }
            catch (Exception ex)
            {
                CoreMessageBox.Error(ex);
            }
        }

        private void showProgress(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(showProgress), message);
                return;
            }

            progress.SetMessage(message);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About ab = new About();
            ab.Show(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void preferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tagDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tagDataToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TagData td = new TagData();
            td.Show(this);
        }

        private void auditTrailToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void preferenceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectColumns selectColumns = new SelectColumns();
            selectColumns.ShowDialog(this);
        }
    }



}
