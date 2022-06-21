using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HashGenerator
{
    internal class Program
    {
        // private static DataContractStateSerializer<string> keyConverter = new DataContractStateSerializer<string>();
        private static BuiltInTypeSerializer<string> keyConverter = BuiltInTypeSerializerBuilder.MakeBuiltInTypeSerializer<string>();

        private static ArraySegment<byte> GetKeyBytes(string key)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    keyConverter.Write(key, binaryWriter);

                    return new ArraySegment<byte>(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
                }
            }
        }


        private static List<string> GetArgValues(dynamic argDesc)
        {
            if (argDesc["Type"] == "string")
            {
                return argDesc["Value"].ToObject<List<string>>();
            }
            else if (argDesc["Type"] == "num")
            {
                List<string> nums = new List<string>();
                for (int i = 0; i <= argDesc["Value"].ToObject<int>(); i++)
                {
                    nums.Add(i.ToString());
                }
                return nums;
            }

            return null;
        }

        static void Main(string[] args)
        {
            Dictionary<ulong, string> hashDict = new Dictionary<ulong, string>();

            using (StreamReader r = new StreamReader("KeyValues.json"))
            {
                string json = r.ReadToEnd();
                List<dynamic> keys = ((IEnumerable<dynamic>)JsonConvert.DeserializeObject<dynamic>(json)).ToList();

                Console.WriteLine(keys.Count);

                foreach (var key in keys)
                {
                    var argDesc = key.ArgDesc;
                    int argNum = argDesc.Count;
                    if (argNum == 0)
                    {
                        string keyString = key.KeyFormat;
                        var keyBytes = GetKeyBytes(keyString);
                        ulong keyHash = CRC64.ToCRC64(keyBytes);
                        hashDict.Add(keyHash, keyString);
                    }
                    else if (argNum == 1)
                    {
                        List<string> argValues1 = GetArgValues(argDesc[0]);
                        foreach (var argValue1 in argValues1)
                        {
                            string keyFormat = key.KeyFormat.ToObject<string>();
                            string keyString = String.Format(keyFormat, argValue1);
                            var keyBytes = GetKeyBytes(keyString);
                            ulong keyHash = CRC64.ToCRC64(keyBytes);
                            hashDict.Add(keyHash, keyString);
                        }
                    }
                    else if (argNum == 2)
                    {
                        List<string> argValues1 = GetArgValues(argDesc[0]);
                        List<string> argValues2 = GetArgValues(argDesc[1]);

                        foreach (var argValue1 in argValues1)
                        {
                            foreach (var argValue2 in argValues2)
                            {
                                string keyFormat = key.KeyFormat.ToObject<string>();
                                string keyString = String.Format(keyFormat, argValue1, argValue2);
                                var keyBytes = GetKeyBytes(keyString);
                                ulong keyHash = CRC64.ToCRC64(keyBytes);
                                hashDict.Add(keyHash, keyString);
                            }
                        }
                    }
                    else if (argNum == 2)
                    {
                        List<string> argValues1 = GetArgValues(argDesc[0]);
                        List<string> argValues2 = GetArgValues(argDesc[1]);
                        List<string> argValues3 = GetArgValues(argDesc[2]);

                        foreach (var argValue1 in argValues1)
                        {
                            foreach (var argValue2 in argValues2)
                            {
                                foreach (var argValue3 in argValues2)
                                {
                                    string keyFormat = key.KeyFormat.ToObject<string>();
                                    string keyString = String.Format(keyFormat, argValue1, argValue2, argValue3);
                                    var keyBytes = GetKeyBytes(keyString);
                                    ulong keyHash = CRC64.ToCRC64(keyBytes);
                                    hashDict.Add(keyHash, keyString);
                                }
                            }
                        }
                    }
                }
                //string hashDictJson = JsonConvert.SerializeObject(hashDict);
                //File.AppendAllText("hashes.json", hashDictJson);

                File.WriteAllLines(
                        "hashes.dmp",
                        hashDict.Select(kvp => string.Format("{0};{1}", kvp.Key, kvp.Value)));
            }
        }
    }
}
