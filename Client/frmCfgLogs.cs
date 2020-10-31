using System;
using System.Windows.Forms;

namespace Client
{
    public partial class FrmCfgLogs : Form
    {
        //private readonly Settings _settings = Settings.getInstance();

        public FrmCfgLogs()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var folderPath = dlgLogBrowse.ShowDialog();

            if(folderPath == DialogResult.OK)
                txtLogDir.Text = dlgLogBrowse.SelectedPath;
        }

        private void frmCfgLogs_Load(object sender, EventArgs e)
        {
            txtLogDir.Text = Settings.LogDir;
            dlgLogBrowse.SelectedPath = Settings.LogDir;
            chkNotice.Checked = Settings.ShowNotice;
            chkInformation.Checked = Settings.ShowInformation;
            chkWarnings.Checked = Settings.ShowWarning;
            chkErrors.Checked = Settings.ShowError;
            chkDebug.Checked = Settings.ShowDebug;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Settings.LogDir = txtLogDir.Text;
            Settings.ShowNotice = chkNotice.Checked;
            Settings.ShowInformation = chkInformation.Checked;
            Settings.ShowWarning = chkWarnings.Checked;
            Settings.ShowError = chkErrors.Checked;
            Settings.ShowDebug = chkDebug.Checked;

        }

    }
}