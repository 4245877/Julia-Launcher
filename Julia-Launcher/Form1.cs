using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Julia_Launcher
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Очищаем panel1 от существующих элементов управления
            panel1.Controls.Clear();

            // Создаем новый экземпляр UserControl1
            UserControl1 userControl = new UserControl1();

            // Устанавливаем свойство Dock, чтобы элемент заполнил всю панель
            userControl.Dock = DockStyle.Fill;

            // Добавляем UserControl1 в panel1
            panel1.Controls.Add(userControl);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

            // Очищаем panel1 от существующих элементов управления
            panel1.Controls.Clear();

            // Создаем новый экземпляр UserControl1
            UserControl2 userControl = new UserControl2();

            // Устанавливаем свойство Dock, чтобы элемент заполнил всю панель
            userControl.Dock = DockStyle.Fill;

            // Добавляем UserControl1 в panel1
            panel1.Controls.Add(userControl);
        }
    }
}
