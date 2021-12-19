using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Subsets
    {
        public bool CanPartition(int[] nums)
        {
            int total = nums.Sum();

            if (total % 2 != 0)
                return false;

            return CanPartition(nums, 0, 0, total, new Dictionary<string, bool>());
        }

        bool CanPartition(int[] nums, int index, int sum, int total, Dictionary<string, bool> visited)
        {
            var current = $"{index},{sum}";
            if (visited.ContainsKey(current))
                return visited[current];

            if (sum * 2 == total)
                return true;
            if (sum > total / 2 || index >= nums.Length)
                return false;

            var found = CanPartition(nums, index + 1, sum, total, visited) || CanPartition(nums, index+1, sum+nums[index], total, visited);
            visited[current] = found;

            return found;

        }

        List<List<int>> GenerateSubsets(int[] nums)
        {
            var subsets = new List<List<int>>();
            GenerateSubsets(0, nums, new List<int>(), subsets);

            return subsets;
        }

        void GenerateSubsets(int index, int[] nums, List<int> current, List<List<int>> subsets)
        {
            subsets.Add(current);
            for(int i=index; i<nums.Length; i++)
            {
                current.Add(nums[i]);
                GenerateSubsets(i + 1, nums, current, subsets);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}
