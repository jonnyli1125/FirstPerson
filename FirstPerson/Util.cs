using System.Drawing;

namespace FirstPerson
{
    public class Util
    {
        public static int ColorToRgba32(Color c) { return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R); }
    }
}
