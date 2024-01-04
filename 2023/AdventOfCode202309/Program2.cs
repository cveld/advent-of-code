using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode202309
{
    internal class Program2
    {
        public void Run(string input)
        {
            var lines = File.ReadAllLines(input);
            var lineindex = 0;
            var total = 0m;
            foreach (var line in lines)
            {
                var numbers = line.Split(' ');
                List<List<Decimal>> list = new List<List<Decimal>>();
                var newlist = new List<Decimal>();
                foreach (var number in numbers)
                {
                    var n = int.Parse(number);
                    newlist.Add(n);
                }
                list.Add(newlist);
                bool allzeroes;
                do
                {
                    var currlist = newlist;
                    newlist = new List<decimal>();
                    allzeroes = true;
                    for (int i = 0; i < currlist.Count - 1; i++)
                    {
                        var diff = currlist[i + 1] - currlist[i];
                        newlist.Add(diff);
                        if (diff != 0)
                        {
                            allzeroes = false;
                        }
                    }
                    list.Add(newlist);
                    
                } while (!allzeroes);

                var result = 0m;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    result = list[i][0] - result;
                }
                lineindex++;
                Console.WriteLine($"Line {lineindex}: {result}");
                total += result;
            } // all lines
            Console.WriteLine($"Total: {total}");
        }
    }
}
