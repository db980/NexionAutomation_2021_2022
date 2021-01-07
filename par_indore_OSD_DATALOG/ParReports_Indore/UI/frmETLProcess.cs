using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParReports_Indore.UI
{
    public partial class frmETLProcess : Form
    {
        public frmETLProcess()
        {
            InitializeComponent();
        }

        private Task _currentETLTask;
        private CancellationTokenSource tokenSource;

        private async void btnStart_Click(object sender, EventArgs e)
        {
            //ETLBatchService etl = new ETLBatchService
            //{
            //    Interval = (int)this.nIntervalInMinutes.Value
            //};
            //btnStart.Enabled = false;
            //tokenSource = new CancellationTokenSource();
            //CancellationToken ct = tokenSource.Token;
            //_currentETLTask = etl.Start(ShowProgress, ct);
        }

        private void ShowProgress(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ShowProgress), message);
                return;
            }

            listLog.Items.Add(message);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tokenSource.Cancel();
            btnStart.Enabled = false;
        }
    }
}
