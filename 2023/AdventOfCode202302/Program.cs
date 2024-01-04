//var alllines = File.ReadAllLines("example.txt");
var alllines = File.ReadAllLines("puzzle1.txt");

var totalindex = 0;
foreach (var line in alllines)
{
    var index = line.Split(":");
    var indexint = int.Parse(index[0].Substring(5));
    var rounds = index[1].Trim().Split(';');
    var cubesmax = new Dictionary<string, int>();
    foreach (var round in rounds)
    {
        var validround = true;
        var colors = round.Split(',');
        foreach (var colorcount in colors)
        {
            var color = colorcount.Trim().Split(" ");
            var count = int.Parse(color[0]);
            if (!cubesmax.TryGetValue(color[1], out int value) || value < count)
            {
                cubesmax[color[1]] = count;
            }
        }
    }
    var power = 1;
    foreach (var kv in cubesmax)
    {
        power *= kv.Value;
    }
    totalindex += power;
}

Console.WriteLine($"Total: {totalindex}");