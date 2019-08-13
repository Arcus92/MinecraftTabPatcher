using System;
using System.IO;
using System.Text;

namespace JavaDecompiler
{
    /// <summary>
    /// DS 2019-08-09: The type of the constant info
    /// </summary>
    public enum ConstantInfoType : byte
    {
        Class = 7,
        FieldRef = 9,
        MethodRef = 10,
        InterfaceMethodRef = 11,
        String = 8,
        Integer = 3,
        Float = 4,
        Long = 5,
        Double = 6,
        NameAndType = 12,
        Utf8 = 1,
        MethodHandle = 15,
        MethodType = 16,
        InvokeDynamic = 18,
    }

    /// <summary>
    /// DS 2019-08-09: The constant info parent class
    /// </summary>
    public abstract class JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant type
        /// </summary>
        public abstract ConstantInfoType Type { get; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public abstract void Read(BinaryReader reader);

        /// <summary>
        /// Writes the constant info data
        /// </summary>
        /// <param name="writer"></param>
        protected abstract void WriteData(BinaryWriter writer);

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            WriteData(writer);
        }

        /// <summary>
        /// Creates and reads a java constant info
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static JavaConstantInfo CreateAndRead(BinaryReader reader)
        {
            // Reads the type
            var type = (ConstantInfoType)reader.ReadByte();
            var constant = Create(type);
            constant.Read(reader);
            return constant;
        }

        /// <summary>
        /// Creates a java constant info by its type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static JavaConstantInfo Create(ConstantInfoType type)
        {
            switch (type)
            {
                case ConstantInfoType.Class:
                    return new JavaConstantClassInfo();
                case ConstantInfoType.String:
                    return new JavaConstantStringInfo();
                case ConstantInfoType.NameAndType:
                    return new JavaConstantNameAndTypeInfo();
                case ConstantInfoType.Utf8:
                    return new JavaConstantUtf8Info();
                case ConstantInfoType.FieldRef:
                    return new JavaConstantFieldRefInfo();
                case ConstantInfoType.MethodRef:
                    return new JavaConstantMethodRefInfo();
                case ConstantInfoType.InterfaceMethodRef:
                    return new JavaConstantInterfaceMethodRefInfo();
                case ConstantInfoType.MethodHandle:
                    return new JavaConstantMethodHandleInfo();
                case ConstantInfoType.MethodType:
                    return new JavaConstantMethodTypeInfo();
                case ConstantInfoType.InvokeDynamic:
                    return new JavaConstantInvokeDynamicInfo();
                case ConstantInfoType.Integer:
                    return new JavaConstantIntegerInfo();
                case ConstantInfoType.Long:
                    return new JavaConstantLongInfo();
                case ConstantInfoType.Float:
                    return new JavaConstantFloatInfo();
                case ConstantInfoType.Double:
                    return new JavaConstantDoubleInfo();
                default:
                    throw new ArgumentException();
            }
        }

