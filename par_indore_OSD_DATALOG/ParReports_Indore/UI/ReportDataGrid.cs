using ParReports_Indore.Logic;
using ParReports_Indore.Model;
using Nexion.Shared.Core;
using Nexion.Shared.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using unvell.ReoGrid;
//using Nexion.Shared.Core;
//using Nexion.Shared.UI;

namespace ParReports_Indore
{

    public partial class ReportDataGrid : UserControl
    {
      //  private NexionDataGrid dataGridView1;
        internal NexionDataGrid dataGridView1;
        private List<string> _header = new List<string>();

        //delegate void delegateAddToGrid(ReportContext rp, IEnumerable<ReportViewModel> vrList, Action<string> showProgress);
        private ReportContext _reportContext;

        public ReportDataGrid()
        {
            InitializeComponent();
            dataGridView1 = new Nexion.Shared.UI.NexionDataGrid();
            dataGridView1.ContextMenuStrip = this.contextMenuStrip1; ;
            groupBox3.Controls.Add(dataGridView1);
            dataGridView1.Dock = DockStyle.Fill;
        }

        private void LoadExcel(string fileName)
        {
            dataGridView1.LoadFile(fileName);
        }



        public Task AddAHULogByLoad(ReportContext rp, Dictionary<DateTime, Dictionary<string, string>> data, CancellationToken cancellationToken, Action<string> showProgress)
        {
            return Task.Run(() =>
            {
                RenderDataLoadWiseAHULog(rp, data, showProgress);

            }, cancellationToken);
        }

        private void RenderDataLoadWiseAHULog(ReportContext rp, Dictionary<DateTime, Dictionary<string, string>> data, Action<string> showProgress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ReportContext, Dictionary<DateTime, Dictionary<string, string>>, Action<string>>(RenderDataLoadWiseAHULog), rp, data, showProgress);
                return;
            }

