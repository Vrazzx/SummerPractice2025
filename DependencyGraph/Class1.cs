using System;
using System.Collections.Generic;
using System.Linq;

public class DependencyGraph<T>
{
    private readonly Dictionary<T, List<T>> _adjacencyList = new Dictionary<T, List<T>>();

    public void AddNode(T node)
    {
        if (!_adjacencyList.ContainsKey(node))
        {
            _adjacencyList[node] = new List<T>();
        }
    }

    public void AddEdge(T from, T to)
    {
        if (!_adjacencyList.ContainsKey(from))
            AddNode(from);
        if (!_adjacencyList.ContainsKey(to))
            AddNode(to);
            
        _adjacencyList[from].Add(to);
    }

    public List<T> TopologicalSort()
    {
        var visited = new HashSet<T>();
        var tempMark = new HashSet<T>();
        var result = new List<T>();

        foreach (var node in _adjacencyList.Keys)
        {
            if (!visited.Contains(node))
            {
                Visit(node, visited, tempMark, result);
            }
        }

        result.Reverse();
        return result;
    }

    private void Visit(T node, HashSet<T> visited, HashSet<T> tempMark, List<T> result)
    {
        if (tempMark.Contains(node))
            throw new InvalidOperationException("Cyclic dependency detected");

        if (visited.Contains(node))
            return;

        tempMark.Add(node);
        
        foreach (var neighbor in _adjacencyList[node])
        {
            Visit(neighbor, visited, tempMark, result);
        }

        tempMark.Remove(node);
        visited.Add(node);
        result.Add(node);
    }
}