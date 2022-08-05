using System;
using System.IO;
using System.Collections.Generic;

namespace MUA.Unpacker
{
    class BinUnpack
    {
        static List<BinEntry> m_EntryTable = new List<BinEntry>();

        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            using (FileStream TBinStream = File.OpenRead(m_Archive))
            {
                var m_Header = new BinHeader();

                m_Header.dwMagic = TBinStream.ReadUInt32();
                m_Header.wVersion = TBinStream.ReadInt16();
                m_Header.wComprType = TBinStream.ReadInt16();
                m_Header.dwTotalFiles = TBinStream.ReadInt32();
                m_Header.dwIndexCompressedSize = TBinStream.ReadInt32();
                m_Header.dwNamesDecompressedSize = TBinStream.ReadInt32();
                m_Header.dwNamesCompressedSize = TBinStream.ReadInt32();
                m_Header.dwUnknown = TBinStream.ReadInt32();

                if (m_Header.dwMagic != 0xB19ACA6E)
                {
                    throw new Exception("[ERROR]: Invalid magic of BIN archive file!");
                }

                if (m_Header.wVersion != 1)
                {
                    throw new Exception("[ERROR]: Invalid version of BIN archive file!");
                }

                if (m_Header.wComprType != 17)
                {
                    throw new Exception("[ERROR]: Invalid compression algorithm!");
                }

                var lpIndexTemp = TBinStream.ReadBytes(m_Header.dwIndexCompressedSize);
                var lpIndexTable = BinCipher.iDecryptAndDecompressData(lpIndexTemp);

                var lpNamesTemp = TBinStream.ReadBytes(m_Header.dwNamesCompressedSize);
                var lpNamesTable = BinCipher.iDecryptAndDecompressData(lpNamesTemp);

                m_Header.dwBaseOffset = TBinStream.Position;

                using (var TIndexReader = new MemoryStream(lpIndexTable))
                {
                    using (var TNamesReader = new MemoryStream(lpNamesTable))
                    {
                        m_EntryTable.Clear();
                        for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                        {
                            UInt64 dwUnknown = TIndexReader.ReadUInt64();
                            Int64 dwNameOffset = TIndexReader.ReadInt64();
                            Int64 dwOffset = TIndexReader.ReadInt64();
                            Int64 dwCompressedSize = TIndexReader.ReadInt64();
                            Int64 dwDecompressedSize = TIndexReader.ReadInt64();
                            
                            TNamesReader.Seek(dwNameOffset, SeekOrigin.Begin);
                            String m_FileName = TNamesReader.ReadString().Replace("/", @"\");

                            var TEntry = new BinEntry
                            {
                                dwUnknown = dwUnknown,
                                dwNameOffset = dwNameOffset,
                                dwOffset = dwOffset + m_Header.dwBaseOffset,
                                dwCompressedSize = dwCompressedSize,
                                dwDecompressedSize = dwDecompressedSize,
                                m_FileName = m_FileName,
                            };

                            m_EntryTable.Add(TEntry);
                        }
                        TNamesReader.Dispose();
                    }
                    TIndexReader.Dispose();
                }

                foreach (var m_Entry in m_EntryTable)
                {
                    String m_FullPath = m_DstFolder + m_Entry.m_FileName;

                    Utils.iSetInfo("[UNPACKING]: " + m_Entry.m_FileName);
                    Utils.iCreateDirectory(m_FullPath);

                    TBinStream.Seek(m_Entry.dwOffset, SeekOrigin.Begin);
                    var lpTemp = TBinStream.ReadBytes((Int32)m_Entry.dwCompressedSize);
                    var lpBuffer = BinCipher.iDecryptAndDecompressData(lpTemp);

                    File.WriteAllBytes(m_FullPath, lpBuffer);
                }

                TBinStream.Dispose();
            }
        }
    }
}
