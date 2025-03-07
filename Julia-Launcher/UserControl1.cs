using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Julia_Launcher
{
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только цифры и управляющие клавиши
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text))
                return;

            if (int.TryParse(textBox4.Text, out int number))
            {
                if (number < 1 || number > 100)
                {
                    // Восстанавливаем предыдущее значение или очищаем TextBox
                    textBox4.Text = number < 1 ? "1" : "100";
                    textBox4.SelectionStart = textBox4.Text.Length; // Устанавливаем курсор в конец
                }
            }
            else
            {
                // Удаляем некорректный ввод
                textBox4.Text = "";
            }
        }

        private void cmbCpuCores_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
