using Minecraft.Graphics;
using System.Windows;

namespace Minecraft.Terrain
{
    enum BlockType
    {
        Air, GrassBlock, Stone, Dirt, OakLeaves, BirchLeaves, OakTrunk, Glass, Sand, Water, Bedrock, Cobblestone, WoodPlank, Grass, SparseGrass, BirchTrunk, BirchWoodPlank, Lava, Allium, Poppy, DeadBush,
        BlueStainedGlass, BrownStainedGlass, CyanStainedGlass, GreenStainedGlass, MagentaStainedGlass, PurpleStainedGlass, RedStainedGlass,
        BlackConcrete, BlueConcrete, BrownConcrete, CyanConcrete, GreyConcrete, GreenConcrete, LimeConcrete, RedConcrete, MagentaConcrete, YellowConcrete
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
            //OakLeaves
            new Position2D[1]{ new Position2D(1,4) },
            //BirchLeaves
            new Position2D[1]{ new Position2D(0,1) },
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
            //SparseGrass
            new Position2D[1]{ new Position2D(1,11) },
            //BirchTrunk
            new Position2D[3]{ new Position2D(0,4), new Position2D(0, 4), new Position2D(0, 3), },
            //BirchWoodPlank
            new Position2D[1]{ new Position2D(0,5) },
            //Lava
            new Position2D[1]{ new Position2D(1,0) },
            //Allium
            new Position2D[1]{ new Position2D(1,13) },
            //Poppy
            new Position2D[1]{ new Position2D(1,14) },
            //DeadBush
            new Position2D[1]{ new Position2D(1,15) },
            //StainedGlasses...:
            new Position2D[1]{ new Position2D(2,1) },
            new Position2D[1]{ new Position2D(2,2) },
            new Position2D[1]{ new Position2D(2,3) },
            new Position2D[1]{ new Position2D(2,4) },
            new Position2D[1]{ new Position2D(2,5) },
            new Position2D[1]{ new Position2D(2,6) },
            new Position2D[1]{ new Position2D(2,7) },
            //Concretes...:
            new Position2D[1]{ new Position2D(3,0) },
            new Position2D[1]{ new Position2D(3,1) },
            new Position2D[1]{ new Position2D(3,2) },
            new Position2D[1]{ new Position2D(3,3) },
            new Position2D[1]{ new Position2D(3,4) },
            new Position2D[1]{ new Position2D(3,5) },
            new Position2D[1]{ new Position2D(3,6) },
            new Position2D[1]{ new Position2D(3,7) },
            new Position2D[1]{ new Position2D(3,8) },
            new Position2D[1]{ new Position2D(3,9) },
        };
        public const int TextureSize = 16;
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
        public static Int32Rect GetTextureRect(BlockType type)
        {
            return new Int32Rect(TexturePositions[(int)type][0].column * TextureSize, TexturePositions[(int)type][0].row * TextureSize, TextureSize, TextureSize);
        }
        public static float[] GetTextureCoords(BlockType type, FaceDirection? face)
        {
            var TexturePosition = TexturePositions[(int)type];

            float PositionDeltaPerRow = TextureSize / (float)Atlas.Height;
            float PositionDeltaPerColumn = TextureSize / (float)Atlas.Width;

            int maxTexturesInRow = Atlas.Width / TextureSize;
            int maxTexturesInColumn = Atlas.Height / TextureSize;

            int retIndex = face == FaceDirection.Top || TexturePosition.Length == 1 ? 0 : face == FaceDirection.Bot ? 1 : 2;

            return new float[] {
                TexturePosition[retIndex].column * PositionDeltaPerColumn,       (maxTexturesInRow - TexturePosition[retIndex].row - 1) * PositionDeltaPerRow,
               (TexturePosition[retIndex].column + 1) * PositionDeltaPerColumn, (maxTexturesInRow - TexturePosition[retIndex].row) * PositionDeltaPerRow
            };
        }
    }
}