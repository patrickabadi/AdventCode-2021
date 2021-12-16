using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TestBinaryTree
    {
        public void Run()
        {
            var tree = new Node(3, new Node(9), new Node(20, new Node(15), new Node(7)));
        }

        List<double> GetAverageValue(Node tree)
        {
            var averages = new List<double>();

            var queue = new Queue<Node>();
            queue.Enqueue(tree);

            while (queue.Count > 0)
            {
                int count = 0;
                int size = queue.Count;
                for(int i=0; i<size; i++)
                {
                    var node = queue.Dequeue();
                    count += node.Value;

                    if (node.Left != null)
                        queue.Enqueue(node.Left);
                    if (node.Right != null)
                        queue.Enqueue(node.Right);
                }
                averages.Add(count / size);
            }

            return averages;
        }


        public class Node
        {
            public int Value;
            public Node Right;
            public Node Left;

            public Node(int value, Node left = null, Node right = null)
            {
                Value = value;
                Left = left;
                Right = right;
            }
        }
    }
}
