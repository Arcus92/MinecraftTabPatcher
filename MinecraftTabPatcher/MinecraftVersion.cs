using JavaDecompiler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinecraftTabPatcher
{
    /// <summary>
    /// DS 2019-08-10: The class of the minecraft version.
    /// A version is a jar file inside the minecraft directory.
    /// Minecraft can have multiple versions each with multiple
    /// profiles.
    /// The versions are located at:
    /// Windows: %appdata%/.minecraft/versions
    /// </summary>
    public class MinecraftVersion
    {
        /// <summary>
        /// Gets the id of the version
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the path of the version
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the path of the jar file
        /// </summary>
        public string PathJar
        {
            get { return System.IO.Path.Combine(Path, ID + ".jar"); }
        }

        /// <summary>
        /// Gets the path of the json file
        /// </summary>
        public string PathJson
        {
            get { return System.IO.Path.Combine(Path, ID + ".json"); }
        }

        /// <summary>
        /// Gets if this version is valid.
        /// This will check if the jar and json file exists.
        /// </summary>
        public bool IsValid
        {
            get { return File.Exists(PathJar) && File.Exists(PathJson); }
        }

        /// <summary>
        /// Creates the miencraft version by the given path
        /// </summary>
        /// <param name="path"></param>
        public MinecraftVersion(string path)
        {
            Path = path;

            // The id is the directory name
            ID = System.IO.Path.GetFileName(path);

            // Creates the directory
            Directory.CreateDirectory(path);
        }      
        
        /// <summary>
        /// Opens the jar file
        /// </summary>
        /// <returns></returns>
        public JavaFile OpenJavaFile()
        {
            return new JavaFile(PathJar);
        }

        /// <summary>
        /// Writes the json file
        /// </summary>
        /// <param name="info"></param>
        public void WriteJsonFile(MinecraftVersionInfo info)
        {
            WriteJsonFile(PathJson, info);
        }

        #region Static

        /// <summary>
        /// Writes the json file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="info"></param>
        public static void WriteJsonFile(string path, MinecraftVersionInfo info)
        {
            // Opens the json file
            using (var stream = new FileStream(path, FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    // Creates the json writer
                    using (var writer = new JsonTextWriter(streamWriter))
                    {
                        writer.Formatting = Formatting.Indented;

                        writer.WriteStartObject();
                        writer.WritePropertyName("id");
                        writer.WriteValue(info.ID);

                        if (!string.IsNullOrEmpty(info.InheritsFrom))
                        {
                            writer.WritePropertyName("inheritsFrom");
                            writer.WriteValue(info.InheritsFrom);
                        }

                        if (!string.IsNullOrEmpty(info.Type))
                        {
                            writer.WritePropertyName("type");
                            writer.WriteValue(info.Type);
                        }

                        if (!string.IsNullOrEmpty(info.MainClass))
                        {
                            writer.WritePropertyName("mainClass");
                            writer.WriteValue(info.MainClass);
                        }

                        if (info.MinimumLauncherVersion != 0)
                        {
                            writer.WritePropertyName("minimumLauncherVersion");
                            writer.WriteValue(info.MinimumLauncherVersion.ToString());
                        }

                        // Writes the empty arguments object.
                        writer.WritePropertyName("arguments");
                        writer.WriteStartObject();
                        writer.WriteEndObject();

                        // Prevents the launcher from redownloading the jar file
                        // by creating an empty downloads object.
                        writer.WritePropertyName("downloads");
                        writer.WriteStartObject();
                        writer.WriteEndObject();

                        writer.WriteEndObject();
                    }
                }
            }
        }

        /// <summary>
        /// Returns the directory for the current version
        /// </summary>
        /// <returns></returns>
        public static string GetSystemVersionDirectory()
        {
            // TODO: Other systems
            return Environment.ExpandEnvironmentVariables("%appdata%/.minecraft/versions");
        }

        /// <summary>
        /// Returns all minecraft version from the system version directory.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MinecraftVersion> GetVersions()
        {
            var path = GetSystemVersionDirectory();

            // Directory is not valid
            if (!Directory.Exists(path))
                yield break;

            foreach(var version in GetVersions(path))
            {
                yield return version;
            }
        }

        /// <summary>
        /// Returns all minecraft version from the version directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<MinecraftVersion> GetVersions(string path)
        {
            // Checks every sub directory for a minecraft version
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                var version = new MinecraftVersion(directory);
                if (version.IsValid)
                {
                    yield return version;
                }
            }
        }

        #endregion Static
    }
}
