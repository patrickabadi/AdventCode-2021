using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv024
    {
        public void Run()
        {
            var instructions = Parse(input);

            //var wxyz = new int[4];
            //var res = RunALU(instructions, wxyz, 99999999999999);
            //var res = RunALU(instructions, wxyz, "99999999999999");
            //var res = FindLargestModelNumber(instructions);
            //low 81111379141811 high 93997999296912
            Runner();

            //Debug.WriteLine($"Result {res}");
        }

        public void Runner()
        {
            var steps = input.Split('\n');
            var ps = new Tuple<int, int, int>[14];
            for (int i = 0; i < 14; i++)
            {
                var s1 = int.Parse(new string(steps[4 + i * 18].Skip(6).ToArray()));
                var s2 = int.Parse(new string(steps[5 + i * 18].Skip(6).ToArray()));
                var s3 = int.Parse(new string(steps[15 + i * 18].Skip(6).ToArray()));
                ps[i] = new Tuple<int,int,int>(s1,s2,s3);
            }

            (long? lowest, long? highest) = (long.MaxValue, long.MinValue);
            for (long i = 10000000000000; i <= 99999999999999; i++)
            {
                var digits = i.ToString().Select(ch => int.Parse(ch.ToString())).ToArray();
                int step = 0;
                long z = 0;

                foreach ((int p1, int p2, int p3) in ps)
                {
                    var w = digits[step];
                    var test = (z % 26) + p2 == w;
                    if (w != 0 && p1 == 26 && test)
                    {
                        z = z / p1;
                    }
                    else if (w != 0 && p1 == 1 && !test)
                    {
                        z = 26 * (z / p1) + w + p3;
                    }
                    else
                    {
                        i += (long)Math.Pow(10, 13 - step);
                        i--;
                        break;
                    }
                    step++;
                }

                if (z == 0)
                {
                    (lowest, highest) = (i < lowest ? i : lowest, i > highest ? i : highest);
                }
            }

            Debug.WriteLine($"low {lowest} high {highest}");
        }

        public string FindLargestModelNumber(List<Instruction> instructions)
        {
            ulong value = 99999999999999;
            //var value = "0000000000000";
            var charValue = new char[14];
            var wxyz = new int[4];
            bool foundzeros = false;
            ulong v;
            while(value != 0)
            {
                value--;

                if (value % 26 != 0)
                    continue;

                foundzeros = false;
                v = value;
                for(int i=13; i>=0; i--)
                {
                    var c = v % 10;
                    if(c == 0)
                    {
                        foundzeros = true;
                        break;
                    }
                    v = v / 10;
                    charValue[i] = (char)(c + '0');
                }
                if (foundzeros)
                    continue;

                wxyz[0] = 0;
                wxyz[1] = 0;
                wxyz[2] = 0;
                wxyz[3] = 0;

                if (RunALU(instructions, wxyz, charValue) == 0)
                    break;
            }

            return new string(charValue);

        }

        bool ContainsZeros(ulong value)
        {
            if (value == 0)
                return true;

            while(value != 0)
            {
                if (value % 10 == 0)
                    return true;
                value = value / 10;
            }

            return false;
        }

        public List<Instruction> Parse(string inp)
        {
            var lines = inp.Split('\n');

            var instructions = new List<Instruction>();

            foreach(var line in lines)
            {
                var parts = line.Split(' ');
                var inst = new Instruction();

                inst.SetInst(parts[0]);
                inst.SetA(parts[1].Trim());
                if(parts.Length > 2)
                    inst.SetB(parts[2].Trim());

                instructions.Add(inst);
            }

            return instructions;
        }

        public int RunALU(List<Instruction> instructions, int[] wxyz, char[] digits)
        {
            int index = 0;

            foreach(var inst in instructions)
            {

                if(inst.action == Instruction.ActionType.Inp)
                {
                    //Debug.WriteLine($"wzyz = {wxyz[0]} {wxyz[1]} {wxyz[2]} {wxyz[3]}");
                    inst.Run(wxyz, digits[index++] - '0');
                }
                else
                {
                    inst.Run(wxyz);
                }

                
            }

            //Debug.WriteLine($"wzyz = {wxyz[0]} {wxyz[1]} {wxyz[2]} {wxyz[3]}");

            return wxyz[3];
        }

        public class Instruction
        {
            public enum ActionType
            {
                Inp,
                Add,
                Mul,
                Div,
                Mod,
                Eql,
            }

            public ActionType action;
            public char a;
            public char b;
            public int value;

            public bool BValid => b != ' ';

            public Instruction()
            {
                b = ' ';
            }

            public void SetA(string str)
            {
                a = str[0];
            }

            public void SetB(string str)
            {
                if (str[0] >= 'w')
                    b = str[0];
                else
                    value = int.Parse(str);
            }

            public void SetInst(string inst)
            {
                switch (inst)
                {
                    case "inp":
                        action = ActionType.Inp;
                        break;
                    case "add":
                        action = ActionType.Add;
                        break;
                    case "mul":
                        action = ActionType.Mul;
                        break;
                    case "div":
                        action = ActionType.Div;
                        break;
                    case "mod":
                        action = ActionType.Mod;
                        break;
                    case "eql":
                        action = ActionType.Eql;
                        break;
                    default:
                        break;
                }
            }

            public void Run(int[] wxyz, int inp=0)
            {
                if(action == ActionType.Inp)
                {
                    //Debug.WriteLine($"wzyz = {wxyz[0]} {wxyz[1]} {wxyz[2]} {wxyz[3]}");
                    wxyz[a - 'w'] = inp;
                    //Debug.WriteLine($"Input {inp} to {a - 'w'}");

                }
                else
                {
                    wxyz[a - 'w'] = Run(wxyz[a - 'w'], BValid ? wxyz[b - 'w']: value);
                }
            }

            protected int Run(int a, int b)
            {
                switch (action)
                {
                    case ActionType.Add:
                        return a + b;
                    case ActionType.Mul:
                        return a * b;
                    case ActionType.Div:
                        return a / b;
                    case ActionType.Mod:
                        return a % b;
                    case ActionType.Eql:
                        return a == b ? 1 : 0;
                    default:
                        throw new Exception("Red alert invalid op");
                }
            }
       
        }

        /*
inp w       inp w
mul x 0     mul x 0
add x z     add x z
mod x 26    mod x 26
div z 1     div z 1
add x 10    add x 14
eql x w     eql x w
eql x 0     eql x 0
mul y 0     mul y 0
add y 25    add y 25
mul y x     mul y x
add y 1     add y 1
mul z y     mul z y
mul y 0     mul y 0
add y w     add y w
add y 2     add y 13
mul y x     mul y x
add z y     add z y

        */

        public string input0 = @"inp w
add z w
mod z 2
div w 2
add y w
mod y 2
div w 2
add x w
mod x 2
div w 2
mod w 2";
        public string input = @"inp w
mul x 0
add x z
mod x 26
div z 1
add x 10
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 2
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 1
add x 14
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 13
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 1
add x 14
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 13
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 26
add x -13
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 9
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 1
add x 10
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 15
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 26
add x -13
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 3
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 26
add x -7
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 6
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 1
add x 11
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 5
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 1
add x 10
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 16
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 1
add x 13
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 1
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 26
add x -4
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 6
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 26
add x -9
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 3
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 26
add x -13
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 7
mul y x
add z y
inp w
mul x 0
add x z
mod x 26
div z 26
add x -9
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y 9
mul y x
add z y";
    }
}