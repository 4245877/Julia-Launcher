namespace Julia_Launcher
{
    partial class Form1
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new System.Windows.Forms.Panel();
            labelStatus = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            pictureBoxInfo = new System.Windows.Forms.PictureBox();
            panel2 = new System.Windows.Forms.Panel();
            btnSetings = new System.Windows.Forms.Button();
            btnModelVoiceSettings = new System.Windows.Forms.Button();
            btnCoreFolder = new System.Windows.Forms.Button();
            panel3 = new System.Windows.Forms.Panel();
            button1 = new System.Windows.Forms.Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxInfo).BeginInit();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.Controls.Add(labelStatus);
            panel1.Controls.Add(progressBar);
            panel1.Location = new System.Drawing.Point(166, 12);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1171, 626);
            panel1.TabIndex = 0;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.BackColor = System.Drawing.Color.White;
            labelStatus.Location = new System.Drawing.Point(1125, 608);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new System.Drawing.Size(0, 15);
            labelStatus.TabIndex = 1;
            // 
            // progressBar
            // 
            progressBar.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
            progressBar.Location = new System.Drawing.Point(3, 600);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(1165, 23);
            progressBar.TabIndex = 0;
            // 
            // pictureBoxInfo
            // 
            pictureBoxInfo.BackColor = System.Drawing.Color.Transparent;
            pictureBoxInfo.Image = (System.Drawing.Image)resources.GetObject("pictureBoxInfo.Image");
            pictureBoxInfo.Location = new System.Drawing.Point(3, 3);
            pictureBoxInfo.Name = "pictureBoxInfo";
            pictureBoxInfo.Size = new System.Drawing.Size(32, 32);
            pictureBoxInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBoxInfo.TabIndex = 1;
            pictureBoxInfo.TabStop = false;
            pictureBoxInfo.Click += pictureBoxInfo_Click;
            // 
            // panel2
            // 
            panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            panel2.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            panel2.Controls.Add(pictureBoxInfo);
            panel2.Controls.Add(btnSetings);
            panel2.Controls.Add(btnModelVoiceSettings);
            panel2.Controls.Add(btnCoreFolder);
            panel2.Location = new System.Drawing.Point(10, 12);
            panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(148, 705);
            panel2.TabIndex = 1;
            // 
            // btnSetings
            // 
            btnSetings.Anchor = System.Windows.Forms.AnchorStyles.None;
            btnSetings.Location = new System.Drawing.Point(3, 657);
            btnSetings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSetings.Name = "btnSetings";
            btnSetings.Size = new System.Drawing.Size(141, 48);
            btnSetings.TabIndex = 4;
            btnSetings.Text = "Setings";
            btnSetings.UseVisualStyleBackColor = true;
            btnSetings.Click += btnSetings_Click;
            // 
            // btnModelVoiceSettings
            // 
            btnModelVoiceSettings.Anchor = System.Windows.Forms.AnchorStyles.None;
            btnModelVoiceSettings.Location = new System.Drawing.Point(3, 602);
            btnModelVoiceSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnModelVoiceSettings.Name = "btnModelVoiceSettings";
            btnModelVoiceSettings.Size = new System.Drawing.Size(141, 48);
            btnModelVoiceSettings.TabIndex = 3;
            btnModelVoiceSettings.Text = "Model & Voice Settings";
            btnModelVoiceSettings.UseVisualStyleBackColor = true;
            btnModelVoiceSettings.Click += btnModelVoiceSettings_Click;
            // 
            // btnCoreFolder
            // 
            btnCoreFolder.Anchor = System.Windows.Forms.AnchorStyles.None;
            btnCoreFolder.FlatAppearance.BorderColor = System.Drawing.Color.White;
            btnCoreFolder.Location = new System.Drawing.Point(3, 548);
            btnCoreFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCoreFolder.Name = "btnCoreFolder";
            btnCoreFolder.Size = new System.Drawing.Size(145, 48);
            btnCoreFolder.TabIndex = 2;
            btnCoreFolder.Text = "CoreFolder";
            btnCoreFolder.UseVisualStyleBackColor = true;
            btnCoreFolder.Click += btnCoreFolder_Click;
            // 
            // panel3
            // 
            panel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel3.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            panel3.Controls.Add(button1);
            panel3.Location = new System.Drawing.Point(166, 644);
            panel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(1171, 73);
            panel3.TabIndex = 2;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            button1.BackColor = System.Drawing.Color.FromArgb(13, 209, 102);
            button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            button1.ForeColor = System.Drawing.Color.White;
            button1.Location = new System.Drawing.Point(464, 3);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(216, 67);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(19, 19, 19);
            ClientSize = new System.Drawing.Size(1350, 729);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "Form1";
            Text = "Julia Launcher ";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxInfo).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnSetings;
        private System.Windows.Forms.Button btnModelVoiceSettings;
        private System.Windows.Forms.Button btnCoreFolder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.Windows.Forms.Label labelStatus;
    }
}

