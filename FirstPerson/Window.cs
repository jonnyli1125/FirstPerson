using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;

namespace FirstPerson
{
    public class Window : GameWindow
    {
        public static readonly string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static Camera Camera;
        private Vector3 SpawnPosition = new Vector3(0f, 5f, 0f);
        private MeshBuffer MeshBuffer;
        private static uint GridWidth = 32;
        private static uint GridLength = 32;
        private static float MoveVelocity = 1;

        public Window() : base(1280, 720, GraphicsMode.Default, "FirstPerson") { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            Camera = new Camera(this, SpawnPosition);
            MeshBuffer = MeshBuffer.Generate(
                new Vector3[] { 
				    new Vector3 (-1.0f, -1.0f, 1.0f), 
				    new Vector3 (1.0f, -1.0f, 1.0f), 
				    new Vector3 (1.0f, 1.0f, 1.0f), 
				    new Vector3 (-1.0f, 1.0f, 1.0f), 
				    new Vector3 (-1.0f, -1.0f, -1.0f), 
				    new Vector3 (1.0f, -1.0f, -1.0f), 
				    new Vector3 (1.0f, 1.0f, -1.0f), 
				    new Vector3 (-1.0f, 1.0f, -1.0f) 
			    }, new Vector3[] { 
				    new Vector3 (-1.0f, -1.0f, 1.0f), 
				    new Vector3 (1.0f, -1.0f, 1.0f), 
				    new Vector3 (1.0f, 1.0f, 1.0f), 
				    new Vector3 (-1.0f, 1.0f, 1.0f), 
				    new Vector3 (-1.0f, -1.0f, -1.0f), 
				    new Vector3 (1.0f, -1.0f, -1.0f), 
				    new Vector3 (1.0f, 1.0f, -1.0f), 
				    new Vector3 (-1.0f, 1.0f, -1.0f) 
			    }, new int[] {
				    Util.ColorToRgba32 (Color.Cyan), // bottom right front
				    Util.ColorToRgba32 (Color.Cyan), // bottom right back
				    Util.ColorToRgba32 (Color.DarkCyan), // top right front
				    Util.ColorToRgba32 (Color.DarkCyan), // top right back
				    Util.ColorToRgba32 (Color.Cyan), // bottom left front
				    Util.ColorToRgba32 (Color.Cyan), // bottom left back
				    Util.ColorToRgba32 (Color.DarkCyan), // top left front
				    Util.ColorToRgba32 (Color.DarkCyan) // top left back
			    }, new uint[] { 
				    0, 1, 2, 2, 3, 0, 
				    3, 2, 6, 6, 7, 3, 
				    7, 6, 5, 5, 4, 7, 
				    4, 0, 3, 3, 7, 4, 
				    0, 1, 5, 5, 4, 0,
				    1, 5, 6, 6, 2, 1 
			    }
            );
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadMatrix(ref Camera.CameraMatrix);
            int halfGridWidth = Convert.ToInt32(GridWidth / 2), halfGridLength = Convert.ToInt32(GridLength / 2);
            for (int x = -halfGridWidth; x <= halfGridWidth; x++)
            {
                for (int z = -halfGridLength; z <= halfGridLength; z++)
                {
                    GL.PushMatrix();
                    GL.Translate((float)x * 5f, 0f, (float)z * 5f);
                    MeshBuffer.DrawBuffers();
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
                Camera.X += (float)Math.Cos(Camera.Facing) * MoveVelocity;
                Camera.Z += (float)Math.Sin(Camera.Facing) * MoveVelocity;
                Camera.Y += (float)Math.Sin(Camera.Pitch) * MoveVelocity;
            }
            if (Keyboard[Key.S])
            {
                Camera.X -= (float)Math.Cos(Camera.Facing) * MoveVelocity;
                Camera.Z -= (float)Math.Sin(Camera.Facing) * MoveVelocity;
                Camera.Y -= (float)Math.Sin(Camera.Pitch) * MoveVelocity;
            }
            if (Keyboard[Key.A])
            {
                Camera.X -= (float)Math.Cos(Camera.Facing + Math.PI / 2) * MoveVelocity;
                Camera.Z -= (float)Math.Sin(Camera.Facing + Math.PI / 2) * MoveVelocity;
            }
            if (Keyboard[Key.D])
            {
                Camera.X += (float)Math.Cos(Camera.Facing + Math.PI / 2) * MoveVelocity;
                Camera.Z += (float)Math.Sin(Camera.Facing + Math.PI / 2) * MoveVelocity;
            }
            if (Keyboard[Key.R]) Camera.Position = SpawnPosition;
            if (Keyboard[Key.Escape]) Exit();
        }

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
                    case "": break;
                    case "help":
                        Console.WriteLine();
                        Console.WriteLine("=== COMMANDS ===");
                        Console.WriteLine("help - Displays a list of commands.");
                        Console.WriteLine("start - Starts the game.");
                        Console.WriteLine("gridwidth [number] - Gets/sets grid width.");
                        Console.WriteLine("gridlength [number] - Gets/sets grid length.");
                        Console.WriteLine("exit - Exit this program.");
                        Console.WriteLine();
                        Console.WriteLine("=== INFORMATION ===");
                        Console.WriteLine("This is FirstPerson v" + Version + ", by Jonny Li.");
                        Console.WriteLine("[] means optional command parameter.");
                        Console.WriteLine("<> means mandatory command parameter.");
                        Console.WriteLine("Use WASD to move left, right, forward, backwards, up, or down.");
                        Console.WriteLine("Use R to respawn.");
                        Console.WriteLine("Use ESC to exit game.");
                        Console.WriteLine();
                        break;
                    case "start":
                        using (Window w = new Window()) w.Run(60f);
                        break;
                    case "gridwidth":
                        if (String.IsNullOrEmpty(paramString)) Console.WriteLine(GridWidth);
                        else
                        {
                            GridWidth = uint.Parse(paramString);
                            Console.WriteLine("GridWidth set to " + GridWidth + ".");
                        }
                        break;
                    case "gridlength":
                        if (String.IsNullOrEmpty(paramString)) Console.WriteLine(GridLength);
                        else
                        {
                            GridLength = uint.Parse(paramString);
                            Console.WriteLine("GridLength set to " + GridLength + ".");
                        }
                        break;
                    case "exit":
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        break;
                    default:
                        Console.WriteLine("Unknown command \"" + command + "\". Type \"help\" for a list of all commands.");
                        break;
                }
            }
        }
    }
}
