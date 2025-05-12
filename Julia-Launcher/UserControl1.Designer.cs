namespace Julia_Launcher
{
    partial class UserControl1
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            btnApply = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            label1 = new System.Windows.Forms.Label();
            txtLogDirectory = new System.Windows.Forms.TextBox();
            txtInstallDirectory = new System.Windows.Forms.TextBox();
            lblExternalModulesPath = new System.Windows.Forms.Label();
            lbl = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            lblInstallationDirectory = new System.Windows.Forms.Label();
            btnSelectModulesDirectory = new System.Windows.Forms.Button();
            btnSelectInstallDirectory = new System.Windows.Forms.Button();
            txtModulesDirectory = new System.Windows.Forms.TextBox();
            btnSelectCacheDirectory = new System.Windows.Forms.Button();
            txtCacheDirectory = new System.Windows.Forms.TextBox();
            btnSelectLogDirectory = new System.Windows.Forms.Button();
            tabPage2 = new System.Windows.Forms.TabPage();
            label7 = new System.Windows.Forms.Label();
            lblMaxSpeed = new System.Windows.Forms.Label();
            lblSelectGPU = new System.Windows.Forms.Label();
            lblLimitGPU = new System.Windows.Forms.Label();
            lblGPUAcceleration = new System.Windows.Forms.Label();
            lblCPULoad = new System.Windows.Forms.Label();
            lblNumberCores = new System.Windows.Forms.Label();
            lblCPULimit = new System.Windows.Forms.Label();
            lblRAMLimit = new System.Windows.Forms.Label();
            txtNetworkSpeed = new System.Windows.Forms.TextBox();
            trackRamUsage = new System.Windows.Forms.TrackBar();
            cmbGpuSelection = new System.Windows.Forms.ComboBox();
            cmbCpuCores = new System.Windows.Forms.ComboBox();
            chkGPUEnable = new System.Windows.Forms.CheckBox();
            txtCPULimit = new System.Windows.Forms.TextBox();
            txtGPULimit = new System.Windows.Forms.TextBox();
            txtCpuLoad = new System.Windows.Forms.TextBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            chkUpdateSrartup = new System.Windows.Forms.CheckBox();
            chkManUpdate = new System.Windows.Forms.CheckBox();
            label16 = new System.Windows.Forms.Label();
            label19 = new System.Windows.Forms.Label();
            label20 = new System.Windows.Forms.Label();
            label18 = new System.Windows.Forms.Label();
            label21 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            label22 = new System.Windows.Forms.Label();
            cmbUpdateBranch = new System.Windows.Forms.ComboBox();
            chkAutoUpdate = new System.Windows.Forms.CheckBox();
            chkUpdPreferen = new System.Windows.Forms.CheckBox();
            chkAutoStart = new System.Windows.Forms.CheckBox();
            tabPage4 = new System.Windows.Forms.TabPage();
            cmbDebugging = new System.Windows.Forms.ComboBox();
            cmbInfoMassages = new System.Windows.Forms.ComboBox();
            chkLogRetention = new System.Windows.Forms.CheckBox();
            cmbLogFormat = new System.Windows.Forms.ComboBox();
            label23 = new System.Windows.Forms.Label();
            label24 = new System.Windows.Forms.Label();
            cmbErrors = new System.Windows.Forms.ComboBox();
            lblErrors = new System.Windows.Forms.Label();
            cmbLogLevel = new System.Windows.Forms.ComboBox();
            lblWarnings = new System.Windows.Forms.Label();
            lblInfoMessag = new System.Windows.Forms.Label();
            label28 = new System.Windows.Forms.Label();
            label29 = new System.Windows.Forms.Label();
            cmbWarnings = new System.Windows.Forms.ComboBox();
            label30 = new System.Windows.Forms.Label();
            tabPage5 = new System.Windows.Forms.TabPage();
            radSystem = new System.Windows.Forms.RadioButton();
            txtHotkeyLounch = new System.Windows.Forms.TextBox();
            label31 = new System.Windows.Forms.Label();
            label32 = new System.Windows.Forms.Label();
            radDark = new System.Windows.Forms.RadioButton();
            label33 = new System.Windows.Forms.Label();
            radWhite = new System.Windows.Forms.RadioButton();
            label34 = new System.Windows.Forms.Label();
            cmbLanguage = new System.Windows.Forms.ComboBox();
            tabPage6 = new System.Windows.Forms.TabPage();
            label35 = new System.Windows.Forms.Label();
            label36 = new System.Windows.Forms.Label();
            label37 = new System.Windows.Forms.Label();
            chkAllowedIPAddresses = new System.Windows.Forms.CheckBox();
            label38 = new System.Windows.Forms.Label();
            chkProtectionWithaPassword = new System.Windows.Forms.CheckBox();
            chkEncryptLogs = new System.Windows.Forms.CheckBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackRamUsage).BeginInit();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            tabPage6.SuspendLayout();
            SuspendLayout();
            // 
            // btnApply
            // 
            btnApply.Location = new System.Drawing.Point(1079, 599);
            btnApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnApply.Name = "btnApply";
            btnApply.Size = new System.Drawing.Size(88, 27);
            btnApply.TabIndex = 130;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(890, 599);
            btnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(88, 27);
            btnSave.TabIndex = 129;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(985, 599);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 27);
            btnCancel.TabIndex = 128;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Controls.Add(tabPage6);
            tabControl1.Location = new System.Drawing.Point(4, 3);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1163, 592);
            tabControl1.TabIndex = 127;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(txtLogDirectory);
            tabPage1.Controls.Add(txtInstallDirectory);
            tabPage1.Controls.Add(lblExternalModulesPath);
            tabPage1.Controls.Add(lbl);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(lblInstallationDirectory);
            tabPage1.Controls.Add(btnSelectModulesDirectory);
            tabPage1.Controls.Add(btnSelectInstallDirectory);
            tabPage1.Controls.Add(txtModulesDirectory);
            tabPage1.Controls.Add(btnSelectCacheDirectory);
            tabPage1.Controls.Add(txtCacheDirectory);
            tabPage1.Controls.Add(btnSelectLogDirectory);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Size = new System.Drawing.Size(1155, 564);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "File and Folder Paths";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(4, 3);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(127, 13);
            label1.TabIndex = 50;
            label1.Text = "File and Folder Paths";
            // 
            // txtLogDirectory
            // 
            txtLogDirectory.Location = new System.Drawing.Point(166, 78);
            txtLogDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtLogDirectory.Name = "txtLogDirectory";
            txtLogDirectory.ReadOnly = true;
            txtLogDirectory.Size = new System.Drawing.Size(288, 23);
            txtLogDirectory.TabIndex = 78;
            txtLogDirectory.TextChanged += txtLogDirectory_TextChanged;
            // 
            // txtInstallDirectory
            // 
            txtInstallDirectory.Location = new System.Drawing.Point(166, 42);
            txtInstallDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtInstallDirectory.Name = "txtInstallDirectory";
            txtInstallDirectory.ReadOnly = true;
            txtInstallDirectory.Size = new System.Drawing.Size(288, 23);
            txtInstallDirectory.TabIndex = 64;
            txtInstallDirectory.TextChanged += txtInstallDirectory_TextChanged;
            // 
            // lblExternalModulesPath
            // 
            lblExternalModulesPath.AutoSize = true;
            lblExternalModulesPath.ForeColor = System.Drawing.Color.White;
            lblExternalModulesPath.Location = new System.Drawing.Point(33, 119);
            lblExternalModulesPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblExternalModulesPath.Name = "lblExternalModulesPath";
            lblExternalModulesPath.Size = new System.Drawing.Size(124, 15);
            lblExternalModulesPath.TabIndex = 60;
            lblExternalModulesPath.Text = "External Modules Path";
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.ForeColor = System.Drawing.Color.White;
            lbl.Location = new System.Drawing.Point(33, 156);
            lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl.Name = "lbl";
            lbl.Size = new System.Drawing.Size(94, 15);
            lbl.TabIndex = 59;
            lbl.Text = "Cache Directory ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.Color.White;
            label4.Location = new System.Drawing.Point(33, 82);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(63, 15);
            label4.TabIndex = 58;
            label4.Text = "Log Folder";
            // 
            // lblInstallationDirectory
            // 
            lblInstallationDirectory.AutoSize = true;
            lblInstallationDirectory.ForeColor = System.Drawing.Color.White;
            lblInstallationDirectory.Location = new System.Drawing.Point(33, 45);
            lblInstallationDirectory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInstallationDirectory.Name = "lblInstallationDirectory";
            lblInstallationDirectory.Size = new System.Drawing.Size(116, 15);
            lblInstallationDirectory.TabIndex = 57;
            lblInstallationDirectory.Text = "Installation Directory";
            // 
            // btnSelectModulesDirectory
            // 
            btnSelectModulesDirectory.Location = new System.Drawing.Point(462, 115);
            btnSelectModulesDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSelectModulesDirectory.Name = "btnSelectModulesDirectory";
            btnSelectModulesDirectory.Size = new System.Drawing.Size(88, 27);
            btnSelectModulesDirectory.TabIndex = 119;
            btnSelectModulesDirectory.Text = "Обзор";
            btnSelectModulesDirectory.UseVisualStyleBackColor = true;
            btnSelectModulesDirectory.Click += btnSelectModulesDirectory_Click;
            // 
            // btnSelectInstallDirectory
            // 
            btnSelectInstallDirectory.Location = new System.Drawing.Point(462, 42);
            btnSelectInstallDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSelectInstallDirectory.Name = "btnSelectInstallDirectory";
            btnSelectInstallDirectory.Size = new System.Drawing.Size(88, 27);
            btnSelectInstallDirectory.TabIndex = 51;
            btnSelectInstallDirectory.Text = "Обзор";
            btnSelectInstallDirectory.UseVisualStyleBackColor = true;
            btnSelectInstallDirectory.Click += btnSelectInstallDirectory_Click;
            // 
            // txtModulesDirectory
            // 
            txtModulesDirectory.Location = new System.Drawing.Point(166, 115);
            txtModulesDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtModulesDirectory.Name = "txtModulesDirectory";
            txtModulesDirectory.ReadOnly = true;
            txtModulesDirectory.Size = new System.Drawing.Size(288, 23);
            txtModulesDirectory.TabIndex = 118;
            txtModulesDirectory.TextChanged += txtModulesDirectory_TextChanged;
            // 
            // btnSelectCacheDirectory
            // 
            btnSelectCacheDirectory.Location = new System.Drawing.Point(462, 149);
            btnSelectCacheDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSelectCacheDirectory.Name = "btnSelectCacheDirectory";
            btnSelectCacheDirectory.Size = new System.Drawing.Size(88, 27);
            btnSelectCacheDirectory.TabIndex = 117;
            btnSelectCacheDirectory.Text = "Обзор";
            btnSelectCacheDirectory.UseVisualStyleBackColor = true;
            btnSelectCacheDirectory.Click += btnSelectCacheDirectory_Click;
            // 
            // txtCacheDirectory
            // 
            txtCacheDirectory.Location = new System.Drawing.Point(166, 152);
            txtCacheDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCacheDirectory.Name = "txtCacheDirectory";
            txtCacheDirectory.ReadOnly = true;
            txtCacheDirectory.Size = new System.Drawing.Size(288, 23);
            txtCacheDirectory.TabIndex = 99;
            txtCacheDirectory.TextChanged += txtCacheDirectory_TextChanged;
            // 
            // btnSelectLogDirectory
            // 
            btnSelectLogDirectory.Location = new System.Drawing.Point(462, 78);
            btnSelectLogDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSelectLogDirectory.Name = "btnSelectLogDirectory";
            btnSelectLogDirectory.Size = new System.Drawing.Size(88, 27);
            btnSelectLogDirectory.TabIndex = 116;
            btnSelectLogDirectory.Text = "Обзор";
            btnSelectLogDirectory.UseVisualStyleBackColor = true;
            btnSelectLogDirectory.Click += btnSelectLogDirectory_Click;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(lblMaxSpeed);
            tabPage2.Controls.Add(lblSelectGPU);
            tabPage2.Controls.Add(lblLimitGPU);
            tabPage2.Controls.Add(lblGPUAcceleration);
            tabPage2.Controls.Add(lblCPULoad);
            tabPage2.Controls.Add(lblNumberCores);
            tabPage2.Controls.Add(lblCPULimit);
            tabPage2.Controls.Add(lblRAMLimit);
            tabPage2.Controls.Add(txtNetworkSpeed);
            tabPage2.Controls.Add(trackRamUsage);
            tabPage2.Controls.Add(cmbGpuSelection);
            tabPage2.Controls.Add(cmbCpuCores);
            tabPage2.Controls.Add(chkGPUEnable);
            tabPage2.Controls.Add(txtCPULimit);
            tabPage2.Controls.Add(txtGPULimit);
            tabPage2.Controls.Add(txtCpuLoad);
            tabPage2.ForeColor = System.Drawing.Color.White;
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage2.Size = new System.Drawing.Size(1155, 564);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Resource Allocation";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label7.Location = new System.Drawing.Point(4, 3);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(121, 13);
            label7.TabIndex = 61;
            label7.Text = "Resource Allocation";
            // 
            // lblMaxSpeed
            // 
            lblMaxSpeed.AutoSize = true;
            lblMaxSpeed.ForeColor = System.Drawing.Color.White;
            lblMaxSpeed.Location = new System.Drawing.Point(33, 303);
            lblMaxSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMaxSpeed.Name = "lblMaxSpeed";
            lblMaxSpeed.Size = new System.Drawing.Size(196, 15);
            lblMaxSpeed.TabIndex = 70;
            lblMaxSpeed.Text = "Maximum download/upload speed.";
            // 
            // lblSelectGPU
            // 
            lblSelectGPU.AutoSize = true;
            lblSelectGPU.ForeColor = System.Drawing.Color.White;
            lblSelectGPU.Location = new System.Drawing.Point(33, 267);
            lblSelectGPU.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSelectGPU.Name = "lblSelectGPU";
            lblSelectGPU.Size = new System.Drawing.Size(244, 15);
            lblSelectGPU.TabIndex = 69;
            lblSelectGPU.Text = "Select a specific GPU if multiple are available.";
            // 
            // lblLimitGPU
            // 
            lblLimitGPU.AutoSize = true;
            lblLimitGPU.ForeColor = System.Drawing.Color.White;
            lblLimitGPU.Location = new System.Drawing.Point(33, 230);
            lblLimitGPU.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLimitGPU.Name = "lblLimitGPU";
            lblLimitGPU.Size = new System.Drawing.Size(208, 15);
            lblLimitGPU.TabIndex = 68;
            lblLimitGPU.Text = "Limit GPU usage (e.g., by percentage).";
            // 
            // lblGPUAcceleration
            // 
            lblGPUAcceleration.AutoSize = true;
            lblGPUAcceleration.ForeColor = System.Drawing.Color.White;
            lblGPUAcceleration.Location = new System.Drawing.Point(33, 193);
            lblGPUAcceleration.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblGPUAcceleration.Name = "lblGPUAcceleration";
            lblGPUAcceleration.Size = new System.Drawing.Size(180, 15);
            lblGPUAcceleration.TabIndex = 67;
            lblGPUAcceleration.Text = "Enable/disable GPU acceleration.";
            // 
            // lblCPULoad
            // 
            lblCPULoad.AutoSize = true;
            lblCPULoad.ForeColor = System.Drawing.Color.White;
            lblCPULoad.Location = new System.Drawing.Point(33, 156);
            lblCPULoad.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblCPULoad.Name = "lblCPULoad";
            lblCPULoad.Size = new System.Drawing.Size(121, 15);
            lblCPULoad.TabIndex = 66;
            lblCPULoad.Text = "CPU load percentage.";
            // 
            // lblNumberCores
            // 
            lblNumberCores.AutoSize = true;
            lblNumberCores.ForeColor = System.Drawing.Color.White;
            lblNumberCores.Location = new System.Drawing.Point(33, 119);
            lblNumberCores.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNumberCores.Name = "lblNumberCores";
            lblNumberCores.Size = new System.Drawing.Size(160, 15);
            lblNumberCores.TabIndex = 65;
            lblNumberCores.Text = "Number of CPU cores to use.";
            // 
            // lblCPULimit
            // 
            lblCPULimit.AutoSize = true;
            lblCPULimit.ForeColor = System.Drawing.Color.White;
            lblCPULimit.Location = new System.Drawing.Point(33, 82);
            lblCPULimit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblCPULimit.Name = "lblCPULimit";
            lblCPULimit.Size = new System.Drawing.Size(116, 15);
            lblCPULimit.TabIndex = 63;
            lblCPULimit.Text = "CPU Load Limitation";
            // 
            // lblRAMLimit
            // 
            lblRAMLimit.AutoSize = true;
            lblRAMLimit.ForeColor = System.Drawing.Color.White;
            lblRAMLimit.Location = new System.Drawing.Point(33, 45);
            lblRAMLimit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblRAMLimit.Name = "lblRAMLimit";
            lblRAMLimit.Size = new System.Drawing.Size(68, 15);
            lblRAMLimit.TabIndex = 62;
            lblRAMLimit.Text = "RAM Usage";
            // 
            // txtNetworkSpeed
            // 
            txtNetworkSpeed.Location = new System.Drawing.Point(252, 301);
            txtNetworkSpeed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtNetworkSpeed.Name = "txtNetworkSpeed";
            txtNetworkSpeed.Size = new System.Drawing.Size(116, 23);
            txtNetworkSpeed.TabIndex = 113;
            txtNetworkSpeed.TextChanged += txtNetworkSpeed_TextChanged;
            // 
            // trackRamUsage
            // 
            trackRamUsage.Location = new System.Drawing.Point(109, 27);
            trackRamUsage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackRamUsage.Maximum = 999999;
            trackRamUsage.Minimum = 4096;
            trackRamUsage.Name = "trackRamUsage";
            trackRamUsage.Size = new System.Drawing.Size(288, 45);
            trackRamUsage.TabIndex = 100;
            trackRamUsage.Value = 4096;
            // 
            // cmbGpuSelection
            // 
            cmbGpuSelection.FormattingEnabled = true;
            cmbGpuSelection.Items.AddRange(new object[] { "Auto", "GPU 0", "Default", "None" });
            cmbGpuSelection.Location = new System.Drawing.Point(300, 263);
            cmbGpuSelection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbGpuSelection.Name = "cmbGpuSelection";
            cmbGpuSelection.Size = new System.Drawing.Size(240, 23);
            cmbGpuSelection.TabIndex = 112;
            // 
            // cmbCpuCores
            // 
            cmbCpuCores.FormattingEnabled = true;
            cmbCpuCores.Location = new System.Drawing.Point(209, 115);
            cmbCpuCores.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbCpuCores.Name = "cmbCpuCores";
            cmbCpuCores.Size = new System.Drawing.Size(140, 23);
            cmbCpuCores.TabIndex = 101;
            // 
            // chkGPUEnable
            // 
            chkGPUEnable.AutoSize = true;
            chkGPUEnable.Location = new System.Drawing.Point(221, 194);
            chkGPUEnable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkGPUEnable.Name = "chkGPUEnable";
            chkGPUEnable.Size = new System.Drawing.Size(15, 14);
            chkGPUEnable.TabIndex = 111;
            chkGPUEnable.UseVisualStyleBackColor = true;
            // 
            // txtCPULimit
            // 
            txtCPULimit.Location = new System.Drawing.Point(160, 78);
            txtCPULimit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCPULimit.Name = "txtCPULimit";
            txtCPULimit.Size = new System.Drawing.Size(116, 23);
            txtCPULimit.TabIndex = 108;
            txtCPULimit.TextChanged += textBox4_TextChanged;
            // 
            // txtGPULimit
            // 
            txtGPULimit.Location = new System.Drawing.Point(261, 226);
            txtGPULimit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtGPULimit.Name = "txtGPULimit";
            txtGPULimit.Size = new System.Drawing.Size(116, 23);
            txtGPULimit.TabIndex = 110;
            txtGPULimit.TextChanged += txtGPULimit_TextChanged;
            // 
            // txtCpuLoad
            // 
            txtCpuLoad.Location = new System.Drawing.Point(209, 152);
            txtCpuLoad.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCpuLoad.Name = "txtCpuLoad";
            txtCpuLoad.Size = new System.Drawing.Size(116, 23);
            txtCpuLoad.TabIndex = 109;
            // 
            // tabPage3
            // 
            tabPage3.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            tabPage3.Controls.Add(chkUpdateSrartup);
            tabPage3.Controls.Add(chkManUpdate);
            tabPage3.Controls.Add(label16);
            tabPage3.Controls.Add(label19);
            tabPage3.Controls.Add(label20);
            tabPage3.Controls.Add(label18);
            tabPage3.Controls.Add(label21);
            tabPage3.Controls.Add(label17);
            tabPage3.Controls.Add(label22);
            tabPage3.Controls.Add(cmbUpdateBranch);
            tabPage3.Controls.Add(chkAutoUpdate);
            tabPage3.Controls.Add(chkUpdPreferen);
            tabPage3.Controls.Add(chkAutoStart);
            tabPage3.Location = new System.Drawing.Point(4, 24);
            tabPage3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage3.Size = new System.Drawing.Size(1155, 564);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Startup and Update Settings";
            // 
            // chkUpdateSrartup
            // 
            chkUpdateSrartup.AutoSize = true;
            chkUpdateSrartup.Location = new System.Drawing.Point(203, 157);
            chkUpdateSrartup.Name = "chkUpdateSrartup";
            chkUpdateSrartup.Size = new System.Drawing.Size(15, 14);
            chkUpdateSrartup.TabIndex = 105;
            chkUpdateSrartup.UseVisualStyleBackColor = true;
            // 
            // chkManUpdate
            // 
            chkManUpdate.AutoSize = true;
            chkManUpdate.Location = new System.Drawing.Point(132, 194);
            chkManUpdate.Name = "chkManUpdate";
            chkManUpdate.Size = new System.Drawing.Size(15, 14);
            chkManUpdate.TabIndex = 104;
            chkManUpdate.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label16.ForeColor = System.Drawing.Color.White;
            label16.Location = new System.Drawing.Point(4, 3);
            label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(168, 13);
            label16.TabIndex = 71;
            label16.Text = "Startup and Update Settings";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.ForeColor = System.Drawing.Color.White;
            label19.Location = new System.Drawing.Point(33, 119);
            label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(111, 15);
            label19.TabIndex = 74;
            label19.Text = "Automatic updates.";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.ForeColor = System.Drawing.Color.White;
            label20.Location = new System.Drawing.Point(33, 156);
            label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(163, 15);
            label20.TabIndex = 75;
            label20.Text = "Check for updates on startup.";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.ForeColor = System.Drawing.Color.White;
            label18.Location = new System.Drawing.Point(33, 82);
            label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(109, 15);
            label18.TabIndex = 73;
            label18.Text = "Update Preferences";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.ForeColor = System.Drawing.Color.White;
            label21.Location = new System.Drawing.Point(33, 193);
            label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(92, 15);
            label21.TabIndex = 76;
            label21.Text = "Manual updates";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.ForeColor = System.Drawing.Color.White;
            label17.Location = new System.Drawing.Point(33, 45);
            label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(62, 15);
            label17.TabIndex = 72;
            label17.Text = "Auto-Start";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.ForeColor = System.Drawing.Color.White;
            label22.Location = new System.Drawing.Point(33, 230);
            label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(247, 15);
            label22.TabIndex = 77;
            label22.Text = "Choose update branch (Stable, Beta, Nightly).";
            // 
            // cmbUpdateBranch
            // 
            cmbUpdateBranch.FormattingEnabled = true;
            cmbUpdateBranch.Items.AddRange(new object[] { "Stable", "Beta", "Nightl" });
            cmbUpdateBranch.Location = new System.Drawing.Point(300, 226);
            cmbUpdateBranch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbUpdateBranch.Name = "cmbUpdateBranch";
            cmbUpdateBranch.Size = new System.Drawing.Size(140, 23);
            cmbUpdateBranch.TabIndex = 96;
            // 
            // chkAutoUpdate
            // 
            chkAutoUpdate.AutoSize = true;
            chkAutoUpdate.Location = new System.Drawing.Point(152, 118);
            chkAutoUpdate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkAutoUpdate.Name = "chkAutoUpdate";
            chkAutoUpdate.Size = new System.Drawing.Size(15, 14);
            chkAutoUpdate.TabIndex = 98;
            chkAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // chkUpdPreferen
            // 
            chkUpdPreferen.AutoSize = true;
            chkUpdPreferen.Location = new System.Drawing.Point(150, 83);
            chkUpdPreferen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkUpdPreferen.Name = "chkUpdPreferen";
            chkUpdPreferen.Size = new System.Drawing.Size(15, 14);
            chkUpdPreferen.TabIndex = 102;
            chkUpdPreferen.UseVisualStyleBackColor = true;
            // 
            // chkAutoStart
            // 
            chkAutoStart.AutoSize = true;
            chkAutoStart.Location = new System.Drawing.Point(103, 46);
            chkAutoStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkAutoStart.Name = "chkAutoStart";
            chkAutoStart.Size = new System.Drawing.Size(15, 14);
            chkAutoStart.TabIndex = 103;
            chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            tabPage4.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            tabPage4.Controls.Add(cmbDebugging);
            tabPage4.Controls.Add(cmbInfoMassages);
            tabPage4.Controls.Add(chkLogRetention);
            tabPage4.Controls.Add(cmbLogFormat);
            tabPage4.Controls.Add(label23);
            tabPage4.Controls.Add(label24);
            tabPage4.Controls.Add(cmbErrors);
            tabPage4.Controls.Add(lblErrors);
            tabPage4.Controls.Add(cmbLogLevel);
            tabPage4.Controls.Add(lblWarnings);
            tabPage4.Controls.Add(lblInfoMessag);
            tabPage4.Controls.Add(label28);
            tabPage4.Controls.Add(label29);
            tabPage4.Controls.Add(cmbWarnings);
            tabPage4.Controls.Add(label30);
            tabPage4.Location = new System.Drawing.Point(4, 24);
            tabPage4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage4.Size = new System.Drawing.Size(1155, 564);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Logging and Debugging";
            tabPage4.Click += tabPage4_Click;
            // 
            // cmbDebugging
            // 
            cmbDebugging.FormattingEnabled = true;
            cmbDebugging.Items.AddRange(new object[] { "verbose", "info", "error" });
            cmbDebugging.Location = new System.Drawing.Point(140, 190);
            cmbDebugging.Name = "cmbDebugging";
            cmbDebugging.Size = new System.Drawing.Size(140, 23);
            cmbDebugging.TabIndex = 128;
            // 
            // cmbInfoMassages
            // 
            cmbInfoMassages.FormattingEnabled = true;
            cmbInfoMassages.Items.AddRange(new object[] { "All", "Basic", "Detailed", "Events", "Status", "None" });
            cmbInfoMassages.Location = new System.Drawing.Point(176, 153);
            cmbInfoMassages.Name = "cmbInfoMassages";
            cmbInfoMassages.Size = new System.Drawing.Size(104, 23);
            cmbInfoMassages.TabIndex = 127;
            // 
            // chkLogRetention
            // 
            chkLogRetention.AutoSize = true;
            chkLogRetention.Location = new System.Drawing.Point(121, 266);
            chkLogRetention.Name = "chkLogRetention";
            chkLogRetention.Size = new System.Drawing.Size(15, 14);
            chkLogRetention.TabIndex = 126;
            chkLogRetention.UseVisualStyleBackColor = true;
            // 
            // cmbLogFormat
            // 
            cmbLogFormat.FormattingEnabled = true;
            cmbLogFormat.Items.AddRange(new object[] { ".txt ", ".json ", ".etc" });
            cmbLogFormat.Location = new System.Drawing.Point(202, 227);
            cmbLogFormat.Name = "cmbLogFormat";
            cmbLogFormat.Size = new System.Drawing.Size(121, 23);
            cmbLogFormat.TabIndex = 125;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label23.ForeColor = System.Drawing.Color.White;
            label23.Location = new System.Drawing.Point(4, 3);
            label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(142, 13);
            label23.TabIndex = 79;
            label23.Text = "Logging and Debugging";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.ForeColor = System.Drawing.Color.White;
            label24.Location = new System.Drawing.Point(33, 45);
            label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(90, 15);
            label24.TabIndex = 80;
            label24.Text = "Log Detail Level";
            // 
            // cmbErrors
            // 
            cmbErrors.FormattingEnabled = true;
            cmbErrors.Items.AddRange(new object[] { "All", "Fatal", "Runtime", "Connection", "FileSystem", "Validation", "Recoverable", "None" });
            cmbErrors.Location = new System.Drawing.Point(140, 78);
            cmbErrors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbErrors.Name = "cmbErrors";
            cmbErrors.Size = new System.Drawing.Size(140, 23);
            cmbErrors.TabIndex = 124;
            cmbErrors.SelectedIndexChanged += cmbErrors_SelectedIndexChanged;
            // 
            // lblErrors
            // 
            lblErrors.AutoSize = true;
            lblErrors.ForeColor = System.Drawing.Color.White;
            lblErrors.Location = new System.Drawing.Point(33, 82);
            lblErrors.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblErrors.Name = "lblErrors";
            lblErrors.Size = new System.Drawing.Size(40, 15);
            lblErrors.TabIndex = 81;
            lblErrors.Text = "Errors.";
            // 
            // cmbLogLevel
            // 
            cmbLogLevel.FormattingEnabled = true;
            cmbLogLevel.Items.AddRange(new object[] { "Errors", "Warnings", "ALL" });
            cmbLogLevel.Location = new System.Drawing.Point(140, 40);
            cmbLogLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbLogLevel.Name = "cmbLogLevel";
            cmbLogLevel.Size = new System.Drawing.Size(140, 23);
            cmbLogLevel.TabIndex = 123;
            cmbLogLevel.SelectedIndexChanged += cmbLogLevel_SelectedIndexChanged;
            // 
            // lblWarnings
            // 
            lblWarnings.AutoSize = true;
            lblWarnings.ForeColor = System.Drawing.Color.White;
            lblWarnings.Location = new System.Drawing.Point(33, 119);
            lblWarnings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblWarnings.Name = "lblWarnings";
            lblWarnings.Size = new System.Drawing.Size(60, 15);
            lblWarnings.TabIndex = 82;
            lblWarnings.Text = "Warnings.";
            // 
            // lblInfoMessag
            // 
            lblInfoMessag.AutoSize = true;
            lblInfoMessag.ForeColor = System.Drawing.Color.White;
            lblInfoMessag.Location = new System.Drawing.Point(33, 156);
            lblInfoMessag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInfoMessag.Name = "lblInfoMessag";
            lblInfoMessag.Size = new System.Drawing.Size(136, 15);
            lblInfoMessag.TabIndex = 83;
            lblInfoMessag.Text = "Informational messages.";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.ForeColor = System.Drawing.Color.White;
            label28.Location = new System.Drawing.Point(33, 193);
            label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label28.Name = "label28";
            label28.Size = new System.Drawing.Size(69, 15);
            label28.TabIndex = 84;
            label28.Text = "Debugging.";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.ForeColor = System.Drawing.Color.White;
            label29.Location = new System.Drawing.Point(33, 230);
            label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label29.Name = "label29";
            label29.Size = new System.Drawing.Size(162, 15);
            label29.TabIndex = 85;
            label29.Text = "Log Format (TXT, JSON, etc.).";
            // 
            // cmbWarnings
            // 
            cmbWarnings.FormattingEnabled = true;
            cmbWarnings.Items.AddRange(new object[] { "All", "Critical", "Performance", "Configuration", "Security", "Minor", "None" });
            cmbWarnings.Location = new System.Drawing.Point(140, 115);
            cmbWarnings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbWarnings.Name = "cmbWarnings";
            cmbWarnings.Size = new System.Drawing.Size(140, 23);
            cmbWarnings.TabIndex = 107;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.ForeColor = System.Drawing.Color.White;
            label30.Location = new System.Drawing.Point(33, 267);
            label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label30.Name = "label30";
            label30.Size = new System.Drawing.Size(81, 15);
            label30.TabIndex = 86;
            label30.Text = "Log Retention";
            // 
            // tabPage5
            // 
            tabPage5.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            tabPage5.Controls.Add(radSystem);
            tabPage5.Controls.Add(txtHotkeyLounch);
            tabPage5.Controls.Add(label31);
            tabPage5.Controls.Add(label32);
            tabPage5.Controls.Add(radDark);
            tabPage5.Controls.Add(label33);
            tabPage5.Controls.Add(radWhite);
            tabPage5.Controls.Add(label34);
            tabPage5.Controls.Add(cmbLanguage);
            tabPage5.Location = new System.Drawing.Point(4, 24);
            tabPage5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage5.Size = new System.Drawing.Size(1155, 564);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "UI and User Preferences";
            // 
            // radSystem
            // 
            radSystem.AutoSize = true;
            radSystem.Location = new System.Drawing.Point(167, 46);
            radSystem.Name = "radSystem";
            radSystem.Size = new System.Drawing.Size(14, 13);
            radSystem.TabIndex = 124;
            radSystem.TabStop = true;
            radSystem.UseVisualStyleBackColor = true;
            // 
            // txtHotkeyLounch
            // 
            txtHotkeyLounch.Location = new System.Drawing.Point(159, 116);
            txtHotkeyLounch.Name = "txtHotkeyLounch";
            txtHotkeyLounch.Size = new System.Drawing.Size(374, 23);
            txtHotkeyLounch.TabIndex = 123;
            txtHotkeyLounch.TextChanged += txtHotkeyLounch_TextChanged;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label31.ForeColor = System.Drawing.Color.White;
            label31.Location = new System.Drawing.Point(4, 3);
            label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label31.Name = "label31";
            label31.Size = new System.Drawing.Size(147, 13);
            label31.TabIndex = 87;
            label31.Text = "UI and User Preferences";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.ForeColor = System.Drawing.Color.White;
            label32.Location = new System.Drawing.Point(33, 45);
            label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label32.Name = "label32";
            label32.Size = new System.Drawing.Size(47, 15);
            label32.TabIndex = 88;
            label32.Text = "Theme ";
            // 
            // radDark
            // 
            radDark.AutoSize = true;
            radDark.Location = new System.Drawing.Point(146, 46);
            radDark.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radDark.Name = "radDark";
            radDark.Size = new System.Drawing.Size(14, 13);
            radDark.TabIndex = 122;
            radDark.TabStop = true;
            radDark.UseVisualStyleBackColor = true;
            radDark.CheckedChanged += radioButton2_CheckedChanged_1;
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.ForeColor = System.Drawing.Color.White;
            label33.Location = new System.Drawing.Point(33, 82);
            label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label33.Name = "label33";
            label33.Size = new System.Drawing.Size(110, 15);
            label33.TabIndex = 89;
            label33.Text = "Language Selection";
            // 
            // radWhite
            // 
            radWhite.AutoSize = true;
            radWhite.Location = new System.Drawing.Point(124, 46);
            radWhite.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radWhite.Name = "radWhite";
            radWhite.Size = new System.Drawing.Size(14, 13);
            radWhite.TabIndex = 121;
            radWhite.TabStop = true;
            radWhite.UseVisualStyleBackColor = true;
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.ForeColor = System.Drawing.Color.White;
            label34.Location = new System.Drawing.Point(33, 119);
            label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label34.Name = "label34";
            label34.Size = new System.Drawing.Size(50, 15);
            label34.TabIndex = 90;
            label34.Text = "Hotkeys";
            // 
            // cmbLanguage
            // 
            cmbLanguage.FormattingEnabled = true;
            cmbLanguage.Items.AddRange(new object[] { "English", "Русский", "Українська " });
            cmbLanguage.Location = new System.Drawing.Point(159, 78);
            cmbLanguage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.Size = new System.Drawing.Size(140, 23);
            cmbLanguage.TabIndex = 95;
            // 
            // tabPage6
            // 
            tabPage6.BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            tabPage6.Controls.Add(label35);
            tabPage6.Controls.Add(label36);
            tabPage6.Controls.Add(label37);
            tabPage6.Controls.Add(chkAllowedIPAddresses);
            tabPage6.Controls.Add(label38);
            tabPage6.Controls.Add(chkProtectionWithaPassword);
            tabPage6.Controls.Add(chkEncryptLogs);
            tabPage6.Location = new System.Drawing.Point(4, 24);
            tabPage6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage6.Name = "tabPage6";
            tabPage6.Size = new System.Drawing.Size(1155, 564);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "Security";
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label35.ForeColor = System.Drawing.Color.White;
            label35.Location = new System.Drawing.Point(4, 3);
            label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label35.Name = "label35";
            label35.Size = new System.Drawing.Size(53, 13);
            label35.TabIndex = 91;
            label35.Text = "Security";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.ForeColor = System.Drawing.Color.White;
            label36.Location = new System.Drawing.Point(33, 45);
            label36.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label36.Name = "label36";
            label36.Size = new System.Drawing.Size(200, 15);
            label36.TabIndex = 92;
            label36.Text = "Log and Data Encryption (if needed).";
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.ForeColor = System.Drawing.Color.White;
            label37.Location = new System.Drawing.Point(33, 82);
            label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label37.Name = "label37";
            label37.Size = new System.Drawing.Size(367, 15);
            label37.TabIndex = 93;
            label37.Text = "Settings Protection with a Password (to prevent accidental changes).";
            // 
            // chkAllowedIPAddresses
            // 
            chkAllowedIPAddresses.AutoSize = true;
            chkAllowedIPAddresses.ForeColor = System.Drawing.Color.White;
            chkAllowedIPAddresses.Location = new System.Drawing.Point(324, 120);
            chkAllowedIPAddresses.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkAllowedIPAddresses.Name = "chkAllowedIPAddresses";
            chkAllowedIPAddresses.Size = new System.Drawing.Size(15, 14);
            chkAllowedIPAddresses.TabIndex = 106;
            chkAllowedIPAddresses.UseVisualStyleBackColor = true;
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.ForeColor = System.Drawing.Color.White;
            label38.Location = new System.Drawing.Point(33, 119);
            label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label38.Name = "label38";
            label38.Size = new System.Drawing.Size(283, 15);
            label38.TabIndex = 94;
            label38.Text = "Allowed IP Addresses (if there’s a server component)";
            // 
            // chkProtectionWithaPassword
            // 
            chkProtectionWithaPassword.AutoSize = true;
            chkProtectionWithaPassword.Location = new System.Drawing.Point(408, 83);
            chkProtectionWithaPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkProtectionWithaPassword.Name = "chkProtectionWithaPassword";
            chkProtectionWithaPassword.Size = new System.Drawing.Size(15, 14);
            chkProtectionWithaPassword.TabIndex = 105;
            chkProtectionWithaPassword.UseVisualStyleBackColor = true;
            // 
            // chkEncryptLogs
            // 
            chkEncryptLogs.AutoSize = true;
            chkEncryptLogs.Location = new System.Drawing.Point(251, 44);
            chkEncryptLogs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkEncryptLogs.Name = "chkEncryptLogs";
            chkEncryptLogs.Size = new System.Drawing.Size(15, 14);
            chkEncryptLogs.TabIndex = 104;
            chkEncryptLogs.UseVisualStyleBackColor = true;
            // 
            // UserControl1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Controls.Add(btnApply);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(tabControl1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "UserControl1";
            Size = new System.Drawing.Size(1171, 626);
            Load += UserControl1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackRamUsage).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            tabPage6.ResumeLayout(false);
            tabPage6.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLogDirectory;
        private System.Windows.Forms.TextBox txtInstallDirectory;
        private System.Windows.Forms.Label lblExternalModulesPath;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblInstallationDirectory;
        private System.Windows.Forms.Button btnSelectModulesDirectory;
        private System.Windows.Forms.Button btnSelectInstallDirectory;
        private System.Windows.Forms.TextBox txtModulesDirectory;
        private System.Windows.Forms.Button btnSelectCacheDirectory;
        private System.Windows.Forms.TextBox txtCacheDirectory;
        private System.Windows.Forms.Button btnSelectLogDirectory;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblMaxSpeed;
        private System.Windows.Forms.Label lblSelectGPU;
        private System.Windows.Forms.Label lblLimitGPU;
        private System.Windows.Forms.Label lblGPUAcceleration;
        private System.Windows.Forms.Label lblCPULoad;
        private System.Windows.Forms.Label lblNumberCores;
        private System.Windows.Forms.Label lblCPULimit;
        private System.Windows.Forms.Label lblRAMLimit;
        private System.Windows.Forms.TextBox txtNetworkSpeed;
        private System.Windows.Forms.TrackBar trackRamUsage;
        private System.Windows.Forms.ComboBox cmbGpuSelection;
        private System.Windows.Forms.ComboBox cmbCpuCores;
        private System.Windows.Forms.CheckBox chkGPUEnable;
        private System.Windows.Forms.TextBox txtCPULimit;
        private System.Windows.Forms.TextBox txtGPULimit;
        private System.Windows.Forms.TextBox txtCpuLoad;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cmbUpdateBranch;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.CheckBox chkUpdPreferen;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.ComboBox cmbErrors;
        private System.Windows.Forms.Label lblErrors;
        private System.Windows.Forms.ComboBox cmbLogLevel;
        private System.Windows.Forms.Label lblWarnings;
        private System.Windows.Forms.Label lblInfoMessag;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.ComboBox cmbWarnings;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.RadioButton radDark;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.RadioButton radWhite;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.CheckBox chkAllowedIPAddresses;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.CheckBox chkProtectionWithaPassword;
        private System.Windows.Forms.CheckBox chkEncryptLogs;
        private System.Windows.Forms.ComboBox cmbLogFormat;
        private System.Windows.Forms.CheckBox chkLogRetention;
        private System.Windows.Forms.ComboBox cmbInfoMassages;
        private System.Windows.Forms.TextBox txtHotkeyLounch;
        private System.Windows.Forms.ComboBox cmbDebugging;
        private System.Windows.Forms.RadioButton radSystem;
        private System.Windows.Forms.CheckBox chkManUpdate;
        private System.Windows.Forms.CheckBox chkUpdateSrartup;
    }
}
