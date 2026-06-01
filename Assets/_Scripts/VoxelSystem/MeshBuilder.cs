using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

public struct MeshBuilder
{
    private Chunk chunk;
    private MeshData meshData;

    public void BuildChunk(Chunk _chunk, MeshData _data)
    {
        chunk = _chunk;
        meshData = _data;
        meshData.Clear();

        GetVoxelsInChunk();
    }

    public void GetVoxelsInChunk()
    {
        for (int x = 0; x < chunk.ChunkSize; x++)
        for (int y = 0; y < chunk.ChunkSize; y++)
        for (int z = 0; z < chunk.ChunkSize; z++)
        {
            Voxel voxel = chunk.GetVoxel(x, y, z);
            
            if (voxel.ID == 0) continue;

            BuildVoxel(x, y, z, voxel);
        }
    }

    private void BuildVoxel(int x, int y, int z, Voxel voxel)
    {
        if(chunk.IsAir(x, y, z - 1)) AddQuad(0, x, y, z, voxel);
        if(chunk.IsAir(x, y, z + 1)) AddQuad(1, x, y, z, voxel);
        if(chunk.IsAir(x - 1, y, z)) AddQuad(2, x, y, z, voxel);
        if(chunk.IsAir(x + 1, y, z)) AddQuad(3, x, y, z, voxel);
        if(chunk.IsAir(x, y - 1, z)) AddQuad(4, x, y, z, voxel);
        if(chunk.IsAir(x, y + 1, z)) AddQuad(5, x, y, z, voxel);
    }

    public void AddQuad(int face, int x, int y, int z, Voxel voxel)
    {
        int startIndex = meshData.Vertices.Count;

        for (int i = 0; i < 4; i++)
        {
            int vertIndex = voxelVertexIndex[face, i];

            Vector3 corner =
                voxelVertices[vertIndex] +
            new Vector3(x, y, z);

            meshData.Vertices.Add(corner);
            meshData.UVs.Add(voxelUVs[i]);
        }

        for (int i = 0; i < 6; i++)
        {
            meshData.Triangles.Add(startIndex + voxelTris[face, i]);
        }
    }

    static readonly Vector3[] voxelVertices = new Vector3[8]
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(0,1,0),
        new Vector3(1,1,0),

        new Vector3(0,0,1),
        new Vector3(1,0,1),
        new Vector3(0,1,1),
        new Vector3(1,1,1)
    };

    static readonly int[,] voxelVertexIndex = new int[6, 4]
    {
        {0,1,2,3},
        {4,5,6,7},
        {4,0,6,2},
        {5,1,7,3},
        {0,1,4,5},
        {2,3,6,7}
    };

    static readonly Vector2[] voxelUVs = new Vector2[4]
    {
        new Vector2(0,0),
        new Vector2(1,0),
        new Vector2(0,1),
        new Vector2(1,1),
    };

    static readonly int[,] voxelTris = new int[6, 6]
    {
        {0,2,3,0,3,1},
        {0,1,2,1,3,2},
        {0,2,3,0,3,1},
        {0,1,2,1,3,2},
        {0,1,3,1,3,2},
        {0,2,3,0,3,1},
    };
}
