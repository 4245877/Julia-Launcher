using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julia_Launcher
{
    // Сохраняем данные ключевых кадров для анимации
    public class KeyFrame
    {
        public float Time { get; private set; }
        public Matrix4 Transform { get; private set; }

        public KeyFrame(float time, Matrix4 transform)
        {
            Time = time;
            Transform = transform;
        }
    }
}
