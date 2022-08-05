using System;

namespace MUA.Unpacker
{
    class BinCipher
    {
        public static Byte[] iDecryptAndDecompressData(Byte[] lpBuffer)
        {
            Int32 j = 0;
            Int32 dwSize = lpBuffer.Length;
            Int32 dwKey = (((Int16)(dwSize + 85) | ((Int16)(dwSize + 85) << 16)) ^ 0x3B9A1D9) & 0xFFFFFFF;

            for (Int32 i = 0; i < dwSize >> 2; i++, j += 4)
            {
                dwKey = (0x3FD * dwKey + 1) & 0xFFFFFFF;

                UInt32 dwData = BitConverter.ToUInt32(lpBuffer, j);
                dwData ^= (UInt32)dwKey;

                lpBuffer[j + 0] = (Byte)dwData;
                lpBuffer[j + 1] = (Byte)(dwData >> 8);
                lpBuffer[j + 2] = (Byte)(dwData >> 16);
                lpBuffer[j + 3] = (Byte)(dwData >> 24);
            }

            return Zlib.iDecompress(lpBuffer);
        }
    }
}
