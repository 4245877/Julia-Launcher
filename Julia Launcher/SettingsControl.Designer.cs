namespace Julia_Launcher
{
    partial class SettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label5 = new Label();
            label6 = new Label();
            button1 = new Button();
            textBoxPath = new TextBox();
            labelDisk = new Label();
            numericUpDown1 = new NumericUpDown();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            labelGPU = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            groupBoxFiles = new GroupBox();
            groupBoxResources = new GroupBox();
            labelCores = new Label();
            numericUpDownCores = new NumericUpDown();
            numericUpDownDisk = new NumericUpDown();
            checkBox1 = new CheckBox();
            comboBox1 = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBoxFiles.SuspendLayout();
            groupBoxResources.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCores).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDisk).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 11);
            label1.Name = "label1";
            label1.Size = new Size(122, 15);
            label1.TabIndex = 0;
            label1.Text = "Настройки лаунчера";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 31);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
            label2.TabIndex = 1;
            label2.Text = "Путь к ИИ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 49);
            label5.Name = "label5";
            label5.Size = new Size(195, 15);
            label5.TabIndex = 4;
            label5.Text = "Динамическое выделение памяти";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(7, 64);
            label6.Name = "label6";
            label6.Size = new Size(121, 15);
            label6.TabIndex = 5;
            label6.Text = "Оперативная память";
            // 
            // button1
            // 
            button1.Location = new Point(706, 3);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 6;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(167, 28);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.Size = new Size(443, 23);
            textBoxPath.TabIndex = 7;
            // 
            // labelDisk
            // 
            labelDisk.AutoSize = true;
            labelDisk.Location = new Point(7, 79);
            labelDisk.Name = "labelDisk";
            labelDisk.Size = new Size(98, 15);
            labelDisk.TabIndex = 8;
            labelDisk.Text = "Память на диске";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(340, 56);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(120, 23);
            numericUpDown1.TabIndex = 9;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 74);
            label8.Name = "label8";
            label8.Size = new Size(97, 15);
            label8.TabIndex = 10;
            label8.Text = "Папка с логами ";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 107);
            label9.Name = "label9";
            label9.Size = new Size(183, 15);
            label9.TabIndex = 11;
            label9.Text = "Директория временных файлов";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(6, 134);
            label10.Name = "label10";
            label10.Size = new Size(140, 15);
            label10.TabIndex = 12;
            label10.Text = "Резервное копирование";
            // 
            // labelGPU
            // 
            labelGPU.AutoSize = true;
            labelGPU.Location = new Point(6, 19);
            labelGPU.Name = "labelGPU";
            labelGPU.Size = new Size(122, 15);
            labelGPU.TabIndex = 13;
            labelGPU.Text = "Использование GPU ";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(6, 34);
            label12.Name = "label12";
            label12.Size = new Size(125, 15);
            label12.TabIndex = 14;
            label12.Text = "Приоритет процесса ";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(7, 94);
            label13.Name = "label13";
            label13.Size = new Size(204, 15);
            label13.TabIndex = 15;
            label13.Text = "Ограничение потребления энергии";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(7, 109);
            label14.Name = "label14";
            label14.Size = new Size(137, 15);
            label14.TabIndex = 16;
            label14.Text = "Сеть и взаимодействие:";
            // 
            // groupBoxFiles
            // 
            groupBoxFiles.Controls.Add(textBoxPath);
            groupBoxFiles.Controls.Add(label8);
            groupBoxFiles.Controls.Add(label2);
            groupBoxFiles.Controls.Add(label9);
            groupBoxFiles.Controls.Add(label10);
            groupBoxFiles.Location = new Point(19, 29);
            groupBoxFiles.Name = "groupBoxFiles";
            groupBoxFiles.Size = new Size(616, 162);
            groupBoxFiles.TabIndex = 17;
            groupBoxFiles.TabStop = false;
            groupBoxFiles.Text = "Properties ";
            // 
            // groupBoxResources
            // 
            groupBoxResources.Controls.Add(comboBox1);
            groupBoxResources.Controls.Add(checkBox1);
            groupBoxResources.Controls.Add(numericUpDownDisk);
            groupBoxResources.Controls.Add(numericUpDownCores);
            groupBoxResources.Controls.Add(labelCores);
            groupBoxResources.Controls.Add(labelGPU);
            groupBoxResources.Controls.Add(label12);
            groupBoxResources.Controls.Add(label14);
            groupBoxResources.Controls.Add(label5);
            groupBoxResources.Controls.Add(label13);
            groupBoxResources.Controls.Add(numericUpDown1);
            groupBoxResources.Controls.Add(labelDisk);
            groupBoxResources.Controls.Add(label6);
            groupBoxResources.Location = new Point(19, 252);
            groupBoxResources.Name = "groupBoxResources";
            groupBoxResources.Size = new Size(616, 222);
            groupBoxResources.TabIndex = 18;
            groupBoxResources.TabStop = false;
            groupBoxResources.Text = "Ресурсы";
            // 
            // labelCores
            // 
            labelCores.AutoSize = true;
            labelCores.Location = new Point(7, 145);
            labelCores.Name = "labelCores";
            labelCores.Size = new Size(33, 15);
            labelCores.TabIndex = 17;
            labelCores.Text = "Ядра";
            // 
            // numericUpDownCores
            // 
            numericUpDownCores.Location = new Point(340, 145);
            numericUpDownCores.Name = "numericUpDownCores";
            numericUpDownCores.Size = new Size(120, 23);
            numericUpDownCores.TabIndex = 18;
            // 
            // numericUpDownDisk
            // 
            numericUpDownDisk.Location = new Point(340, 77);
            numericUpDownDisk.Name = "numericUpDownDisk";
            numericUpDownDisk.Size = new Size(120, 23);
            numericUpDownDisk.TabIndex = 19;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(340, 19);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(82, 19);
            checkBox1.TabIndex = 20;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(339, 34);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 21;
            // 
            // SettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxResources);
            Controls.Add(groupBoxFiles);
            Controls.Add(button1);
            Controls.Add(label1);
            Location = new Point(20, 85);
            Name = "SettingsControl";
            Size = new Size(784, 494);
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            groupBoxFiles.ResumeLayout(false);
            groupBoxFiles.PerformLayout();
            groupBoxResources.ResumeLayout(false);
            groupBoxResources.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCores).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDisk).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label5;
        private Label label6;
        private Button button1;
        private TextBox textBoxPath;
        private Label labelDisk;
        private NumericUpDown numericUpDown1;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label labelGPU;
        private Label label12;
        private Label label13;
        private Label label14;
        private GroupBox groupBoxFiles;
        private GroupBox groupBoxResources;
        private Label labelCores;
        private NumericUpDown numericUpDownCores;
        private NumericUpDown numericUpDownDisk;
        private CheckBox checkBox1;
        private ComboBox comboBox1;
    }
}
