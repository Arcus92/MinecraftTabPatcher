namespace MinecraftTabPatcher
{
    /// <summary>
    /// DS 2019-08-11: A small struct to store the content of the minecraft version json file.
    /// </summary>
    public struct MinecraftVersionInfo
    {
        /// <summary>
        /// Gets and sets the id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets and sets the version id of the parent version
        /// </summary>
        public string InheritsFrom { get; set; }

        /// <summary>
        /// Gets and sets the type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets and sets the main class
        /// </summary>
        public string MainClass { get; set; }

        /// <summary>
        /// Gets and sets the minimum launcher version
        /// </summary>
        public int MinimumLauncherVersion { get; set; }
    }
}
