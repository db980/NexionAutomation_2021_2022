using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using unvell.ReoGrid;
using unvell.ReoGrid.IO;
using Nexion.Shared.Core;
namespace Nexion.Shared.UI
{
  public class NexionDataReportGrid<T> : UserControl
	{
		private Button btnXls;
		private Button btnPrint;
		private Button btnPdf;
		private Panel pnlData;
		private Panel pnlFooter;
		private ReoGridControl gridControl;
		private Panel pnlHeader;
		int columns = 0;

		public NexionDataReportGrid() : base()
		{
			InitializeComponent();

			var currentsheet = gridControl.CurrentWorksheet;
			//create header rows
			var propertyInfoWithDisplayNames = typeof(T).GetProperties().Where(a => a.GetCustomAttribute(typeof(FieldNameAttribute)) != null);
			int i = 0;

			foreach (var prop in propertyInfoWithDisplayNames.OrderBy(a => (a.GetCustomAttribute(typeof(FieldNameAttribute)) as FieldNameAttribute).SequenceNumber))
			{
				var displayName = prop.GetCustomAttribute(typeof(FieldNameAttribute)) as FieldNameAttribute;
				setHeaderCellStyle(currentsheet.Cells[StartHeaderRow, i]);

				if (prop.PropertyType.IsNumericType())
				{
					currentsheet.Cells[StartHeaderRow, i].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.General;
				}
				else if (prop.PropertyType == typeof(DateTime))
				{
					currentsheet.Cells[StartHeaderRow, i].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
				}
				else
				{
					currentsheet.Cells[StartHeaderRow, i].DataFormat = unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text;
				}

				currentsheet.Cells[StartHeaderRow, i].Data = displayName.DisplayName;
				currentsheet.SetColumnsWidth(i, 1, 150);
				i++;
			}
			columns = i + 1;
		}

		private void setHeaderCellStyle(unvell.ReoGrid.Cell cell)
		{
			cell.Style.Bold = true;
			cell.Style.VAlign = ReoGridVerAlign.Middle;
			cell.Style.HAlign = ReoGridHorAlign.Center;
			cell.Style.BackColor = System.Drawing.Color.LightGray;
			cell.Border.All = new RangeBorderStyle(System.Drawing.Color.Black, BorderLineStyle.Solid);
		}

		private void setDataRowStyle(unvell.ReoGrid.Cell cell)
		{
			cell.Style.Bold = true;
			cell.Style.VAlign = ReoGridVerAlign.Middle;
			cell.Style.HAlign = ReoGridHorAlign.Center;
			cell.Style.BackColor = System.Drawing.Color.LightGray;
			cell.Border.All = new RangeBorderStyle(System.Drawing.Color.Black, BorderLineStyle.Solid);
		}

		/// <summary>
		/// Header start from Row number eg. default 0
		/// </summary>
		public int StartHeaderRow { get; set; } = 0;

		public IEnumerable<T> DataSource(IEnumerable<T> data)
		{
			var sheet = gridControl.CurrentWorksheet;
			sheet.Rows = 2;
			sheet.Columns = columns;
			sheet.InsertRows(2, data.Count());
			sheet.SetSettings(WorksheetSettings.Edit_Readonly, true);
			sheet.PrintableRange = new RangePosition(0, 0, sheet.RowCount, sheet.Columns);

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

			int rowIdx = 1;

			var propertyInfoWithDisplayNames = typeof(T).GetProperties().Where(a => a.GetCustomAttribute(typeof(FieldNameAttribute)) != null);

			foreach (T item in data)
			{
				var propertyInfo = item.GetType().GetProperties().Where(a => a.GetCustomAttribute(typeof(FieldNameAttribute)) != null);
				int colIndx = 0;

				foreach (var prop in propertyInfo.OrderBy(a => (a.GetCustomAttribute(typeof(FieldNameAttribute)) as FieldNameAttribute).SequenceNumber))
				{
					sheet.Cells[rowIdx, colIndx].Data = item.GetPropValue(prop.Name);
					colIndx++;
				}

				rowIdx++;
				yield return item;
			}
		}

		public void LoadFile(string file)
		{
			if (!File.Exists(file))
				throw new FileNotFoundException($"File {file} does not exists.");

			gridControl.Load(file, FileFormat.Excel2007);
		}

		public void SaveFile(string file)
		{
			gridControl.Save(file, FileFormat._Auto);
		}

		//public Task ExportAsPdfAsync(NexionPdfDocument pdfDocument, Action<string> showProgress, CancellationToken cancellationToken)
		//{
		//	return PDFWriter.WriteAsync(pdfDocument, gridControl, showProgress, cancellationToken);
		//}		

		public bool ExportAsExcel(string fileName, bool showConfirmation = true)
		{
			return Export(fileName, unvell.ReoGrid.IO.FileFormat.Excel2007,showConfirmation);
		}

