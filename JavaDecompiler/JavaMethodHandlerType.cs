namespace JavaDecompiler
{
    /// <summary>
    /// DS 2019-08-09: The type of the method hander refernece
    /// </summary>
    public enum JavaMethodHandlerType : byte
    {
        GetField = 1,
        GetStatic = 2,
        PutField = 3,
        PutStatic = 4,
        InvokeVirtual = 5,
        InvokeStatic = 6,
        InvokeSpecial = 7,
        NewInvokeSpecial = 8,
        InvokeInterface = 9,
    }
}
