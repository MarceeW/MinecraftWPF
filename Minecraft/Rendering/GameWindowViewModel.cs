using Microsoft.Toolkit.Mvvm.ComponentModel;
using Minecraft.Terrain;
using Minecraft.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Rendering
{
    internal class GameWindowViewModel : ObservableRecipient
    {
        IUILogic logic;

        public GameWindowViewModel(IUILogic logic)
        {
            this.logic = logic;
            logic.ReadWorlds();
        }
    }
}
