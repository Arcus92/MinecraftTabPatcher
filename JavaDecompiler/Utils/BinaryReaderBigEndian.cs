using System;
using System.IO;
using System.Text;

namespace JavaDecompiler.Utils
{
    /// <summary>
    /// DS 2019-08-09: A binary reader that reads files in big endian.
    /// Not all method are supported yet! 
    /// </summary>
    public class BinaryReaderBigEndian : BinaryReader
    {
        /// <summary>
        /// Creates the binary reader from stream
        /// </summary>
        /// <param name="input"></param>
        public BinaryReaderBigEndian(Stream input) : base(input, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Creates the binary reader from stream
        /// </summary>
        /// <param name="input"></param>
        /// <param name="leaveOpen"></param>
        public BinaryReaderBigEndian(Stream input, bool leaveOpen) : base(input, Encoding.UTF8, leaveOpen)
        {
        }
        
        /// <summary>
        /// Reads the next float
        /// </summary>
        /// <returns></returns>
        public override float ReadSingle()
        {
            var data = base.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// Reads the next double
        /// </summary>
        /// <returns></returns>
        public override double ReadDouble()
        {
            var data = base.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }

        /// <summary>
        /// Reasd the next short
        /// </summary>
        /// <returns></returns>
        public override short ReadInt16()
        {
            var data = base.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// Reads the next integer
        /// </summary>
        /// <returns></returns>
        public override int ReadInt32()
        {
            var data = base.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Reads the next long
        /// </summary>
        /// <returns></returns>
        public override long ReadInt64()
        {
            var data = base.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }
        
        /// <summary>
        /// Reads the next unsigned short
        /// </summary>
        /// <returns></returns>
        public override ushort ReadUInt16()
        {
            var data = base.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        /// Reads the next unsigned integer
        /// </summary>
        /// <returns></returns>
        public override uint ReadUInt32()
        {
            var data = base.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        /// Reads the next unsigned long
        /// </summary>
        /// <returns></returns>
        public override ulong ReadUInt64()
        {
            var data = base.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }

    }
}
