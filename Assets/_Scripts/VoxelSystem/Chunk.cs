using System;
using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    [SerializeField] private Vector3 chunkPosition;
    private MeshData meshData = new();
    private MeshBuilder meshBuilder = new();
    private MeshUtils meshUtils = new();
    private Mesh mesh;

    public int ChunkSize;
    private Voxel[,,] voxels;

    public void InitializeChunk(Material material, Vector3 position)
    {
        ConfigureComponents();
        meshRenderer.sharedMaterial = material;
        chunkPosition = position;

        mesh = new Mesh();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        voxels = new Voxel[ChunkSize, ChunkSize, ChunkSize];
        for (int x = 0; x < ChunkSize; x++)
        for (int y = 0; y < ChunkSize; y++)
        for (int z = 0; z < ChunkSize; z++)
        {
            voxels[x, y, z].ID = VoxelType.Dirt;
        }

        meshBuilder.BuildChunk(this, meshData);

        meshUtils.ApplyMesh(mesh, meshData);
    }

    public Voxel GetVoxel(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= ChunkSize || y >= ChunkSize || z >= ChunkSize) return new Voxel { ID = VoxelType.Air };

        return voxels[x, y, z];
    }

    public bool IsAir(int x, int y, int z)
    {
        return GetVoxel(x, y, z).ID == VoxelType.Air;
    }

    private void ConfigureComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
    }
}
