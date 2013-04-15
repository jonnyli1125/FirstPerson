using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FirstPerson
{
    public class BoundingBox
    {
        public Vector3 Center;
        public Vector3 DistanceToEdge;
        public float Width
        {
            get { return DistanceToEdge.X; }
            set { DistanceToEdge.X = value; }
        }
        public float Height
        {
            get { return DistanceToEdge.Y; }
            set { DistanceToEdge.Y = value; }
        }
        public float Depth
        {
            get { return DistanceToEdge.Z; }
            set { DistanceToEdge.Z = value; }
        }
        public float Left
        {
            get { return Center.X - DistanceToEdge.X; }
            set { Center.X = value + DistanceToEdge.X; }
        }
        public float Right
        {
            get { return Center.X + DistanceToEdge.X; }
            set { Center.X = value - DistanceToEdge.X; }
        }
        public float Bottom
        {
            get { return Center.Y - DistanceToEdge.Y; }
            set { Center.Y = value + DistanceToEdge.Y; }
        }
        public float Top
        {
            get { return Center.Y + DistanceToEdge.Y; }
            set { Center.Y = value - DistanceToEdge.Y; }
        }
        public float Back
        {
            get { return Center.Z - DistanceToEdge.Z; }
            set { Center.Z = value + DistanceToEdge.Z; }
        }
        public float Front
        {
            get { return Center.Z + DistanceToEdge.Z; }
            set { Center.Z = value - DistanceToEdge.Z; }
        }
        public Vector3[] VertexData
        {
            get
            {
                return new Vector3[]
                {
                    new Vector3(Right, Bottom, Front),
                    new Vector3(Right, Bottom, Back),
                    new Vector3(Right, Top, Front),
                    new Vector3(Right, Top, Back),
                    new Vector3(Left, Bottom, Front),
                    new Vector3(Left, Bottom, Back),
                    new Vector3(Left, Top, Front),
                    new Vector3(Left, Top, Back)
                };
            }
        }
        
        public BoundingBox() { }
        public BoundingBox(Vector3 center, Vector3 distanceToEdge)
        {
            Center = center;
            DistanceToEdge = distanceToEdge;
        }
    }
}
