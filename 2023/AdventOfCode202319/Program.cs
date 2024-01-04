//var alllines = File.ReadAllLines("example.txt");
var alllines = File.ReadAllLines("puzzle1.txt");
var rules = new Dictionary<string, Rule>();
var parts = new List<string>();
var keys = new HashSet<string>();

var rules_phase = true;
foreach (var line in alllines)
{
    if (line == String.Empty)
    {
        rules_phase = false;
        continue;
    }
    if (rules_phase)
    {
        var rule = new Rule(line);
        rules.Add(rule.name, rule);
        foreach (var e in rule.expressions)
        {
            if (e.compare != Compare.Done) {
                if (!keys.Contains(e.key!)) keys.Add(e.key!);
            }
        }
    }
    else parts.Add(line);
}

var allresults = new List<Eval>();
var evalstack = new Stack<Eval>();

var firsteval = new Eval();
foreach (var k in keys)
{
    firsteval.vars.Add(k, (1, 4000));
}
firsteval.rule = "in";
evalstack.Push(firsteval);

while (evalstack.Count > 0) { 
    var currenteval = evalstack.Pop();
    var currentRule = rules[currenteval.rule]; 
    foreach (var expression in currentRule.expressions)
    {
        var neweval = currenteval.CreateEval();
        if (expression.compare == Compare.Done)
        {
            if (expression.result == "R")
            {
                // don't count rejected paths
                break;
            }
            if (expression.result == "A")
            {
                allresults.Add(neweval);                    
                break;
            }

            neweval.rule = expression.result;
            evalstack.Push(neweval);
            break;
        }

        if (expression.compare == Compare.LessThan)
        {
            neweval.SetRange(expression.key!, (1, expression.value!.Value - 1));
            currenteval.SetRange(expression.key!, (expression.value!.Value, 4000));
        }

        if (expression.compare == Compare.GreaterThan)
        {
            neweval.SetRange(expression.key!, (expression.value!.Value + 1, 4000));
            currenteval.SetRange(expression.key!, (1, expression.value!.Value));
        }

        if (expression.result == "R")
        {
            // don't count rejected paths
            continue;
        }
        if (expression.result == "A")
        {
            allresults.Add(neweval!);
            continue;
        }
        neweval!.rule = expression.result;
        evalstack.Push(neweval);            
    }
}

Decimal totalcount = 0;
foreach (var eval in allresults)
{
    Decimal mult = 1;
    foreach (var k in eval.vars)
    {
        mult *= k.Value.heigh - k.Value.low + 1;
    }
    totalcount += mult;
}
Console.WriteLine($"Total count = {totalcount}");

enum Result
{
    Undefined,
    Rejected,
    Accepted
}

class Eval
{
    public Dictionary<string, (int low, int heigh)> vars = new Dictionary<string, (int, int)>();
    public string rule;
    public Eval CreateEval()
    {
        var vars = new Dictionary<string, (int low, int heigh)>();
        foreach (var v in this.vars)
        {
           vars.Add(v.Key, v.Value);
        }
        var eval = new Eval();
        eval.vars = vars;
        return eval;
    }

    public void SetRange(string key, (int low, int heigh) range)
    {
        var v = vars[key];
        v.low = v.low < range.low ? range.low : v.low;
        v.heigh = v.heigh > range.heigh ? range.heigh : v.heigh;
        vars[key] = v;
    }
}

class Rule
{
    public List<Expression> expressions = new List<Expression>();
    public string name;
    public Rule(string rule)
    {
        var split = rule.Split('{');
        name = split[0];
        var segments = split[1].Substring(0, split[1].Length - 1).Split(',');
        foreach (var segment in segments)
        {
            expressions.Add(new Expression(segment));
        }
    }
}
enum Compare
{
    Undefined,
    GreaterThan,
    LessThan,
    Done
}
class Expression
{
    public string? key;
    public int? value;
    public string result;
    public Compare compare;

    public Expression(string expression)
    {
        var segments = expression.Split(":");
        if (segments.Length == 1 ) {
            compare = Compare.Done;
            result = expression;
            return;
        }
        result = segments[1];
        if (segments[0][1] == '<')
        {
            compare = Compare.LessThan;
        }
        if (segments[0][1] == '>')
        {
            compare = Compare.GreaterThan;
        }
        key = segments[0][0].ToString();
        value = int.Parse(segments[0].Substring(2));
    }
}