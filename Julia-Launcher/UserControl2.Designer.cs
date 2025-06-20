﻿namespace Julia_Launcher
{
    partial class UserControl2
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
            label1 = new System.Windows.Forms.Label();
            trkHeight = new System.Windows.Forms.TrackBar();
            trkWeight = new System.Windows.Forms.TrackBar();
            trkAge = new System.Windows.Forms.TrackBar();
            trkTone = new System.Windows.Forms.TrackBar();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            glControl1 = new OpenTK.GLControl.GLControl();
            btnModel = new System.Windows.Forms.Button();
            trkSpeechRate = new System.Windows.Forms.TrackBar();
            trkVolume = new System.Windows.Forms.TrackBar();
            trkTimbre = new System.Windows.Forms.TrackBar();
            btnAddClothing = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)trkHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkAge).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkTone).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkSpeechRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkTimbre).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(4, 3);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(131, 13);
            label1.TabIndex = 0;
            label1.Text = "Model & Voice Settings";
            // 
            // trkHeight
            // 
            trkHeight.Location = new System.Drawing.Point(182, 67);
            trkHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trkHeight.Maximum = 210;
            trkHeight.Minimum = 145;
            trkHeight.Name = "trkHeight";
            trkHeight.Size = new System.Drawing.Size(290, 45);
            trkHeight.TabIndex = 2;
            trkHeight.Value = 145;
            trkHeight.Scroll += trkHeight_Scroll;
            // 
            // trkWeight
            // 
            trkWeight.Location = new System.Drawing.Point(182, 118);
            trkWeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trkWeight.Maximum = 120;
            trkWeight.Minimum = 39;
            trkWeight.Name = "trkWeight";
            trkWeight.Size = new System.Drawing.Size(290, 45);
            trkWeight.TabIndex = 3;
            trkWeight.Value = 39;
            trkWeight.Scroll += trkWeight_Scroll;
            // 
            // trkAge
            // 
            trkAge.Location = new System.Drawing.Point(182, 169);
            trkAge.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trkAge.Maximum = 105;
            trkAge.Minimum = 4;
            trkAge.Name = "trkAge";
            trkAge.Size = new System.Drawing.Size(290, 45);
            trkAge.TabIndex = 5;
            trkAge.Value = 105;
            trkAge.Scroll += trkAge_Scroll;
            // 
            // trkTone
            // 
            trkTone.Location = new System.Drawing.Point(182, 258);
            trkTone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trkTone.Name = "trkTone";
            trkTone.Size = new System.Drawing.Size(290, 45);
            trkTone.TabIndex = 6;
            trkTone.Scroll += trkTone_Scroll;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(33, 67);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(43, 15);
            label2.TabIndex = 8;
            label2.Text = "Height";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(33, 169);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(28, 15);
            label3.TabIndex = 9;
            label3.Text = "Age";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.Color.White;
            label4.Location = new System.Drawing.Point(33, 258);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(33, 15);
            label4.TabIndex = 10;
            label4.Text = "Tone";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = System.Drawing.Color.White;
            label5.Location = new System.Drawing.Point(33, 360);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(79, 15);
            label5.TabIndex = 11;
            label5.Text = "Speech speed";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = System.Drawing.Color.White;
            label10.Location = new System.Drawing.Point(33, 118);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(45, 15);
            label10.TabIndex = 16;
            label10.Text = "Weight";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label11.ForeColor = System.Drawing.Color.White;
            label11.Location = new System.Drawing.Point(15, 221);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(87, 13);
            label11.TabIndex = 17;
            label11.Text = "Voice settings";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.ForeColor = System.Drawing.Color.White;
            label12.Location = new System.Drawing.Point(33, 309);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(45, 15);
            label12.TabIndex = 18;
            label12.Text = "Timbre";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.ForeColor = System.Drawing.Color.White;
            label13.Location = new System.Drawing.Point(33, 411);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(47, 15);
            label13.TabIndex = 19;
            label13.Text = "Volume";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = System.Drawing.Color.White;
            label6.Location = new System.Drawing.Point(15, 32);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(103, 15);
            label6.TabIndex = 20;
            label6.Text = "3D Model Settings";
            // 
            // glControl1
            // 
            glControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            glControl1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl1.APIVersion = new System.Version(3, 3, 0, 0);
            glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl1.IsEventDriven = true;
            glControl1.Location = new System.Drawing.Point(478, 0);
            glControl1.Name = "glControl1";
            glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl1.SharedContext = null;
            glControl1.Size = new System.Drawing.Size(693, 626);
            glControl1.TabIndex = 21;
            glControl1.Click += glControl1_Click_2;
            // 
            // btnModel
            // 
            btnModel.Location = new System.Drawing.Point(15, 540);
            btnModel.Name = "btnModel";
            btnModel.Size = new System.Drawing.Size(457, 71);
            btnModel.TabIndex = 23;
            btnModel.Text = "Choose";
            btnModel.UseVisualStyleBackColor = true;
            btnModel.Click += btnModel_Click;
            // 
            // trkSpeechRate
            // 
            trkSpeechRate.Location = new System.Drawing.Point(182, 360);
            trkSpeechRate.Name = "trkSpeechRate";
            trkSpeechRate.Size = new System.Drawing.Size(290, 45);
            trkSpeechRate.TabIndex = 24;
            trkSpeechRate.Scroll += trkSpeechRate_Scroll;
            // 
            // trkVolume
            // 
            trkVolume.Location = new System.Drawing.Point(182, 411);
            trkVolume.Maximum = 100;
            trkVolume.Name = "trkVolume";
            trkVolume.Size = new System.Drawing.Size(290, 45);
            trkVolume.TabIndex = 25;
            trkVolume.Scroll += trkVolume_Scroll;
            // 
            // trkTimbre
            // 
            trkTimbre.Location = new System.Drawing.Point(182, 309);
            trkTimbre.Name = "trkTimbre";
            trkTimbre.Size = new System.Drawing.Size(290, 45);
            trkTimbre.TabIndex = 26;
            trkTimbre.Scroll += trkTimbre_Scroll;
            // 
            // btnAddClothing
            // 
            btnAddClothing.Location = new System.Drawing.Point(182, 451);
            btnAddClothing.Name = "btnAddClothing";
            btnAddClothing.Size = new System.Drawing.Size(290, 23);
            btnAddClothing.TabIndex = 27;
            btnAddClothing.Text = "Test";
            btnAddClothing.UseVisualStyleBackColor = true;
            btnAddClothing.Click += btnAddClothing_Click;
            // 
            // UserControl2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            Controls.Add(btnAddClothing);
            Controls.Add(trkTimbre);
            Controls.Add(trkVolume);
            Controls.Add(trkSpeechRate);
            Controls.Add(btnModel);
            Controls.Add(glControl1);
            Controls.Add(label6);
            Controls.Add(label13);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(trkTone);
            Controls.Add(trkAge);
            Controls.Add(trkWeight);
            Controls.Add(trkHeight);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "UserControl2";
            Size = new System.Drawing.Size(1171, 626);
            ((System.ComponentModel.ISupportInitialize)trkHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkAge).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkTone).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkSpeechRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkTimbre).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trkHeight;
        private System.Windows.Forms.TrackBar trkWeight;
        private System.Windows.Forms.TrackBar trkAge;
        private System.Windows.Forms.TrackBar trkTone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label6;
        private OpenTK.GLControl.GLControl glControl1;
        private System.Windows.Forms.Button btnModel;
        private System.Windows.Forms.TrackBar trkSpeechRate;
        private System.Windows.Forms.TrackBar trkVolume;
        private System.Windows.Forms.TrackBar trkTimbre;
        private System.Windows.Forms.Button btnAddClothing;
    }
}
