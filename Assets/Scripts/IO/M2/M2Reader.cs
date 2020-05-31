using System;
using System.Collections.Generic;
using System.IO;
using Casc;
using Constants;
using UnityEngine;
using Util;

namespace IO.M2
{
    public static partial class M2Reader
    {
        public static void ReadM2(uint fileDataId, M2Model model)
        {
            var stream = CASC.OpenFile(fileDataId);
            if (stream == null)
                return;

            using (var reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var chunkId = (Chunks)reader.ReadUInt32().Flip();
                    var chunkSize = reader.ReadUInt32();

                    switch (chunkId)
                    {
                        case Chunks.MD21:
                            ReadMD20(reader, model);
                            reader.BaseStream.Position = 8 + chunkSize;
                            break;
                        case Chunks.SFID:
                            ReadSFID(reader, chunkSize);
                            break;
                        case Chunks.TXID:
                            ReadTXID(reader, model, chunkSize);
                            break;
                        default:
                            reader.Skip(chunkSize);
                            Debug.Log($"Skipping {chunkId} (0x{chunkId:X}) with size: {chunkSize}..");
                            break;
                    }
                }
            }
        }
    }
}