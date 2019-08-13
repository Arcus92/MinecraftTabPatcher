using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using JavaDecompiler;
using System;
using System.IO;
using System.Linq;

namespace MinecraftTabPatcher
{
    /// <summary>
    /// DS 2019-08-09: The main application entry point
    /// </summary>
    class Program
    {
        /// <summary>
        /// You start here
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Gets the version path
            var versionPath = MinecraftVersion.GetSystemVersionDirectory();

            Console.WriteLine("Minecraft-Tab-Patcher");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("This tool can patch any existing minecraft version and");
            Console.WriteLine("remove the ability to 'tab' to the next ui element. ");
            Console.WriteLine("The only purpose of this patch is to allow the user");
            Console.WriteLine("to enter and exit the inventory with the 'tab' key.");
            Console.WriteLine("Since minecraft 1.13 'tab' will focus the new recipe");
            Console.WriteLine("book when trying to close the inventory.");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Minecraft version directory:");
            Console.WriteLine(versionPath);
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Please continue at your own risk!");
            Console.WriteLine("<Press any key to continue>");
            Console.ReadKey();

            

            Console.Clear();
            var versions = MinecraftVersion.GetVersions(versionPath).OrderByDescending(v => v.ID).ToArray();
            if (versions.Length == 0)
            {
                Console.WriteLine("Could not find any installed minecraft versions at:");
                Console.WriteLine(versionPath);
                Console.ReadKey();
                return;
            }

            int selection = 0;
            bool loop = true;
            do {
                Console.WriteLine("Select the minecraft version you want to patch.");
                Console.WriteLine("Use the arrow keys to navigate and press enter to confirm.");
                for (int i = 0; i < versions.Length; i++)
                {
                    var version = versions[i];
                    if (selection == i)
                        Console.WriteLine(" -> {0}", version.ID);
                    else
                        Console.WriteLine("    {0}", version.ID);
                }

                var key = Console.ReadKey();
                switch (key.Key)
                {
                    // Cancel
                    case ConsoleKey.Escape:
                        return;

                    // Move selection down
                    case ConsoleKey.DownArrow:
                        selection = (selection + 1) % versions.Length;
                        break;

                    // Move selection up
                    case ConsoleKey.UpArrow:
                        selection = selection > 0 ? selection - 1 : versions.Length - 1;
                        break;

                    // Confirm
                    case ConsoleKey.Enter:
                        loop = false;
                        break;
                }
                Console.Clear();
            } while (loop);


            // The selected version
            MinecraftVersion selectedVersion = versions[selection];

            // Patch the version
            var patchedVersion = Patch(selectedVersion, "tabFix");

            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Open the minecraft launcher and create a new profile.");
            Console.WriteLine("Select the new version '{0}' and click 'save'.", patchedVersion.ID);
            Console.WriteLine("Now launch the game with your new profile selected!");
            Console.WriteLine("Bye!");
            Console.WriteLine("<Press any key to exit>");
            Console.ReadKey();
        }


        /// <summary>
        /// The original code. This code must be replaced.
        /// </summary>
        static readonly byte[] OrginalCode = new byte[] { 0x11, 0x01, 0x02 };

