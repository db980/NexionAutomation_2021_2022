using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using unvell.ReoGrid;

namespace Nexion.Shared.UI
{
    public enum PdfPageOrientation
    {
        Portrait,
        Landscape
    }
    public class NexionPdfDocument
    {
        public string Title { get; set; } = "Nexion Automation";
        public string FileName { get; set; }
        public string Author { get; set; } = "Nexion Automation";
        public string Subject { get; set; } = "Data Reports";
        /// <summary>
        /// can be html
        /// </summary>
        public string Header { get; set; }

        public string SubHeader { get; set; }

        /// <summary>
        /// can be html
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Zero based Index 
        /// </summary>
        public int DataRowHeaderIndex { get; set; } = 0;
        /// <summary>
        /// Zero based Index 
        /// </summary>
        public int DataRowColIndex { get; set; } = 0;

        //public int NbrOfDataRowsPortrait { get; set; } = 67;

        //public int NbrOfDataRowsLandscape { get; set; } = 42;
        public int NbrOfDataRows { get; set; }

        public int NbrOfCOlumns { get; set; }

        /// <summary>
        /// Full name with path 
        /// </summary>
        public string LogoImageFileName { get; set; }

        public int DataRowStartIndex { get; set; } = 0;

        public int FontSize { get; set; } = 8;
        public double Scale { get; set; } = .7;

        public PdfPageOrientation PageOrientation { get; set; }
        public PdfPageWidthOption PageWidthOption { get; set; }


    }
    public class PDFWriter
    {
        // Methods
        private static void AddPageFooter(Section section, NexionPdfDocument pdfDocument,int page,int j)
        {
            Paragraph paragraph1 = section.AddParagraph();
            Table tbl = new Table();
            paragraph1.AddLineBreak();

            paragraph1.AddFormattedText(pdfDocument.Footer);
            paragraph1.AddLineBreak();
            paragraph1.AddLineBreak();
            paragraph1.AddLineBreak();
            paragraph1.AddFormattedText(pdfDocument.Footer);
           
            tbl.AddColumn(15);
            paragraph1.AddLineBreak();
            Paragraph paragraph2 = section.Footers.Primary.AddParagraph();

            //int su=(j );

            //paragraph2.Document.DefaultPageSetup.BottomMargin = Unit.FromCm(2.0);
            //paragraph2.AddPageField();
            // paragraph2.AddFormattedText(page + "  Of  " + su);
            // paragraph2.AddFormattedText(page+"  Of  " + su ) ;
            //paragraph2.AddPageField();

            Table table = section.AddTable();
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;
            table.Borders.Visible = true;


            Column column1 = table.AddColumn();
            column1.Format.Alignment = ParagraphAlignment.Center;
            //column1.Width = PDF;
            //table.Format.Font.Size = Unit.FromPoint(grid.CurrentWorksheet.Cells[0, 1].Style.FontSize * 0.7);
            Row row = table.AddRow();
            SetHeaderRowStyle(row);
            //WriteRow(new RangePosition(pdfDocument.DataRowHeaderIndex, pdfDocument.DataRowColIndex, 1, 5), null, row);


        }

        private static void AddPageHeader(NexionPdfDocument pdfDocument, Section section)
        {
            Paragraph paragraph = section.AddParagraph();
            Paragraph paragraph2 = section.AddParagraph();
            if (pdfDocument.LogoImageFileName != null)
            {
                paragraph.AddImage(pdfDocument.LogoImageFileName).Width = "5cm";
                
                paragraph.Format.Alignment = ParagraphAlignment.Right;
               
            }
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph2.Format.Alignment = ParagraphAlignment.Center;
            paragraph2.Format.Font.Size = 17;
            paragraph2.Format.Font.Bold = true;
            paragraph2.Format.Font.Color = Colors.Black; 
            paragraph2.AddFormattedText(pdfDocument.Header);
           
            paragraph2.AddFormattedText("\n");
            paragraph2.AddFormattedText(pdfDocument.SubHeader);
            paragraph.AddFormattedText("\n");
            paragraph.AddLineBreak();

            

        }

