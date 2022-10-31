using Minecraft.Graphics;
using System.Windows;

namespace Minecraft.Terrain
{
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
        public static string TexturePath
        {   
            get => texturePath; 

            set 
            { 
                texturePath = value; 
                atlas = new Texture(texturePath, false); 
                TextureSize = atlas.Width / 16;
            } 
        }
        private static string texturePath = @"..\..\..\Assets\Textures\terrain.png";
        //Top = 1st Bot = 2nd Other = 3rd
        public static readonly Position2D[][] TexturePositions =
        {
            //Air
            new Position2D[0],
            //GrassBlock
            new Position2D[3]{ new Position2D(0,13), new Position2D(0, 9), new Position2D(0, 11), },
            //Podzol
            new Position2D[3]{ new Position2D(8,1), new Position2D(0, 9), new Position2D(8, 0), },
            //SnowGrassBlock
            new Position2D[3]{ new Position2D(4,14), new Position2D(0, 9), new Position2D(4, 13), },
            //MushroomStem
            new Position2D[1]{ new Position2D(3,12) },
            //RedMushroomBlock
            new Position2D[1]{ new Position2D(3,13) }, 
            //MushroomBlock
            new Position2D[1]{ new Position2D(3,14) }, 
            //BrownMushroomBlock
            new Position2D[1]{ new Position2D(3,15) }, 
            //Stone
            new Position2D[1]{ new Position2D(1,7) },
            //Dirt
            new Position2D[1]{ new Position2D(0,9) }, 
            //Leaves...:
            new Position2D[1]{ new Position2D(1,4) },
            new Position2D[1]{ new Position2D(0,1) },
            new Position2D[1]{ new Position2D(7,9) },
            new Position2D[1]{ new Position2D(7,8) }, 
            //Trunks...:
            new Position2D[3]{ new Position2D(1,3), new Position2D(1, 3), new Position2D(1, 1), },
            new Position2D[3]{ new Position2D(0,4), new Position2D(0, 4), new Position2D(0, 3), },
            new Position2D[3]{ new Position2D(4,0), new Position2D(4, 0), new Position2D(4, 1), },
            new Position2D[3]{ new Position2D(7,6), new Position2D(7, 6), new Position2D(7, 5), },
            //Planks
            new Position2D[1]{ new Position2D(1,6) },
            new Position2D[1]{ new Position2D(0,5) },
            new Position2D[1]{ new Position2D(4,2) },
            new Position2D[1]{ new Position2D(4,15) }, 
            //Glasses...:
            new Position2D[1]{ new Position2D(2,0) },
            new Position2D[1]{ new Position2D(2,1) },
            new Position2D[1]{ new Position2D(2,2) },
            new Position2D[1]{ new Position2D(2,3) },
            new Position2D[1]{ new Position2D(2,4) },
            new Position2D[1]{ new Position2D(2,5) },
            new Position2D[1]{ new Position2D(2,6) },
            new Position2D[1]{ new Position2D(2,7) },
            new Position2D[1]{ new Position2D(2,8) },
            //ConcretePowders...:
            new Position2D[1]{ new Position2D(7,0) },
            new Position2D[1]{ new Position2D(7,1) },
            new Position2D[1]{ new Position2D(7,2) },
            new Position2D[1]{ new Position2D(7,3) },
            new Position2D[1]{ new Position2D(7,4) },
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
            new Position2D[1]{ new Position2D(3,10) },
            new Position2D[1]{ new Position2D(3,11) },
            //Sand
            new Position2D[1]{ new Position2D(0,12) },
            //SandStone
            new Position2D[3]{ new Position2D(8,4), new Position2D(8, 3), new Position2D(8, 2), },
            //Clay
            new Position2D[1]{ new Position2D(5,5) },
            //Water
            new Position2D[1]{ new Position2D(1,10) },
            //Ice
            new Position2D[1]{ new Position2D(1,12) },
            //Bedrock
            new Position2D[1]{ new Position2D(0,2) },
            //Cobblestone
            new Position2D[1]{ new Position2D(0,8) },
            //Grass
            new Position2D[1]{ new Position2D(0,10) },
            //SparseGrass
            new Position2D[1]{ new Position2D(1,11) },
            //Lava
            new Position2D[1]{ new Position2D(1,0) },
            //Allium
            new Position2D[1]{ new Position2D(1,13) },
            //Poppy
            new Position2D[1]{ new Position2D(1,14) },
            //DeadBush
            new Position2D[1]{ new Position2D(1,15) },
            //Saplings...:
            new Position2D[1]{ new Position2D(0,7) },
            new Position2D[1]{ new Position2D(1,5) },
            new Position2D[1]{ new Position2D(4,3) },
            new Position2D[1]{ new Position2D(7,7) },
            //RedstoneTorch
            new Position2D[1]{ new Position2D(2,11) },
            //RedMushroom
            new Position2D[1]{ new Position2D(2,12) },
            //BrownMushroom
            new Position2D[1]{ new Position2D(2,13) },
            //BlueOrchid
            new Position2D[1]{ new Position2D(2,14) },
            //AzureBluet
            new Position2D[1]{ new Position2D(2,15) },
            //Torch
            new Position2D[1]{ new Position2D(6,15) },
            //CobWeb
            new Position2D[1]{ new Position2D(5,7) },
            //Ores...:
            new Position2D[1]{ new Position2D(6,0) },
            new Position2D[1]{ new Position2D(6,1) },
            new Position2D[1]{ new Position2D(6,2) },
            new Position2D[1]{ new Position2D(6,3) },
            new Position2D[1]{ new Position2D(6,4) },
            new Position2D[1]{ new Position2D(6,5) },
            new Position2D[1]{ new Position2D(6,11) },
            //OreBlocks
            new Position2D[1]{ new Position2D(6,6) },
            new Position2D[1]{ new Position2D(5,10) },
            new Position2D[1]{ new Position2D(5,9) },
            new Position2D[1]{ new Position2D(6,7) },
            new Position2D[1]{ new Position2D(6,8) },
            new Position2D[1]{ new Position2D(7,10) },
            new Position2D[1]{ new Position2D(6,9) },
            new Position2D[1]{ new Position2D(6,10) },
            new Position2D[1]{ new Position2D(6,12) },
            //AmethystCluster
            new Position2D[1]{ new Position2D(4,4) },
            //AmethystBlock
            new Position2D[1]{ new Position2D(4,5) },
            //Fire
            new Position2D[1]{ new Position2D(5,1) },
            //NoteBlock
            new Position2D[1]{ new Position2D(2,9) },
            //RedstoneLamp
            new Position2D[1]{ new Position2D(2,10) },
            //TNT
            new Position2D[3]{ new Position2D(8,10), new Position2D(8, 9), new Position2D(8, 8), },
            //Scaffolding
            new Position2D[3]{ new Position2D(8,5), new Position2D(8, 6), new Position2D(8, 7), },
            //Cactus
            new Position2D[3]{ new Position2D(4,8), new Position2D(4, 6), new Position2D(4, 7), },
            //Furnace
            new Position2D[3]{ new Position2D(4,11), new Position2D(4, 11), new Position2D(4, 9), },
            //GlowStone
            new Position2D[1]{ new Position2D(4,12) },
            //QuartzBlock
            new Position2D[3]{ new Position2D(6,13), new Position2D(6, 13), new Position2D(6, 14), },
            //PurpurBlock
            new Position2D[1]{ new Position2D(5,13) },
            //PurpurPillarBlock
            new Position2D[3]{ new Position2D(5,15), new Position2D(5, 15), new Position2D(5, 14), },
            //BookShelf
            new Position2D[3]{ new Position2D(1,6), new Position2D(1, 6), new Position2D(5, 0), },
            //Bricks...:
            new Position2D[1]{ new Position2D(7,11) },
            new Position2D[1]{ new Position2D(5,11) },
            new Position2D[1]{ new Position2D(1,9) },
            new Position2D[1]{ new Position2D(5,8) },
        };
        public static int TextureSize = 16;
        public static Texture Atlas
        {
            get
            {
                if (atlas == null)
                {
                    atlas = new Texture(texturePath, false);
                }
                return atlas;
            }
        }

        private static Texture? atlas;

        public static Int32Rect GetTextureRect(BlockType type)
        {
            if(TexturePositions[(int)type].Length > 1)
                return new Int32Rect(TexturePositions[(int)type][2].column * TextureSize, TexturePositions[(int)type][2].row * TextureSize, TextureSize, TextureSize);

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
               (TexturePosition[retIndex].column + 1) * PositionDeltaPerColumn, (maxTexturesInColumn - TexturePosition[retIndex].row) * PositionDeltaPerRow
            };
        }
    }
}