using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv006
    {
        public void Run()
        {
            //var school = BuildSchool(input2);

            //var ret = RunCycles(school);

            var school = BuildSchool2(input2);

            var ret = RunCycles2(school);

            Debug.WriteLine($"Return {ret}");
        }

        public long[] BuildSchool2(string inp)
        {
            var arr = new long[9];

            var timers = inp.Split(',');

            foreach (var timer in timers)
            {
                var t = int.Parse(timer);
                arr[t] = arr[t] + 1;
            }

            return arr;
        }

        public long RunCycles2(long[] school)
        {
            for (int c = 0; c < 256; c++)
            {
                var newFish = school[0];
                for(int j=1; j<9; j++)
                {
                    school[j - 1] = school[j];
                }
                school[6] += newFish;
                school[8] = newFish;
            }

            long count = 0;
            foreach(var s in school)
            {
                count += s;
            }
            return count;
        }

        public List<LanternFish> BuildSchool(string inp)
        {
            var arr = new List<LanternFish>();

            var timers = inp.Split(',');

            foreach(var timer in timers)
            {
                var t = int.Parse(timer);
                arr.Add(new LanternFish(t));
            }

            return arr;
        }

        public int RunCycles(List<LanternFish> school)
        {
            var adders = new List<LanternFish>();
            for(int c=0; c<80; c++)
            {
                foreach(var s in school)
                {
                    s.Timer--;
                    if(s.Timer < 0)
                    {
                        adders.Add(new LanternFish(8, 6));
                        s.Timer = s.Cycle;
                    }
                }

                foreach (var adder in adders)
                {
                    school.Add(adder);
                }
                adders.Clear();

            }

            return school.Count;
        }

        public class LanternFish
        {
            public int Timer;
            public int Cycle;

            public LanternFish(int timer, int cycle = 6)
            {
                Timer = timer;
                Cycle = cycle;
            }
        }

        string input = "3,4,3,1,2";
        string input2 = "3,4,3,1,2,1,5,1,1,1,1,4,1,2,1,1,2,1,1,1,3,4,4,4,1,3,2,1,3,4,1,1,3,4,2,5,5,3,3,3,5,1,4,1,2,3,1,1,1,4,1,4,1,5,3,3,1,4,1,5,1,2,2,1,1,5,5,2,5,1,1,1,1,3,1,4,1,1,1,4,1,1,1,5,2,3,5,3,4,1,1,1,1,1,2,2,1,1,1,1,1,1,5,5,1,3,3,1,2,1,3,1,5,1,1,4,1,1,2,4,1,5,1,1,3,3,3,4,2,4,1,1,5,1,1,1,1,4,4,1,1,1,3,1,1,2,1,3,1,1,1,1,5,3,3,2,2,1,4,3,3,2,1,3,3,1,2,5,1,3,5,2,2,1,1,1,1,5,1,2,1,1,3,5,4,2,3,1,1,1,4,1,3,2,1,5,4,5,1,4,5,1,3,3,5,1,2,1,1,3,3,1,5,3,1,1,1,3,2,5,5,1,1,4,2,1,2,1,1,5,5,1,4,1,1,3,1,5,2,5,3,1,5,2,2,1,1,5,1,5,1,2,1,3,1,1,1,2,3,2,1,4,1,1,1,1,5,4,1,4,5,1,4,3,4,1,1,1,1,2,5,4,1,1,3,1,2,1,1,2,1,1,1,2,1,1,1,1,1,4";
    }
}
