using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode202310_Program2
{
    internal class Program2
    {
        public void Run(string file)
        {
            var lines = File.ReadAllLines(file);
            var xmax = lines[0].Length;
            var ymax = lines.Length;
            var map = new Char[xmax, ymax];
            var pipes = new bool[xmax, ymax];
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
            var startchar = cursor1.DeterminePipe();
            map[startx, starty] = startchar;
            pipes[startx, starty] = true;
            var options = cursor1.GetOptions();
            cursor1.SetPosition(options[0]);
            cursor2.SetPosition(options[1]);
            int steps = 1;
            do {
                pipes[cursor1.x, cursor1.y] = true;
                pipes[cursor2.x, cursor2.y] = true;
                cursor1.SetPosition(cursor1.GetOptions()[0]);
                cursor2.SetPosition(cursor2.GetOptions()[0]);
                steps++;
            }
            while (cursor1.x != cursor2.x || cursor1.y != cursor2.y);
            pipes[cursor1.x, cursor1.y] = true;

            Console.WriteLine($"Steps: {steps}");

            // now we have determined where the pipes are residing, we start scanning vertically from left to right
            var totalcount = 0;
            for (int y = 2; y < ymax; y++) {
                var outside = true;
                var lastup = false;
                var lastchar = '.';
                int count = 0;
                var foundleft = false;
                for (int x = 0; x < xmax; x++)
                {
                    var orig = outside;
                    var pipe = pipes[x, y] ? map[x, y] : '.';
                    if (pipes[x, y])
                    {
                        switch (pipe)
                        {
                            case '|':
                                outside = !outside;
                                break;
                            case '-':
                                // do nothing
                                break;
                            case '7':
                            case 'F':
                            case 'J':
                            case 'L':
                                var up = "JL".IndexOf(pipe) != -1;
                                //if (lastchar != '.' && lastchar != '|')
                                {
                                    if (up == !lastup && foundleft)
                                    {
                                        outside = !outside;
                                    }
                                }
                                //else
                                {
                                    //outside = !outside;
                                }
                                if (!foundleft) lastup = up;
                                foundleft = !foundleft;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(pipe));
                        }
                    }
                    else
                    {
                        if (!outside)
                        {
                            count++;
                        }
                    }
                    lastchar = pipe;
                }
                if (foundleft)
                {
                    throw new Exception("Turns always come in pairs");
                }
                if (!outside)
                {
                    throw new Exception("The right of the puzzle should always be outside");
                }
                totalcount += count;
                Console.WriteLine($"Line {y}: {count}");
            }
            Console.WriteLine($"total: {totalcount}");
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

        public char DeterminePipe()
        {
            var s = string.Empty;
            if ("S-J7".IndexOf(map[x, y]) != -1 && x > 0 && !(x - 1 == oldx && y == oldy) && "-FL".IndexOf(map[x - 1, y]) != -1)
            {
                s += "L";
            }
            if ("S-FL".IndexOf(map[x, y]) != -1 && x < maxx - 1 && !(x + 1 == oldx && y == oldy) && "-J7".IndexOf(map[x + 1, y]) != -1)
            {
                s += "R";
            }
            if ("S|JL".IndexOf(map[x, y]) != -1 && y > 0 && !(x == oldx && y - 1 == oldy) && "|F7".IndexOf(map[x, y - 1]) != -1)
            {
                s += "U";
            }
            if ("S|F7".IndexOf(map[x, y]) != -1 && y < maxy - 1 && !(x == oldx && y + 1 == oldy) && "|JL".IndexOf(map[x, y + 1]) != -1)
            {
                s += "D";
            }

            switch (s)
            {
                case "LU":
                    return 'J';
                case "LD":
                    return '7';
                case "LR":
                    return '-';
                case "RU":
                    return 'L';
                case "RD":
                    return 'F';
                case "UD":
                    return '|';
                default:
                    throw new ArgumentOutOfRangeException(nameof(s));
            }
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
