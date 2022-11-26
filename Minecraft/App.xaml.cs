using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Graphics;
using Minecraft.Logic;
using Minecraft.UI;
using Minecraft.UI.Logic;
using System.Windows;

namespace Minecraft
{
    public partial class App : Application
    {
        public App()
        {
            Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<ICamera, Camera>()
                .AddSingleton<IPlayerLogic, PlayerLogic>()
                .AddSingleton<IHotbar, Hotbar>()
                .AddSingleton<IForce, Force>()
                .AddSingleton<IUILogic, UILogic>()
                .AddSingleton<IInventoryLogic, InventoryLogic>()
                .BuildServiceProvider()
                );
        }
    }
}
