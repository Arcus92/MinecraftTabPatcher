using System.IO;

namespace JavaDecompiler
{
    /// <summary>
    /// DS 2019-08-09: The attribute info for method, fields and classes
    /// </summary>
    public abstract class JavaAttributeInfo
    {
        /// <summary>
        /// Gets and sets the index of the name
        /// </summary>
        public ushort NameIndex { get; set; }

        /// <summary>
        /// Gets the data length
        /// </summary>
        protected uint Length { get; set; }

        #region Read & write

        /// <summary>
        /// Recalculates the length.
        /// This will be executed when the class file is saved.
        /// </summary>
        /// <returns></returns>
        public abstract uint CalculateLength();

        /// <summary>
        /// Reads the attribute info
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        public abstract void Read(BinaryReader reader, JavaClass javaClass);

        /// <summary>
        /// Writes the attribute info data
        /// </summary>
        /// <param name="writer"></param>
        protected abstract void WriteData(BinaryWriter writer);

        /// <summary>
        /// Writes the attribute info
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            // Updates the length
            Length = CalculateLength();

            writer.Write(NameIndex);
            writer.Write(Length);

            // Writes the data
            WriteData(writer);
        }

        /// <summary>
        /// Creates and read the attribute info
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        /// <returns></returns>
        public static JavaAttributeInfo CreateAndRead(BinaryReader reader, JavaClass javaClass)
        {
            var nameIndex = reader.ReadUInt16();
            var length = reader.ReadUInt32();

            // Gets the type
            var type = javaClass.GetConstantUtf8(nameIndex);
            var attribute = Create(type);
            attribute.NameIndex = nameIndex;
            attribute.Length = length;
            attribute.Read(reader, javaClass);
            return attribute;
        }

        /// <summary>
        /// Creates a new attribute by the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static JavaAttributeInfo Create(string type)
        {
            switch (type)
            {
                case "Code":
                    return new JavaAttributeCodeInfo();
                default:
                    return new JavaAttributeUnknwonInfo();
            }
        }

        #endregion Read & write
    }

    #region Child classes

    // TODO: Add all the other classes if needed

    #region Code

    /// <summary>
    /// DS 2019-08-09: The attribute info for the code
    /// </summary>
    public class JavaAttributeCodeInfo : JavaAttributeInfo
    {
        /// <summary>
        /// Gets and sets the max stack size
        /// </summary>
        public ushort MaxStack { get; set; }

        /// <summary>
        /// Gets and sets the max size for local variables
        /// </summary>
        public ushort MaxLocals { get; set; }

        /// <summary>
        /// Gets and sets the java byte code
        /// </summary>
        public byte[] Code { get; set; }

        /// <summary>
        /// Gets and sets the exceptions
        /// </summary>
        public JavaExceptionTable[] Exceptions { get; set; }

        /// <summary>
        /// Gets and sets the attributes
        /// </summary>
        public JavaAttributeInfo[] Attributes { get; set; }

        #region Read & write

        /// <summary>
        /// Recalculates the length of the data
        /// </summary>
        /// <returns></returns>
        public override uint CalculateLength()
        {
            uint size = sizeof(ushort) * 2; // MaxStack and MaxLocals
            size += sizeof(uint) + (uint)Code.Length; // Code
            size += sizeof(ushort) + sizeof(ushort) * 4 * (uint)Exceptions.Length; // Exception table

            // Attributes
            size += sizeof(ushort);
            var attributes = Attributes.Length;
            for (int i = 0; i < attributes; i++)
            {
                size += sizeof(ushort) + sizeof(uint) + Attributes[i].CalculateLength();
            }

            return size;
        }

        /// <summary>
        /// Reads the attribute info
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        public override void Read(BinaryReader reader, JavaClass javaClass)
        {
            MaxStack = reader.ReadUInt16();
            MaxLocals = reader.ReadUInt16();

            // Reads the code
            var len = reader.ReadUInt32();
            Code = reader.ReadBytes((int)len);

            // Reads the exception
            var exceptions = reader.ReadUInt16();
            Exceptions = new JavaExceptionTable[exceptions];
            for (int i = 0; i < exceptions; i++)
            {
                Exceptions[i] = JavaExceptionTable.CreateAndRead(reader, javaClass);
            }

            // Reads the attributes
            var attributes = reader.ReadUInt16();
            Attributes = new JavaAttributeInfo[attributes];
            for (int i = 0; i < attributes; i++)
            {
                Attributes[i] = JavaAttributeInfo.CreateAndRead(reader, javaClass);
            }
        }

        /// <summary>
        /// Writes the attribute info data
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(MaxStack);
            writer.Write(MaxLocals);

            // Writes the code
            writer.Write((uint)Code.Length);
            writer.Write(Code);

            // Writes the exceptions
            var exceptions = (ushort)Exceptions.Length;
            writer.Write(exceptions);
            for (int i = 0; i < exceptions; i++)
            {
                Exceptions[i].Write(writer);
            }

            // Writes the attributes
            var attributes = (ushort)Attributes.Length;
            writer.Write(attributes);
            for (int i = 0; i < attributes; i++)
            {
                Attributes[i].Write(writer);
            }
        }

        #endregion Read & write
    }

    #endregion Code

    #region Unknwon

    /// <summary>
    /// DS 2019-08-09: The attribute info for unknown types
    /// </summary>
    public class JavaAttributeUnknwonInfo : JavaAttributeInfo
    {
        /// <summary>
        /// Gets and sets the unknown data
        /// </summary>
        public byte[] Data { get; set; }

        #region Read & write

        /// <summary>
        /// Recalculates the length of the data
        /// </summary>
        /// <returns></returns>
        public override uint CalculateLength()
        {
            return (uint)Data.Length;
        }

        /// <summary>
        /// Reads the attribute info
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        public override void Read(BinaryReader reader, JavaClass javaClass)
        {
            Data = reader.ReadBytes((int)Length);
        }

        /// <summary>
        /// Writes the attribute info data
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(Data);
        }

        #endregion Read & write
    }

    #endregion Unknown

    #endregion Child classes
}
