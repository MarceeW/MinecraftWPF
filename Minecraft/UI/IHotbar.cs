using Minecraft.Terrain;
using System;

namespace Minecraft.UI
{
    public interface IHotbar
    {
        Action? BlockChangeOnSelect { get; set; }
        BlockType[] Items { get; }
        int MaxItems { get; }
        int SelectedItemIndex { get; }
        void Reset();
        void ChangeBlock(int index, BlockType toChange);
        void Deserialize(string rawData);
        BlockType GetSelectedBlock();
        string ToString();
        void UpdateSelectedIndex(int delta);
    }
}