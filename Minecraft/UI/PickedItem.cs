using Minecraft.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Minecraft.UI
{
    internal class PickedItem
    {
        public ImageSource src;
        public BlockType type;

        public PickedItem(ImageSource src, BlockType type)
        {
            this.src = src;
            this.type = type;
        }
    }
}
