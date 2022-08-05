using System;

namespace MUA.Unpacker
{
    class BinHeader
    {
        public UInt32 dwMagic { get; set; } // 0xB19ACA6E
        public Int16 wVersion { get; set; } // 1
        public Int16 wComprType { get; set; } // 17 (Zlib)
        public Int32 dwTotalFiles { get; set; }
        public Int32 dwIndexCompressedSize { get; set; }
        public Int32 dwNamesDecompressedSize { get; set; }
        public Int32 dwNamesCompressedSize { get; set; }
        public Int32 dwUnknown { get; set; }
        public Int64 dwBaseOffset { get; set; }
    }
}
