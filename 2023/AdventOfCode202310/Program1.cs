using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode202310_Program1
{
    internal class Program1
    {
        public void Run(string file)
        {
            var lines = File.ReadAllLines(file);
            var map = new Char[lines[0].Length, lines.Length];
            var lineindex = 0;
            int startx = -1, starty = -1;
            foreach (var line in lines)
            {
                var colindex = 0;
                foreach (var c in line)
                {
                    map[colindex, lineindex] = c;
                    if (c == 'S')
                    {
                        startx = colindex;
                        starty = lineindex;
                    }
                    colindex++;
                }
                lineindex++;
            }

            var cursor1 = new Cursor(map);
            var cursor2 = new Cursor(map);
            cursor1.SetPosition((startx, starty));
            cursor2.SetPosition((startx, starty));
            var options = cursor1.GetOptions();
            cursor1.SetPosition(options[0]);
            cursor2.SetPosition(options[1]);
            int steps = 1;
            do {
                cursor1.SetPosition(cursor1.GetOptions()[0]);
                cursor2.SetPosition(cursor2.GetOptions()[0]);
                steps++;
            }
            while (cursor1.x != cursor2.x || cursor1.y != cursor2.y);
            Console.WriteLine($"Steps: {steps}");
        }
    }

    class Cursor
    {
        private readonly char[,] map;

        public Cursor(char[,] map)
        {
            this.map = map;
            maxx = map.GetLength(0);
            maxy = map.GetLength(1);
        }

        public int maxx, maxy;
        public int x = -1, y = -1;
        public int oldx = -1, oldy = -1;
        public void SetPosition((int x, int y) position)
        {
            this.oldx = this.x;
            this.oldy = this.y;
            this.x = position.x;
            this.y = position.y;
        }

        public List<(int x, int y)> GetOptions()
        {
            var options = new List<(int, int)>();
            if ("S-J7".IndexOf(map[x, y]) != -1 && x > 0 && !(x - 1 == oldx && y == oldy) && "-FL".IndexOf(map[x - 1, y]) != -1)
            {
                options.Add((x - 1, y));
            }
            if ("S-FL".IndexOf(map[x, y]) != -1 && x < maxx - 1 && !(x + 1 == oldx && y == oldy) && "-J7".IndexOf(map[x + 1, y]) != -1)
            {
                options.Add((x + 1, y));
            }
            if ("S|JL".IndexOf(map[x, y]) != -1 && y > 0 && !(x == oldx && y - 1 == oldy) && "|F7".IndexOf(map[x, y - 1]) != -1)
            {
                options.Add((x, y - 1));
            }
            if ("S|F7".IndexOf(map[x, y]) != -1 && y < maxy - 1 && !(x == oldx && y + 1 == oldy) && "|JL".IndexOf(map[x, y + 1]) != -1)
            {
                options.Add((x, y + 1));
            }
            return options;
        }
    }
}