        public static Document CreateDocument(ReoGridControl grid, NexionPdfDocument pdfDocument, Action<string> showProgress)
        {
            Document document = new Document
            {
                Info = {
                Title = pdfDocument.Title,
                Subject = pdfDocument.Subject,
                Author = pdfDocument.Author
            },
                DefaultPageSetup = { Orientation = (pdfDocument.PageOrientation == PdfPageOrientation.Landscape) ? Orientation.Landscape : Orientation.Portrait }
            };
            DefineStyles(document);
            document.DefaultPageSetup.TopMargin = Unit.FromMillimeter(5.0);
            document.DefaultPageSetup.LeftMargin = Unit.FromMillimeter(5.0);
            document.DefaultPageSetup.RightMargin = Unit.FromMillimeter(5.0);
            document.DefaultPageSetup.BottomMargin = Unit.FromMillimeter(5.0);
            int page = 0;
            int nbrOfDataRows = pdfDocument.NbrOfDataRows;
            int maxContentRow = grid.CurrentWorksheet.MaxContentRow;
            int nbrOfCOlumns = pdfDocument.NbrOfCOlumns;
            int num5 = 0;


            int datafind = 0;
                
                if(nbrOfDataRows <= 60)
            {
                datafind = (grid.CurrentWorksheet.RowCount / 40)+ (grid.CurrentWorksheet.RowCount / 40);
            }
            else
            {
                datafind = grid.CurrentWorksheet.RowCount / 32;
            }
                
                
            for (int i = 0; i < nbrOfCOlumns; i++)
            {
                num5 += grid.CurrentWorksheet.GetColumnWidth(i);
            }
            if (pdfDocument.PageWidthOption == PdfPageWidthOption.FitToWidth)
            {
                pdfDocument.Scale = ((document.DefaultPageSetup.PageWidth.Point - document.DefaultPageSetup.LeftMargin.Point) - document.DefaultPageSetup.RightMargin.Point) / ((double)num5);
            }
            Table table = new Table();
            bool flag = true;
            int num6 = 0;
            for (int j = 0; j <= maxContentRow; j++)
            {
                showProgress($"Rows created {j}/{maxContentRow}");
                if (j >= pdfDocument.DataRowStartIndex)
                {
                    if (((num6 % nbrOfDataRows) == 0) | flag)
                    {
                        flag = false;
                        page++;
                        Section section = document.AddSection();
                        table = CreatePage(grid, pdfDocument, document, section, page, datafind, nbrOfCOlumns, pdfDocument.Scale);
                       
                        if (j == 0)
                        {
                            j = j + 1;
                        }
                        
                    }
                    
                    //else
                    //{
                        FillContent(grid, pdfDocument, page, nbrOfCOlumns, table, j);
                    //}
                    num6++;
                }
            }
            return document;
        }


