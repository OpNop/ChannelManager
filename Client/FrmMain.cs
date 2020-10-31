using System;
using System.ComponentModel;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Client
{
    public partial class FrmMain : Form
    {
        private bool _changed;
        
        public FrmMain()
        {
            InitializeComponent();
            
            if(Settings.Error)
                MessageBox.Show(Settings.LastError, "Error getting settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnTestMySQL_Click(object sender, EventArgs e)
        {
            MySqlConnection connection;

            var myConString = "SERVER="+txtMYSQLAddress.Text+";" +
                              "DATABASE="+txtMYSQLDatabase.Text+";" +
                              "UID="+txtMYSQLUser.Text+";" +
                              "PASSWORD="+txtMYSQLPass.Text+";";

            try
            {
                connection = new MySqlConnection(myConString);
                connection.Open();
            }
            catch (Exception err)
            {

                MessageBox.Show("Error Connecting to Server\n\n" + err.Message, "Connection Test...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Sucsesfully Connected to remote server", "Conncetion Test...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            connection.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.SqlHost = txtMYSQLAddress.Text;
            Settings.SqlDb = txtMYSQLDatabase.Text;
            Settings.SqlUser = txtMYSQLUser.Text;
            Settings.SqlPass = txtMYSQLPass.Text;
            Settings.FtpHost = txtFTPAddress.Text;
            Settings.FtpUser = txtFTPUser.Text;
            Settings.FtpPass = txtFTPPass.Text;
            Settings.ProdUpload = chkProdUp.Checked;
            Settings.ProdUploadInt = Convert.ToInt32(numProdUpInterval.Text);
            Settings.ShipUpload = chkShipUp.Checked;
            Settings.OrderDownload = chkOrderDown.Checked;
            Settings.OrderDownloadInt = Convert.ToInt32(numOrderDownInterval.Text);
            Settings.OperatorId = Convert.ToInt32(txtOperator.Text);

            if (!Settings.SaveSettings())
            {
                MessageBox.Show(Settings.LastError, "Error Saving settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnSave.Enabled = false;
            _changed = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
            //Load the settings into the form
            txtMYSQLAddress.Text = Settings.SqlHost;
            txtMYSQLDatabase.Text = Settings.SqlDb;
            txtMYSQLUser.Text = Settings.SqlUser;
            txtMYSQLPass.Text = Settings.SqlPass;
            txtFTPAddress.Text = Settings.FtpHost;
            txtFTPUser.Text = Settings.FtpUser;
            txtFTPPass.Text = Settings.FtpPass;
            chkProdUp.Checked = Settings.ProdUpload;
            numProdUpInterval.Text = Settings.ProdUploadInt.ToString();
            chkShipUp.Checked = Settings.ShipUpload;
            chkOrderDown.Checked = Settings.OrderDownload;
            numOrderDownInterval.Text = Settings.OrderDownloadInt.ToString();
            txtOperator.Text = Settings.OperatorId.ToString();

            //reset the checks and save button
            _changed = false;
            btnSave.Enabled = false;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_changed)
            {
                var confirm = MessageBox.Show("Any unsaved changes will be lost!\nAre you sure you want to continue?", "Save Settings?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.No)
                    e.Cancel = true;
            }
            
            base.OnClosing(e);
        }

        #region ItemChanged functions
        private void ItemChanged(object sender, KeyPressEventArgs e)
        {
            _changed = true;
            btnSave.Enabled = true;
        }

        private void ItemChanged(object sender, MouseEventArgs e)
        {
            _changed = true;
            btnSave.Enabled = true;
        }

        private void ItemChanged(object sender, EventArgs e)
        {
            _changed = true;
            btnSave.Enabled = true;
        }
        #endregion

        private void btnLogSettings_Click(object sender, EventArgs e)
        {
            var fcl = new FrmCfgLogs();
            var result = fcl.ShowDialog();
            if (result == DialogResult.OK)
            {
                btnSave.Enabled = true;
                _changed = true;
            }

        }

    }
}