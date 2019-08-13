namespace MinecraftTabPatcher
{
    /// <summary>
    /// DS 2019-08-09: A class for utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Returns the index of <paramref name="search"/> in <paramref name="array"/>.
        /// Returns -1 if the search array was not found.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int BinaryIndexOf(byte[] array, byte[] search, int start = 0)
        {
            int found = 0;
            int foundMax = search.Length;
            int len = array.Length;
            for (int i = start; i < len; i++)
            {
                var c = array[i];

                if (c == search[found])
                {
                    found++;
                    if (found == foundMax)
                    {
                        return i - foundMax + 1;
                    }
                }
                else
                {
                    found = 0;
                }
            }
            return -1;
        }
    }
}
