using System;
using System.Drawing;
using System.Windows.Forms;

namespace Julia_Launcher
{
    public class CustomTrackBar : TrackBar
    {
        public CustomTrackBar()
        {
            // Подписываемся на событие ValueChanged для обновления отображения при изменении значения
            this.ValueChanged += (s, e) => this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Вызываем базовую реализацию для отрисовки стандартного TrackBar
            base.OnPaint(e);

            // Вычисляем относительную позицию ползунка
            double relativePosition = (double)(this.Value - this.Minimum) / (this.Maximum - this.Minimum);
            int thumbWidth = 10; // Примерная ширина ползунка (можно уточнить при необходимости)
            int x = (int)(relativePosition * (this.Width - thumbWidth));

            // Подготавливаем текст для отображения
            string valueText = this.Value.ToString();
            Font font = new Font("Arial", 10);
            SizeF textSize = e.Graphics.MeasureString(valueText, font);

            // Вычисляем позицию текста над ползунком
            PointF textPosition = new PointF(
                x + (thumbWidth - textSize.Width) / 2, // Центрируем текст по горизонтали
                0 // Размещаем текст в верхней части TrackBar
            );

            // Отрисовываем текст
            e.Graphics.DrawString(valueText, font, Brushes.Black, textPosition);
        }
    }
}