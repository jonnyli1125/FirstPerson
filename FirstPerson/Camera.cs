using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FirstPerson
{
    public class Camera
    {
        public Vector3 Position;
        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }
        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }
        public float Z
        {
            get { return Position.Z; }
            set { Position.Z = value; }
        }
        public Vector3 Up = Vector3.UnitY;
        public Matrix4 CameraMatrix = Matrix4.Identity;
        public float Pitch = 0;
        public float Facing = 0;
        public float HorizontalSensitivity = 3;
        public float VerticalSensitivity = 6;
        public float Fog = 10000;
        public Point ScreenCenter { get { return new Point(Window.Bounds.Left + (Window.Bounds.Width / 2), Window.Bounds.Top + (Window.Bounds.Height / 2)); } }
        public Point WindowCenter { get { return new Point(Window.Width / 2, Window.Height / 2); } }
        public Point MouseDelta { get; private set; }
        public GameWindow Window { get; private set; }

        public Camera() { }
        public Camera(GameWindow window, float x, float y, float z) : this(window, new Vector3(x, y, z)) { }
        public Camera(GameWindow window, Vector3 position) : this(window, position, Vector3.UnitY) { }
        public Camera(GameWindow window, Vector3 position, Vector3 up)
        {
            Window = window;
            Position = position;
            Up = up;

            MouseDelta = new Point();

            Window.Resize += (sender, e) =>
            {
                Cursor.Position = ScreenCenter;
                Cursor.Hide();

                GL.Viewport(Window.ClientRectangle.X, Window.ClientRectangle.Y, Window.ClientRectangle.Width, Window.ClientRectangle.Height);

                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Window.Width / (float)Window.Height, 1f, Fog);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref projection);
            };

            Window.UpdateFrame += (sender, e) =>
            {
                MouseDelta = new Point(Window.Mouse.X - WindowCenter.X, Window.Mouse.Y - WindowCenter.Y);
                Point p = Cursor.Position;
                p.X -= MouseDelta.X;
                p.Y -= MouseDelta.Y;
                Cursor.Position = p;
                Facing += MouseDelta.X / (1000 - (float)(HorizontalSensitivity * 100));
                Pitch -= MouseDelta.Y / (1000 - (float)(VerticalSensitivity * 100));
                // because looking straight up or straight down (tand(90)) is a no-no.
                if (Pitch < -1.5f) Pitch = -1.5f; // 4 decimal places seems pretty smooth!?!?!?
                if (Pitch > 1.5f) Pitch = 1.5f;
                Vector3 lookatPoint = new Vector3((float)Math.Cos(Facing), (float)Math.Tan(Pitch), (float)Math.Sin(Facing));
                CameraMatrix = Matrix4.LookAt(Position, Position + lookatPoint, Up);
            };
        }
    }
}
