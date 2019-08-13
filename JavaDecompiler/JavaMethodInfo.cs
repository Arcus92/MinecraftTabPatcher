using System.IO;
using System.Linq;

namespace JavaDecompiler
{
    /// <summary>
    /// DS 2019-08-09: The method info of a java class
    /// </summary>
    public class JavaMethodInfo
    {
        /// <summary>
        /// Gets and sets the access flag
        /// </summary>
        public JavaAccessFlag AccessFlag { get; set; }

        /// <summary>
        /// Gets and sets the index of the name
        /// </summary>
        public ushort NameIndex { get; set; }

        /// <summary>
        /// Gets and sets the index of the descriptor
        /// </summary>
        public ushort DescriptorIndex { get; set; }

        /// <summary>
        /// Gets and sets the attributes
        /// </summary>
        public JavaAttributeInfo[] Attributes { get; set; }

        #region Read & write

        /// <summary>
        /// Reads the method info
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        public void Read(BinaryReader reader, JavaClass javaClass)
        {
            AccessFlag = (JavaAccessFlag)reader.ReadUInt16();
            NameIndex = reader.ReadUInt16();
            DescriptorIndex = reader.ReadUInt16();

            // Reads the attributes
            var attributes = reader.ReadUInt16();
            Attributes = new JavaAttributeInfo[attributes];
            for (int i = 0; i < attributes; i++)
            {
                Attributes[i] = JavaAttributeInfo.CreateAndRead(reader, javaClass);
            }
        }

        /// <summary>
        /// Writes the method info
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write((ushort)AccessFlag);
            writer.Write(NameIndex);
            writer.Write(DescriptorIndex);

            // Writes the attributes
            var attributes = (ushort)Attributes.Length;
            writer.Write(attributes);
            for (int i = 0; i < attributes; i++)
            {
                Attributes[i].Write(writer);
            }
        }

        /// <summary>
        /// Creates and reads the method info
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="javaClass"></param>
        /// <returns></returns>
        public static JavaMethodInfo CreateAndRead(BinaryReader reader, JavaClass javaClass)
        {
            var method = new JavaMethodInfo();
            method.Read(reader, javaClass);
            return method;
        }

        #endregion Read & write

        #region Utils

        /// <summary>
        /// Returns the code attribute of this method
        /// </summary>
        /// <returns></returns>
        public JavaAttributeCodeInfo GetCodeAttribute()
        {
            return (JavaAttributeCodeInfo)Attributes.FirstOrDefault(a => a is JavaAttributeCodeInfo);
        }

        #endregion Utils
    }
}
