using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace FirstPerson
{
    public class MeshBuffer
    {
        public Vector3[] VertexData;
        public Vector3[] NormalData;
        public int[] ColorData;
        public uint[] IndicesData;

        public int VertexBufferID;
        public int NormalBufferID;
        public int ColorBufferID;
        public int IndicesBufferID;

        public MeshBuffer() { }
        public MeshBuffer(Vector3[] vertexData, Vector3[] normalData, int[] colorData, uint[] indicesData)
        {
            VertexData = vertexData;
            NormalData = normalData;
            ColorData = colorData;
            IndicesData = indicesData;
        }

        public void GenerateBuffers()
        {
            if (VertexData != null)
            {
                GL.GenBuffers(1, out VertexBufferID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                       new IntPtr(VertexData.Length * Vector3.SizeInBytes),
                                       VertexData, BufferUsageHint.StaticDraw);
            }

            if (NormalData != null)
            {
                GL.GenBuffers(1, out NormalBufferID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBufferID);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                       new IntPtr(NormalData.Length * Vector3.SizeInBytes),
                                       NormalData, BufferUsageHint.StaticDraw);
            }

            if (ColorData != null)
            {
                GL.GenBuffers(1, out ColorBufferID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferID);
                GL.BufferData<int>(BufferTarget.ArrayBuffer,
                                   new IntPtr(ColorData.Length * sizeof(int)),
                                   ColorData, BufferUsageHint.StaticDraw);
            }

            if (IndicesData != null)
            {
                GL.GenBuffers(1, out IndicesBufferID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicesBufferID);
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                                    new IntPtr(IndicesData.Length * sizeof(uint)),
                                    IndicesData, BufferUsageHint.StaticDraw);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void DrawBuffers()
        {
            if (VertexBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID);
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
                GL.EnableClientState(ArrayCap.VertexArray);
            }

            if (ColorBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferID);
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(int), IntPtr.Zero);
                GL.EnableClientState(ArrayCap.ColorArray);
            }

            if (IndicesBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicesBufferID);
                GL.DrawElements(BeginMode.Triangles, IndicesData.Length,
                                DrawElementsType.UnsignedInt, IntPtr.Zero);
            }
        }

        public static MeshBuffer Generate(Vector3[] vertexData, Vector3[] normalData, int[] colorData, uint[] indicesData)
        {
            var meshBuffer = new MeshBuffer(vertexData, normalData, colorData, indicesData);
            meshBuffer.GenerateBuffers();
            return meshBuffer;
        }
    }
}
