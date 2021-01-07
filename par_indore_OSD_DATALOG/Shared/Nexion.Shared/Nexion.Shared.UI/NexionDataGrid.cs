using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using unvell.ReoGrid;
using unvell.ReoGrid.IO;

namespace Nexion.Shared.UI
{
	public class NexionDataGrid : unvell.ReoGrid.ReoGridControl
	{

		public NexionDataGrid() : base()
		{
            WorkbookLoaded += NexionDataGrid_WorkbookLoaded;  
        }

        private void NexionDataGrid_WorkbookLoaded(object sender, EventArgs e)
        {
            this.CurrentWorksheet.SetSettings(WorksheetSettings.Behavior_MouseWheelToZoom, true);
        }

        public void LoadFile(string file)
		{
			if (!File.Exists(file))
				throw new FileNotFoundException($"File {file} does not exists.");

			this.Load(file, FileFormat.Excel2007);
		}

		public void SaveFile(string file)
		{
			this.Save(file, FileFormat._Auto);
		}

        public Task ExportToHTML(NexionPdfDocument document, Action<string> showProgress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                MemoryStream ms = new MemoryStream();
                this.CurrentWorksheet.ExportAsHTML(ms);
                string result = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }, cancellationToken);
        }

        public Task ExportAsPdfAsync(NexionPdfDocument document, Action<string> showProgress, CancellationToken cancellationToken)
		{
           return PDFWriter.WriteAsync(document, this, showProgress, cancellationToken);
        }
		
		public bool ExportAsExcel(string fileName)
		{
			return Export(fileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
		}

		private bool Export(string path, unvell.ReoGrid.IO.FileFormat fileFormat = unvell.ReoGrid.IO.FileFormat.Excel2007)
		{
			var workbook = this;
			if (File.Exists(path))
			{
				var res = CoreMessageBox.Question($"File {path} already Exists. Do you want to replace it.", "Confirmation");
				if (res == DialogResult.Yes)
				{

					this.Save(path, fileFormat);

				}
				else
				{
					CoreMessageBox.Warning($"File could not save at {path}");
					return false;
				}
			}
			else
			{
				this.Save(path, fileFormat);
				CoreMessageBox.Show($"File Saved at {path}");
			}
			return true;
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}

	//public class LayoutHelper
	//{
	//	private readonly PdfDocument _document;
	//	private readonly XUnit _topPosition;
	//	private readonly XUnit _bottomMargin;
	//	private XUnit _currentPosition;

	//	public LayoutHelper(PdfDocument document, XUnit topPosition, XUnit bottomMargin)
	//	{
	//		_document = document;
	//		_topPosition = topPosition;
	//		_bottomMargin = bottomMargin;
	//		// Set a value outside the page - a new page will be created on the first request.
	//		_currentPosition = bottomMargin + 10000;
	//	}

	//	public XUnit GetLinePosition(XUnit requestedHeight)
	//	{
	//		return GetLinePosition(requestedHeight, -1f);
	//	}

	//	public XUnit GetLinePosition(XUnit requestedHeight, XUnit requiredHeight)
	//	{
	//		XUnit required = requiredHeight == -1f ? requestedHeight : requiredHeight;
	//		if (_currentPosition + required > _bottomMargin)
	//			CreatePage();
	//		XUnit result = _currentPosition;
	//		_currentPosition += requestedHeight;
	//		return result;
	//	}

	//	public XGraphics Gfx { get; private set; }
	//	public PdfPage Page { get; private set; }

 //       void CreatePage()
 //       {
 //           Page = _document.AddPage();
 //           Page.Size = PageSize.A4;
 //           Gfx = XGraphics.FromPdfPage(Page);
 //           _currentPosition = _topPosition;
 //       }
 //   }

	public static class DataGridExtensions
	{
		public static void SetHAlignStyle(this Worksheet sheet, int rowindex, int colindex)
		{
			sheet.Cells[rowindex, colindex].Style.HAlign = ReoGridHorAlign.Center;
		}


		public static void SetHAlignStyle(this unvell.ReoGrid.Cell cell)
		{
			cell.Style.HAlign = ReoGridHorAlign.Center;
		}

		public static void SetAutoFitColumnWidthAll(this Worksheet worksheet)
		{
			for (int col = 0; col < worksheet.ColumnCount; col++)
			{
				worksheet.AutoFitColumnWidth(col);
			}
		}

		public static void SetAutoFitRowHeightAll(this Worksheet worksheet)
		{
			for (int row = 0; row < worksheet.Rows; row++)
			{
				worksheet.AutoFitRowHeight(row);
			}
		}
		/// <summary>
		/// Bold all cells in a row
		/// </summary>
		/// <param name=""></param>
		public static void BoldRow(this Worksheet worksheet, int rowindex)
		{
			for (int col = 0; col < worksheet.ColumnCount; col++)
			{
				worksheet.Cells[rowindex, col].Style.Bold = true;
			}
		}
	}
}
