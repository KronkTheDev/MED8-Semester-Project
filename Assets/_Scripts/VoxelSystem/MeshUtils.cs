using UnityEngine;

// Removed the UnityEditor using statement that was causing the build to fail
// as it is not needed for mesh generation.

public struct MeshUtils
{
    // Uploads the meshdata to the correct mesh
    public void ApplyMesh(Mesh mesh, MeshData meshData)
    {
        mesh.Clear();

        mesh.SetVertices(meshData.Vertices);
        mesh.SetTriangles(meshData.Triangles, 0);
        mesh.SetUVs(0, meshData.UVs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
