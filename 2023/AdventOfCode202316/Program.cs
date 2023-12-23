// See https://aka.ms/new-console-template for more information
Console.WriteLine("Solving first puzzle of advent of code 2023 day 16");
var all = await File.ReadAllLinesAsync("puzzleinput.txt");
//var all = await File.ReadAllLinesAsync("example.txt");
var xsize = all[0].Length;
var ysize = all.Length;

var arr = new char[xsize, ysize];
for (int x = 0; x < xsize; x++)
{
    for (int y = 0; y < ysize; y++)
    {
        arr[x, y] = all[y][x];
    }
}

// puzzle 1
var puzzle1_result = GetNumberOfEnergizedTiles(new Cursor(string.Empty, 0, 0, 0, Direction.Right));

// puzzle 2
var max_cursor_x = -1;
var max_cursor_y = -1;
var max_cursor_direction = Direction.Right;
var max_energized = 0;

// test top
for (var x = 0; x < xsize; x++)
{
    var result = GetNumberOfEnergizedTiles(new Cursor(string.Empty, 0, x: x, 0, Direction.Down));
    if (result > max_energized)
    {
        max_cursor_direction = Direction.Down;
        max_cursor_x = x;
        max_cursor_y = 0;
        max_energized = result;
    }
}

// test left
for (var y = 0; y < ysize; y++)
{
    var result = GetNumberOfEnergizedTiles(new Cursor(string.Empty, 0, 0, y: y, Direction.Right));
    if (result > max_energized)
    {
        max_cursor_direction = Direction.Right;
        max_cursor_x = 0;
        max_cursor_y = y;
        max_energized = result;
    }
}

// test bottom
for (var x = 0; x < xsize; x++)
{
    var result = GetNumberOfEnergizedTiles(new Cursor(string.Empty, 0, x: x, ysize - 1, Direction.Up));
    if (result > max_energized)
    {
        max_cursor_direction = Direction.Down;
        max_cursor_x = x;
        max_cursor_y = ysize - 1;
        max_energized = result;
    }
}

// test right
for (var y = 0; y < ysize; y++)
{
    var result = GetNumberOfEnergizedTiles(new Cursor(string.Empty, 0, xsize - 1, y: y, Direction.Up));
    if (result > max_energized)
    {
        max_cursor_direction = Direction.Up;
        max_cursor_x = xsize - 1;
        max_cursor_y = y;
        max_energized = result;
    }
}

Console.WriteLine($"Result is: {max_energized}, for cursor {max_cursor_x}, {max_cursor_y}, {max_cursor_direction}");

