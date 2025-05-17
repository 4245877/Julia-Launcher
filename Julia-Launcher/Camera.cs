using System;
using OpenTK.Mathematics;

namespace Julia_Launcher
{
    // Класс для управления камерой в 3D-пространстве
    public class Camera
    {
        // Позиция камеры в мировых координатах
        public Vector3 Position { get; set; }
        // Вектор направления взгляда камеры (куда смотрит камера)
        public Vector3 Front { get; private set; }
        // Вектор "вверх" камеры, определяющий ориентацию
        public Vector3 Up { get; private set; }
        // Вектор "вправо" камеры, ортогональный Front и Up
        public Vector3 Right { get; private set; }
        // Соотношение сторон для матрицы проекции (ширина/высота)
        public float AspectRatio { get; set; }

        // Постоянный вектор "вверх" в мировом пространстве (по умолчанию Y-ось)
        private Vector3 worldUp = new Vector3(0, 1, 0);
        // Угол рыскания (вращение вокруг Y-оси) в градусах
        private float yaw = -90.0f;
        // Угол тангажа (вращение вокруг X-оси) в градусах
        private float pitch = 0.0f;
        // Угол обзора (field of view) в градусах для зума
        private float zoom = 45.0f;

        // Конструктор камеры с начальной позицией и соотношением сторон
        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
            UpdateCameraVectors(); // Инициализируем векторы камеры
        }

        // Метод для настройки камеры на основе данных из Assimp.Camera
        // Параметр convertCoordinateSystem определяет, нужно ли конвертировать систему координат
        public void SetFromAssimpCamera(Assimp.Camera assimpCamera, bool convertCoordinateSystem = true)
        {
            // Извлекаем исходные данные из Assimp
            Vector3 originalPosition = new Vector3(assimpCamera.Position.X, assimpCamera.Position.Y, assimpCamera.Position.Z);
            Vector3 originalTarget = new Vector3(
                assimpCamera.Position.X + assimpCamera.Direction.X,
                assimpCamera.Position.Y + assimpCamera.Direction.Y,
                assimpCamera.Position.Z + assimpCamera.Direction.Z);
            Vector3 originalUp = new Vector3(assimpCamera.Up.X, assimpCamera.Up.Y, assimpCamera.Up.Z);

            if (convertCoordinateSystem)
            {
                // Конвертируем систему координат Assimp (Y-up) для OpenTK
                // Инвертируем Z-координату, что часто требуется для FBX-файлов
                Position = new Vector3(originalPosition.X, originalPosition.Y, -originalPosition.Z);
                Vector3 target = new Vector3(originalTarget.X, originalTarget.Y, -originalTarget.Z);
                Vector3 up = new Vector3(originalUp.X, originalUp.Y, -originalUp.Z);

                // Вычисляем направление взгляда (Front) как нормализованный вектор от позиции к цели
                Front = Vector3.Normalize(target - Position);

                // Вычисляем вектор "вправо" (Right) через перекрестное произведение
                Right = Vector3.Normalize(Vector3.Cross(up, Front));

                // Пересчитываем вектор "вверх" (Up) для ортогональности
                Up = Vector3.Normalize(Vector3.Cross(Front, Right));
            }
            else
            {
                // Используем данные без конвертации системы координат
                Position = originalPosition;
                Front = Vector3.Normalize(new Vector3(assimpCamera.Direction.X, assimpCamera.Direction.Y, assimpCamera.Direction.Z));
                Up = Vector3.Normalize(originalUp);
                Right = Vector3.Normalize(Vector3.Cross(Up, Front));
            }

            // Вычисляем углы yaw и pitch на основе вектора Front
            yaw = MathHelper.RadiansToDegrees((float)Math.Atan2(Front.Z, Front.X));
            pitch = MathHelper.RadiansToDegrees((float)Math.Asin(Front.Y));

            // Устанавливаем угол обзора (FOV), с запасным значением по умолчанию
            zoom = MathHelper.RadiansToDegrees(assimpCamera.FieldOfview);
            if (zoom <= 0) zoom = 45.0f;

            // Устанавливаем соотношение сторон, с запасным значением по умолчанию
            AspectRatio = assimpCamera.AspectRatio > 0 ? assimpCamera.AspectRatio : 1.0f;

            // Обновляем векторы камеры для согласованности
            UpdateCameraVectors();

            // Выводим параметры камеры в консоль для отладки
            Console.WriteLine($"Imported camera - Position: {Position}, Front: {Front}, Up: {Up}");
            Console.WriteLine($"FOV: {zoom}, Yaw: {yaw}, Pitch: {pitch}");
        }

        // Метод для ориентации камеры на заданную цель
        public void LookAt(Vector3 target)
        {
            // Вычисляем нормализованное направление от позиции камеры к цели
            Vector3 direction = Vector3.Normalize(target - Position);
            if (direction.LengthSquared > 0) // Проверяем, что вектор не нулевой
            {
                // Обновляем углы pitch и yaw на основе направления
                pitch = MathHelper.RadiansToDegrees((float)Math.Asin(direction.Y));
                yaw = MathHelper.RadiansToDegrees((float)Math.Atan2(direction.Z, direction.X));
                UpdateCameraVectors(); // Обновляем векторы камеры
            }
        }

        // Возвращает матрицу вида (view matrix) для рендеринга сцены
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        // Возвращает матрицу проекции (projection matrix) для перспективного отображения
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(zoom), // Угол обзора в радианах
                AspectRatio,                       // Соотношение сторон
                0.1f,                              // Ближняя плоскость отсечения
                1000.0f);                          // Дальняя плоскость отсечения
        }

        // Обрабатывает движение мыши для вращения камеры
        public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
        {
            yaw += xOffset;   // Изменяем угол рыскания
            pitch += yOffset; // Изменяем угол тангажа

            if (constrainPitch)
            {
                // Ограничиваем угол тангажа, чтобы камера не переворачивалась
                pitch = Math.Clamp(pitch, -89.0f, 89.0f);
            }

            UpdateCameraVectors(); // Обновляем векторы после изменения углов
        }

        // Обрабатывает прокрутку мыши для изменения угла обзора (зума)
        public void ProcessMouseScroll(float yOffset)
        {
            zoom -= yOffset; // Уменьшаем или увеличиваем FOV
            zoom = Math.Clamp(zoom, 1.0f, 90.0f); // Ограничиваем диапазон зума
        }

        // Вспомогательный метод для пересчета векторов камеры на основе углов yaw и pitch
        private void UpdateCameraVectors()
        {
            // Вычисляем новый вектор направления (Front)
            Vector3 front;
            front.X = (float)(Math.Cos(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)(Math.Sin(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            Front = Vector3.Normalize(front);

            // Вычисляем вектор "вправо" (Right) как перекрестное произведение Front и worldUp
            Right = Vector3.Normalize(Vector3.Cross(Front, worldUp));

            // Вычисляем вектор "вверх" (Up) как перекрестное произведение Right и Front
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}