using System;
using OpenTK.Mathematics;
using Assimp;

namespace Julia_Launcher
{
    // Класс камеры для обработки преобразований камеры
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public float AspectRatio { get; set; }

        private Vector3 worldUp = new Vector3(0, 1, 0);
        private float yaw = -90.0f;
        private float pitch = 0.0f;
        private float zoom = 45.0f;


        public void SetFromAssimpCamera(Assimp.Camera assimpCamera, bool convertCoordinateSystem = true)
        {
            // Store the original position and target
            Vector3 originalPosition = new Vector3(assimpCamera.Position.X, assimpCamera.Position.Y, assimpCamera.Position.Z);
            Vector3 originalTarget = new Vector3(
                assimpCamera.Position.X + assimpCamera.Direction.X,
                assimpCamera.Position.Y + assimpCamera.Direction.Y,
                assimpCamera.Position.Z + assimpCamera.Direction.Z);
            Vector3 originalUp = new Vector3(assimpCamera.Up.X, assimpCamera.Up.Y, assimpCamera.Up.Z);

            // Convert from Assimp to OpenTK coordinate system if needed
            if (convertCoordinateSystem)
            {
                // Convert from Y-up (Assimp) to Y-up (OpenTK) - may need adjustment based on your specific case
                // For typical FBX files we might need to flip Z or invert other axes
                Position = new Vector3(originalPosition.X, originalPosition.Y, -originalPosition.Z);
                Vector3 target = new Vector3(originalTarget.X, originalTarget.Y, -originalTarget.Z);
                Vector3 up = new Vector3(originalUp.X, originalUp.Y, -originalUp.Z);

                // Calculate front direction from position to target
                Front = Vector3.Normalize(target - Position);

                // Calculate the right vector from front and up
                Right = Vector3.Normalize(Vector3.Cross(up, Front));

                // Recalculate the up vector to ensure orthogonality
                Up = Vector3.Normalize(Vector3.Cross(Front, Right));
            }
            else
            {
                // Use directly without conversion
                Position = originalPosition;
                Front = Vector3.Normalize(new Vector3(assimpCamera.Direction.X, assimpCamera.Direction.Y, assimpCamera.Direction.Z));
                Up = Vector3.Normalize(originalUp);
                Right = Vector3.Normalize(Vector3.Cross(Up, Front));
            }

            // Calculate yaw and pitch from the Front vector
            yaw = MathHelper.RadiansToDegrees((float)Math.Atan2(Front.Z, Front.X));
            pitch = MathHelper.RadiansToDegrees((float)Math.Asin(Front.Y));

            // Set field of view (zoom)
            zoom = MathHelper.RadiansToDegrees(assimpCamera.FieldOfview);
            if (zoom <= 0) zoom = 45.0f;

            // Set aspect ratio
            AspectRatio = assimpCamera.AspectRatio > 0 ? assimpCamera.AspectRatio : 1.0f;

            // Make sure all vectors are updated
            UpdateCameraVectors();

            // Log camera import details for debugging
            Console.WriteLine($"Imported camera - Position: {Position}, Front: {Front}, Up: {Up}");
            Console.WriteLine($"FOV: {zoom}, Yaw: {yaw}, Pitch: {pitch}");
        }


        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
            UpdateCameraVectors();
        }

        public void LookAt(Vector3 target)
        {
            Vector3 direction = Vector3.Normalize(target - Position);
            if (direction.LengthSquared > 0)
            {
                pitch = MathHelper.RadiansToDegrees((float)Math.Asin(direction.Y));
                yaw = MathHelper.RadiansToDegrees((float)Math.Atan2(direction.Z, direction.X));
                UpdateCameraVectors();
            }
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(zoom),
                AspectRatio,
                0.1f,
                1000.0f);
        }

        public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
        {
            yaw += xOffset;
            pitch += yOffset;

            if (constrainPitch)
            {
                pitch = Math.Clamp(pitch, -89.0f, 89.0f);
            }

            UpdateCameraVectors();
        }

        public void ProcessMouseScroll(float yOffset)
        {
            zoom -= yOffset;
            zoom = Math.Clamp(zoom, 1.0f, 90.0f);
        }

        private void UpdateCameraVectors()
        {
            // Calculate new Front vector
            Vector3 front;
            front.X = (float)(Math.Cos(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)(Math.Sin(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            Front = Vector3.Normalize(front);

            // Recalculate Right and Up vectors
            Right = Vector3.Normalize(Vector3.Cross(Front, worldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}