            this.SuspendLayout();
            try
            {
                _reportContext = rp;

                //	data = convertToTimeFrame(rp, data);

                showProgress("Rendering data. Please wait..");
                string file = $"{ConfigurationManager.AppSettings["ImportPath"]}\\AHU.xlsx";

                dataGridView1.LoadFile(file);

                //_data = data;

                if (data.Count() == 0)
                {
                    return;
                }

                var sheet = dataGridView1.CurrentWorksheet;
                float margin = 0.5f;
                sheet.PrintSettings.Margins = new unvell.ReoGrid.Print.PageMargins(margin, margin, margin, margin);

                sheet.Rows = data.Count();
                List<TrendMappingDEHLog> headers = new DataReportDB().TrendMappingDEHLogs.Where(x => x.AHUName == rp.Criteria.ReportType).ToList();
                //     var selectedcolumns = Util.AppProfile.AhuColumns.Where(a => a.Selected);
                var headersColumns = headers.Where(a => a.Show).OrderBy(a => a.ColumnIndex).ToList();

                sheet.Columns = headersColumns.Count()+3 ;
                sheet.InsertRows(0, data.Count());
                sheet.SetColumnsWidth(0, 1, 100);
                sheet.SetSettings(WorksheetSettings.Edit_Readonly, true);
                

                sheet.PrintableRange = new RangePosition(0, 0, sheet.RowCount, sheet.ColumnCount);
                sheet.Columns = sheet.ColumnCount;
                if (sheet.Columns > 9)
                {
                    sheet.PrintSettings.Landscape = true;
                    sheet.PrintSettings.PageScaling = 0.7f;
                    sheet.AutoSplitPage();

                }
                else
                {
                    sheet.PrintSettings.PageScaling = 0.7f;
                    sheet.PrintSettings.Landscape = false;
                    sheet.AutoSplitPage();
                }

                sheet.FreezeToCell(0, 0);
                


                //sheet.Cells["D2"].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                //sheet.Cells["D2"].Data = rp.Criteria.StartDateTime.Date.ToString("dd-MMM-yyyy");

                //sheet.Cells["D3"].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                //sheet.Cells["D3"].Data = rp.Criteria.LoadInfo.LoadNumber;

                DataClient dc = new DataClient();
                //List<HotPressDBStoredProcedures.GetHWGLogLoadWiseHeaderResult> headerdata =
                //        dc.GetHWGLogLoadWiseHeader(rp.Criteria.LoadInfo.StartDateTime, rp.Criteria.LoadInfo.EndDateTime);

                //writing header values

                int rowIdx = 0;


                //Creating header 

                int visibleColumnIndex = 3;

                UpdateHeaderText(sheet, 0, 0, "SNO");
                UpdateHeaderText(sheet, 0, 1, "Date");
                UpdateHeaderText(sheet, 0, 2, "Time");
                foreach (var col in headersColumns)
                {
                    col.ColumnIndex = visibleColumnIndex;

                    if (!string.IsNullOrEmpty(col.Logic))
                    {
                        var logictext = col.Logic.Split(';');
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        foreach (var str in logictext)
                        {
                            var splittedstri = str.Split('=');
                            dict.Add(splittedstri[0], splittedstri[1]);
                        }
                        col.ValuesToReplace = dict;
                    }

                    UpdateHeaderText(sheet, rowIdx, visibleColumnIndex, col.Description);
                    visibleColumnIndex++;
                }

                var lastdataSet = new DataClient().GetLastHWGLogDataLoadWiseResult(rp.Criteria.StartDateTime, rp.Criteria.EndDateTime, rp.Criteria.ReportType).ToList();
                //writing data rows
                rowIdx = 1;

                data.ForEach(row =>
                {
                    sheet.Cells[rowIdx, 0].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.General;
                    sheet.Cells[rowIdx, 0].Data = rowIdx - 1;

                    sheet.Cells[rowIdx, 0].Style.FontSize = 11;
                    sheet.Cells[rowIdx, 0].Style.HAlign = ReoGridHorAlign.Center;
                    sheet.Cells[rowIdx, 0].Style.Bold = false;
                    sheet.Cells[rowIdx, 0].Border.All = RangeBorderStyle.BlackSolid;

                    sheet.Cells[rowIdx, 1].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                    sheet.Cells[rowIdx, 1].Data = row.Key.Date.ToString("dd-MMM-yyyy");

                    sheet.Cells[rowIdx, 1].Style.FontSize = 11;
                    sheet.Cells[rowIdx, 1].Style.HAlign = ReoGridHorAlign.Center;
                    sheet.Cells[rowIdx, 1].Style.Bold = false;
                    sheet.Cells[rowIdx, 1].Border.All = RangeBorderStyle.BlackSolid;

                    sheet.Cells[rowIdx, 2].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                    sheet.Cells[rowIdx, 2].Data = row.Key.ToString("HH:mm:ss");

                    sheet.Cells[rowIdx, 2].Style.FontSize = 11;
                    sheet.Cells[rowIdx, 2].Style.HAlign = ReoGridHorAlign.Center;
                    sheet.Cells[rowIdx, 2].Style.Bold = false;
                    sheet.Cells[rowIdx, 2].Border.All = RangeBorderStyle.BlackSolid;
                    // sheet.Cells[rowIdx, 2].ExpandColumnWidth();
                    sheet.AutoFitColumnWidth(1,true);
                    int colIndx = 3;

                    foreach (var prop in row.Value)
                    {
                        if (headersColumns.Any(a => a.PLCTAG.ToLower() == prop.Key.ToLower()))
                        {
                            var rowheader = headersColumns.FirstOrDefault(a => a.PLCTAG.ToLower() == prop.Key.ToLower());
                            if (rowheader != null)
                            {
                                colIndx = Convert.ToInt32(rowheader.ColumnIndex);

                                if (rowheader.ValuesToReplace != null)
                                {
                                    sheet.Cells[rowIdx, colIndx].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                                    sheet.Cells[rowIdx, colIndx].Data = string.IsNullOrEmpty(prop.Value) ? "" : rowheader.ValuesToReplace[prop.Value];


                                    sheet.Cells[rowIdx, colIndx].Style.FontSize = 11;
                                    sheet.Cells[rowIdx, colIndx].Style.HAlign = ReoGridHorAlign.Center;
                                    sheet.Cells[rowIdx, colIndx].Style.Bold = false;
                                    sheet.Cells[rowIdx, colIndx].Border.All = RangeBorderStyle.BlackSolid;
                                }
                                else
                                {
                                    sheet.Cells[rowIdx, colIndx].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                                    sheet.Cells[rowIdx, colIndx].Data = string.IsNullOrEmpty(prop.Value) ? "" : prop.Value;


                                    sheet.Cells[rowIdx, colIndx].Style.FontSize = 11;
                                    sheet.Cells[rowIdx, colIndx].Style.HAlign = ReoGridHorAlign.Center;
                                    sheet.Cells[rowIdx, colIndx].Style.Bold = false;
                                    sheet.Cells[rowIdx, colIndx].Border.All = RangeBorderStyle.BlackSolid;

                                }
                                sheet.Cells[rowIdx, colIndx].Style.VAlign = ReoGridVerAlign.Middle;
                                sheet.Cells[rowIdx, colIndx].Style.HAlign = ReoGridHorAlign.Center;
                                sheet.AutoFitColumnWidth(colIndx, true);
                            }
                        }
                    }

                    if (rowIdx == 1)//first data row
                    {
                        sheet.IterateCells(new RangePosition(rowIdx, 3, 1, headersColumns.Count()), false, (rw, col, cell) =>
                        {
                            if (sheet.Cells[rw, col].Data == null)
                            {
                                try
                                {
                                    var lastdata = lastdataSet.Where(a => a.TagName.ToLower() == headersColumns[col - 3].PLCTAG.ToLower());
                                    if (lastdata != null && lastdata.Any())
                                    {
                                        sheet.Cells[rw, col].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                                        sheet.Cells[rw, col].Data = lastdata.First().Val == null ? "" : headersColumns[col - 3].ValuesToReplace == null ? Convert.ToString(lastdata.First().Val) : headersColumns[col - 3].ValuesToReplace[Convert.ToString(lastdata.First().Val)];
                                        sheet.Cells[rw, col].Style.VAlign = ReoGridVerAlign.Middle;
                                        sheet.Cells[rw, col].Style.HAlign = ReoGridHorAlign.Center;
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                            return true;
                        });
                    }

                    if (rowIdx != 1)
                    {
                        //Copy previous value
                        sheet.IterateCells(new RangePosition(rowIdx, 3, 1, headersColumns.Count()), false, (rw, col, cell) =>
                          {
                              if (sheet.Cells[rw, col].Data == null || string.IsNullOrEmpty(sheet.Cells[rw, col].Data as string))
                              //copy data from previous row
                              {
                                  sheet.Cells[rw, col].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
                                  sheet.Cells[rw, col].Data = sheet.Cells[rowIdx - 1, col].Data;
                                  sheet.Cells[rw, col].Style.VAlign = ReoGridVerAlign.Middle;
                                  sheet.Cells[rw, col].Style.HAlign = ReoGridHorAlign.Center;
                              }
                              return true;
                          });
                    }
                    rowIdx++;
                });

                setPrintSetting(sheet);

                showProgress("Rendering Completed.");
            }
            catch (Exception ex)
            {
                CoreMessageBox.Error(ex);
            }
            this.ResumeLayout(false);
        }

        private static void setPrintSetting(Worksheet sheet)
        {
            sheet.PrintSettings.ShowGridLines = true;
            //Now set print setting
            sheet.PrintableRange = new RangePosition(0, 0, sheet.RowCount, sheet.Columns);

            if (sheet.Columns > 9)
            {
                sheet.PrintSettings.Landscape = true;
                sheet.PrintSettings.PageScaling = 0.7f;
                sheet.PrintSettings.ShowMargins = true;
                // sheet.AutoSplitPage();
            }
            else
            {
                sheet.PrintSettings.PageScaling = 0.7f;
                sheet.PrintSettings.Landscape = false;
                sheet.PrintSettings.ShowMargins = true;

                //  sheet.AutoSplitPage();
            }
            sheet.EnableSettings(WorksheetSettings.View_ShowPageBreaks);
        }


        private void UpdateHeaderText(Worksheet sheet, int rowindex, int colIndex, string headerText)
        {
            string text = string.Empty;
            //	var custom = headerText.GetCustomAttribute(typeof(DisplayNameAttribute));

            //text = custom != null ? (custom as DisplayNameAttribute).DisplayName : headerText.Name;
            text = headerText;

            _header.Add(text);

            var prof = Util.AppProfile.AhuColumns.FirstOrDefault(a => a.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase));

            Cell cell = sheet.Cells[rowindex, colIndex];
            cell.Style.VAlign = ReoGridVerAlign.Middle;
            cell.Style.HAlign = ReoGridHorAlign.Center;
            cell.Style.BackColor = Color.LightGray;
            cell.Border.All = new RangeBorderStyle(Color.Black, BorderLineStyle.Solid);

            if (prof != null && !string.IsNullOrEmpty(prof.Caption))
            {
                cell.Data = prof.Caption;
            }
            else
            {
                cell.Data = text;
            }

            if (prof != null && prof.ColumnWidth > 0)
            {
                sheet.SetColumnsWidth(colIndex, 1, (ushort)prof.ColumnWidth);
            }
            else
            {
                //sheet.SetColumnsWidth(colIndex, 1, 75);
                sheet.Cells[rowindex, colIndex].ExpandColumnWidth();
            }
        }

        private void ShowAhuList(Worksheet sheet, int i, ReportViewModel row)
        {
            Type t = typeof(ReportViewModel);
            var properties = t.GetProperties();
            int order = 0;

            foreach (var field in Util.AppProfile.AhuColumns.Where(a => a.Selected).OrderBy(a => a.Order))
            {
                var cell = sheet.Cells[i, order];
                foreach (var prop in properties)
                {
                    var attrib = prop.GetCustomAttribute<DisplayNameAttribute>();
                    if (attrib != null && attrib.DisplayName == field.Name)
                    {
                        cell.Data = prop.GetValue(row) ?? "";
                        SetCellStyle(i, order);
                    }
                }
                order++;

            }

        }

        private void SetCellStyle(int rowindex, int colindex)
        {
            var cell = dataGridView1.CurrentWorksheet.Cells[rowindex, colindex];
            cell.Style.HAlign = ReoGridHorAlign.Center;
            cell.Style.VAlign = ReoGridVerAlign.Middle;
        }

        public void ExportAsExcel(string path)
        {
            dataGridView1.ExportAsExcel(path);
            Process.Start(path);
        }

        //public Task ExportAsPdf(string path, string header, string subheader, Action<string> showProgress, CancellationToken cancellationToken)
        //{
        //   // return dataGridView1.ExportAsPdfAsync(path, header, subheader, showProgress, cancellationToken);
        //}

        public void Print()
        {
            var sheet = dataGridView1.CurrentWorksheet;

            using (var session = sheet.CreatePrintSession())
            {
                using (PrintPreviewDialog ppd = new PrintPreviewDialog())
                {
                    ppd.Document = session.PrintDocument;
                    ppd.SetBounds(200, 200, 1024, 768);
                    ppd.PrintPreviewControl.Zoom = 1d;
                    ppd.ShowDialog(this);
                }
            }
        }
        public void ShowPageSetting()
        {
            using (PageSetupDialog psd = new PageSetupDialog())
            {
                // transform ReoGrid page settings to System page settings
                psd.PageSettings = dataGridView1.CurrentWorksheet.PrintSettings.CreateSystemPageSettings();

                psd.AllowMargins = true;
                psd.AllowPrinter = true;
                psd.AllowPaper = true;
                psd.EnableMetric = true;

                if (psd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // transform System page settings to ReoGrid page settings
                    dataGridView1.CurrentWorksheet.PrintSettings.ApplySystemPageSettings(psd.PageSettings);
                }
            }
        }
        
        private void printSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPageSetting();
        }

        public Task ExportAsPdf(NexionPdfDocument pdfDocument, Action<string> showProgress, CancellationToken cancellationToken)
        {
            return dataGridView1.ExportAsPdfAsync(pdfDocument, showProgress, cancellationToken);
        }




    }
}