        private static void AddFooterData(Section section)
        {
            // add prepared by. approved by etc

            //var rightFooterSection = new Paragraph
            //{
            //    Format = { Alignment = ParagraphAlignment.Left }

            //};
            //rightFooterSection.AddText("Print By");

            //rightFooterSection.AddLineBreak();
            //rightFooterSection.AddText("hello");

            //// rightFooterSection.AddNumPagesField();

            //section.Footers.Primary.Add(rightFooterSection);
            //var rightFooterSection2 = new Paragraph
            //{
            //    Format = { Alignment = ParagraphAlignment.Center }
            //};
            //rightFooterSection2.AddText("Checked By");



            //section.Footers.Primary.Add(rightFooterSection2);




            //var date = DateTime.Now.ToString("yyyy/MM/dd");
            //var leftSection = new Paragraph
            //{
            //    Format = { Alignment = ParagraphAlignment.Right }
            //};
            //leftSection.AddText("Verified By");

            //leftSection.AddLineBreak();
            //leftSection.AddText(date);
            //section.Footers.Primary.Add(leftSection);






            //MigraDoc.DocumentObjectModel.Tables.Table table = new MigraDoc.DocumentObjectModel.Tables.Table();
            ////table = page.AddTable();
            //table.Style = "Table";
            //table.Borders.Color = Colors.Black;
            //table.Borders.Width = 0.25;
            //table.Borders.Left.Width = 0.5;
            //table.Borders.Right.Width = 0.5;
            //table.Rows.LeftIndent = 0;

            ////create column
            //MigraDoc.DocumentObjectModel.Tables.Column column = new MigraDoc.DocumentObjectModel.Tables.Column();
            //column = table.AddColumn("10cm");
            //column.Format.Alignment = ParagraphAlignment.Left;



            //MigraDoc.DocumentObjectModel.Tables.Row tableRow = table.AddRow();
            //tableRow.Cells[1].AddParagraph("hello");
            //tableRow.Cells[3].AddParagraph("bye");

            //tableRow.Cells[5].AddParagraph("bye");
            //section.Footers.Primary.Add(table);













            


            Table table =new Table();
            table.Borders.Visible = true;
            table.Format.Shading.Color = Colors.LightGray;
           // table.Shading.Color = Colors.Salmon;
           // table.TopPadding = 5;
            //table.BottomPadding = 5;
            
            Column column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;
            column.Width = 190;
            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;
            column.Width = 190;
            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;
            column.Width = 190;

            //table.Rows.Height = 35;
            

            Row row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Height = 15;
            row.Cells[0].AddParagraph("Printed By");
            row.Cells[1].AddParagraph("Checked By");
            row.Cells[2].AddParagraph("Verified By");

            row = table.AddRow();
            row.Format.Shading.Color = Colors.White;
            row.Height = 30;
            row.VerticalAlignment = VerticalAlignment.Center;
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            row.Cells[0].AddParagraph(userName);
            row.Cells[1].AddParagraph(" ");
            row.Cells[2].AddParagraph(" ");



          

            section.Add(table);





            //{
            //    Format = { Alignment = ParagraphAlignment.Center }
            //};

            //Section section1 = section;

            //var paragraph1 = new Paragraph();
            //{
            //    Format = { Alignment = ParagraphAlignment.Center }
            //};
            //paragraph.AddText("Page ");
            //paragraph.AddPageField();
            //paragraph.AddText(" of ");
            //paragraph.AddNumPagesField();

            //section1.Headers.Primary.Add(paragraph);










            //var date = DateTime.Now.ToString("yyyy/MM/dd");
            //var leftSection = new Paragraph
            //{
            //    Format = { Alignment = ParagraphAlignment.Right }
            //};
            //leftSection.AddNumPagesField();
         
            //leftSection.AddLineBreak();
            //leftSection.AddText("Page ");
            //leftSection.AddPageField();
            //leftSection.AddText(" of ");
            //leftSection.AddNumPagesField();
            //leftSection.AddText(" \n ");
            //leftSection.AddText(" \n ");
            //section.Footers.Section.Add(leftSection);
















        }
        private static Table CreatePage(ReoGridControl grid, NexionPdfDocument pdfDocument, Document document, Section section, int page, int j, int columns, double scale)
        {
            AddPageHeader(pdfDocument, section);
            Table table = section.AddTable();
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;
            table.Borders.Visible = true;
            table.Format.Font.Size = Unit.FromPoint(grid.CurrentWorksheet.Cells[0, 1].Style.FontSize * 0.7);
            for (int i = 0; i < columns; i++)
            {
                Column column1 = table.AddColumn();
                column1.Format.Alignment = ParagraphAlignment.Center;
                column1.Width = grid.CurrentWorksheet.GetColumnWidth(i) * scale;
            }
            Row row = table.AddRow();
            SetHeaderRowStyle(row);
            WriteRow(new RangePosition(pdfDocument.DataRowHeaderIndex, pdfDocument.DataRowColIndex, 1, columns), grid, row);

            Paragraph paragraph1 = section.AddParagraph();
            Table tbl = new Table();
            paragraph1.AddLineBreak();

            //paragraph1.AddFormattedText(pdfDocument.Footer);
            paragraph1.AddLineBreak();
            paragraph1.AddLineBreak();
            paragraph1.AddLineBreak();


            AddFooterData(section);


           // AddPageFooter(section, pdfDocument, page, j);






            // Document document = new Document();






            // var section = documentWrapper.CurrentSection;
            //var footer = section.Footers.Primary;
            // var reportMeta = documentWrapper.AdminReport.ReportMeta;

            // Format, then add the report date to the footer
            // var footerDate = string.Format("{0:MM/dd/yyyy}", reportMeta.ReportDate);











































            //AddFooterData(section);




















            return table;
        }





     
        private static void DefineContentSection(Document document)
        {
            Section section1 = document.AddSection();
            section1.PageSetup.OddAndEvenPagesHeaderFooter = false;
            section1.PageSetup.StartingNumber = 1;
            section1.Headers.Primary.AddParagraph("\tOdd Page Header");
            section1.Headers.EvenPage.AddParagraph("Even Page Header");
            Paragraph paragraph = new Paragraph();
            paragraph.AddTab();
            paragraph.AddPageField();
            section1.Footers.Primary.Add(paragraph);
            section1.Footers.EvenPage.Add(paragraph.Clone());
        }

