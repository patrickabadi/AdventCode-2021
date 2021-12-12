using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv012
    {
        public void Run()
        {
            var graph = BuildGraph(input2);

            var res = FindPaths(graph, true);

            Debug.WriteLine($"Result is {res}");
        }

        public int FindPaths(Node graph, bool allowSmallCaveTwice = false)
        {
            var paths = new List<string>();

            foreach(var node in graph.Nodes)
            {
                CurrentPath(node, graph.Value, paths, allowSmallCaveTwice, false);
            }

            return paths.Count;
        }

        public void CurrentPath(Node current, string path, List<string> paths, bool allowSmallCaveTwice, bool hasMultipleSmallCave)
        {

            if (current == null || !current.Nodes.Any() || current.IsStart)
                return;

            // so we don't traverse small caves more than maximim allowed
            if (!current.IsBigCave && !current.IsEnd)
            {
                var occurrances = FindOccurrances(path, current.Value);
                if(allowSmallCaveTwice && occurrances >= 1)
                {
                    if (hasMultipleSmallCave || occurrances == 2)
                        return;
                    hasMultipleSmallCave = true;
                }
                else if(!allowSmallCaveTwice && occurrances == 1)
                {
                    return;
                }
            }

            path += ("," + current.Value);

            if (current.IsEnd)
            {
                //Debug.WriteLine($"Added Path {path}");
                paths.Add(path);
                return;
            }

            foreach(var node in current.Nodes)
            {
                CurrentPath(node, path, paths, allowSmallCaveTwice, hasMultipleSmallCave);
            }
        }

        public int FindOccurrances(string path, string value)
        {
            int index = 0;
            int startIndex = 0;
            int occurances = 0;

            while((index = path.IndexOf(value,startIndex)) >=0)
            {
                startIndex = index + 1;
                occurances++;
            }

            return occurances;
        }

        public Node BuildGraph(string inp)
        {
            Node startNode = null;
            Node graph = null;
            var nodes = new List<Node>();

            var lines = inp.Split('\n');

            foreach(var line in lines)
            {
                var items = line.Trim().Split('-');

                var node1 = FindOrCreateNode(items[0], nodes);
                var node2 = FindOrCreateNode(items[1], nodes);

                node1.Connect(node2);
                node2.Connect(node1);

                if(node2.IsStart)
                {
                    startNode = graph = node2;
                }
                else if(node1.IsStart)
                {
                    startNode = graph = node1;
                }
                else if(graph == null)
                {
                    graph = node1;
                }

            }

            return startNode;
        }

        public Node FindOrCreateNode(string value, List<Node> nodes)
        {
            //var node = FindNode(value, graph, new HashSet<string>());
            var node = nodes.FirstOrDefault(_ => _.Value == value);
            if (node != null)
                return node;

            node = new Node(value);
            nodes.Add(node);

            return node;
        }

        public Node FindNode(string value, Node graph, HashSet<string> visited)
        {
            if (graph == null)
                return null;

            if (visited.Contains(graph.Value))
                return null;

            if (graph.Value == value)
                return graph;

            visited.Add(graph.Value);

            foreach(var node in graph.Nodes)
            {
                var n = FindNode(value, node, visited);
                if (n != null)
                    return n;
            }

            return null;
        }

        public class Node
        {
            public string Value;
            public List<Node> Nodes;
            public bool IsStart => Value == "start";
            public bool IsEnd => Value == "end";
            public bool IsBigCave => Value.ToUpper() == Value;

            public Node(string value)
            {
                Value = value;
                Nodes = new List<Node>();
            }

            public void Connect(Node node)
            {
                if (Nodes.Any(_ => node.Value == _.Value))
                    return;

                Nodes.Add(node);
            }
        }

        public string input0 = @"start-A
start-b
A-c
A-b
b-d
A-end
b-end";

        public string input = @"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc";

        public string input2 = @"yb-start
de-vd
rj-yb
rj-VP
OC-de
MU-de
end-DN
vd-end
WK-vd
rj-de
DN-vd
start-VP
DN-yb
vd-MU
DN-rj
de-VP
yb-OC
start-rj
oa-MU
yb-de
oa-VP
jv-MU
yb-MU
end-OC";
    }
}
