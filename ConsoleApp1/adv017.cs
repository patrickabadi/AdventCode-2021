using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv017
    {
        public void Run()
        {
            var target = Parse(input2);

            var res = FindHighestPosition(target);

            Debug.WriteLine($"Result {res}");
        }

        private int FindHighestPosition(Target target)
        {
            int total_max_y = int.MinValue;
            int total_target_hits = 0;

            int start = Math.Min(target.YMax, target.YMin);

            for(int x=1; x <= target.XMax; x++)
            {
                int y = start;
                int current_max_y = int.MinValue;
                int current_step_distance = int.MaxValue;
                bool has_found_target = false;
                bool break_early = false;

                while (true)
                { 
                    Point previous = new Point { x = 0, y=0 };
                    int previous_distance = target.ClosestDistanceTo(previous);
                    int current_distance = int.MaxValue;
                    int step_max_y = int.MinValue;
                    int x_velocity = x;
                    int y_velocity = y;
                    bool found_target = false;
                   
                    // run the steps
                    while (true)
                    {
                        Point current = Increment(previous, ref x_velocity, ref y_velocity);
                        step_max_y = Math.Max(current.y, step_max_y);

                        if (target.IsWithin(current))
                        {
                            has_found_target = true;
                            found_target = true;
                            total_target_hits++;
                            Debug.WriteLine($"Target hit velocity={x},{y} maxy={step_max_y}");
                            break;
                        }
                        current_distance = target.ClosestDistanceTo(current);

                        // break out if we're going away from the target and we are going downward
                        /*if (previous_distance < current_distance && previous.y < current.y)
                        {
                            //Debug.WriteLine($"Failed - missed target {x},{y}");
                            break;
                        }*/

                        if(current.x > target.XMax)
                        {
                            break_early = true;
                            break;
                        }

                        if(current.x < target.XMin && x_velocity == 0)
                        {
                            break;
                        }

                        if(current.y < target.YMin && y_velocity < 0)
                        {
                            break;
                        }

                        previous = current;
                        previous_distance = current_distance;
                    }

                    if (break_early)
                        break;

                    if(found_target)
                    {
                        if (step_max_y > current_max_y)
                        {
                            current_max_y = step_max_y;

                            total_max_y = Math.Max(current_max_y, total_max_y);
                        }                        
                        else
                        {
                            break;
                        }
                    }
                    /*else if(has_found_target)
                    {
                        // if we didn't find a target but we have found one previously break out
                        break;
                    }*/
                    /*if (current_step_distance > current_distance)
                    {
                        current_step_distance = current_distance;
                        // means we're at least getting closer to the target
                    }
                    else
                    {
                        break;
                    }*/


                    y++;

                    // NOTE: there must be a smarter way to handle this
                    if (y == 10000)
                    {
                        //Debug.WriteLine("Red Alert y is crazy");
                        break;
                    }
                }
                

            }

            Debug.WriteLine($"Total target hits {total_target_hits}");
            return total_max_y;
        }

        private Point Increment(Point pt, ref int x_velocity, ref int y_velocity)
        {
            pt.x += x_velocity;
            pt.y += y_velocity;

            if (x_velocity > 0)
                x_velocity--;
            else if (x_velocity < 0)
                x_velocity++;

            y_velocity--;

            return new Point{ x = pt.x, y = pt.y};
        }
        private Target Parse(string input)
        {
            var indexStart = input.IndexOf("x=");
            var indexEnd = input.IndexOf(",");
            var xvals = input.Substring(indexStart + 2, indexEnd - indexStart - 2);
            var sep = new string[] { ".." };
            var items = xvals.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            var xmin = int.Parse(items[0]);
            var xmax = int.Parse(items[1]);
            var yvals = input.Substring(indexEnd + 4);
            items = yvals.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            var ymin = int.Parse(items[0]);
            var ymax = int.Parse(items[1]);

            return new Target(xmin, xmax, ymin, ymax);
        }

        public struct Point
        {
            public int x;
            public int y;            
        }

        class Target
        {
            public int XMin;
            public int XMax;
            public int YMin;
            public int YMax;

            public Target(int xmin, int xmax, int ymin, int ymax)
            {
                XMin = xmin;
                XMax = xmax;
                YMin = ymin;
                YMax = ymax;
            }

            public bool IsWithin(Point pt) => (pt.x >= XMin && pt.x <= XMax && pt.y >= YMin && pt.y <= YMax);
            
            public int ClosestDistanceTo(Point pt)
            {
                int checkX, checkY;

                if (pt.x >= XMin && pt.x <= XMax)
                    checkX = pt.x;
                else if (pt.x < XMin)
                    checkX = XMin;
                else
                    checkX = XMax;

                if (pt.y >= YMin && pt.y <= YMax)
                    checkY = pt.y;
                else if (pt.y < YMin)
                    checkY = YMin;
                else
                    checkY = YMax;

                var distX = Math.Abs(checkX - pt.x);
                var distY = Math.Abs(checkY - pt.y);

                // don't need square root because it's just used for comparison
                return distX * distX + distY * distY;
            }

        }

        string input = "target area: x=20..30, y=-10..-5";
        string input2 = "target area: x=282..314, y=-80..-45";
    }
}