        public static void DefineParagraphs(Document document)
        {
            document.LastSection.AddParagraph("Paragraph Layout Overview", "Heading1").AddBookmark("Paragraphs");
        }

        public static void DefineStyles(Document document)
        {
            document.Styles["Normal"].Font.Name = "Times New Roman";
            Style style1 = document.Styles["Heading1"];
            style1.Font.Name = "Tahoma";
            style1.Font.Size = 14;
            style1.Font.Bold = true;
            style1.Font.Color = Colors.DarkBlue;
            style1.ParagraphFormat.PageBreakBefore = true;
            style1.ParagraphFormat.SpaceAfter = 6;
            Style style2 = document.Styles["Heading2"];
            style2.Font.Size = 12;
            style2.Font.Bold = true;
            style2.ParagraphFormat.PageBreakBefore = false;
            style2.ParagraphFormat.SpaceBefore = 6;
            style2.ParagraphFormat.SpaceAfter = 6;
            Style style3 = document.Styles["Heading3"];
            style3.Font.Size = 10;
            style3.Font.Bold = true;
            style3.Font.Italic = true;
            style3.ParagraphFormat.SpaceBefore = 6;
            style3.ParagraphFormat.SpaceAfter = 3;
            document.Styles["Header"].ParagraphFormat.AddTabStop("16cm", TabAlignment.Center);
            document.Styles["Footer"].ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
            Style style4 = document.Styles.AddStyle("TextBox", "Normal");
            style4.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style4.ParagraphFormat.Borders.Width = 2.5;
            style4.ParagraphFormat.Borders.Distance = "3pt";
            style4.ParagraphFormat.Shading.Color = Colors.SkyBlue;
            Style style5 = document.Styles.AddStyle("TOC", "Normal");
            style5.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style5.ParagraphFormat.Font.Color = Colors.Blue;
            Style style6 = document.Styles.AddStyle("Reference", "Normal");
            style6.ParagraphFormat.SpaceBefore = "5mm";
            style6.ParagraphFormat.SpaceAfter = "5mm";
            style6.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        public static void DefineTables(Document document)
        {
            document.LastSection.AddParagraph("Table Overview", "Heading1").AddBookmark("Tables");
            DemonstrateSimpleTable(document);
            DemonstrateCellMerge(document);
        }

        public static void DemonstrateCellMerge(Document document)
        {
            document.LastSection.AddParagraph("Cell Merge", "Heading2");
            Table table1 = document.LastSection.AddTable();
            table1.Borders.Visible = true;
            table1.TopPadding = 5;
            table1.BottomPadding = 5;
            table1.AddColumn().Format.Alignment = ParagraphAlignment.Left;
            table1.AddColumn().Format.Alignment = ParagraphAlignment.Center;
            table1.AddColumn().Format.Alignment = ParagraphAlignment.Right;
            table1.Rows.Height = 0x23;
            Row row1 = table1.AddRow();
            row1.Cells[0].AddParagraph("Merge Right");
            row1.Cells[0].MergeRight = 1;
            Row row2 = table1.AddRow();
            row2.VerticalAlignment = VerticalAlignment.Bottom;
            row2.Cells[0].MergeDown = 1;
            row2.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row2.Cells[0].AddParagraph("Merge Down");
            table1.AddRow();
        }

        public static void DemonstrateSimpleTable(Document document)
        {
            document.LastSection.AddParagraph("Simple Tables", "Heading2");
            Table table = new Table
            {
                Borders = { Width = 0.75 }
            };
            table.AddColumn(Unit.FromCentimeter(2.0)).Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn(Unit.FromCentimeter(5.0));
            Row row1 = table.AddRow();
            row1.Shading.Color = Colors.PaleGoldenrod;
            row1.Cells[0].AddParagraph("Itemus");
            row1.Cells[1].AddParagraph("Descriptum");
            Row row2 = table.AddRow();
            row2.Cells[0].AddParagraph("1");
            MigraDoc.DocumentObjectModel.Tables.Cell cell1 = row2.Cells[1];
            Row row3 = table.AddRow();
            row3.Cells[0].AddParagraph("2");
            MigraDoc.DocumentObjectModel.Tables.Cell cell2 = row3.Cells[1];
            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
            document.LastSection.Add(table);
        }

        private static void FillContent(ReoGridControl grid, NexionPdfDocument pdfDocument, int page, int columns, Table table, int rowcntr)
        {
            Row row = table.AddRow();
            row.Height = grid.CurrentWorksheet.GetRowHeight(rowcntr) * pdfDocument.Scale;
            WriteRow(new RangePosition(rowcntr, pdfDocument.DataRowColIndex, 1, columns), grid, row);

        }

        private static void SetHeaderRowStyle(Row row)
        {
            row.HeadingFormat = true;
            row.Shading.Color = Colors.LightGray;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
        }

        public static void Write(NexionPdfDocument pdfDocument, ReoGridControl grid, Action<string> showProgress, string header = "", string subheader = "", string imagefile = null, int headerrowindex = 0, int headercolstart = 0)
        {
            Document docObject = CreateDocument(grid, pdfDocument, showProgress);
            DdlWriter.WriteToFile(docObject, "MigraDoc.mdddl");
            PdfDocumentRenderer renderer1 = new PdfDocumentRenderer(true)
            {
                Document = docObject
            };
            showProgress("Creating document");
            renderer1.RenderDocument();
            showProgress("Saving the file");
            renderer1.PdfDocument.Save(pdfDocument.FileName);
            showProgress("Opening file");
            Process.Start(pdfDocument.FileName);
        }

        public static Task WriteAsync(NexionPdfDocument pdfDocument, ReoGridControl grid, Action<string> showProgress, CancellationToken cancellationToken) =>
            Task.Run(delegate {
                Write(pdfDocument, grid, showProgress, "", "", null, 0, 0);
            }, cancellationToken);

        private static void WriteRow(RangePosition range, ReoGridControl grid, Row row)
        {
            grid.CurrentWorksheet.IterateCells(range, delegate (int currRow, int col, unvell.ReoGrid.Cell cell) {
                try
                {
                    MigraDoc.DocumentObjectModel.Tables.Cell cell2 = row.Cells[col];
                    if (cell.Data != null)
                    {
                        cell2.AddParagraph(cell.Data.ToString());
                    }
                }
                catch (Exception)
                {
                    return true;
                }
                return true;
            });
        }
    }



}
