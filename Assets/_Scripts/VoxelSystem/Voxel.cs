using UnityEngine;

public struct Voxel
{
    public byte ID;
}

public static class VoxelType
{
    public const byte Air = 0;
    public const byte Dirt = 1;
    public const byte Stone = 2;
}

public struct VoxelMaterials
{
    [SerializeField] private Material[] materials;

    public Material GetVoxelMaterial(int _voxelType) { return materials[_voxelType]; }
}