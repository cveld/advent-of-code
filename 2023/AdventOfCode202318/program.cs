using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text;

var lines = File.ReadAllLines("puzzle1.txt");
//var lines = File.ReadAllLines("example.txt");
var edges = new Dictionary<int, Dictionary<int, Edge>>();
var x = 0;
var y = 0;
var xsizemax = 0;
var xsizemin = 0;
var ysizemax = 0;
var ysizemin = 0;

var linesegments = new SortedList<int, LineSegment>(new DuplicateKeyComparer<int>());

foreach (var line in lines)
{
    var split = line.Split(' ');
    var direction = split[2][7];
    int count = int.Parse(split[2].Substring(2, 5), System.Globalization.NumberStyles.HexNumber);
    //var direction = split[0];
    //var count = int.Parse(split[1]);
    var origEdge = FetchEdge(edges, x, y, null, null);
    switch (direction)
        {
            case '0': // "R":
                x += count;
                var newEdge = FetchEdge(edges, x, y, Horizontal_Direction.Left, null);
                newEdge.SetHorizontalNeighbour(origEdge);
                break;
        case '2': // "L":
                x -= count;
                var newEdgeLeft = FetchEdge(edges, x, y, Horizontal_Direction.Right, null);
                newEdgeLeft.SetHorizontalNeighbour(origEdge);
                break;
        case '1': // "D":
                y += count;
                var newEdgeDown = FetchEdge(edges, x, y, null, Vertical_Direction.Up);
                linesegments.Add(x, new LineSegment(origEdge, newEdgeDown));
                break;
        case '3': // "U":
                y -= count;
                var newEdgeUp = FetchEdge(edges, x, y, null, Vertical_Direction.Down);
                linesegments.Add(x, new LineSegment(newEdgeUp, origEdge));
            break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction));
        }
        if (xsizemax < x + 1) xsizemax = x + 1;
        if (x < xsizemin) xsizemin = x;
        if (y < ysizemin) ysizemin = y;
        if (ysizemax < y + 1) ysizemax = y + 1;
}
Console.WriteLine($"Size = {xsizemin}, {ysizemin} - {xsizemax}, {ysizemax}, line segments = {linesegments.Count}, edges = {edges.Count}");

Decimal totalcount = 0;
for (int yi = ysizemin; yi < ysizemax; yi++)
{
    var inside = false;
    var beforeinside = false;
    var linecount = 0;
    var xi = xsizemin;
    foreach (var linesegment in linesegments)
    {
        if (yi < linesegment.Value.top.y) continue;
        if (yi > linesegment.Value.bottom.y) continue;
        
        if (!inside)
        {
            xi = linesegment.Value.top.x;
        }
        if (yi != linesegment.Value.top.y && yi != linesegment.Value.bottom.y)
        {
            // optimization opportunity: fast forward to next linesegment
            beforeinside = inside;
            inside = !inside;
        }
        else
        {
            var comparer = linesegment.Value.top.y == yi ? linesegment.Value.top : linesegment.Value.bottom;
            if (comparer.horizontal_direction == Horizontal_Direction.Right)
            {
                beforeinside = inside;
                inside = true;
                // optimization opportunity: fast forward to connected line segment
            }
            if (comparer.horizontal_direction == Horizontal_Direction.Left)
            {
                if (comparer.horizontal_neighbour!.vertical_direction == comparer.vertical_direction)
                {
                    inside = beforeinside;
                }
                else
                {
                    inside = !beforeinside;
                }
            }
        }
        if (!inside)
        {
            linecount += linesegment.Value.top.x - xi + 1;
            xi = linesegment.Value.top.x + 1;
        }
    }
    if (yi % 100000 == 0) Console.WriteLine($"Line {yi}: {linecount}");
    totalcount += linecount;
}

Console.WriteLine($"Total count = {totalcount}");

Edge FetchEdge(Dictionary<int, Dictionary<int, Edge>> edges, int x, int y, Horizontal_Direction? horizontal_direction, Vertical_Direction? vertical_direction)
{    
    if (!edges.ContainsKey(y))
    {
        edges.Add(y, new Dictionary<int, Edge>());
    }
    if (!edges[y].ContainsKey(x))
    {
        var edge = new Edge(x, y, horizontal_direction, vertical_direction);
        edges[y].Add(x, edge);
        return edge;
    }
    else
    {
        // this can only be the start edge
        // no need to add it
        return edges[y][x];
    }
}

class LineSegment
{
    public Edge top, bottom;

    public LineSegment(Edge top, Edge bottom)
    {
        this.top = top;
        this.bottom = bottom;
        top.vertical_direction = Vertical_Direction.Down;
        bottom.vertical_direction = Vertical_Direction.Up;
    }
}

class Edge
{
    public int x;
    public int y;
    public Horizontal_Direction? horizontal_direction;
    public Vertical_Direction? vertical_direction;
    public Edge? horizontal_neighbour;
    public Edge? vertical_neighbour;
    public Edge(int x, int y, Horizontal_Direction? horizontal_direction, Vertical_Direction? vertical_direction)
    {
        this.x = x;
        this.y = y;
        this.horizontal_direction = horizontal_direction;
        this.vertical_direction = vertical_direction;
    }

    public void SetHorizontalNeighbour(Edge edge)
    {
        this.horizontal_neighbour = edge;
        edge.horizontal_neighbour = this;
        switch (horizontal_direction)
        {
            case Horizontal_Direction.Left:
                edge.horizontal_direction = Horizontal_Direction.Right; 
                break;
            case Horizontal_Direction.Right:
                edge.horizontal_direction = Horizontal_Direction.Left;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(horizontal_direction));
        }
    }
    public string ToString()
    {
        return $"{x}, {y}";
    }
}

enum Horizontal_Direction
{
    Left, Right
}

enum Vertical_Direction
{
    Up, Down
}

/// <summary>
/// Comparer for comparing two keys, handling equality as beeing greater
/// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
{
    #region IComparer<TKey> Members

    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1; // Handle equality as being greater. Note: this will break Remove(key) or
        else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
            return result;
    }

    #endregion
}