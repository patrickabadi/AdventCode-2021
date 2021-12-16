using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv014
    {
        public void Run()
        {
            var rules = Parse(input2);

            var res = RunSteps(starter2, rules, 40);

            Debug.WriteLine($"Result is {res}");
        }

        public Dictionary<string,char> Parse(string inp)
        {
            var dict = new Dictionary<string, char>();
            var lines = inp.Split('\n');
            string[] sep = { " -> " };
            foreach(var line in lines)
            {
                var sp = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                dict.Add(sp[0].Trim(), sp[1].Trim()[0]);
            }
            return dict;
        }

        public long RunSteps(string str, Dictionary<string,char> rules, int numSteps)
        {

            Dictionary<char, long> letters = new Dictionary<char, long>();
            Dictionary<string, long> template = new Dictionary<string, long>();
            for(int i=0; i<str.Length; i++)
            {
                AddLetter(letters, str[i]);
                if (i == str.Length - 1)
                    break;
                var twoLetter = str.Substring(i, 2);
                AddTemplate(template, twoLetter);                
            }


            for (int step=0; step< numSteps; step++)
            {
                Dictionary<string, long> new_template = new Dictionary<string, long>();

                foreach(var kvp in template)
                {
                    if(rules.TryGetValue(kvp.Key, out char val))
                    {
                        AddTemplate(new_template, kvp.Key[0], val, kvp.Value);
                        AddTemplate(new_template, val, kvp.Key[1], kvp.Value);
                        AddLetter(letters, val, kvp.Value);
                    }
                }
                template = new_template;

                //Debug.WriteLine($"**** After Step {step+1} ****");
                //DebugOutLetters(letters);
                //DebugOutTemplate(template);
            }


            var ordered = letters.OrderByDescending(_ => _.Value);
  

            return ordered.First().Value - ordered.Last().Value;
        }


        public void DebugOutTemplate(Dictionary<string, long> template)
        {
            foreach (var kvp in template)
            {
                Debug.WriteLine($"{kvp.Key} = {kvp.Value}");
            }
        }

        public void DebugOutLetters(Dictionary<char, long> letters)
        {
            foreach (var kvp in letters)
            {
                Debug.WriteLine($"{kvp.Key} = {kvp.Value}");
            }
        }

        public void AddTemplate(Dictionary<string, long> template, char a, char b, long count = 1)
        {
            char[] chars = { a, b };
            var str = new string(chars);
            AddTemplate(template, str, count);
        }

        public void AddTemplate(Dictionary<string, long> template, string twoLetter, long count = 1)
        {
            if (!template.ContainsKey(twoLetter))
            {
                template[twoLetter] = count;
            }
            else
            {
                template[twoLetter] = template[twoLetter] + count;
            }
        }

        public void AddLetter(Dictionary<char, long> letters, char letter, long count = 1)
        {
            if(!letters.ContainsKey(letter))
            {
                letters[letter] = count;
            }
            else
            {
                letters[letter] = letters[letter] + count;
            }
        }

        public string starter = "NNCB";
        public string input = @"CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C";

        public string starter2 = "NNSOFOCNHBVVNOBSBHCB";
        public string input2 = @"HN -> S
FK -> N
CH -> P
VP -> P
VV -> C
PB -> H
CP -> F
KO -> P
KN -> V
NO -> K
NF -> N
CO -> P
HO -> H
VH -> V
OV -> C
VS -> F
PK -> H
OS -> S
BF -> S
SN -> P
NK -> N
SV -> O
KB -> O
ON -> O
FN -> H
FO -> N
KV -> S
CS -> C
VO -> O
SP -> O
VK -> H
KP -> S
SK -> N
NC -> B
PN -> N
HV -> O
HS -> C
CN -> N
OO -> V
FF -> B
VC -> V
HK -> K
CC -> H
BO -> H
SC -> O
HH -> C
BV -> P
OB -> O
FC -> H
PO -> C
FV -> C
BK -> F
HB -> B
NH -> P
KF -> N
BP -> H
KK -> O
OH -> K
CB -> H
CK -> C
OK -> H
NN -> F
VF -> N
SO -> K
OP -> F
NP -> B
FS -> S
SH -> O
FP -> O
SF -> V
HF -> N
KC -> K
SB -> V
FH -> N
SS -> C
BB -> C
NV -> K
OC -> S
CV -> N
HC -> P
BC -> N
OF -> K
BH -> N
NS -> K
BN -> F
PC -> C
CF -> N
HP -> F
BS -> O
PF -> S
PV -> B
KH -> K
VN -> V
NB -> N
PH -> V
KS -> B
PP -> V
PS -> C
VB -> N
FB -> N";
    }
}
