using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv019
    {
        public void Run()
        {
            var scanners = Parse(input);
            var orientations = BuildOrientationList();
            var result = FindBeacons(scanners, orientations);

            Debug.WriteLine($"Result {result}");
        }

        private int FindBeacons(List<Scanner> scanners, List<int[,]> orientations)
        {
            var baseScanner = scanners[0];
            baseScanner.referencesScannerIndex = -1;
            baseScanner.orientation = orientations[0]; // identity

            int baseScannerIndex = 0;
            while(true)
            {
                baseScanner = scanners[baseScannerIndex];
                for(int i=1; i<scanners.Count; i++)
                {
                    var scanner = scanners[i];
                    if (scanner.referencesScannerIndex != 0)
                        continue;

                    if(HasOrientedOverlap(baseScanner, scanner, orientations))
                    {
                        scanner.referencesScannerIndex = baseScannerIndex;
                        baseScannerIndex = i;
                        break;
                    }
                }

                if (!scanners.Any(_ => _.referencesScannerIndex == 0))
                    break;
            }

            // now that we have alignments just create a new list
            HashSet<Point> finalPoints = new HashSet<Point>();
            foreach(var scanner in scanners)
            {
                foreach(var pt in scanner.original_points)
                {
                    finalPoints.Add(pt.Multiply(scanner.orientation));
                }
            }

            return finalPoints.Count;
        }

        bool HasOrientedOverlap(Scanner baseScanner, Scanner checkScanner, List<int[,]> orientations)
        {
            for(int i=1; i<orientations.Count; i++)
            {
                var orientation = orientations[i];
                int overlap_count = 0;
                checkScanner.oriented_points.Clear();
                foreach (var pt in checkScanner.original_points)
                {
                    var pt_rotated = pt.Multiply(orientation);
                    checkScanner.oriented_points.Add(pt_rotated);
                }

                for(int j=0; j<baseScanner.original_points.Count; j++)
                {
                    var pt_base_check = baseScanner.original_points.ElementAt(j);
                }
                    /*if(baseScanner.original_points.Contains(pt_rotated))
                    {
                        overlap_count++;
                        if(overlap_count >= 12)
                        {
                            checkScanner.orientation = MultiplyMatrices(baseScanner.orientation, orientation);
                            orientations.RemoveAt(i);
                            return true;
                        }
                    }
                }*/
            }

            return false;
        }

        List<int[,]> BuildOrientationList()
        {
            // from https://www.euclideanspace.com/maths/algebra/matrix/transforms/examples/index.htm
            return new List<int[,]>
            {
                // 0
            new int[3, 3]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            },
            new int[3, 3]
            {
                { 1, 0, 0 },
                { 0, 0,-1 },
                { 0, 1, 0 }
            },
            new int[3, 3]
            {
                { 1, 0, 0 },
                { 0,-1, 0 },
                { 0, 0,-1 }
            },
            new int[3, 3]
            {
                { 1, 0, 0 },
                { 0, 0, 1 },
                { 0,-1, 0 }
            },

            // 1
            new int[3, 3]
            {
                { 0,-1, 0 },
                { 1, 0, 0 },
                { 0, 0, 1 }
            },
            new int[3, 3]
            {
                { 0, 0, 1 },
                { 1, 0, 0 },
                { 0, 1, 0 }
            },
            new int[3, 3]
            {
                { 0, 1, 0 },
                { 1, 0, 0 },
                { 0, 0,-1 }
            },
            new int[3, 3]
            {
                { 0, 0,-1 },
                { 1, 0, 0 },
                { 0,-1, 0 }
            },

            // 2
            new int[3, 3]
            {
                {-1, 0, 0 },
                { 0,-1, 0 },
                { 0, 0, 1 }
            },
            new int[3, 3]
            {
                {-1, 0, 0 },
                { 0, 0,-1 },
                { 0,-1, 0 }
            },
            new int[3, 3]
            {
                {-1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0,-1 }
            },
            new int[3, 3]
            {
                {-1, 0, 0 },
                { 0, 0, 1 },
                { 0, 1, 0 }
            },

            // 3
            new int[3, 3]
            {
                { 0, 1, 0 },
                {-1, 0, 0 },
                { 0, 0, 1 }
            },
            new int[3, 3]
            {
                { 0, 0, 1 },
                {-1, 0, 0 },
                { 0,-1, 0 }
            },
            new int[3, 3]
            {
                { 0,-1, 0 },
                {-1, 0, 0 },
                { 0, 0,-1 }
            },
            new int[3, 3]
            {
                { 0, 0,-1 },
                {-1, 0, 0 },
                { 0, 1, 0 }
            },

            // 4
            new int[3, 3]
            {
                { 0, 0,-1 },
                { 0, 1, 0 },
                { 1, 0, 0 }
            },
            new int[3, 3]
            {
                { 0, 1, 0 },
                { 0, 0, 1 },
                { 1, 0, 0 }
            },
            new int[3, 3]
            {
                { 0, 0, 1 },
                { 0,-1, 0 },
                { 1, 0, 0 }
            },
            new int[3, 3]
            {
                { 0,-1, 0 },
                { 0, 0,-1 },
                { 1, 0, 0 }
            },

            // 5
            new int[3, 3]
            {
                { 0, 0,-1 },
                { 0,-1, 0 },
                {-1, 0, 0 }
            },
            new int[3, 3]
            {
                { 0,-1, 0 },
                { 0, 0, 1 },
                {-1, 0, 0 }
            },
            new int[3, 3]
            {
                { 0, 0, 1 },
                { 0, 1, 0 },
                {-1, 0, 0 }
            },
            new int[3, 3]
            {
                { 0, 1, 0 },
                { 0, 0,-1 },
                {-1, 0, 0 }
            },

            };
        }

        int [,] MultiplyMatrices(int[,] a, int[,] b)
        {
            var m = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int temp = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        temp += a[i, k] * b[k, j];
                    }
                    m[i, j] = temp;
                }
            }
            return m;
        }

        public List<Scanner> Parse(string inp)
        {
            var lines = inp.Split('\n');

            var scanners = new List<Scanner>();

            Scanner current = null;
            foreach(var line in lines)
            {
                var l = line.Trim();
                if (string.IsNullOrWhiteSpace(l))
                    continue;

                if(l.StartsWith("--- scanner"))
                {
                    current = new Scanner();

                    scanners.Add(current);
                }
                else
                {
                    var pts = l.Split(',');
                    var x = int.Parse(pts[0].Trim());
                    var y = int.Parse(pts[1].Trim());
                    var z = int.Parse(pts[2].Trim());
                    current.original_points.Add(new Point(x, y, z));
                }

            }
            return scanners;
        }



        public class Point: Tuple<int,int,int>
        {
            public int x => Item1;
            public int y => Item2;
            public int z => Item3;
            public Point(int x, int y, int z) : base(x, y, z) { }

            public override bool Equals(object obj)
            {
                var pt = obj as Point;
                return x == pt.x && y == pt.y && z == pt.z;
            }

            public Point Multiply(int[,] m)
            {
                var nx = m[0, 0] * x + m[0, 1] * y + m[0, 2] * z;
                var ny = m[1, 0] * x + m[1, 1] * y + m[1, 2] * z;
                var nz = m[2, 0] * x + m[2, 1] * y + m[2, 2] * z;
                return new Point(nx,ny,nz);
            }

            public Point Difference(Point pt)
            {
                return new Point(pt.x - x, pt.y - y, pt.z - z);
            }
        }

        public class Scanner
        {
            public int[,] orientation;
            public HashSet<Point> original_points;
            public HashSet<Point> oriented_points;
            public int referencesScannerIndex;

            public Scanner()
            {
                original_points = new HashSet<Point>();
                oriented_points = new HashSet<Point>();
            }
        }

        string input = @"--- scanner 0 ---