int GetNumberOfEnergizedTiles(Cursor startcursor)
{

    var hit = new bool[xsize, ysize];
    var directionloop = new Dictionary<Direction, bool>[xsize, ysize];
    var cursors = new List<Cursor>();
    cursors.Add(startcursor);

    var cursorscount = 0;
    while (cursors.Count > 0)
    {
        if (cursors.Count != cursorscount)
        {
            cursorscount = cursors.Count;
            Console.WriteLine($"Number of cursors: {cursorscount}");
        }
        var newlist = new List<Cursor>();
        foreach (var cursor in cursors)
        {
            //Console.WriteLine((cursor.baseId == string.Empty ? "" : cursor.baseId + ":") + cursor.childId + $": {cursor.x}, {cursor.y}, {cursor.direction}");
            if (hit[cursor.x, cursor.y])
            {
                if (directionloop[cursor.x, cursor.y].ContainsKey(cursor.direction))
                {
                    Console.WriteLine($"cursor is looping: {(cursor.baseId == string.Empty ? "" : cursor.baseId + ":") + cursor.childId} {cursor.x}, {cursor.y}, {cursor.direction}");
                    continue;
                }
                directionloop[cursor.x, cursor.y].Add(cursor.direction, true);
            }
            else
            {
                hit[cursor.x, cursor.y] = true;
                directionloop[cursor.x, cursor.y] = new Dictionary<Direction, bool>();
            }
            switch (arr[cursor.x, cursor.y])
            {
                case '.':
                    switch (cursor.direction)
                    {
                        case Direction.Right:
                            cursor.x += 1;
                            break;
                        case Direction.Left:
                            cursor.x -= 1;
                            break;
                        case Direction.Up:
                            cursor.y -= 1;
                            break;
                        case Direction.Down:
                            cursor.y += 1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(cursor.direction));
                    }
                    newlist.Add(cursor);
                    break;
                case '/':
                    switch (cursor.direction)
                    {
                        case Direction.Right:
                            cursor.y -= 1;
                            cursor.direction = Direction.Up;
                            break;
                        case Direction.Left:
                            cursor.y += 1;
                            cursor.direction = Direction.Down;
                            break;
                        case Direction.Up:
                            cursor.direction = Direction.Right;
                            cursor.x += 1;
                            break;
                        case Direction.Down:
                            cursor.x -= 1;
                            cursor.direction = Direction.Left;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(cursor.direction));
                    }
                    newlist.Add(cursor);
                    break;
                case '\\':
                    switch (cursor.direction)
                    {
                        case Direction.Right:
                            cursor.y += 1;
                            cursor.direction = Direction.Down;
                            break;
                        case Direction.Left:
                            cursor.y -= 1;
                            cursor.direction = Direction.Up;
                            break;
                        case Direction.Up:
                            cursor.direction = Direction.Left;
                            cursor.x -= 1;
                            break;
                        case Direction.Down:
                            cursor.x += 1;
                            cursor.direction = Direction.Right;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(cursor.direction));
                    }
                    newlist.Add(cursor);
                    break;
                case '-':
                    switch (cursor.direction)
                    {
                        case Direction.Right:
                            cursor.x += 1;
                            break;
                        case Direction.Left:
                            cursor.x -= 1;
                            break;
                        case Direction.Up:
                        case Direction.Down:
                            newlist.Add(new Cursor((cursor.baseId == String.Empty ? "" : cursor.baseId + ":") + cursor.childId, cursor.children, cursor.x - 1, cursor.y, Direction.Left));
                            cursor.direction = Direction.Right;
                            cursor.x += 1;
                            cursor.children++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(cursor.direction));
                    }
                    newlist.Add(cursor);

                    break;
                case '|':
                    switch (cursor.direction)
                    {
                        case Direction.Down:
                            cursor.y += 1;
                            break;
                        case Direction.Up:
                            cursor.y -= 1;
                            break;
                        case Direction.Left:
                        case Direction.Right:
                            newlist.Add(new Cursor((cursor.baseId == String.Empty ? "" : cursor.baseId + ":") + cursor.childId, cursor.children, cursor.x, cursor.y - 1, Direction.Up));
                            cursor.direction = Direction.Down;
                            cursor.y += 1;
                            cursor.children++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(cursor.direction));
                    }
                    newlist.Add(cursor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arr));
            }

        } // end loop through all existing cursors

        // check if any cursor is outside of grid
        cursors = new List<Cursor>();
        foreach (var cursor in newlist)
        {
            if (cursor.x < 0 || cursor.y < 0 || cursor.x >= xsize || cursor.y >= ysize)
            {
                continue;
            }
            cursors.Add(cursor);
        }
    }

    Console.WriteLine("Total hits:");
    int totalhits = 0;
    for (int x = 0; x < xsize; x++)
    {
        for (int y = 0; y < ysize; y++)
        {
            if (hit[x, y])
            {
                totalhits++;
            }
        }
    }
    Console.WriteLine(totalhits);
    return totalhits;
}

public enum Direction
{
    Left, Right, Up, Down
}
public class Cursor {
    public string baseId;
    public int childId;
    public int x;
    public int y;
    public Direction direction;
    public int children = 0;
    public Cursor(string baseId, int childId, int x, int y, Direction direction)
    {
        this.baseId = baseId;
        this.childId = childId;
        this.x = x;
        this.y = y;
        this.direction = direction;
    }
}