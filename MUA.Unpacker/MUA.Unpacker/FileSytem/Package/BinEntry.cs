using System;

namespace MUA.Unpacker
{
    class BinEntry
    {
        public UInt64 dwUnknown { get; set; } // F0 F9 94 0A 21 00 00 00
        public Int64 dwNameOffset { get; set; }
        public Int64 dwOffset { get; set; }
        public Int64 dwCompressedSize { get; set; }
        public Int64 dwDecompressedSize { get; set; }
        public String m_FileName { get; set; }
    }
}
