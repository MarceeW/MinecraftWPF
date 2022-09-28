using Minecraft.Graphics;

namespace Minecraft.Terrain
{
    internal struct AtlasTexturePosition
    {
        public int column, row;

        public AtlasTexturePosition(int row, int column)
        {
            this.column = column;
            this.row = row;
        }
    }
    internal static class AtlasTexturesData
    {
        //Top = 1st Bot = 2nd Other = 3rd
        public static readonly AtlasTexturePosition[][] TexturePositions =
        {
            new AtlasTexturePosition[0],
            //Grass
            new AtlasTexturePosition[3]{ new AtlasTexturePosition(0,13), new AtlasTexturePosition(0, 9), new AtlasTexturePosition(0, 11), },
            //Stone
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(1,7) },
            //Dirt
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(0,9) },
            //Leaves
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(1,4) },
            //OakTrunk
            new AtlasTexturePosition[3]{ new AtlasTexturePosition(1,3), new AtlasTexturePosition(1, 5), new AtlasTexturePosition(1, 4), },
            //Glass
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(0,-0) },
            //Sand
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(0,12) },
            //Water
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(1,10) },
            //Bedrock
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(0,2) },
            //Cobblestone
            new AtlasTexturePosition[1]{ new AtlasTexturePosition(0,8) },

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
        public static float[] GetTextureCoords(BlockType type, FaceDirection face)
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