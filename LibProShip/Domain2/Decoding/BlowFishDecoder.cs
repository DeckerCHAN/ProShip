using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using LibProShip.Domain.Decode;
using LibProShip.Domain2.Analysis;
using LibProShip.Infrastructure;
using LibProShip.Infrastructure.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibProShip.Domain2.Decoding
{
    public class BlowFishDecoder : IDecoder
    {
        private readonly Blowfish BlowFish;

        public BlowFishDecoder()
        {
            var key = new byte[]
                {0x29, 0xB7, 0xC9, 0x09, 0x38, 0x3F, 0x84, 0x88, 0xFA, 0x98, 0xEC, 0x4E, 0x13, 0x19, 0x79, 0xFB};

            this.BlowFish = new Blowfish(key);
        }


        public Replay.Replay DecodeReplay(FileInfo replayFile)
        {
            using (var f =
                new FileStream(replayFile.FullName, FileMode.Open))
            {
                var signtureBytes = new byte[4];
                f.Read(signtureBytes, 0, 4);

                var blockCountBytes = new byte[4];
                f.Read(blockCountBytes, 0, 4);


                var blockSizeBytes = new byte [4];
                f.Read(blockSizeBytes, 0, 4);

                var areaBlockSize = BitConverter.ToInt32(blockSizeBytes, 0);


                var areaInfoBytes = new byte [areaBlockSize];
                f.Read(areaInfoBytes, 0, areaInfoBytes.Length);
                var areaInfoString = Encoding.UTF8.GetString(areaInfoBytes);


                var zLibBytes = new byte[f.Length - f.Position];
                f.Read(zLibBytes, 0, zLibBytes.Length);

                var s = zLibBytes.Length % 8;


                var resStream = new MemoryStream();
                var secondChunk = new ArraySegment<byte>(zLibBytes, 8, 8).ToArray();
                this.BlowFish.Decipher(secondChunk);
                resStream.Write(secondChunk, 0, secondChunk.Length);

                var previousChunk = new byte[8];
                Array.Copy(secondChunk, previousChunk, 8);


                for (int i = 16; i < zLibBytes.Length; i += 8)
                {
                    var chunk = new ArraySegment<byte>(zLibBytes, i, 8).ToArray();
                    this.BlowFish.Decipher(chunk);

                    var thisChunkLong = BitConverter.ToInt64(chunk, 0);
                    var previousChunkLong = BitConverter.ToInt64(previousChunk, 0);

                    var xor = thisChunkLong ^ previousChunkLong;
                    var xorBytes = BitConverter.GetBytes(xor);

                    resStream.Write(xorBytes, 0, xorBytes.Length);
                    Array.Copy(xorBytes, previousChunk, 8);
                }

                var battle = JsonConvert.DeserializeObject<Battle>(areaInfoString);
                var jobj = JObject.Parse(areaInfoString);
                var version = jobj.SelectToken("clientVersionFromExe").ToObject<string>();
                var mapName = jobj.SelectToken("mapDisplayName").ToObject<string>();
                var duration = jobj.SelectToken("duration").ToObject<int>();
                var vehiclesJToken = jobj.SelectToken("vehicles");
                    

                for (int i = 0; i < vehiclesJToken.Children().Count(); i++)
                {
                    var child = vehiclesJToken.Children()[i] as JToken;
                    var playerId = child.SelectToken("id").ToObject<long>();
                    var teamId = child.SelectToken("relation").ToObject<int>();
                    var shipId = child.SelectToken("shipId").ToObject<long>();
                    var playerName = child.SelectToken("name").ToObject<string>();

                    var player = new Player(playerId, playerName);
                    var vehlcle = new Vehicle(i,teamId, shipId, player);
                }
                
                

                battle = new Battle();
                var data = new byte[resStream.Length];
                resStream.Read(data, 0, data.Length);

                var md5 = HashUtils.Hash(data);
                
                var resultRawPlay = new Replay.Replay(md5,null,new Dictionary<string, AnalysisResult>());
//                return resultRawPlay;



                return null;
//
//                    var resBytes = new byte[resStream.Length];
//
//                    resStream.Position = 0;
//                    resStream.Read(resBytes, 0, resBytes.Length);
//
//
//                    var resST = this.Decompress(resBytes);


//                var msSinkDecompressed = new System.IO.MemoryStream();
//                var zOut = new ZlibStream(msSinkDecompressed, Ionic.Zlib.CompressionMode.Decompress, true);
//                CopyStream(zlibStream, zOut);
//
//                Console.Write(zOut.Length);
//
//                //new Blowfish()
            }
        }

        private Stream Decompress(byte[] data)
        {
            var output = new MemoryStream();
            using (var compressedStream = new MemoryStream(new ArraySegment<byte>(data, 2, data.Length - 2).ToArray()))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                zipStream.CopyTo(output);
                zipStream.Close();
                output.Position = 0;
                return output;
            }
        }
    }
}