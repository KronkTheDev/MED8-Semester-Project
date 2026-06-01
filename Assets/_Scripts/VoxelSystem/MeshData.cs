using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> Vertices = new();
    public List<int> Triangles = new();
    public List<Vector2> UVs = new();

    public void Clear()
    {
        Vertices.Clear();
        Triangles.Clear();
        UVs.Clear();
    }
}