404,-588,-901
528,-643,409
-838,591,734
390,-675,-793
-537,-823,-458
-485,-357,347
-345,-311,381
-661,-816,-575
-876,649,763
-618,-824,-621
553,345,-567
474,580,667
-447,-329,318
-584,868,-557
544,-627,-890
564,392,-477
455,729,728
-892,524,684
-689,845,-530
423,-701,434
7,-33,-71
630,319,-379
443,580,662
-789,900,-551
459,-707,401

--- scanner 1 ---
686,422,578
605,423,415
515,917,-361
-336,658,858
95,138,22
-476,619,847
-340,-569,-846
567,-361,727
-460,603,-452
669,-402,600
729,430,532
-500,-761,534
-322,571,750
-466,-666,-811
-429,-592,574
-355,545,-477
703,-491,-529
-328,-685,520
413,935,-424
-391,539,-444
586,-435,557
-364,-763,-893
807,-499,-711
755,-354,-619
553,889,-390

--- scanner 2 ---
649,640,665
682,-795,504
-784,533,-524
-644,584,-595
-588,-843,648
-30,6,44
-674,560,763
500,723,-460
609,671,-379
-555,-800,653
-675,-892,-343
697,-426,-610
578,704,681
493,664,-388
-671,-858,530
-667,343,800
571,-461,-707
-138,-166,112
-889,563,-600
646,-828,498
640,759,510
-630,509,768
-681,-892,-333
673,-379,-804
-742,-814,-386
577,-820,562

--- scanner 3 ---
-589,542,597
605,-692,669
-500,565,-823
-660,373,557
-458,-679,-417
-488,449,543
-626,468,-788
338,-750,-386
528,-832,-391
562,-778,733
-938,-730,414
543,643,-506
-524,371,-870
407,773,750
-104,29,83
378,-903,-323
-778,-728,485
426,699,580
-438,-605,-362
-469,-447,-387
509,732,623
647,635,-688
-868,-804,481
614,-800,639
595,780,-596

--- scanner 4 ---
727,592,562
-293,-554,779
441,611,-461
-714,465,-776
-743,427,-804
-660,-479,-426
832,-632,460
927,-485,-438
408,393,-506
466,436,-512
110,16,151
-258,-428,682
-393,719,612
-211,-452,876
808,-476,-593
-575,615,604
-485,667,467
-680,325,-822
-627,-443,-432
872,-547,-609
833,512,582
807,604,487
839,-516,451
891,-625,532
-652,-548,-490
30,-46,-14";
    }
}
