using System;
using System.Collections.Generic;

namespace BasicAsyncServer
{
    class ShapePackage
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ShapePackage(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        // Serialize object
        public ShapePackage(byte[] data)
        {
            X = BitConverter.ToInt32(data, 0);
            Y = BitConverter.ToInt32(data, 4);
        }

        public byte[] ToByteArray()
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(X));
            byteList.AddRange(BitConverter.GetBytes(Y));

            return byteList.ToArray();
        }

    }
}
