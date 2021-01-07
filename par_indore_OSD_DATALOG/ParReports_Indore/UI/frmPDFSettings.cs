using Nexion.Shared.UI;
using System;
using System.Windows.Forms;


namespace ParReports_Indore
{
    public partial class frmPDFSettings : Form
    {
        private PdfSettings _pdfSettings;


        public frmPDFSettings()
        {
            InitializeComponent();

        }


        public frmPDFSettings(PdfSettings pdfSettings)
        {
            InitializeComponent();
            _pdfSettings = pdfSettings;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            numColumns.Value = _pdfSettings.NbrOfColumnsToBeIncluded;
            numFontSize.Value = _pdfSettings.FontSize;
            if (_pdfSettings.Orientation == PageOrientation.Landscape)
            {
                rbLandscape.Checked = true;
            }
            else
            {
                rbPortrait.Checked = true;
            }

            numRows.Value = _pdfSettings.NbrOfRowsInPage;
            txtScale.Text = _pdfSettings.Scale.ToString();
            cmbPageWidth.SelectedItem = "Select One";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            _pdfSettings.NbrOfColumnsToBeIncluded = (int)numColumns.Value;
            _pdfSettings.FontSize = numFontSize.Value;

            if (rbLandscape.Checked == true)
            {
                _pdfSettings.Orientation = PageOrientation.Landscape;
            }
            else
            {
                _pdfSettings.Orientation = PageOrientation.Portrait;
            }

            if (cmbPageWidth.SelectedItem == "Select One")
            {
                CoreMessageBox.Show("Please Select Page Width Option.");
                return;
            }
            else if (cmbPageWidth.SelectedItem == "Scale As Worksheet")
            {
                _pdfSettings.WidthOption = PageWidthOption.ScaleAsWorksheet;
            }
            else if (cmbPageWidth.SelectedItem == "Fit To Width")
            {
                _pdfSettings.WidthOption = PageWidthOption.FitToWidth;
            }
            else
            {
                CoreMessageBox.Show("Please Select Page Width Option.");
                return;
            }

            _pdfSettings.NbrOfRowsInPage = (int)numRows.Value;
            _pdfSettings.Scale = Convert.ToDouble(txtScale.Text.ToString());

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmPDFSettings_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    public class PdfSettings
    {
        public int NbrOfColumnsToBeIncluded { get; set; }
        public int NbrOfRowsInPage { get; set; }
        public PageOrientation Orientation { get; set; }
        public decimal FontSize { get; set; }
        public double Scale { get; set; }
        public PageWidthOption WidthOption { get; set; }
    }

    public enum PageOrientation
    {
        Portrait,
        Landscape
    }
    public enum PageWidthOption
    {
        ScaleAsWorksheet,
        FitToWidth
    }

}
