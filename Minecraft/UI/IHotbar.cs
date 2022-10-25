using Minecraft.Terrain;
using System;

namespace Minecraft.UI
{
    public interface IHotbar
    {
        Action? BlockChangeOnSelect { get; set; }
        int SelectedItemIndex { get; }

        void ChangeBlock(int index, BlockType toChange);
        BlockType GetSelectedBlock();
        BlockType[] Items { get; set; }
        int MaxItems { get; }
        void UpdateSelectedIndex(int delta);
    }
}