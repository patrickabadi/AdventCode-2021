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
            string input = "5*4+16*8";

            /*
             *               +
             *             *    *
             *            5 4  16 8
             *            
             *            5
             *            
             *            
             */

            Node tree = BuildTree(input);
        }

        private Node BuildTree(string input)
        {
            int index = 0;

            Node tree = null;

            while(index < input.Length)
            {
                Node node = null;
                var checker = input[index];
                if(checker == '*' || checker == '+')
                {
                    node = new Node(checker.ToString());
                }
                else
                {
                    string num = checker.ToString();
                    for(int i=index+1; i<input.Length; i++)
                    {
                        var check = input[i];
                        if(check <= '9')
                        {
                            num += check.ToString();
                        }
                        else
                        {
                            index = i;
                            break;
                        }
                    }

                    node = new Node(num);
                }

                if(tree == null)
                {
                    tree = node;
                }
                else
                {

                }
            }

            return tree;
        }

        public class Node
        {
            public string Value;
            public Node Right;
            public Node Left;
            public Node Parent;

            public bool IsNumber => !IsMultiply && !IsPlus;
            public bool IsMultiply => Value == "*";
            public bool IsPlus => Value == "+";

            public Node(string value)
            {
                Value = value;
            }
        }
    }
}
