//var all = File.ReadAllLines("example.txt");
var all = File.ReadAllLines("puzzle1.txt");

var xsize = all[0].Length;
var ysize = all.Length;

var puzzle = new int[xsize, ysize];
var nodes = new Dictionary<Direction, Node>[xsize, ysize];

var nodeBuilder = new NodeBuilder(puzzle, nodes);
for (int x = 0; x < xsize; x++)
{
    for (int y = 0; y < ysize; y++)
    {
        puzzle[x, y] = all[y][x] - '0';
        nodes[x, y] = new Dictionary<Direction, Node>();
        nodes[x, y].Add(Direction.Horizontal, nodeBuilder.CreateNode(x, y, Direction.Horizontal));
        nodes[x, y].Add(Direction.Vertical, nodeBuilder.CreateNode(x, y, Direction.Vertical));
    }
}

nodes[0, 0].Add(Direction.All, nodeBuilder.CreateNode(0, 0, Direction.All));
nodes[0, 0][Direction.All].AddNeighbours();

for (int x = 0; x < xsize; x++)
{
    for (int y = 0; y < ysize; y++)
    {
        nodes[x, y][Direction.Horizontal].AddNeighbours();
        nodes[x, y][Direction.Vertical].AddNeighbours();
    }
}

// Dijkstra's shortest path
// 1. All nodes have distance set to infinite
// 2. All nodes have visited-flag set to false
// 3. Set the root to distance 0
nodes[0, 0][Direction.All].distance = 0;

// The following loop takes the non-visited node with the least distance to the root
int visitedCount = 0;
do
{
    Node? foundNode = null;
    int foundDistance = int.MaxValue;
    for (int x = 0; x < xsize; x++)
    {
        for (int y = 0; y < ysize; y++)
        {
            var crosspoint = nodes[x, y];
            if (crosspoint.TryGetValue(Direction.All, out Node? n1) && !n1.visited && n1.distance < foundDistance)
            {
                foundNode = n1;
                foundDistance = n1.distance;
            }
            if (crosspoint.TryGetValue(Direction.Horizontal, out Node? n2) && !n2.visited && n2.distance < foundDistance)
            {
                foundNode = n2;
                foundDistance = n2.distance;
            }
            if (crosspoint.TryGetValue(Direction.Vertical, out Node? n3) && !n3.visited && n3.distance < foundDistance)
            {
                foundNode = n3;
                foundDistance = n3.distance;
            }
        }
    }
    if (foundNode == null)
    {
        // everything visited. we are done
        break;
    }
    foundNode.visited = true;
    visitedCount++;
    Console.WriteLine($"Visited index: {visitedCount}; [{foundNode.x}, {foundNode.y}, {foundNode.allowedDirections}], distance: {foundNode.distance}");

    // Let's loop through all neighbours and adjust the distance table
    foreach (var node_weight in foundNode.neighbours)
    {
        var newDistance = foundNode.distance + node_weight.Value;
        if (newDistance < node_weight.Key.distance)
        {
            node_weight.Key.distance = newDistance;
            node_weight.Key.previousNode = foundNode;
        }
    }

} while (true);

// Dijkstra's shortest path results in a database which we can use to reproduce the shortest path from root to any other node
// For day 17's puzzle we only require to provide the shortest distance. In this puzzle we can reach the ultimate node from two directions:
Console.WriteLine(nodes[xsize - 1, ysize - 1][Direction.Horizontal].distance);
Console.WriteLine(nodes[xsize - 1, ysize - 1][Direction.Vertical].distance);


enum Direction
{
    All,
    Horizontal,
    Vertical
}


class NodeBuilder
{
    private readonly int[,] puzzle;
    Dictionary<Direction, Node>[,] nodes;

    public NodeBuilder(int[,] puzzle, Dictionary<Direction, Node>[,] nodes)
    {
        this.puzzle = puzzle;
        this.nodes = nodes;
    }
    public Node CreateNode(int x, int y, Direction direction)
    {
        var node = new Node(puzzle, nodes, x, y, direction);
        return node;
    }
}

//record Node(int x, int y);
class Node
{
    int xsize, ysize;
    private readonly int[,] puzzle;
    private readonly Dictionary<Direction, Node>[,] nodes;
    public readonly int x;
    public readonly int y;
    public Direction allowedDirections;
    public Node? previousNode;
    public int distance = int.MaxValue;

    public Node(int[,] puzzle, Dictionary<Direction, Node>[,] nodes, int x, int y, Direction alloweddirections)
    {
        xsize = puzzle.GetLength(0);
        ysize = puzzle.GetLength(1);
        this.puzzle = puzzle;
        this.nodes = nodes;
        this.x = x;
        this.y = y;
        this.allowedDirections = alloweddirections;
    }
    public Dictionary<Node, int> neighbours = new Dictionary<Node, int> ();
    public bool visited;
    public void AddNeighbours()
    {
        if (allowedDirections == Direction.All || allowedDirections == Direction.Horizontal)
        {
            int weight = 0;
            for (int xi = 1; xi <= 10; xi++)
            {
                if (x + xi < xsize)
                {
                    weight += puzzle[x + xi, y];
                    if (xi >= 4) neighbours.Add(nodes[x + xi, y][Direction.Vertical], weight);
                }
            }
            weight = 0;
            for (int xi = 1; xi <= 10; xi++)
            {
                if (x - xi >= 0)
                {
                    weight += puzzle[x - xi, y];
                    if (xi >= 4) neighbours.Add(nodes[x - xi, y][Direction.Vertical], weight);
                }
            }
        }
        if (allowedDirections == Direction.All || allowedDirections == Direction.Vertical)
        {
            int weight = 0;
            for (int yi = 1; yi <= 10; yi++)
            {
                if (y + yi < ysize)
                {
                    weight += puzzle[x, y + yi];
                    if (yi >= 4) neighbours.Add(nodes[x, y + yi][Direction.Horizontal], weight);
                }
                else break;
            }
            weight = 0;
            for (int yi = 1; yi <= 10; yi++)
            {
                if (y - yi >= 0)
                {
                    weight += puzzle[x, y - yi];
                    if (yi >= 4) neighbours.Add(nodes[x, y - yi][Direction.Horizontal], weight);
                }
            }
        }
    }
    public void AddNeighbours1()
    {
        if (allowedDirections == Direction.All || allowedDirections == Direction.Horizontal)
        {
            int weight = 0;
            for (int xi = 1; xi < 4; xi++)
            {
                if (x + xi < xsize)
                {
                    weight += puzzle[x + xi, y];
                    neighbours.Add(nodes[x + xi, y][Direction.Vertical], weight);
                }
            }
            weight = 0;
            for (int xi = 1; xi < 4; xi++)
            {
                if (x - xi >= 0)
                {
                    weight += puzzle[x - xi, y];
                    neighbours.Add(nodes[x - xi, y][Direction.Vertical], weight);
                }
            }
        }
        if (allowedDirections == Direction.All || allowedDirections == Direction.Vertical)
        {
            int weight = 0;
            for (int yi = 1; yi < 4; yi++)
            {
                if (y + yi < ysize)
                {
                    weight += puzzle[x, y + yi];
                    neighbours.Add(nodes[x, y + yi][Direction.Horizontal], weight);
                }
            }
            weight = 0;
            for (int yi = 1; yi < 4; yi++)
            {
                if (y - yi >= 0)
                {
                    weight += puzzle[x, y - yi];
                    neighbours.Add(nodes[x, y - yi][Direction.Horizontal], weight);
                }
            }
        }
    }
}
