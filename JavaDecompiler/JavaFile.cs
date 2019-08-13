using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace JavaDecompiler
{
    /// <summary>
    /// DS 2019-08-09: The primary java file (jar). This wile contains all ressources and class files.
    /// A jar file is basically just a fancy zip file.
    /// </summary>
    public class JavaFile : IDisposable
    {
        /// <summary>
        /// Creates the zip file from the given stream
        /// </summary>
        /// <param name="stream"></param>
        public JavaFile(Stream stream)
        {
            m_Zip = new ZipFile(stream);
        }

        /// <summary>
        /// Creates the zip file from the given stream
        /// </summary>
        /// <param name="path"></param>
        public JavaFile(string path) : this(new FileStream(path, FileMode.Open, FileAccess.Read))
        {
        }

        #region Zip

        /// <summary>
        /// The zip file
        /// </summary>
        private ZipFile m_Zip;

        /// <summary>
        /// Gets the internal zip reader
        /// </summary>
        public ZipFile Zip
        {
            get { return m_Zip; }
        }

        /// <summary>
        /// Returns the reader file stream for a file inside the jar file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Stream GetFileStream(string path)
        {
            var entry = m_Zip.GetEntry(path);
            return GetFileStream(entry);
        }

        /// <summary>
        /// Returns the reader file stream for a file inside the jar file
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Stream GetFileStream(ZipEntry entry)
        {
            return m_Zip.GetInputStream(entry);
        }

        /// <summary>
        /// Returns a list of all class files
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetClassFiles()
        {
            var len = m_Zip.Count;
            for (int i = 0; i < len; i++)
            {
                var entry = m_Zip[i];
                if (Path.GetExtension(entry.Name) == ".class")
                {
                    yield return entry.Name;
                }
            }
        }

        /// <summary>
        /// Returns a class file by its name
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public JavaClass GetClass(string path)
        {
            using (var stream = GetFileStream(path))
            {
                var javaClass = new JavaClass();
                javaClass.Read(stream);
                return javaClass;
            }
        }

        /// <summary>
        /// Returns a class file from the zip entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public JavaClass GetClass(ZipEntry entry)
        {
            using (var stream = GetFileStream(entry))
            {
                var javaClass = new JavaClass();
                javaClass.Read(stream);
                return javaClass;
            }
        }

        /// <summary>
        /// Close the file
        /// </summary>
        public void Close()
        {
            m_Zip.Close();
        }

        #endregion Zip

        #region IDisposable

        /// <summary>
        /// Dispose the file
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion IDisposable
    }
}
