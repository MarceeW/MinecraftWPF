using Minecraft.Graphics;

namespace Minecraft.Terrain
{
    internal enum BlockType
    {
        Air, GrassBlock, Stone, Dirt, Leaves, OakTrunk, Glass, Sand, Water, Bedrock, Cobblestone, WoodPlank, Grass
    }
    internal struct Position2D
    {
        public int column, row;

        public Position2D(int row, int column)
        {
            this.column = column;
            this.row = row;
        }
    }
    internal static class AtlasTexturesData
    {
        //Top = 1st Bot = 2nd Other = 3rd
        public static readonly Position2D[][] TexturePositions =
        {
            new Position2D[0],
            //Grass
            new Position2D[3]{ new Position2D(0,13), new Position2D(0, 9), new Position2D(0, 11), },
            //Stone
            new Position2D[1]{ new Position2D(1,7) },
            //Dirt
            new Position2D[1]{ new Position2D(0,9) },
            //Leaves
            new Position2D[1]{ new Position2D(1,4) },
            //OakTrunk
            new Position2D[3]{ new Position2D(1,3), new Position2D(1, 3), new Position2D(1, 1), },
            //Glass
            new Position2D[1]{ new Position2D(2,0) },
            //Sand
            new Position2D[1]{ new Position2D(0,12) },
            //Water
            new Position2D[1]{ new Position2D(1,10) },
            //Bedrock
            new Position2D[1]{ new Position2D(0,2) },
            //Cobblestone
            new Position2D[1]{ new Position2D(0,8) },
            //WoodPlank
            new Position2D[1]{ new Position2D(1,6) },
            //Grass
            new Position2D[1]{ new Position2D(0,10) },

        };
        public static readonly int TextureSize = 16;
        public static Texture Atlas
        {
            get
            {
                if (atlas == null)
                {
                    atlas = new Texture(@"..\..\..\Assets\Textures\terrain.png", false);
                }
                return atlas;
            }
        }

        private static Texture? atlas;
        public static float[] GetTextureCoords(BlockType type, FaceDirection? face)
        {
            var TexturePosition = TexturePositions[(int)type];

            float PositionDeltaPerRow = TextureSize / (float)Atlas.Height;
            float PositionDeltaPerColumn = TextureSize / (float)Atlas.Width;

            int maxTexturesInRow = Atlas.Width / TextureSize;
            int maxTexturesInColumn = Atlas.Height / TextureSize;

            int retIndex = face == FaceDirection.Top || TexturePosition.Length == 1 ? 0 : face == FaceDirection.Bot ? 1 : 2;

            return new float[] {
                TexturePosition[retIndex].column * PositionDeltaPerColumn,       (maxTexturesInColumn - TexturePosition[retIndex].row - 1) * PositionDeltaPerRow,
               (TexturePosition[retIndex].column + 1) * PositionDeltaPerColumn, (maxTexturesInRow - TexturePosition[retIndex].row) * PositionDeltaPerRow
            };
        }
    }
}