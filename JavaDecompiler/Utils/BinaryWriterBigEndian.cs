using System;
using System.IO;
using System.Text;

namespace JavaDecompiler.Utils
{
    /// <summary>
    /// DS 2019-08-09: A binary writer that writes files in big endian.
    /// Not all method are supported yet! 
    /// </summary>
    public class BinaryWriterBigEndian : BinaryWriter
    {
        /// <summary>
        /// Creates the binary writer from stream
        /// </summary>
        /// <param name="output"></param>
        public BinaryWriterBigEndian(Stream output) : base(output, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Creates the binary writer from stream with the given encoding
        /// </summary>
        /// <param name="output"></param>
        /// <param name="leaveOpen"></param>
        public BinaryWriterBigEndian(Stream output, bool leaveOpen) : base(output, Encoding.UTF8, leaveOpen)
        {
        }

        /// <summary>
        /// Writes the float
        /// </summary>
        /// <param name="value"></param>
        public override void Write(float value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }

        /// <summary>
        /// Writes the double
        /// </summary>
        /// <param name="value"></param>
        public override void Write(double value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }

        /// <summary>
        /// Writes the short
        /// </summary>
        /// <param name="value"></param>
        public override void Write(short value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }

        /// <summary>
        /// Writes the integer
        /// </summary>
        /// <param name="value"></param>
        public override void Write(int value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }

        /// <summary>
        /// Writes the long
        /// </summary>
        /// <param name="value"></param>
        public override void Write(long value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }

        /// <summary>
        /// Writes the unsigned short
        /// </summary>
        /// <param name="value"></param>
        public override void Write(ushort value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }

        /// <summary>
        /// Writes the unsigned integer
        /// </summary>
        /// <param name="value"></param>
        public override void Write(uint value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }

        /// <summary>
        /// Writes the unsigned long
        /// </summary>
        /// <param name="value"></param>
        public override void Write(ulong value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            base.Write(data);
        }
    }
}
