using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FirstPerson
{
    public class Window : GameWindow
    {
        public static readonly string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private Matrix4 CameraMatrix;
        private Vector3 SpawnLocation = new Vector3(0f, 5f, 0f);
        private Vector3 CameraLocation;
        private Vector3 Up = Vector3.UnitY;
        private float Pitch = 0.0f;
        private float Facing = 0.0f;
        private Point MouseDelta;
        private Point ScreenCenter;
        private Point WindowCenter;
        private MeshBuffers MeshBuffers;
        private static float HorizontalSensitivity = 3;
        private static float VerticalSensitivity = 3;
        private static float Fog = 10000;
        private static float FPS = 60;

        public Window() : base(1280, 720, GraphicsMode.Default, "FirstPerson") { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(Color.Black);

            GL.Enable(EnableCap.DepthTest);

            CameraMatrix = Matrix4.Identity;
            CameraLocation = SpawnLocation;
            MouseDelta = new Point();

            Cursor.Position = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
            Cursor.Hide();

            MeshBuffers = new MeshBuffers();

            MeshBuffers.VertexData = new Vector3[] { 
				new Vector3 (-1.0f, -1.0f, 1.0f), 
				new Vector3 (1.0f, -1.0f, 1.0f), 
				new Vector3 (1.0f, 1.0f, 1.0f), 
				new Vector3 (-1.0f, 1.0f, 1.0f), 
				new Vector3 (-1.0f, -1.0f, -1.0f), 
				new Vector3 (1.0f, -1.0f, -1.0f), 
				new Vector3 (1.0f, 1.0f, -1.0f), 
				new Vector3 (-1.0f, 1.0f, -1.0f) 
			};

            MeshBuffers.NormalData = new Vector3[] { 
				new Vector3 (-1.0f, -1.0f, 1.0f), 
				new Vector3 (1.0f, -1.0f, 1.0f), 
				new Vector3 (1.0f, 1.0f, 1.0f), 
				new Vector3 (-1.0f, 1.0f, 1.0f), 
				new Vector3 (-1.0f, -1.0f, -1.0f), 
				new Vector3 (1.0f, -1.0f, -1.0f), 
				new Vector3 (1.0f, 1.0f, -1.0f), 
				new Vector3 (-1.0f, 1.0f, -1.0f) 
			};

            MeshBuffers.ColorData = new int[] {
				ColorToRgba32 (Color.Cyan), 
				ColorToRgba32 (Color.Cyan), 
				ColorToRgba32 (Color.DarkCyan), 
				ColorToRgba32 (Color.DarkCyan), 
				ColorToRgba32 (Color.Cyan), 
				ColorToRgba32 (Color.Cyan), 
				ColorToRgba32 (Color.DarkCyan), 
				ColorToRgba32 (Color.DarkCyan) 
			};

            MeshBuffers.IndicesData = new uint[] { 
				0, 1, 2, 2, 3, 0, 
				3, 2, 6, 6, 7, 3, 
				7, 6, 5, 5, 4, 7, 
				4, 0, 3, 3, 7, 4, 
				0, 1, 5, 5, 4, 0,
				1, 5, 6, 6, 2, 1 
			};

            MeshBuffers.GenerateBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ScreenCenter = new Point(Bounds.Left + (Bounds.Width / 2), Bounds.Top + (Bounds.Height / 2));
            WindowCenter = new Point(Width / 2, Height / 2);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1f, Fog);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadMatrix(ref CameraMatrix);

            for (int x = -10; x <= 10; x++)
            {
                for (int z = -10; z <= 10; z++)
                {
                    GL.PushMatrix();
                    GL.Translate((float)x * 5f, 0f, (float)z * 5f);
                    MeshBuffers.DrawBuffers();
                    GL.PopMatrix();
                }
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.W])
            {
                CameraLocation.X += (float)Math.Cos(Facing) * 0.5f;//Originally was 0.1f on all.
                CameraLocation.Z += (float)Math.Sin(Facing) * 0.5f;
            }

            if (Keyboard[Key.S])
            {
                CameraLocation.X -= (float)Math.Cos(Facing) * 0.5f;
                CameraLocation.Z -= (float)Math.Sin(Facing) * 0.5f;
            }

            if (Keyboard[Key.A])
            {
                CameraLocation.X -= (float)Math.Cos(Facing + Math.PI / 2) * 0.5f;
                CameraLocation.Z -= (float)Math.Sin(Facing + Math.PI / 2) * 0.5f;
            }

            if (Keyboard[Key.D])
            {
                CameraLocation.X += (float)Math.Cos(Facing + Math.PI / 2) * 0.5f;
                CameraLocation.Z += (float)Math.Sin(Facing + Math.PI / 2) * 0.5f;
            }

            if (Keyboard[Key.LShift]) CameraLocation.Y -= 0.5f;

            if (Keyboard[Key.Space]) CameraLocation.Y += 0.5f;

            if (Keyboard[Key.R]) CameraLocation = SpawnLocation;

            MouseDelta = new Point(Mouse.X - WindowCenter.X, Mouse.Y - WindowCenter.Y);

            Point p = Cursor.Position;
            p.X -= MouseDelta.X;
            p.Y -= MouseDelta.Y;
            Cursor.Position = p;

            Facing += MouseDelta.X / (1000 - (float)(HorizontalSensitivity * 100));
            Pitch -= MouseDelta.Y / (1000 - (float)(VerticalSensitivity * 2 * 100));
            // because looking straight up or straight down (tand(90)) is a no-no.
            if (Pitch <= -1.5f) Pitch = -1.4999f; // 4 decimal places seems pretty smooth!?!?!?
            if (Pitch >= 1.5f) Pitch = 1.4999f;
            Vector3 lookatPoint = new Vector3((float)Math.Cos(Facing), (float)Math.Tan(Pitch), (float)Math.Sin(Facing));
            CameraMatrix = Matrix4.LookAt(CameraLocation, CameraLocation + lookatPoint, Up);

            if (Keyboard[Key.Escape]) Exit();
        }

        public static int ColorToRgba32(Color c) { return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R); }

        static void Main(string[] args)
        {
            Console.WriteLine("FirstPerson v" + Version + ", by Jonny Li");
            Console.WriteLine("Type \"help\" for a list of commands.");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                string command = input.Split(' ')[0].ToLower();
                string paramString = (input.Split(' ').Length > 1) ? input.Split(new char[] { ' ' }, 2)[1] : "";
                switch (command)
                {
                    case "help":
                        Console.WriteLine();
                        Console.WriteLine("=== COMMANDS ===");
                        Console.WriteLine("help - Displays a list of commands.");
                        Console.WriteLine("start - Starts the game.");
                        Console.WriteLine("fog [number] - Gets/sets fog limit.");
                        Console.WriteLine("hsensitivity [number] - Gets/sets horizontal mouse sensitivity.");
                        Console.WriteLine("vsensitivity [number] - Gets/sets vertical mouse sensitivity.");
                        Console.WriteLine("fps [number] - Gets/sets frames per second which the game should run on.");
                        Console.WriteLine();
                        Console.WriteLine("=== INFORMATION ===");
                        Console.WriteLine("This is FirstPerson v" + Version + ", by Jonny Li.");
                        Console.WriteLine("[] means optional command parameter.");
                        Console.WriteLine("<> means mandatory command parameter.");
                        Console.WriteLine("Use WASD to move left, right, forward, or backwards.");
                        Console.WriteLine("Use SPACE to go up vertically.");
                        Console.WriteLine("Use LSHIFT to go down vertically.");
                        Console.WriteLine("Use R to respawn.");
                        Console.WriteLine();
                        break;
                    case "start":
                        using (Window w = new Window()) w.Run(FPS);
                        break;
                    case "fog":
                        if (String.IsNullOrEmpty(paramString)) Console.WriteLine(Fog);
                        else
                        {
                            Fog = float.Parse(paramString);
                            Console.WriteLine("Fog set to " + Fog + ".");
                        }
                        break;
                    case "hsensitivity":
                        if (String.IsNullOrEmpty(paramString)) Console.WriteLine(HorizontalSensitivity);
                        else
                        {
                            HorizontalSensitivity = float.Parse(paramString);
                            Console.WriteLine("HorizontalSensitivity set to " + HorizontalSensitivity + ".");
                        }
                        break;
                    case "vsensitivity":
                        if (String.IsNullOrEmpty(paramString)) Console.WriteLine(VerticalSensitivity);
                        else
                        {
                            Fog = float.Parse(paramString);
                            Console.WriteLine("VerticalSensitivity set to " + VerticalSensitivity + ".");
                        }
                        break;
                    case "fps":
                        if (String.IsNullOrEmpty(paramString)) Console.WriteLine(FPS);
                        else
                        {
                            Fog = float.Parse(paramString);
                            Console.WriteLine("FPS set to " + FPS + ".");
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown command \"" + command + "\". Type \"help\" for a list of all commands.");
                        break;
                }
            }
        }
    }
}
