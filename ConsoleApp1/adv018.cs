using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv018
    {
        public void Run()
        {
            //var pair = Parse(input);
            //var pairAddr = Parse(addr);

            //Debug.WriteLine($"Out {pair.DebugOut()} {pairAddr.DebugOut()}");

            //var added = new Pair(pair, pairAddr);

            //var pair = Parse("[[[[[9,8],1],2],3],4]");
            //var pair = Parse("[7,[6,[5,[4,[3,2]]]]]");
            //var pair = Parse("[[6,[5,[4,[3,2]]]],1]");
            //var pair = Parse("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]");
            //var pair = Parse("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]");
            //var pair = Parse("[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]");
            //var pair = Parse("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]");


            //Debug.WriteLine($"magnitude {CalcMagnitude(pair)}");

            //Reduce(pair, true);

            //Debug.WriteLine($"{pair.DebugOut()}");

            var lines = input2.Split('\n');

            //var result = AddSnailList(lines);
            var result = LargestMagnitude(lines);

            Debug.WriteLine($"Result {result}");

        }

        int LargestMagnitude(string [] lines)
        {
            int largestMag = int.MinValue;

            for(int i=0; i < lines.Length-1; i++)
            {
                for(int j = i+1; j<lines.Length; j++)
                {
                    // NOTE: I have to parse every time because my reduce is changing values in place
                    var a = Parse(lines[i].Trim());
                    var b = Parse(lines[j].Trim());

                    var p = new Pair(a, b);
                    Reduce(p);
                    var mag = CalcMagnitude(p);

                    //Debug.WriteLine($"mag {mag} -\t{stra} AND {strb} REDUCED {p}");
                    largestMag = Math.Max(largestMag, mag);

                    a = Parse(lines[i].Trim());
                    b = Parse(lines[j].Trim());

                    p = new Pair(b, a);
                    Reduce(p);
                    mag = CalcMagnitude(p);
                    //Debug.WriteLine($"mag {mag} -\t{strb} AND {stra} REDUCED {p}");
                    largestMag = Math.Max(largestMag, mag);
                
                    
                }
            }

            return largestMag;
        }

        int AddSnailList(string[] lines)
        {
            Pair previous = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                var current = Parse(line);
                //Reduce(current);

                if (previous == null)
                {
                    previous = current;
                    continue;
                }

                Debug.WriteLine($"{previous.DebugOut()}");
                Debug.WriteLine($"+ {current.DebugOut()}");

                // add together
                previous = new Pair(previous, current);
                Reduce(previous, false);

                Debug.WriteLine($"= {previous.DebugOut()}\r\n");

            }

            Reduce(previous);

            return CalcMagnitude(previous);
        }

        int CalcMagnitude(Pair p)
        {
            // shouldn't happen
            if (p == null)
                return 0;
            if (p.isValue)
                return p.val;

            return 3 * CalcMagnitude(p.x) + 2 * CalcMagnitude(p.y);
        }

        void Reduce(Pair p, bool debugOut = false)
        {
            bool keep_going = true;

            if (debugOut) Debug.WriteLine($"Reducing {p.DebugOut()}");

            while(keep_going)
            {
                keep_going = false;

                var exploder = CanExplode(p, 0);
                if(exploder != null)
                {
                    keep_going = true;

                    var exp = exploder.DebugOut();

                    Explode(p, exploder);

                    if (debugOut) Debug.WriteLine($"After Exploding {exp} = {p.DebugOut()}");

                    continue;
                }

                var splitter = CanSplit(p);
                if(splitter != null)
                {
                    keep_going = true;

                    var spl = splitter.DebugOut();

                    Split(splitter);

                    if (debugOut) Debug.WriteLine($"After Splitting {spl} = \t{p.DebugOut()}");
                }

            }
        }

        void Split(Pair splitter)
        {
            float sp = (float)splitter.val / 2.0f;
            splitter.val = -1;
            splitter.x = new Pair((int)(Math.Floor(sp)));
            splitter.y = new Pair((int)(Math.Ceiling(sp)));
        }

        void Explode(Pair p, Pair exploder)
        {
            var flatList = new List<Pair>();
            BuildFlatValueList(p, flatList);

            for (int i = 0; i < flatList.Count; i++)
            {
                var check = flatList[i];
                if (check == exploder.x)
                {
                    if (i != 0)
                    {
                        // has a left one
                        flatList[i - 1].val += exploder.x.val;

                        if (i < flatList.Count - 2)
                        {
                            // has a right one too (have to jump two because I store left,right individual and i need the next one after)
                            flatList[i + 2].val += exploder.y.val;
                        }
                    }
                    else
                    {
                        // has a right one
                        flatList[i + 2].val += exploder.y.val;
                    }

                    exploder.val = 0;
                    exploder.x = null;
                    exploder.y = null;
                    break;
                }
            }
        }

        void BuildFlatValueList(Pair p, List<Pair> flatValues)
        {
            if (p == null)
                return;

            if(p.isValue)
            {
                flatValues.Add(p);
            }
            else
            {
                BuildFlatValueList(p.x, flatValues);
                BuildFlatValueList(p.y, flatValues);
            }
        }

        Pair CanSplit(Pair p)
        {
            if (p == null)
                return null;

            if(p.isValue && p.val >= 10)
            {
                return p;
            }

            var pLeft = CanSplit(p.x);
            if (pLeft != null)
                return pLeft;

            return CanSplit(p.y);
        }

        Pair CanExplode(Pair p, int depth)
        {
            if (p == null || p.isValue)
                return null;

            if(depth == 4 && p.x.isValue && p.y.isValue)
            {
                // explode
                return p;
            }

            var pLeft = CanExplode(p.x, depth + 1);
            if (pLeft != null)
                return pLeft;
            
            return CanExplode(p.y, depth + 1);
        }

        Pair Parse(string inp)
        {
            //Debug.WriteLine($"In  {inp}");

            int index = 0;
            var stack = new Stack<Pair>();
            Pair current = null;
            while(index < inp.Length)
            {
                if(inp[index] == '[')
                {
                    var p = new Pair();
                    stack.Push(p);
                    current = stack.Peek();
                }
                else if(inp[index] == ']')
                {
                    stack.Pop();
                    if (stack.Count == 0)
                        break;
                    var previous = stack.Peek();
                    current.parent = previous;
                    if (previous.x == null)
                        previous.x = current;
                    else
                        previous.y = current;
                    current = previous;
                }
                else if(char.IsNumber(inp[index]))
                {
                    var p = new Pair(inp[index] - '0');
                    p.parent = current;
                    if(current.x == null)
                    {
                        current.x = p;
                    }
                    else
                    {
                        current.y = p;
                    }
                }

                index++;
            }

            return current;
        }

        public class Pair
        {
            public Pair parent;
            public Pair x;
            public Pair y;
            public int val;
            public bool isValue => val >= 0;

            public Pair()
            {
                val = -1;
            }
            public Pair(Pair x, Pair y)
            {
                x.parent = this;
                y.parent = this;
                this.x = x;
                this.y = y;
                val = -1;
            }

            public Pair(int val)
            {
                this.val = val;
            }

            public string DebugOut()
            {
                if(isValue)
                {
                    return val.ToString();
                }
                else
                {
                    string xs = x?.DebugOut() ?? "null";
                    string ys = y?.DebugOut() ?? "null";
                    return $"[{xs},{ys}]";
                }
            }

        }

        string input0000 = @"[1,1]
