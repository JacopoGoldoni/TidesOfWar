using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GraphVertex<T>
{
    public T value;
}
public class GraphEdge<T>
{
    public (int, int) nodes;
    public T value;
    public bool oriented = false;
}

public class Graph<T,U>
{
    public List<GraphVertex<T>> vertices;
    public List<GraphEdge<U>> edges;

    //VERTEX METHODS
    public int VertexDegree(int vertex)
    {
        int d = 0;
        foreach (GraphEdge<U> e in edges)
        {
            if (e.nodes.Item1 == vertex || e.nodes.Item2 == vertex) d++;
        }
        return d;
    }
    public int Order() { return vertices.Count; }
    public int[] AdjacentVertices(int vertex)
    {
        List<int> v = new List<int>();
        foreach(GraphEdge<U> e in edges)
        {
            if (e.nodes.Item1 == vertex)
            {
                v.Add(e.nodes.Item2);
            }
            else if(e.nodes.Item2 == vertex)
            {
                v.Add(e.nodes.Item1);
            }
        }
        return v.ToArray();
    }
    public int[] ConnectedEdges(int vertex)
    {
        List<int> connectedEdges = new List<int>();
        for(int i = 0; i < edges.Count; i++)
        {
            if (edges[i].nodes.Item1 == vertex || edges[i].nodes.Item2 == vertex)
            {
                connectedEdges.Add(i);
            }
        }

        return connectedEdges.ToArray();
    }
    
    //EDGE METHODS
    public int Size() { return edges.Count; }
    public int[] AdjacentEdges(int edge)
    {
        List<int> adjacentEdges = new List<int>();
        int v1 = edges[edge].nodes.Item1;
        int v2 = edges[edge].nodes.Item2;

        for(int i = 0; i < edges.Count; i++)
        {
            if(i == edge)
            {
                //NO SELF DETECT
                continue;
            }

            GraphEdge<U> e = edges[i];
            if(e.nodes.Item1 == v1 || e.nodes.Item1 == v2 || 
                e.nodes.Item2 == v1 || e.nodes.Item2 == v2)
            {
                adjacentEdges.Add(i);
            }
        }

        return adjacentEdges.ToArray();
    }
    public U GetEdgeValue(int v1, int v2)
    {
        foreach(GraphEdge<U> e in edges)
        {
            if(e.oriented)
            {
                if(e.nodes.Item1 == v1 && e.nodes.Item2 == v2)
                {
                    return e.value;
                }
            }
            else
            {
                if ((e.nodes.Item1 == v1 && e.nodes.Item2 == v2) || (e.nodes.Item1 == v2 && e.nodes.Item2 == v1))
                {
                    return e.value;
                }
            }
        }
        return default;
    }

    //GRAPH OPERATIONS
    public void RemoveVertex(int vertex)
    {
        int[] connectedEdgeIndexes = ConnectedEdges(vertex);
        List<GraphEdge<U>> connectedEdges = new List<GraphEdge<U>> ();
        foreach(int eIndex in connectedEdgeIndexes)
        {
            connectedEdges.Add(edges[eIndex]);
        }

        foreach(GraphEdge<U> e in connectedEdges)
        {
            edges.Remove(e);
        }
        vertices.RemoveAt(vertex);
    }
    public void RemoveEdge(int v1, int v2)
    {
        foreach(GraphEdge<U> e in edges)
        {
            if(e.nodes.Item1 == v1 && e.nodes.Item2 == v2)
            {
                edges.Remove(e);
                return;
            }
            else if(e.nodes.Item1 == v2 && e.nodes.Item2 == v1)
            {
                edges.Remove(e);
                return;
            }
        }
    }
    public void EdgeContraction(int edge)
    {
        int[] affectedEdges = AdjacentEdges(edge);
        int removedVertex = Mathf.Max(edges[edge].nodes.Item1, edges[edge].nodes.Item2);
        int collapsedVertex = Mathf.Min(edges[edge].nodes.Item1, edges[edge].nodes.Item2);

        foreach(int e in affectedEdges)
        {
            if (edges[e].nodes.Item1 == removedVertex)
            {
                edges[e].nodes.Item1 = collapsedVertex;
            }
            else if (edges[e].nodes.Item2 == removedVertex)
            {
                edges[e].nodes.Item2 = collapsedVertex;
            }
        }

        edges.RemoveAt(edge);
        vertices.RemoveAt(removedVertex);
    }
    public Matrix AdjacencyMatrix()
    {
        Matrix A = new Matrix(vertices.Count, vertices.Count);
        
        for(int i = 0; i < edges.Count; i++)
        {
            if(edges[i].oriented)
            {
                A.values[edges[i].nodes.Item1, edges[i].nodes.Item2] = 1;
            }
            else
            {
                A.values[edges[i].nodes.Item1, edges[i].nodes.Item2] = 1;
                A.values[edges[i].nodes.Item2, edges[i].nodes.Item1] = 1;
            }
        }

        return A;
    }
    public Matrix DegreeMatrix()
    {
        Matrix A = new Matrix(vertices.Count, vertices.Count);

        for (int i = 0; i < vertices.Count; i++)
        {
            int d = 0;
            for(int j = 0; j < edges.Count; j++)
            {
                if (edges[i].oriented)
                {
                    if (edges[j].nodes.Item1 == i)
                    {
                        d++;
                    }
                }
                else
                {
                    if (edges[j].nodes.Item1 == i || edges[j].nodes.Item2 == i)
                    {
                        d++;
                    }
                }
            }
            A.values[i, i] = d;
        }

        return A;
    }
    public Matrix LaplacianMatrix()
    {
        return DegreeMatrix() - AdjacencyMatrix();
    }
}