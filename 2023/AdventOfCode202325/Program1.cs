using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdventOfCode202325_Program1
{
    internal class Program1
    {
        public void Run(string file)
        {
            var lines = File.ReadAllLines(file);
            var edgesList = new List<Edge>();
            var nodesDict = new Dictionary<string, Node>();
            var nodesList = new List<Node>();
            var edgesDict = new Dictionary<string, Dictionary<string, Edge>>();

            // Scan all nodes
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (!nodesDict.ContainsKey(parts[0]))
                {
                    var node = new Node(parts[0]);
                    nodesDict.Add(parts[0], node);
                    nodesList.Add(node);
                }

                var edges = parts[1].Trim().Split(' ');
                foreach (var target in edges)
                {
                    if (!nodesDict.ContainsKey(target))
                    {
                        var targetnode = new Node(target);
                        nodesDict.Add(target, targetnode);
                        nodesList.Add(targetnode);
                    }
                }
            }

            // Add all edges
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                var edges = parts[1].Trim().Split(' ');
                var node = new Node(parts[0]);
                foreach (var target in edges) {
                    if (edgesDict.ContainsKey(parts[0]) && edgesDict[parts[0]].ContainsKey(target))
                    {
                        // edge already added
                        continue;
                    } 
                    var edge = new Edge(nodesDict[parts[0]], nodesDict[target]);
                    if (!edgesDict.ContainsKey(parts[0]))
                    {
                        edgesDict.Add(parts[0], new Dictionary<string, Edge>());
                    }
                    if (!edgesDict.ContainsKey(target))
                    {
                        edgesDict.Add(target, new Dictionary<string, Edge>());
                    }
                    edgesDict[parts[0]][target] = edge;
                    nodesDict[parts[0]].Edges.Add(target, edge);
                    edgesDict[target][parts[0]] = edge;
                    nodesDict[target].Edges.Add(parts[0], edge);
                    edgesList.Add(edge);
                }
            }

            var printindex = 0;
            for (var i1 = 0;  i1 < edgesList.Count - 2; i1++)
            {
                for (var i2 = i1 + 1; i2 < edgesList.Count - 1; i2++)
                {
                    for (var i3 = i2 + 1; i3 < edgesList.Count; i3++)
                    {
                        var excludedEdges = new List<Edge> { edgesList[i1], edgesList[i2], edgesList[i3] };
                        var result = FindGroups(edgesDict, nodesList, nodesDict, excludedEdges);
                        if (printindex % 1000 == 0 || result.Count == 2) Console.WriteLine($"{i1}, {i2}, {i3}: {result.Count}");
                        printindex++;
                        if (result.Count == 2) {
                            foreach (var  edge in excludedEdges) {
                                Console.WriteLine($"{edge.node1.name}/{edge.node2.name}");
                            }
                            Console.WriteLine($"Sizes: {result[0].Count}, {result[1].Count}. Multiplied: {result[0].Count * result[1].Count}");
                            goto found;
                        }
                    }

                }

            }
        found:;
        }

        private List<Dictionary<string, Node>> FindGroups(Dictionary<string, Dictionary<string, Edge>> edgesDict, List<Node> nodesList, Dictionary<string, Node> nodesDict, List<Edge> excludedEdges)
        {
            Stack<Node> nodes = new Stack<Node>();
            Dictionary<string, Node> done = new Dictionary<string, Node>();
            List<Dictionary<string, Node>> result = new List<Dictionary<string, Node>>();
            var groupindex = 0; 

            for (int i = 0; i < nodesList.Count; i++)
            {
                if (done.ContainsKey(nodesList[i].name))
                {
                    continue;
                }
                nodes.Push(nodesList[i]);
                var group = new Dictionary<string, Node>();
                result.Add(group);
                done.Add(nodesList[i].name, nodesList[i]);
                group.Add(nodesList[i].name, nodesList[i]);

                while (nodes.Count > 0) {
                    var node = nodes.Pop();

                    foreach (var edge in node.Edges)
                    {
                        if (done.ContainsKey(edge.Key))
                        {
                            continue;
                        }
                        // check if we must skip this edge:
                        var found = false;
                        foreach (var excludedEdge in excludedEdges)
                        {
                            if (excludedEdge.node1.name == node.name && excludedEdge.node2.name == edge.Key)
                            {
                                found = true;
                                break;
                            }
                            if (excludedEdge.node2.name == node.name && excludedEdge.node1.name == edge.Key)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            continue;
                        }
                        nodes.Push(nodesDict[edge.Key]);
                        done.Add(edge.Key, nodesDict[edge.Key]);
                        group.Add(edge.Key, nodesDict[edge.Key]);
                    }
                }
                groupindex++;
            }

            return result;
        }
    }

    class Node
    {
        public readonly string name;

        public Node(string name)
        {
            this.name = name;
            this.Edges = new Dictionary<string, Edge>();
        }

        public Dictionary<string, Edge> Edges { get; set; }
    }

    class Edge
    {
        public readonly Node node1;
        public readonly Node node2;
        public Edge(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }
    }
}