[2,2]
[3,3]
[4,4]
[5,5]
[6,6]";

        string input000 = @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]";

        string input00 = @"[[[[4,3],4],4],[7,[[8,4],9]]]
[1,1]";

        string input0 = @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
[7,[5,[[3,8],[1,4]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]";

        string input = @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]";

        string input2 = @"[[[[2,8],[4,6]],[[2,4],[9,4]]],[[[0,6],[4,6]],[1,6]]]
[7,[[5,7],1]]
[[[[8,8],7],5],[[[5,6],1],6]]
[[[8,5],[[0,0],[4,9]]],[2,8]]
[7,[[5,2],[[3,0],[7,7]]]]
[[6,[6,8]],[3,[5,2]]]
[6,[[[8,9],[9,9]],[3,8]]]
[[[1,[0,2]],[7,[3,0]]],8]
[[9,6],6]
[[[2,3],1],[9,[3,7]]]
[5,[[[5,8],3],9]]
[[[[8,8],3],[2,2]],[2,3]]
[[[4,9],3],[[[7,3],8],5]]
[[[3,5],[3,7]],[[[9,7],9],[9,[7,8]]]]
[[7,1],8]
[0,[[[6,8],[1,1]],[1,[5,8]]]]
[[[[2,2],[9,5]],[0,[1,0]]],[4,[[2,4],4]]]
[[[[2,5],[7,3]],[7,6]],[[6,[4,4]],[3,8]]]
[[3,[[7,9],2]],[[0,[4,4]],[[6,9],9]]]
[[[7,7],[[1,4],[1,6]]],[7,[[6,3],6]]]
[[0,8],[[[1,6],2],4]]
[[0,[[2,7],[0,4]]],[[[3,8],[7,7]],5]]
[[[[9,9],[1,3]],[9,[4,3]]],[[[3,4],[6,4]],1]]
[[[9,[0,9]],[2,[7,6]]],[2,[[1,9],[3,3]]]]
[[4,[5,6]],[[[1,5],6],[[1,5],[5,2]]]]
[1,[[3,[2,1]],5]]
[[4,[3,8]],[3,[6,3]]]
[[7,1],[[3,[6,0]],[5,[1,1]]]]
[[8,7],[[[0,1],[2,6]],[5,[4,7]]]]
[9,[[[1,6],[8,9]],[6,6]]]
[4,9]
[[[[0,8],[8,5]],9],[7,[1,3]]]
[[[[8,5],0],[[4,6],4]],[8,4]]
[[[[8,9],8],[[3,1],[7,6]]],2]
[[[[6,3],0],[2,[4,8]]],[[[0,3],[3,5]],4]]
[0,[[9,[0,6]],5]]
[[[[1,9],[2,7]],[[4,0],[9,9]]],[[8,[3,6]],[3,4]]]
[[[[0,7],[8,4]],1],[[8,3],[[3,5],[8,0]]]]
[[[[3,5],4],[0,9]],[[[1,7],5],[9,[8,0]]]]
[[[8,[6,8]],[[3,7],[0,8]]],[[[5,2],[1,7]],[9,5]]]
[[[[5,1],[0,7]],4],[0,4]]
[[[[9,8],[3,9]],[[0,6],3]],[[[9,1],[8,7]],2]]
[[9,[[0,3],6]],[[3,4],[[8,9],5]]]
[[1,[1,8]],[[6,[4,2]],1]]
[7,[[1,[5,2]],[[9,7],0]]]
[0,[8,6]]
[1,4]
[[8,[4,1]],[[[4,0],[0,0]],[7,[3,4]]]]
[2,[[1,[1,8]],[[3,4],1]]]
[[8,[[1,2],[3,1]]],[[[4,4],[7,9]],1]]
[[4,[0,[6,4]]],[9,[0,[1,2]]]]
[[6,[3,1]],[[7,8],[8,[2,5]]]]
[[[2,[3,3]],[[6,4],[9,4]]],[[[1,5],[7,4]],[0,6]]]
[[[[8,0],3],[[4,0],3]],[[7,5],4]]
[[[2,[4,3]],[[2,1],5]],1]
[[[8,1],[0,4]],[9,[[1,4],[9,0]]]]
[[[5,0],[[7,7],9]],[[6,[6,2]],7]]
[[[[5,9],0],[[4,6],[3,8]]],[6,[6,5]]]
[[[6,[7,8]],[5,3]],[[3,[6,5]],[[8,7],[4,7]]]]
[[9,[[8,7],4]],[[[6,3],0],[[2,3],[5,9]]]]
[[[[1,8],6],1],[[[7,8],4],[7,2]]]
[[[[7,1],[6,2]],[[7,8],2]],0]
[[[4,5],[0,3]],[[2,4],1]]
[[[9,1],7],[[[8,8],[0,7]],[8,0]]]
[[5,[[7,5],[7,5]]],[3,[4,8]]]
[[7,[1,0]],[[3,[1,5]],0]]
[[[5,1],[[5,2],[7,3]]],[[7,[3,9]],9]]
[5,[1,[[9,9],[3,0]]]]
[[2,0],[9,[6,[3,3]]]]
[[[[0,4],[4,8]],[[1,9],[5,8]]],[[[7,0],5],[5,1]]]
[[[[1,5],[9,2]],[6,[3,6]]],[4,[1,[1,5]]]]
[[[[1,4],[4,6]],[[5,5],[3,5]]],[[[7,1],4],[[0,7],4]]]
[[6,[3,5]],1]
[8,[[1,[0,7]],[[2,5],6]]]
[[[[1,6],3],[[9,7],9]],[[7,8],3]]
[[[[9,9],[2,0]],0],[1,4]]
[[[[1,3],[5,1]],[[0,4],2]],0]
[[3,2],[7,[[9,3],8]]]
[[9,0],[4,[[8,7],[5,5]]]]
[[[[7,4],8],[[4,4],1]],9]
[[9,[[7,9],1]],[[[6,5],7],[[2,5],2]]]
[7,2]
[[[6,6],[[9,4],4]],6]
[[1,[[5,0],3]],[5,[4,4]]]
[[[3,2],[[4,6],6]],[[3,[9,5]],[[0,2],[4,6]]]]
[5,[[0,[3,0]],[7,[7,9]]]]
[[[[0,4],[1,5]],4],[8,[[4,7],8]]]
[[[[9,1],0],0],4]
[[[[8,4],[4,2]],[9,[1,7]]],[6,3]]
[2,[[[8,3],2],[[3,1],8]]]
[[[[9,0],[7,8]],[[2,7],[0,3]]],[[[8,5],3],[9,[6,8]]]]
[[[[8,9],[9,1]],[4,[0,1]]],[[[7,8],2],2]]
[[[[2,2],[4,1]],[2,[2,8]]],[[[6,5],1],9]]
[[[[3,0],7],7],[[[9,3],7],4]]
[[[[7,5],1],3],[[[0,7],7],[[2,6],[9,9]]]]
[[[[5,2],8],[9,[8,8]]],[2,[[0,8],[5,6]]]]
[[[[7,7],[1,2]],[6,6]],[8,[5,8]]]
[[7,[4,[8,9]]],[[4,[7,2]],8]]
[[[6,4],[7,7]],[[[3,7],0],[0,1]]]
[[1,[5,9]],[8,[4,6]]]";


    }
}