		private bool Export(string path, unvell.ReoGrid.IO.FileFormat fileFormat = unvell.ReoGrid.IO.FileFormat.Excel2007,bool showConfirmation=true)
		{
			var workbook = this;
			if (File.Exists(path))
			{
				var res = CoreMessageBox.Question($"File {path} already Exists. Do you want to replace it.", "Confirmation");
				if (res == DialogResult.Yes)
				{

					gridControl.Save(path, fileFormat);
				}
				else
				{
					CoreMessageBox.Warning($"File could not save at {path}");
					return false;
				}
			}
			else
			{
				gridControl.Save(path, fileFormat);
				CoreMessageBox.Show($"File Saved at {path}");
			}
			return true;
		}

		private void InitializeComponent()
		{
			this.pnlHeader = new System.Windows.Forms.Panel();
			this.btnXls = new System.Windows.Forms.Button();
			this.btnPrint = new System.Windows.Forms.Button();
			this.btnPdf = new System.Windows.Forms.Button();
			this.pnlData = new System.Windows.Forms.Panel();
			this.gridControl = new unvell.ReoGrid.ReoGridControl();
			this.pnlFooter = new System.Windows.Forms.Panel();
			this.pnlHeader.SuspendLayout();
			this.pnlData.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlHeader
			// 
			this.pnlHeader.Controls.Add(this.btnXls);
			this.pnlHeader.Controls.Add(this.btnPrint);
			this.pnlHeader.Controls.Add(this.btnPdf);
			this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlHeader.Location = new System.Drawing.Point(0, 0);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(942, 28);
			this.pnlHeader.TabIndex = 1;
			// 
			// btnXls
			// 
			this.btnXls.Location = new System.Drawing.Point(60, 2);
			this.btnXls.Name = "btnXls";
			this.btnXls.Size = new System.Drawing.Size(42, 23);
			this.btnXls.TabIndex = 2;
			this.btnXls.Text = "Xls";
			this.btnXls.UseVisualStyleBackColor = true;
			this.btnXls.Click += new System.EventHandler(this.btnXls_Click);
			// 
			// btnPrint
			// 
			this.btnPrint.Location = new System.Drawing.Point(108, 2);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(42, 23);
			this.btnPrint.TabIndex = 1;
			this.btnPrint.Text = "Print";
			this.btnPrint.UseVisualStyleBackColor = true;
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// btnPdf
			// 
			this.btnPdf.Location = new System.Drawing.Point(12, 2);
			this.btnPdf.Name = "btnPdf";
			this.btnPdf.Size = new System.Drawing.Size(42, 23);
			this.btnPdf.TabIndex = 0;
			this.btnPdf.Text = "Pdf";
			this.btnPdf.UseVisualStyleBackColor = true;
			this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
			// 
			// pnlData
			// 
			this.pnlData.Controls.Add(this.gridControl);
			this.pnlData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlData.Location = new System.Drawing.Point(0, 28);
			this.pnlData.Name = "pnlData";
			this.pnlData.Size = new System.Drawing.Size(942, 421);
			this.pnlData.TabIndex = 2;
			// 
			// gridControl
			// 
			this.gridControl.BackColor = System.Drawing.Color.White;
			this.gridControl.ColumnHeaderContextMenuStrip = null;
			this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl.LeadHeaderContextMenuStrip = null;
			this.gridControl.Location = new System.Drawing.Point(0, 0);
			this.gridControl.Name = "gridControl";
			this.gridControl.RowHeaderContextMenuStrip = null;
			this.gridControl.Script = null;
			this.gridControl.SheetTabContextMenuStrip = null;
			this.gridControl.SheetTabNewButtonVisible = true;
			this.gridControl.SheetTabVisible = true;
			this.gridControl.SheetTabWidth = 60;
			this.gridControl.ShowScrollEndSpacing = true;
			this.gridControl.Size = new System.Drawing.Size(942, 421);
			this.gridControl.TabIndex = 0;
			this.gridControl.Text = "reoGridControl1";
			// 
			// pnlFooter
			// 
			this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlFooter.Location = new System.Drawing.Point(0, 428);
			this.pnlFooter.Name = "pnlFooter";
			this.pnlFooter.Size = new System.Drawing.Size(942, 21);
			this.pnlFooter.TabIndex = 3;
			// 
			// NexionDataReportGrid
			// 
			this.Controls.Add(this.pnlFooter);
			this.Controls.Add(this.pnlData);
			this.Controls.Add(this.pnlHeader);
			this.Name = "NexionDataReportGrid";
			this.Size = new System.Drawing.Size(942, 449);
			this.pnlHeader.ResumeLayout(false);
			this.pnlData.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private async void btnPdf_Click(object sender, EventArgs e)
		{
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.DefaultExt = ".pdf";
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				//await ExportAsPdfAsync(fileDialog.FileName, "", "", null, CancellationToken.None);
			}
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{

		}

		private void btnXls_Click(object sender, EventArgs e)
		{
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.DefaultExt = ".pdf";

			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				SaveFile(fileDialog.FileName);
			}
		}
	}

}
