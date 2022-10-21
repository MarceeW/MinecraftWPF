using OpenTK.Mathematics;

namespace Minecraft.Terrain
{
    internal enum EntityType
    {
        Tree
    }
    internal static class EntityData
    {
        public static VoxelEntity[] Entities { get; } = new VoxelEntity[]
        {
            new VoxelEntity(5,5,5,new Block[]
            {
                new Block(new Vector3(0,0,0),BlockType.OakTrunk),
                new Block(new Vector3(0,1,0),BlockType.OakTrunk),
                new Block(new Vector3(0,2,0),BlockType.OakTrunk),
                new Block(new Vector3(0,3,0),BlockType.OakTrunk),

                new Block(new Vector3(0,3,-1),BlockType.Leaves),
                new Block(new Vector3(0,3,-2),BlockType.Leaves),
                new Block(new Vector3(0,3,1),BlockType.Leaves),
                new Block(new Vector3(0,3,2),BlockType.Leaves),

                new Block(new Vector3(-2,3,0),BlockType.Leaves),
                new Block(new Vector3(-1,3,0),BlockType.Leaves),
                new Block(new Vector3( 1,3,0),BlockType.Leaves),
                new Block(new Vector3( 2,3,0),BlockType.Leaves),

                new Block(new Vector3(-2,3,1),BlockType.Leaves),
                new Block(new Vector3(-1,3,1),BlockType.Leaves),
                new Block(new Vector3( 1,3,1),BlockType.Leaves),
                new Block(new Vector3( 2,3,1),BlockType.Leaves),

                new Block(new Vector3(-1,3,2),BlockType.Leaves),
                new Block(new Vector3( 1,3,2),BlockType.Leaves),

                new Block(new Vector3(-2,3,-1),BlockType.Leaves),
                new Block(new Vector3(-1,3,-1),BlockType.Leaves),
                new Block(new Vector3( 1,3,-1),BlockType.Leaves),
                new Block(new Vector3( 2,3,-1),BlockType.Leaves),

                new Block(new Vector3(-1,3,-2),BlockType.Leaves),
                new Block(new Vector3( 1,3,-2),BlockType.Leaves),

                new Block(new Vector3(0,4,0),BlockType.Leaves),
                new Block(new Vector3(-1,4,0),BlockType.Leaves),
                new Block(new Vector3(0,4,1),BlockType.Leaves),
                new Block(new Vector3(1,4,0),BlockType.Leaves),
                new Block(new Vector3(0,4,-1),BlockType.Leaves),
                new Block(new Vector3(-1,4,1),BlockType.Leaves),
                new Block(new Vector3(1,4,-1),BlockType.Leaves),
                new Block(new Vector3(-1,4,-1),BlockType.Leaves),
                new Block(new Vector3(1,4,1),BlockType.Leaves),

                new Block(new Vector3(0, 5,0),BlockType.Leaves),
                new Block(new Vector3(-1, 5,0),BlockType.Leaves),
                new Block(new Vector3(0, 5,-1),BlockType.Leaves),
                new Block(new Vector3(1, 5,0),BlockType.Leaves),
                new Block(new Vector3(0, 5,1),BlockType.Leaves),
            })
        };
    }
}
