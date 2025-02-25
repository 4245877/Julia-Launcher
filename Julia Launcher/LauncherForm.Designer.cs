namespace Julia_Launcher
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            button1 = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            label1 = new Label();
            button2 = new Button();
            buttonSettings = new Button();
            panel1 = new Panel();
            button4 = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(565, 656);
            button1.Name = "button1";
            button1.Size = new Size(209, 61);
            button1.TabIndex = 0;
            button1.Text = "Start";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Grand Aventure", 215.249969F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(412, 162);
            label1.Name = "label1";
            label1.Size = new Size(667, 287);
            label1.TabIndex = 2;
            label1.Text = "Julia";
            // 
            // button2
            // 
            button2.Location = new Point(0, 621);
            button2.Name = "button2";
            button2.Size = new Size(206, 45);
            button2.TabIndex = 3;
            button2.Text = "S";
            button2.UseVisualStyleBackColor = true;
            // 
            // buttonSettings
            // 
            buttonSettings.Location = new Point(0, 672);
            buttonSettings.Name = "buttonSettings";
            buttonSettings.Size = new Size(206, 45);
            buttonSettings.TabIndex = 4;
            buttonSettings.Text = "S";
            buttonSettings.UseVisualStyleBackColor = true;
            buttonSettings.Click += buttonSettings_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(buttonSettings);
            panel1.Location = new Point(6, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(206, 717);
            panel1.TabIndex = 6;
            // 
            // button4
            // 
            button4.Location = new Point(0, 3);
            button4.Name = "button4";
            button4.Size = new Size(206, 45);
            button4.TabIndex = 5;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.WindowFrame;
            ClientSize = new Size(1350, 729);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Julia";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private ContextMenuStrip contextMenuStrip1;
        private Label label1;
        private Button button2;
        private Button buttonSettings;
        private Panel panel1;
        private Button button4;
    }
}
