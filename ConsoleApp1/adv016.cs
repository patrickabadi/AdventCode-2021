using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv016
    {
        public void Run()
        {
            //var p = new Packet("D2FE28");
            var p = new Packet();
            //p.Parse("38006F45291200");
            //p.Parse("8A004A801A8002F478");
            //p.Parse("620080001611562C8802118E34");
            //p.Parse("C0015000016115A2E0802F182340");
            //p.Parse("9C0141080250320F1802104A08");
            p.Parse(input2);

            //var result = p.VersionSum();
            var result = p.CalcValue(false);
            Debug.WriteLine($"Result {result}");
            

        }

        public class Packet
        {
            public int Version;
            public int TypeID;
            public long Value;
            public List<Packet> SubPackets;

            public Packet()
            {
                SubPackets = new List<Packet>();
            }

            public long CalcValue(bool printResult = false)
            {
                long result = 0;
                string resDescription = string.Empty;
                switch(TypeID)
                {
                    case 0: // sum
                        if (printResult) resDescription = "Sum";
                        long sum = 0;
                        foreach(var s in SubPackets)
                        {
                            sum += s.CalcValue(printResult);
                        }
                        result = sum;
                        break;
                    case 1: // product
                        if (printResult) resDescription = "Product";
                        long prod = 1;
                        foreach (var s in SubPackets)
                        {
                            prod *= s.CalcValue(printResult);
                        }
                        result = prod;
                        break;
                    case 2: // min
                        if (printResult) resDescription = "Min";
                        long min = long.MaxValue;
                        foreach(var s in SubPackets)
                        {
                            min = Math.Min(s.CalcValue(printResult), min);
                        }
                        result = min;
                        break;
                    case 3: // max
                        if (printResult) resDescription = "Max";
                        long max = long.MinValue;
                        foreach (var s in SubPackets)
                        {
                            max = Math.Max(s.CalcValue(printResult), max);
                        }
                        result = max;
                        break;
                    case 4: //single number
                        if (printResult) resDescription = "Number";
                        result = Value;
                        break;
                    case 5: // greater than
                        if (printResult) resDescription = "GT";
                        result = SubPackets[0].CalcValue(printResult) > SubPackets[1].CalcValue(printResult) ? 1 : 0;
                        break;
                    case 6: // less than
                        if (printResult) resDescription = "LT";
                        result = SubPackets[0].CalcValue(printResult) < SubPackets[1].CalcValue(printResult) ? 1 : 0;
                        break;
                    case 7: // equal
                        if (printResult) resDescription = "EQ";
                        result = SubPackets[0].CalcValue(printResult) == SubPackets[1].CalcValue(printResult) ? 1 : 0;
                        break;
                    default:
                        throw new Exception("Alarm bells!");
                }

                if(printResult)
                {
                    Debug.WriteLine($"{resDescription} = {result}");
                }
                return result;
            }

            public int VersionSum()
            {
                var sum = 0;
                foreach(var s in SubPackets)
                {
                    sum += s.VersionSum();
                }

                return sum + Version;
            }

            public int Parse(string inp, bool isBinary=false)
            {
                SubPackets.Clear();

                var bin = isBinary?inp:ConvertToBinary(inp);
                Version = ParseNumber(bin.Substring(0, 3));
                TypeID = ParseNumber(bin.Substring(3, 3));

                if (TypeID == 4)
                {
                    return 6 + ParseLiteralValue(bin.Substring(6));
                     
                }
                else
                {
                    var bit = bin[6];
                    int length = bit == '0' ? 15 : 11;

                    var subLength = bin.Substring(7, length);
                    var packetLength = Convert.ToInt32(subLength, 2);

                    if (bit == '0')
                    {
                        var subPackets = bin.Substring(7 + length, packetLength);

                        int index = 0;
                        int totalIndex = 0;
                        while (totalIndex < packetLength - 1)
                        {
                            var sp = new Packet();
                            index = sp.Parse(subPackets, true);
                            SubPackets.Add(sp);

                            subPackets = subPackets.Substring(index);
                            totalIndex += index;
                        }

                        return 7 + length + packetLength;
                    }
                    else
                    {
                        int index = 0;
                        int totalIndex = 0;
                        var subPackets = bin.Substring(7 + length);
                        for (int i=0; i<packetLength; i++)
                        {
                            var sp = new Packet();
                            index = sp.Parse(subPackets, true);
                            SubPackets.Add(sp);

                            subPackets = subPackets.Substring(index);
                            totalIndex += index;
                        }
                        return 7 + length + totalIndex;
                    }

                }
            }

            int ParseNumber(string inp)
            {
                return Convert.ToInt32(inp, 2);
            }

            string ConvertToBinary(string inp)
            {
                var res = new StringBuilder();

                foreach(var letter in inp)
                {
                    string bin = string.Empty;

                    switch(letter)
                    {
                        case '0': bin = "0000"; break;
                        case '1': bin = "0001"; break;
                        case '2': bin = "0010"; break;
                        case '3': bin = "0011"; break;
                        case '4': bin = "0100"; break;
                        case '5': bin = "0101"; break;
                        case '6': bin = "0110"; break;
                        case '7': bin = "0111"; break;
                        case '8': bin = "1000"; break;
                        case '9': bin = "1001"; break;
                        case 'A': bin = "1010"; break;
                        case 'B': bin = "1011"; break;
                        case 'C': bin = "1100"; break;
                        case 'D': bin = "1101"; break;
                        case 'E': bin = "1110"; break;
                        case 'F': bin = "1111"; break;
                    }
                    res.Append(bin);
                }

                return res.ToString();
            }

            int ParseLiteralValue(string inp)
            {
                int index = 0;
                var val = new StringBuilder();
                while(index <= inp.Length-5)
                {
                    var bit = inp.Substring(index, 1);
                    var part = inp.Substring(index+1, 4);

                    val.Append(part);

                    if (bit[0] == '0')
                        break;

                    index += 5;
                }
                Value = Convert.ToInt64(val.ToString(), 2);
                return index + 5;
            }
        }

        public string input = "110100101111111000101000";
        public string input2 = "00569F4A0488043262D30B333FCE6938EC5E5228F2C78A017CD78C269921249F2C69256C559CC01083BA00A4C5730FF12A56B1C49A480283C0055A532CF2996197653005FC01093BC4CE6F5AE49E27A7532200AB25A653800A8CAE5DE572EC40080CD26CA01CAD578803CBB004E67C573F000958CAF5FC6D59BC8803D1967E0953C68401034A24CB3ACD934E311004C5A00A4AB9CAE99E52648401F5CC4E91B6C76801F59DA63C1F3B4C78298014F91BCA1BAA9CBA99006093BFF916802923D8CC7A7A09CA010CD62DF8C2439332A58BA1E495A5B8FA846C00814A511A0B9004C52F9EF41EC0128BF306E4021FD005CD23E8D7F393F48FA35FCE4F53191920096674F66D1215C98C49850803A600D4468790748010F8430A60E1002150B20C4273005F8012D95EC09E2A4E4AF7041004A7F2FB3FCDFA93E4578C0099C52201166C01600042E1444F8FA00087C178AF15E179802F377EC695C6B7213F005267E3D33F189ABD2B46B30042655F0035300042A0F47B87A200EC1E84306C801819B45917F9B29700AA66BDC7656A0C49DB7CAEF726C9CEC71EC5F8BB2F2F37C9C743A600A442B004A7D2279125B73127009218C97A73C4D1E6EF64A9EFDE5AF4241F3FA94278E0D9005A32D9C0DD002AB2B7C69B23CCF5B6C280094CE12CDD4D0803CF9F96D1F4012929DA895290FF6F5E2A9009F33D796063803551006E3941A8340008743B8D90ACC015C00DDC0010B873052320002130563A4359CF968000B10258024C8DF2783F9AD6356FB6280312EBB394AC6FE9014AF2F8C381008CB600880021B0AA28463100762FC1983122D2A005CBD11A4F7B9DADFD110805B2E012B1F4249129DA184768912D90B2013A4001098391661E8803D05612C731007216C768566007280126005101656E0062013D64049F10111E6006100E90E004100C1620048009900020E0006DA0015C000418000AF80015B3D938";
    }
}
