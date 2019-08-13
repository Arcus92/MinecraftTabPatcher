using System.IO;

namespace JavaDecompiler
{
    /// <summary>
    /// DS 2019-08-09: The java exception table for the code attributes
    /// </summary>
    public class JavaExceptionTable
    {
        /// <summary>
        /// Gets and sets the start position of the exception handler
        /// </summary>
        public ushort StartPC { get; set; }

        /// <summary>
        /// Gets and sets the end position of the exception handler
        /// </summary>
        public ushort EndPC { get; set; }

        /// <summary>
        /// Gets and sets the handler
        /// </summary>
        public ushort HandlerPC { get; set; }

        /// <summary>
        /// Gets and sets the catch type
        /// </summary>
        public ushort CatchType { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the exception table
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        public void Read(BinaryReader reader, JavaClass javaClass)
        {
            StartPC = reader.ReadUInt16();
            EndPC = reader.ReadUInt16();
            HandlerPC = reader.ReadUInt16();
            CatchType = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the exception table
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(StartPC);
            writer.Write(EndPC);
            writer.Write(HandlerPC);
            writer.Write(CatchType);
        }

        /// <summary>
        /// Creates and reads the exception table
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        /// <returns></returns>
        public static JavaExceptionTable CreateAndRead(BinaryReader reader, JavaClass javaClass)
        {
            var exceptionTable = new JavaExceptionTable();
            exceptionTable.Read(reader, javaClass);
            return exceptionTable;
        }

        #endregion Read & write
    }
}
