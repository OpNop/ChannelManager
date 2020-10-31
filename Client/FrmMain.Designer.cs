namespace Client
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLogSettings = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMYSQLDatabase = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtOperator = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numOrderDownInterval = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.chkOrderDown = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkShipUp = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numProdUpInterval = new System.Windows.Forms.NumericUpDown();
            this.chkProdUp = new System.Windows.Forms.CheckBox();
            this.btnTestMySQL = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMYSQLAddress = new System.Windows.Forms.TextBox();
            this.txtMYSQLPass = new System.Windows.Forms.TextBox();
            this.txtMYSQLUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTestFTP = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFTPAddress = new System.Windows.Forms.TextBox();
            this.txtFTPPass = new System.Windows.Forms.TextBox();
            this.txtFTPUser = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOrderDownInterval)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProdUpInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtFTPAddress);
            this.groupBox1.Controls.Add(this.txtFTPPass);
            this.groupBox1.Controls.Add(this.txtFTPUser);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.btnTestFTP);
            this.groupBox1.Controls.Add(this.btnLogSettings);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtMYSQLDatabase);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnTestMySQL);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtMYSQLAddress);
            this.groupBox1.Controls.Add(this.txtMYSQLPass);
            this.groupBox1.Controls.Add(this.txtMYSQLUser);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(419, 357);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PrestaShop Setup";
            // 
            // btnLogSettings
            // 
            this.btnLogSettings.Location = new System.Drawing.Point(6, 326);
            this.btnLogSettings.Name = "btnLogSettings";
            this.btnLogSettings.Size = new System.Drawing.Size(75, 23);
            this.btnLogSettings.TabIndex = 1;
            this.btnLogSettings.Text = "Log Settings";
            this.btnLogSettings.UseVisualStyleBackColor = true;
            this.btnLogSettings.Click += new System.EventHandler(this.btnLogSettings_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(338, 326);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "MySQL Database:";
            // 
            // txtMYSQLDatabase
            // 
            this.txtMYSQLDatabase.Location = new System.Drawing.Point(108, 45);
            this.txtMYSQLDatabase.Name = "txtMYSQLDatabase";
            this.txtMYSQLDatabase.Size = new System.Drawing.Size(297, 20);
            this.txtMYSQLDatabase.TabIndex = 1;
            this.txtMYSQLDatabase.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtOperator);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.numOrderDownInterval);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.chkOrderDown);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(223, 201);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(190, 119);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Order Download";
            // 
            // txtOperator
            // 
            this.txtOperator.Location = new System.Drawing.Point(83, 68);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.Size = new System.Drawing.Size(101, 20);
            this.txtOperator.TabIndex = 10;
            this.txtOperator.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Operator ID:";
            // 
            // numOrderDownInterval
            // 
            this.numOrderDownInterval.Location = new System.Drawing.Point(137, 42);
            this.numOrderDownInterval.Name = "numOrderDownInterval";
            this.numOrderDownInterval.Size = new System.Drawing.Size(47, 20);
            this.numOrderDownInterval.TabIndex = 9;
            this.numOrderDownInterval.ValueChanged += new System.EventHandler(this.ItemChanged);
            this.numOrderDownInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Download Interval  (Hrs.)";
            // 
            // chkOrderDown
            // 
            this.chkOrderDown.AutoSize = true;
            this.chkOrderDown.Location = new System.Drawing.Point(7, 20);
            this.chkOrderDown.Name = "chkOrderDown";
            this.chkOrderDown.Size = new System.Drawing.Size(143, 17);
            this.chkOrderDown.TabIndex = 8;
            this.chkOrderDown.Text = "Activate Order download";
            this.chkOrderDown.UseVisualStyleBackColor = true;
            this.chkOrderDown.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ItemChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkShipUp);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(6, 275);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(211, 45);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Shipping Confirmation Upload";
            // 
            // chkShipUp
            // 
            this.chkShipUp.AutoSize = true;
            this.chkShipUp.Location = new System.Drawing.Point(6, 19);
            this.chkShipUp.Name = "chkShipUp";
            this.chkShipUp.Size = new System.Drawing.Size(204, 17);
            this.chkShipUp.TabIndex = 7;
            this.chkShipUp.Text = "Activate Shipping confirmation upload";
            this.chkShipUp.UseVisualStyleBackColor = true;
            this.chkShipUp.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ItemChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numProdUpInterval);
            this.groupBox2.Controls.Add(this.chkProdUp);
            this.groupBox2.Location = new System.Drawing.Point(6, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 68);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Product Upload";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Upload Interval (Mins.)";
            // 
            // numProdUpInterval
            // 
            this.numProdUpInterval.Location = new System.Drawing.Point(125, 37);
            this.numProdUpInterval.Name = "numProdUpInterval";
            this.numProdUpInterval.Size = new System.Drawing.Size(54, 20);
            this.numProdUpInterval.TabIndex = 6;
            this.numProdUpInterval.ValueChanged += new System.EventHandler(this.ItemChanged);
            this.numProdUpInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // chkProdUp
            // 
            this.chkProdUp.AutoSize = true;
            this.chkProdUp.Location = new System.Drawing.Point(6, 19);
            this.chkProdUp.Name = "chkProdUp";
            this.chkProdUp.Size = new System.Drawing.Size(142, 17);
            this.chkProdUp.TabIndex = 5;
            this.chkProdUp.Text = "Activate Product Upload";
            this.chkProdUp.UseVisualStyleBackColor = true;
            this.chkProdUp.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ItemChanged);
            // 
            // btnTestMySQL
            // 
            this.btnTestMySQL.Location = new System.Drawing.Point(87, 326);
            this.btnTestMySQL.Name = "btnTestMySQL";
            this.btnTestMySQL.Size = new System.Drawing.Size(75, 23);
            this.btnTestMySQL.TabIndex = 4;
            this.btnTestMySQL.Text = "Test MySQL";
            this.btnTestMySQL.UseVisualStyleBackColor = true;
            this.btnTestMySQL.Click += new System.EventHandler(this.btnTestMySQL_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "MySQL Password:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "MySQL Address:";
            // 
            // txtMYSQLAddress
            // 
            this.txtMYSQLAddress.Location = new System.Drawing.Point(108, 19);
            this.txtMYSQLAddress.Name = "txtMYSQLAddress";
            this.txtMYSQLAddress.Size = new System.Drawing.Size(297, 20);
            this.txtMYSQLAddress.TabIndex = 0;
            this.txtMYSQLAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // txtMYSQLPass
            // 
            this.txtMYSQLPass.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.txtMYSQLPass.Location = new System.Drawing.Point(108, 97);
            this.txtMYSQLPass.Name = "txtMYSQLPass";
            this.txtMYSQLPass.PasswordChar = 'l';
            this.txtMYSQLPass.Size = new System.Drawing.Size(297, 20);
            this.txtMYSQLPass.TabIndex = 3;
            this.txtMYSQLPass.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // txtMYSQLUser
            // 
            this.txtMYSQLUser.Location = new System.Drawing.Point(108, 71);
            this.txtMYSQLUser.Name = "txtMYSQLUser";
            this.txtMYSQLUser.Size = new System.Drawing.Size(297, 20);
            this.txtMYSQLUser.TabIndex = 2;
            this.txtMYSQLUser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "MySQL Username:";
            // 
            // btnTestFTP
            // 
            this.btnTestFTP.Location = new System.Drawing.Point(168, 326);
            this.btnTestFTP.Name = "btnTestFTP";
            this.btnTestFTP.Size = new System.Drawing.Size(75, 23);
            this.btnTestFTP.TabIndex = 13;
            this.btnTestFTP.Text = "Test FTP";
            this.btnTestFTP.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 178);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "FTP Password:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 126);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "FTP Address:";
            // 
            // txtFTPAddress
            // 
            this.txtFTPAddress.Location = new System.Drawing.Point(108, 123);
            this.txtFTPAddress.Name = "txtFTPAddress";
            this.txtFTPAddress.Size = new System.Drawing.Size(297, 20);
            this.txtFTPAddress.TabIndex = 14;
            this.txtFTPAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // txtFTPPass
            // 
            this.txtFTPPass.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.txtFTPPass.Location = new System.Drawing.Point(108, 175);
            this.txtFTPPass.Name = "txtFTPPass";
            this.txtFTPPass.PasswordChar = 'l';
            this.txtFTPPass.Size = new System.Drawing.Size(297, 20);
            this.txtFTPPass.TabIndex = 17;
            this.txtFTPPass.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // txtFTPUser
            // 
            this.txtFTPUser.Location = new System.Drawing.Point(108, 149);
            this.txtFTPUser.Name = "txtFTPUser";
            this.txtFTPUser.Size = new System.Drawing.Size(297, 20);
            this.txtFTPUser.TabIndex = 16;
            this.txtFTPUser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 152);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "FTP Username:";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 381);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "MCM Settings";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOrderDownInterval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProdUpInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMYSQLAddress;
        private System.Windows.Forms.TextBox txtMYSQLPass;
        private System.Windows.Forms.TextBox txtMYSQLUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numProdUpInterval;
        private System.Windows.Forms.CheckBox chkProdUp;
        private System.Windows.Forms.Button btnTestMySQL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkOrderDown;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkShipUp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numOrderDownInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMYSQLDatabase;
        private System.Windows.Forms.Button btnLogSettings;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtFTPAddress;
        private System.Windows.Forms.TextBox txtFTPPass;
        private System.Windows.Forms.TextBox txtFTPUser;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnTestFTP;
    }
}

