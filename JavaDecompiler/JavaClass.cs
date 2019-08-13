using JavaDecompiler.Exceptions;
using JavaDecompiler.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JavaDecompiler
{
    /// <summary>
    /// DS 2019-08-09: The parser for the java class files.
    /// The format is described here: https://docs.oracle.com/javase/specs/jvms/se7/html/jvms-4.html
    /// </summary>
    public class JavaClass
    {
        #region Const

        /// <summary>
        /// The magic number for all java classes
        /// </summary>
        public const uint MagicNumber = 0xCAFEBABE;

        #endregion Const

        /// <summary>
        /// The minor version of java
        /// </summary>
        public ushort MinorVersion { get; set; }

        /// <summary>
        /// The major version of java
        /// </summary>
        public ushort MajorVersion { get; set; }

        /// <summary>
        /// The constant pool
        /// </summary>
        public JavaConstantInfo[] ConstantPool { get; set; }

        /// <summary>
        /// The access flag for this class
        /// </summary>
        public JavaAccessFlag AccessFlag { get; set; }

        /// <summary>
        /// The constant index for the current class.
        /// </summary>
        public ushort ThisClass { get; set; }

        /// <summary>
        /// The constant index for the super class.
        /// Zero means the super class is java.object.
        /// </summary>
        public ushort SuperClass { get; set; }

        /// <summary>
        /// The constant indices for all interfaces.
        /// </summary>
        public ushort[] Interfaces { get; set; }

        /// <summary>
        /// The field info for this class
        /// </summary>
        public JavaFieldInfo[] Fields { get; set; }

        /// <summary>
        /// The method info for this class
        /// </summary>
        public JavaMethodInfo[] Methods { get; set; }

        /// <summary>
        /// The attributes for this class
        /// </summary>
        public JavaAttributeInfo[] Attributes { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the class file from the given file path
        /// </summary>
        /// <param name="path"></param>
        public void Read(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                Read(stream);
            }
        }

        /// <summary>
        /// Reads the class file by the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            Read(new BinaryReaderBigEndian(stream));
        }

        /// <summary>
        /// Reads the class file by the given binary reader.
        /// Remember: This must be an big endian reader!
        /// </summary>
        /// <param name="reader"></param>
        public void Read(BinaryReader reader)
        {
            // Reads the magic number
            var magic = reader.ReadUInt32();
            if (magic != MagicNumber)
            {
                throw new JavaClassMagicNumberException();
            }
            MinorVersion = reader.ReadUInt16();
            MajorVersion = reader.ReadUInt16();

            // Reads the constant pool.
            // The first constnat (0) is always zero.
            // The first valid index is 1.
            var constants = reader.ReadUInt16();
            ConstantPool = new JavaConstantInfo[constants];
            for (int i = 1; i < constants; i++)
            {
                var constant = JavaConstantInfo.CreateAndRead(reader);
                ConstantPool[i] = constant;

                // There is an odd special case for double and long.
                // For some reasons you have to add an empty index
                // if you read an double or long constant.
                if (constant.Type == ConstantInfoType.Double ||
                    constant.Type == ConstantInfoType.Long)
                {
                    i++;
                }
            }

            AccessFlag = (JavaAccessFlag)reader.ReadUInt16();
            ThisClass = reader.ReadUInt16();
            SuperClass = reader.ReadUInt16();

            // Reads the interfaces
            var interfaces = reader.ReadUInt16();
            Interfaces = new ushort[interfaces];
            for (int i = 0; i < interfaces; i++)
            {
                Interfaces[i] = reader.ReadUInt16();
            }

            // Reads the fields
            var fields = reader.ReadUInt16();
            Fields = new JavaFieldInfo[fields];
            for (int i = 0; i < fields; i++)
            {
                Fields[i] = JavaFieldInfo.CreateAndRead(reader, this);
            }

            // Reads the methods
            var methods = reader.ReadUInt16();
            Methods = new JavaMethodInfo[methods];
            for (int i = 0; i < methods; i++)
            {
                Methods[i] = JavaMethodInfo.CreateAndRead(reader, this);
            }

            // Reads the attributes
            var attributes = reader.ReadUInt16();
            Attributes = new JavaAttributeInfo[attributes];
            for (int i = 0; i < attributes; i++)
            {
                Attributes[i] = JavaAttributeInfo.CreateAndRead(reader, this);
            }
        }

        /// <summary>
        /// Writes the class file to the given file path
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                Write(stream);
            }
        }

        /// <summary>
        /// Writes the class file to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            using (var writer = new BinaryWriterBigEndian(stream, true))
            {
                Write(writer);
            }
        }

        /// <summary>
        /// Writes the class file to the given binary writer.
        /// Remember: This must be an big endian writer!
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(MagicNumber);
            writer.Write(MinorVersion);
            writer.Write(MajorVersion);

            // Writes the constant pool
            var constants = (ushort)ConstantPool.Length;
            writer.Write(constants);
            for (int i = 0; i < constants; i++)
            {
                var constant = ConstantPool[i];
                if (constant != null)
                {
                    constant.Write(writer);
                }
            }

            writer.Write((ushort)AccessFlag);
            writer.Write(ThisClass);
            writer.Write(SuperClass);

            // Writes the interfaces
            var interfaces = (ushort)Interfaces.Length;
            writer.Write(interfaces);
            for (int i = 0; i < interfaces; i++)
            {
                writer.Write(Interfaces[i]);
            }

            // Writes the fields
            var fields = (ushort)Fields.Length;
            writer.Write(fields);
            for (int i = 0; i < fields; i++)
            {
                Fields[i].Write(writer);
            }

            // Writes the methods
            var methods = (ushort)Methods.Length;
            writer.Write(methods);
            for (int i = 0; i < methods; i++)
            {
                Methods[i].Write(writer);
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

        #region Method

        /// <summary>
        /// Returns a method with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JavaMethodInfo GetMethod(string name)
        {
            var nameIndex = GetConstantUtf8Index(name);

            return Methods.FirstOrDefault(m => m.NameIndex == nameIndex);
        }

        /// <summary>
        /// Returns all methods with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<JavaMethodInfo> GetMethods(string name)
        {
            var nameIndex = GetConstantUtf8Index(name);

            return Methods.Where(m => m.NameIndex == nameIndex);
        }

        /// <summary>
        /// Returns a method with the given name and descriptor
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JavaMethodInfo GetMethod(string name, string descriptor)
        {
            var nameIndex = GetConstantUtf8Index(name);
            var descriptorIndex = GetConstantUtf8Index(descriptor);

            return Methods.FirstOrDefault(m => m.NameIndex == nameIndex && m.DescriptorIndex == descriptorIndex);
        }


        #endregion Method

        #region Constants

        /// <summary>
        /// Returns the constant with the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public JavaConstantInfo GetConstant(int index)
        {
            return ConstantPool[index];
        }

        /// <summary>
        /// Returns the constant with the given index and the given type
        /// </summary>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConstant<T>(int index) where T : JavaConstantInfo
        {
            return ConstantPool[index] as T;
        }

        /// <summary>
        /// Returns the utf8 text at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetConstantUtf8(int index)
        {
            return GetConstant<JavaConstantUtf8Info>(index).Value;
        }

        /// <summary>
        /// Returns the index from the constant pool of the given utf8
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ushort GetConstantUtf8Index(string value)
        {
            var len = ConstantPool.Length;
            for (ushort i = 0; i < len; i++)
            {
                var constant = ConstantPool[i];
                if (constant is JavaConstantUtf8Info utf8)
                {
                    if (utf8.Value == value)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        #endregion Constants
    }
}