        /// <summary>
        /// Patches the given minecraft version and returns the new patched version.
        /// The patcher will create a new version directory and create a new jar file.
        /// The jar file is based on the original <paramref name="version"/>.
        /// The patcher will copy any assets from the orignal jar file.
        /// The content of the META-INF directory will not be copied.
        /// All classes will be analysed and scanned for a keyPressed method.
        /// The patcher will replace 0x110102 from the keyPressed methods.
        /// 0x11 (sipush) is the java bytecode instruction that loads the next two
        /// bytes as short value onto the stack. Where 0x0102 is the short value for
        /// 258 which is the minecraft keycode for tab. 0x0102 will be replaced with
        /// 0xFFFF which is an invalid keycode. 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        private static MinecraftVersion Patch(MinecraftVersion version, string postfix)
        {
            Console.WriteLine("Start pathching '{0}'...", version.ID);
            Console.WriteLine("------------------------------------------------------");

            // Creates a new version
            var parentPath = Path.GetDirectoryName(version.Path);

            // Searches for the next free directory.
            // Is this really necessary?
            var template = version.ID + "-" + postfix;
            var id = template;
            var path = Path.Combine(parentPath, id);
            var counter = 1;
            while (Directory.Exists(path))
            {
                counter++;
                id = template + counter;
                path = Path.Combine(parentPath, id);
            }

            Console.WriteLine("Creating patched version '{0}'...", id);

            // Creates the patched version
            var patchedVersion = new MinecraftVersion(path);

            // The copy buffer
            var buffer = new byte[1024 * 8];

            // Opens the java file
            using (var javaFile = version.OpenJavaFile())
            {
                // Creates the output file
                using (var output = new FileStream(patchedVersion.PathJar, FileMode.Create))
                {
                    // Creates the output zip stream
                    using (var outputZip = new ZipOutputStream(output))
                    {
                        // Do not use 64 bit zip
                        outputZip.UseZip64 = UseZip64.Off;

                        var files = javaFile.Zip.Count;
                        for (int i = 0; i < files; i++)
                        {
                            var entry = javaFile.Zip[i];
                            

                            // Ignore the META-INF folder
                            if (entry.Name.Contains("META-INF/"))
                            {
                                continue;
                            }

                            // Creates the output entry file
                            var outputEntry = new ZipEntry(entry.Name);
                            outputEntry.DateTime = entry.DateTime;
                            outputEntry.Size = entry.Size;
                            outputZip.PutNextEntry(outputEntry);

                            // This is a class file
                            if (Path.GetExtension(entry.Name) == ".class")
                            {
                                // Loads the class
                                var javaClass = javaFile.GetClass(entry);

                                // Gets the class info
                                var javaClassInfo = javaClass.GetConstant<JavaConstantClassInfo>(javaClass.ThisClass);
                                var javaClassName = javaClass.GetConstantUtf8(javaClassInfo.NameIndex);

                                // Gets the method
                                var javaMethod = javaClass.GetMethod("keyPressed");
                                
                                
                                if (javaMethod != null)
                                {
                                    // Gets the method info
                                    var javaMethodName = javaClass.GetConstantUtf8(javaMethod.NameIndex);
                                    var javaMethodDescriptor = javaClass.GetConstantUtf8(javaMethod.DescriptorIndex);

                                    // Gets the code attribute of the method
                                    var javaCodeAttribute = javaMethod.GetCodeAttribute();
                                    if (javaCodeAttribute != null)
                                    {
                                        var code = javaCodeAttribute.Code;
                                        var index = 0;
                                        while ((index = Utils.BinaryIndexOf(code, OrginalCode, index)) >= 0)
                                        {
                                            Console.WriteLine("Patching bytecode from '{0}.{1}{2}' at position {3}...", javaClassName, javaMethodName, javaMethodDescriptor, index);

                                            // Change the code
                                            code[index + 1] = 0xFF;
                                            code[index + 2] = 0xFF;


                                            index++;
                                        }
                                    }
                                }

                                // Writes the class
                                javaClass.Write(outputZip);
                            }
                            else
                            {
                                // Just copy the file
                                using (var inputStream = javaFile.GetFileStream(entry))
                                {
                                    StreamUtils.Copy(inputStream, outputZip, buffer);
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Creating json file...");

            // Creates the json file
            var patchedInfo = new MinecraftVersionInfo()
            {
                ID = id,
                InheritsFrom = version.ID,
                Type = "custom",
                MainClass = "net.minecraft.client.main.Main",
                MinimumLauncherVersion = 21,
            };

            // Write the version json
            patchedVersion.WriteJsonFile(patchedInfo);

            Console.WriteLine("Version got patched!");

            return patchedVersion;
        }
        
    }
}
