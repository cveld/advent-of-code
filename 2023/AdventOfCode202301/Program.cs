using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

//var alllines = File.ReadAllLines("example2.txt");
var alllines = File.ReadAllLines("puzzle1.txt");
var founddigitslines = new Dictionary<int, Found2>();
var total = 0;
for (var line_index = 0; line_index < alllines.Length; line_index++)
{
    var found = new Found2();
    var pos = 0;
    var line = alllines[line_index];
    found.line = line;
    while (pos < line.Length) { 
        found.Process(line, pos);
        pos++;
    }
    founddigitslines.Add(line_index, found);
    var result = found.Result();
    total += result;
}
var file = new StringBuilder();
var recount = 0;
foreach (var found in founddigitslines)
{
    file.AppendLine($"{found.Value.line}, {found.Value.Result()}");
    recount += found.Value.Result();
}
File.WriteAllText("output.txt", file.ToString()); ;

Console.WriteLine($"Total is: {total} and {recount}");


void FirstPuzzle()
{
    //var alllines = File.ReadAllLines("example.txt");
    var alllines = File.ReadAllLines("puzzle1.txt");

    var founddigitslines = new Dictionary<int, Found1>();
    var total = 0;
    for (var line_index = 0; line_index < alllines.Length; line_index++)
    {
        var line = alllines[line_index];
        var founddigits = new Found1();
        for (int i = 0; i < line.Length; i++)
        {
            var digit = line[i];
            founddigits.Process(digit);
        }
        founddigitslines.Add(line_index, founddigits);
        total += founddigits.Result();
    }

    Console.WriteLine($"Total is: {total}");
}

class Found2
{
    int first;
    int last;
    Step process;
    public string? line;
    readonly Dictionary<string, int> tokens = new Dictionary<string, int>
    {
        //{ "0", 0 },
        { "1", 1 },
        { "2", 2 },
        { "3", 3 },
        { "4", 4 },
        { "5", 5 },
        { "6", 6 },
        { "7", 7 },
        { "8", 8 },
        { "9", 9 },
        //{ "zero", 0 },
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
        { "eight", 8 },
        { "nine", 9 },
    };
    public int Process(string line, int pos)
    {
        var found = false;
        var increment = 1;
        foreach (var token in tokens)
        {
            if (pos + token.Key.Length <= line.Length && line.Substring(pos, token.Key.Length) == token.Key)
            {
                if (process == Step.First)
                {
                    process = Step.Last;
                    first = token.Value;
                }
                last = token.Value;
                found = true;
                increment = token.Key.Length;
                break;
            }
        }
        return pos + increment;
    }
    public int Result()
    {
        return first * 10 + last;
    }
}


enum Step
{
    First,
    Last
}

class Found1 {
    public int firstDigit;
    public int lastDigit;
    public Step process = Step.First;
    public void Process(char digit)
    {
        if (digit >= '0' && digit <= '9')
        {
            var value = digit - '0';
            if (process == Step.First)
            {
                firstDigit = value;
                lastDigit = value;
                process = Step.Last;
            }
            else
            {
                lastDigit = value;
            }
        }
    }
    public int Result()
    {
        return firstDigit * 10 + lastDigit;
    }
}