        #endregion Read & write
    }

    #region Child classes

    #region Class

    /// <summary>
    /// DS 2019-08-09: The constant info for classes
    /// </summary>
    public class JavaConstantClassInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.Class;

        /// <summary>
        /// Gets and sets the index of the name
        /// </summary>
        public ushort NameIndex { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            NameIndex = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(NameIndex);
        }

        #endregion Read & write
    }

    #endregion Class

    #region String

    /// <summary>
    /// DS 2019-08-09: The constant info for strings
    /// </summary>
    public class JavaConstantStringInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.String;

        /// <summary>
        /// Gets and sets the index of the string
        /// </summary>
        public ushort StringIndex { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            StringIndex = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(StringIndex);
        }

        #endregion Read & write
    }

    #endregion String

    #region Name and type

    /// <summary>
    /// DS 2019-08-09: The constant info for name and types
    /// </summary>
    public class JavaConstantNameAndTypeInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.NameAndType;

        /// <summary>
        /// Gets and sets the index of the name
        /// </summary>
        public ushort NameIndex { get; set; }

        /// <summary>
        /// Gets and sets the index of the descriptor
        /// </summary>
        public ushort DescriptorIndex { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            NameIndex = reader.ReadUInt16();
            DescriptorIndex = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(NameIndex);
            writer.Write(DescriptorIndex);
        }

        #endregion Read & write
    }

    #endregion Name and type

    #region Utf8

    /// <summary>
    /// DS 2019-08-09: The constant info for utf8 texts
    /// </summary>
    public class JavaConstantUtf8Info : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.Utf8;

        /// <summary>
        /// Gets the text value.
        /// TODO: Write a setter. Java uses a wierd big endian version of utf8 when dealing with mulibytes.
        /// I will ignore this for now. We can only read single byte chars. Any other char will be gibberish
        /// and can not be converted back!  
        /// You are not able to modify the string directly but you can modify the bytes.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets and sets the bytes of the utf8 string
        /// </summary>
        public byte[] Bytes { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            var len = reader.ReadUInt16();
            Bytes = reader.ReadBytes(len);
            Value = Encoding.UTF8.GetString(Bytes);
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            // TODO: Write a custom encoding for java utf8 and convert the string back to bytes.
            writer.Write((ushort)Bytes.Length);
            writer.Write(Bytes);
        }

        #endregion Read & write
    }

    #endregion Utf8

    #region Ref

    /// <summary>
    /// DS 2019-08-09: The constant info for references
    /// </summary>
    public abstract class JavaConstantRefInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets and sets the index of the class
        /// </summary>
        public ushort ClassIndex { get; set; }

        /// <summary>
        /// Gets and sets the index of the name and type
        /// </summary>
        public ushort NameAndTypeIndex { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            ClassIndex = reader.ReadUInt16();
            NameAndTypeIndex = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(ClassIndex);
            writer.Write(NameAndTypeIndex);
        }

        #endregion Read & write
    }

    /// <summary>
    /// DS 2019-08-09: The constant info for field references
    /// </summary>
    public class JavaConstantFieldRefInfo : JavaConstantRefInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.FieldRef;
    }

    /// <summary>
    /// DS 2019-08-09: The constant info for method references
    /// </summary>
    public class JavaConstantMethodRefInfo : JavaConstantRefInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.MethodRef;
    }

    /// <summary>
    /// DS 2019-08-09: The constant info for interface method references
    /// </summary>
    public class JavaConstantInterfaceMethodRefInfo : JavaConstantRefInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.InterfaceMethodRef;
    }

    #endregion Ref

    #region Method handler

    /// <summary>
    /// DS 2019-08-09: The constant info for method handles
    /// </summary>
    public class JavaConstantMethodHandleInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.MethodHandle;

        /// <summary>
        /// Gets and sets the type of the reference
        /// </summary>
        public JavaMethodHandlerType ReferenceType { get; set; }

        /// <summary>
        /// Gets and sets the index of the reference
        /// </summary>
        public ushort ReferenceIndex { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            ReferenceType = (JavaMethodHandlerType)reader.ReadByte();
            ReferenceIndex = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write((byte)ReferenceType);
            writer.Write(ReferenceIndex);
        }

        #endregion Read & write
    }

    #endregion Method handler

    #region Method type

    /// <summary>
    /// DS 2019-08-09: The constant info for method types
    /// </summary>
    public class JavaConstantMethodTypeInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.MethodType;

        /// <summary>
        /// Gets and sets the index of the descriptor
        /// </summary>
        public ushort DescriptorIndex { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            DescriptorIndex = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(DescriptorIndex);
        }

        #endregion Read & write
    }

    #endregion Method type

    #region Invoke dynamic

    /// <summary>
    /// DS 2019-08-09: The constant info for invoke dynamics
    /// </summary>
    public class JavaConstantInvokeDynamicInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.InvokeDynamic;

        /// <summary>
        /// Gets and sets the index of the method attribute index
        /// </summary>
        public ushort BootstrapMethodAttrIndex { get; set; }

        /// <summary>
        /// Gets and sets the index for the name and type
        /// </summary>
        public ushort NameAndTypeIndex { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            BootstrapMethodAttrIndex = reader.ReadUInt16();
            NameAndTypeIndex = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(BootstrapMethodAttrIndex);
            writer.Write(NameAndTypeIndex);
        }

        #endregion Read & write
    }

    #endregion Invoke dynamic

    #region Integer

    /// <summary>
    /// DS 2019-08-09: The constant info for integers
    /// </summary>
    public class JavaConstantIntegerInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.Integer;

        /// <summary>
        /// Gets and sets the value
        /// </summary>
        public int Value { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = reader.ReadInt32();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        #endregion Read & write
    }

    #endregion Integer

    #region Long

    /// <summary>
    /// DS 2019-08-09: The constant info for longs
    /// </summary>
    public class JavaConstantLongInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.Long;

        /// <summary>
        /// Gets and sets the value
        /// </summary>
        public long Value { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = reader.ReadInt64();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        #endregion Read & write
    }

    #endregion Long

    #region Float

    /// <summary>
    /// DS 2019-08-09: The constant info for floats
    /// </summary>
    public class JavaConstantFloatInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.Float;

        /// <summary>
        /// Gets and sets the value
        /// </summary>
        public float Value { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = reader.ReadSingle();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        #endregion Read & write
    }

    #endregion Float

    #region Double

    /// <summary>
    /// DS 2019-08-09: The constant info for doubles
    /// </summary>
    public class JavaConstantDoubleInfo : JavaConstantInfo
    {
        /// <summary>
        /// Gets the constant info type
        /// </summary>
        public override ConstantInfoType Type => ConstantInfoType.Double;

        /// <summary>
        /// Gets and sets the value
        /// </summary>
        public double Value { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the constant info
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = reader.ReadDouble();
        }

        /// <summary>
        /// Writes the constant info
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        #endregion Read & write
    }

    #endregion Double

    #endregion Chils classes
}
