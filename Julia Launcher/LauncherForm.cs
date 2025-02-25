using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Julia_Launcher
{
    public partial class MainForm : Form
    {

        private SettingsControl settingsControl;

        public MainForm()
        {
            InitializeComponent();

            // Настройка формы для современного вида
            this.BackColor = Color.FromArgb(30, 30, 30); // Темный фон
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Фиксированный размер
            this.MaximizeBox = false; // Отключение кнопки "Развернуть"

            // Добавление заголовка
            Label titleLabel = new Label();
            titleLabel.Text = "Julia Launcher";
            titleLabel.ForeColor = Color.White; // Белый текст
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold); // Современный шрифт
            titleLabel.AutoSize = true;
            titleLabel.BackColor = Color.Transparent; // Прозрачный фон для слияния с градиентом
            this.Controls.Add(titleLabel);

            // Позиционирование заголовка в событии Load для центрирования
            this.Load += (s, e) =>
            {
                titleLabel.Location = new Point((this.ClientSize.Width - titleLabel.Width) / 2, 20);
            };

            // Стилизация кнопки запуска (button1)
            button1.Text = "LAUNCH"; // Текст в верхнем регистре
            button1.FlatStyle = FlatStyle.Flat; // Плоский стиль
            button1.FlatAppearance.BorderSize = 0; // Без границ
            button1.BackColor = Color.FromArgb(50, 50, 50); // Темный фон кнопки
            button1.ForeColor = Color.White; // Белый текст
            button1.Font = new Font("Segoe UI", 12, FontStyle.Bold); // Жирный шрифт
            button1.Size = new Size(200, 50); // Большой размер
            button1.Location = new Point((this.ClientSize.Width - button1.Width) / 2, 100); // Центрирование
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70); // Цвет при наведении
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(90, 90, 90); // Цвет при нажатии

            // Стилизация кнопки настроек
            buttonSettings.Text = "SETTINGS";
            buttonSettings.FlatStyle = FlatStyle.Flat;
            buttonSettings.FlatAppearance.BorderSize = 0;
            buttonSettings.BackColor = Color.FromArgb(50, 50, 50);
            buttonSettings.ForeColor = Color.White;
            buttonSettings.Font = new Font("Segoe UI", 10); // Чуть меньший шрифт
            buttonSettings.Size = new Size(100, 30);
            buttonSettings.Location = new Point((this.ClientSize.Width - buttonSettings.Width) / 2, 160); // Под кнопкой запуска
            buttonSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            buttonSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(90, 90, 90);

            // Добавление градиентного фона (опционально)
            this.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                    Color.FromArgb(30, 30, 30), Color.FromArgb(50, 50, 50), 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };
        }


        private void buttonSettings_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }

  
    // Классы данных
    public class Settings
    {
        public string AIPath { get; set; } = "";
        public int AllocatedRAM { get; set; } = 4096;
        public CharacterSettings Character { get; set; } = new CharacterSettings();
    }

    public class CharacterSettings
    {
        public string HairColor { get; set; } = "";
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
    }

    public class SystemInfo
    {
        public string CPU { get; set; }
        public long RAM { get; set; } // в байтах
    }
}