using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Julia_Launcher.UserControl2;

namespace Julia_Launcher
{
    // Хранить информацию о костях и иерархию
    public class Bone
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Matrix4 OffsetMatrix { get; private set; }
        public List<KeyFrame> KeyFrames { get; private set; }
        public Matrix4 FinalTransformation { get; set; }

        public Bone(int id, string name, Matrix4 offsetMatrix)
        {
            ID = id;
            Name = name;
            OffsetMatrix = offsetMatrix;
            KeyFrames = new List<KeyFrame>();
            FinalTransformation = Matrix4.Identity;
        }

        public void AddKeyFrame(KeyFrame keyFrame)
        {
            KeyFrames.Add(keyFrame);
        }

        // Интерполяция между ключевыми кадрами на основе времени анимации
        public Matrix4 InterpolateTransform(float animationTime)
        {
            if (KeyFrames.Count == 0) return Matrix4.Identity;
            if (KeyFrames.Count == 1) return KeyFrames[0].Transform;

            // Найти, между какими ключевыми кадрами следует выполнить интерполяцию
            int frameIndex = FindFrameIndex(animationTime);
            int nextFrameIndex = (frameIndex + 1) % KeyFrames.Count;

            KeyFrame currentFrame = KeyFrames[frameIndex];
            KeyFrame nextFrame = KeyFrames[nextFrameIndex];

            float delta = CalculateDelta(animationTime, currentFrame, nextFrame);

            // Интерполяция между текущим кадром и следующим кадром
            return InterpolateMatrices(currentFrame.Transform, nextFrame.Transform, delta);
        }

        private int FindFrameIndex(float animationTime)
        {
            for (int i = 0; i < KeyFrames.Count - 1; i++)
            {
                if (animationTime < KeyFrames[i + 1].Time)
                    return i;
            }
            return KeyFrames.Count - 1;
        }

        private float CalculateDelta(float animationTime, KeyFrame currentFrame, KeyFrame nextFrame)
        {
            float framesDiff = nextFrame.Time - currentFrame.Time;
            if (framesDiff < 0.0001f) return 0;

            float delta = (animationTime - currentFrame.Time) / framesDiff;
            return Math.Clamp(delta, 0.0f, 1.0f);
        }

        private Matrix4 InterpolateMatrices(Matrix4 start, Matrix4 end, float factor)
        {
            // Извлечь положение, поворот и масштаб из матриц
            Vector3 startPos = start.ExtractTranslation();
            Vector3 endPos = end.ExtractTranslation();

            Quaternion startRot = start.ExtractRotation();
            Quaternion endRot = end.ExtractRotation();

            Vector3 startScale = start.ExtractScale();
            Vector3 endScale = end.ExtractScale();

            // Интерполировать компоненты
            Vector3 pos = Vector3.Lerp(startPos, endPos, factor);
            Quaternion rot = Quaternion.Slerp(startRot, endRot, factor);
            Vector3 scale = Vector3.Lerp(startScale, endScale, factor);

            // Объединить в новую трансформацию
            Matrix4 result = Matrix4.CreateScale(scale) *
                              Matrix4.CreateFromQuaternion(rot) *
                              Matrix4.CreateTranslation(pos);

            return result;
        }
    }
}
