using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskCoordination
{
    class Ex2
    {
        public static ConcurrentDictionary<string, int> dict = new ConcurrentDictionary<string, int>();

        static void Main(string[] arg)
        {
            TaskSum();
        }

        public static void TaskSum()
        {
            Console.WriteLine("Start task");

            #region Array
            int[] intArray = new int[1000];

            for (int i = 1; i < 1000; i++)
            {
                intArray[i - 1] = i;
            }
            #endregion

            try
            {
                Task t1 = null;
                for (int i = 0, j = 50; i < 1000; i += 50, j += 50)
                {
                    List<int> curArr = intArray.Where(num => num > i && num < j).ToList();
                    t1 = new Task(() =>
                    {
                        var resSum = Sum(curArr.ToArray());
                        Console.WriteLine($"Sum: {resSum}");
                        dict.TryAdd("Res" + j / 50, resSum);
                    });

                    t1.Start();
                    t1.Wait();

                }
                Console.WriteLine($"Numbers of items in concurrent dictionary: {dict.Count}");

                Console.WriteLine("End task");
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static int Sum(int[] nums)
        {
            int sumNum = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                sumNum += nums[i];
            }
            return sumNum;
        }
    }
}
