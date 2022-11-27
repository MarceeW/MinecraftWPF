using Minecraft.Terrain;
using System;

namespace Minecraft.UI
{
    public class Hotbar : IHotbar
    {
        public int SelectedItemIndex { get; private set; }
        public Action? BlockChangeOnSelect { get; set; }
        public BlockType[] Items { get; private set; }
        public int MaxItems { get; private set; } = 9;
        public Hotbar()
        {
            Items = new BlockType[MaxItems];
        }
        public void Reset()
        {
            Items = new BlockType[MaxItems];
        }
        public void ChangeBlock(int index, BlockType toChange)
        {
            Items[index] = toChange;

            if (index == SelectedItemIndex)
                BlockChangeOnSelect?.Invoke();
        }
        public BlockType GetSelectedBlock()
        {
            return Items[SelectedItemIndex];
        }
        public void SetSelectedIndex(int index)
        {
            SelectedItemIndex = index;
            BlockChangeOnSelect?.Invoke();
        }
        public void UpdateSelectedIndex(int delta)
        {
            SelectedItemIndex += delta < 0 ? 1 : -1;

            if (SelectedItemIndex < 0)
                SelectedItemIndex = MaxItems - 1;

            else if (SelectedItemIndex == MaxItems)
                SelectedItemIndex = 0;

            BlockChangeOnSelect?.Invoke();
        }
        public void Deserialize(string rawData)
        {
            var splitted = rawData.Split(';');

            for (int i = 0; i < splitted.Length; i++)
                Items[i] = (BlockType)int.Parse(splitted[i]);
        }
        public override string ToString()
        {
            string ret = "";
            for (int i = 0; i < Items.Length; i++)
            {
                ret += (int)Items[i];

                if (i != Items.Length - 1)
                    ret += ";";
            }
            return ret;
        }
    }
}
