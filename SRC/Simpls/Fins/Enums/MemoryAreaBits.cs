namespace PLC_Omron_Standard.Enums
{
    /// <summary>
    /// PLC内存区域类型
    /// </summary>
    public enum AreaType
    {
        CIO_Bit = 0x30,
        WR_Bit = 0x31,
        HR_Bit = 0x32,
        AR_Bit = 0x33,
        DM_Bit = 0x02,
        CIO_Word = 0xB0,
        WR_Word = 0xB1,
        HR_Word = 0xB2,
        AR_Word = 0xB3,
        DM_Word = 0x82
    }

    /// <summary>
    /// A listing of bits available in the PLC memory area
    /// </summary>
    public enum MemoryAreaBits : byte
    {
        /// <summary>
        /// Accesses the data memory area
        /// </summary>
        DataMemory = 0x82,

        /// <summary>
        /// Accesses the common IO area
        /// </summary>
        CommonIO = 0x30,

        /// <summary>
        /// 
        /// </summary>
        Work = 0x31,

        /// <summary>
        /// 
        /// </summary>
        Holding = 0x32,

        /// <summary>
        /// 
        /// </summary>
        Auxiliary = 0x33
    }
}
