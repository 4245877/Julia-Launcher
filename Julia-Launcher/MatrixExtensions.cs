using OpenTK.Mathematics;

namespace Julia_Launcher
{
    public static class Matrix4Extensions
    {
        public static Vector3 ExtractTranslation(this Matrix4 matrix)
        {
            return new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }

        public static Quaternion ExtractRotation(this Matrix4 matrix)
        {
            // Удалить масштабирование
            Vector3 scale = matrix.ExtractScale();
            Matrix4 rotMat = matrix;

            if (scale.X != 0)
            {
                rotMat.M11 /= scale.X;
                rotMat.M12 /= scale.X;
                rotMat.M13 /= scale.X;
            }

            if (scale.Y != 0)
            {
                rotMat.M21 /= scale.Y;
                rotMat.M22 /= scale.Y;
                rotMat.M23 /= scale.Y;
            }

            if (scale.Z != 0)
            {
                rotMat.M31 /= scale.Z;
                rotMat.M32 /= scale.Z;
                rotMat.M33 /= scale.Z;
            }
            // Создание Matrix3 из верхних левых 3x3 элементов
            Matrix3 rotationMatrix = new Matrix3(
                rotMat.M11, rotMat.M12, rotMat.M13,
                rotMat.M21, rotMat.M22, rotMat.M23,
                rotMat.M31, rotMat.M32, rotMat.M33
            );

            // Извлечение кватерниона из Matrix3
            return Quaternion.FromMatrix(rotationMatrix);
        }

        public static Vector3 ExtractScale(this Matrix4 matrix)
        {
            return new Vector3(
                new Vector3(matrix.M11, matrix.M12, matrix.M13).Length,
                new Vector3(matrix.M21, matrix.M22, matrix.M23).Length,
                new Vector3(matrix.M31, matrix.M32, matrix.M33).Length
            );
        }

        public static Matrix4 ClearTranslation(this Matrix4 matrix)
        {
            Matrix4 result = matrix;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            return result;
        }
    }
}