namespace Julia_Launcher
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
            trackBar1 = new System.Windows.Forms.TrackBar();
            trackBar2 = new System.Windows.Forms.TrackBar();
            trackBar3 = new System.Windows.Forms.TrackBar();
            trackBar5 = new System.Windows.Forms.TrackBar();
            trackBar6 = new System.Windows.Forms.TrackBar();
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
            label7 = new System.Windows.Forms.Label();
            btnModel = new System.Windows.Forms.Button();
            trackBar7 = new System.Windows.Forms.TrackBar();
            trackBar4 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar4).BeginInit();
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
            // trackBar1
            // 
            trackBar1.Location = new System.Drawing.Point(182, 298);
            trackBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new System.Drawing.Size(290, 45);
            trackBar1.TabIndex = 1;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // trackBar2
            // 
            trackBar2.Location = new System.Drawing.Point(182, 67);
            trackBar2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackBar2.Name = "trackBar2";
            trackBar2.Size = new System.Drawing.Size(290, 45);
            trackBar2.TabIndex = 2;
            // 
            // trackBar3
            // 
            trackBar3.Location = new System.Drawing.Point(182, 118);
            trackBar3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackBar3.Name = "trackBar3";
            trackBar3.Size = new System.Drawing.Size(290, 45);
            trackBar3.TabIndex = 3;
            // 
            // trackBar5
            // 
            trackBar5.Location = new System.Drawing.Point(182, 178);
            trackBar5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackBar5.Name = "trackBar5";
            trackBar5.Size = new System.Drawing.Size(290, 45);
            trackBar5.TabIndex = 5;
            // 
            // trackBar6
            // 
            trackBar6.Location = new System.Drawing.Point(182, 247);
            trackBar6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackBar6.Name = "trackBar6";
            trackBar6.Size = new System.Drawing.Size(290, 45);
            trackBar6.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(33, 67);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(32, 15);
            label2.TabIndex = 8;
            label2.Text = "Рост";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(33, 169);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(50, 15);
            label3.TabIndex = 9;
            label3.Text = "Возраст";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.Color.White;
            label4.Location = new System.Drawing.Point(33, 247);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(28, 15);
            label4.TabIndex = 10;
            label4.Text = "Тон";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = System.Drawing.Color.White;
            label5.Location = new System.Drawing.Point(33, 349);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(89, 15);
            label5.TabIndex = 11;
            label5.Text = "Скорость речи";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = System.Drawing.Color.White;
            label10.Location = new System.Drawing.Point(33, 118);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(26, 15);
            label10.TabIndex = 16;
            label10.Text = "Вес";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            label11.ForeColor = System.Drawing.Color.White;
            label11.Location = new System.Drawing.Point(15, 210);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(116, 13);
            label11.TabIndex = 17;
            label11.Text = "Настройки голоса";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.ForeColor = System.Drawing.Color.White;
            label12.Location = new System.Drawing.Point(33, 298);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(43, 15);
            label12.TabIndex = 18;
            label12.Text = "Тембр";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.ForeColor = System.Drawing.Color.White;
            label13.Location = new System.Drawing.Point(33, 400);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(66, 15);
            label13.TabIndex = 19;
            label13.Text = "Громкость";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = System.Drawing.Color.White;
            label6.Location = new System.Drawing.Point(15, 32);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(131, 15);
            label6.TabIndex = 20;
            label6.Text = "Настройки 3D-модели";
            // 
            // glControl1
            // 
            glControl1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl1.APIVersion = new System.Version(3, 3, 0, 0);
            glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl1.IsEventDriven = true;
            glControl1.Location = new System.Drawing.Point(478, 0);
            glControl1.Name = "glControl1";
            glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl1.SharedContext = null;
            glControl1.Size = new System.Drawing.Size(693, 667);
            glControl1.TabIndex = 21;
            glControl1.Click += glControl1_Click_2;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(33, 482);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(38, 15);
            label7.TabIndex = 22;
            label7.Text = "label7";
            // 
            // btnModel
            // 
            btnModel.Location = new System.Drawing.Point(15, 520);
            btnModel.Name = "btnModel";
            btnModel.Size = new System.Drawing.Size(457, 45);
            btnModel.TabIndex = 23;
            btnModel.Text = "button1";
            btnModel.UseVisualStyleBackColor = true;
            btnModel.Click += btnModel_Click;
            // 
            // trackBar7
            // 
            trackBar7.Location = new System.Drawing.Point(182, 349);
            trackBar7.Name = "trackBar7";
            trackBar7.Size = new System.Drawing.Size(290, 45);
            trackBar7.TabIndex = 24;
            // 
            // trackBar4
            // 
            trackBar4.Location = new System.Drawing.Point(182, 400);
            trackBar4.Name = "trackBar4";
            trackBar4.Size = new System.Drawing.Size(290, 45);
            trackBar4.TabIndex = 25;
            // 
            // UserControl2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            Controls.Add(trackBar4);
            Controls.Add(trackBar7);
            Controls.Add(btnModel);
            Controls.Add(label7);
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
            Controls.Add(trackBar6);
            Controls.Add(trackBar5);
            Controls.Add(trackBar3);
            Controls.Add(trackBar2);
            Controls.Add(trackBar1);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "UserControl2";
            Size = new System.Drawing.Size(1171, 667);
            Load += UserControl2_Load;
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar3).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar5).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar6).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar7).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar4).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TrackBar trackBar5;
        private System.Windows.Forms.TrackBar trackBar6;
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
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnModel;
        private System.Windows.Forms.TrackBar trackBar7;
        private System.Windows.Forms.TrackBar trackBar4;
    }
}
