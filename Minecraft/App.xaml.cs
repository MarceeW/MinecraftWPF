using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Controller;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Logic;
using Minecraft.Render;
using Minecraft.Terrain;
using Minecraft.UI;
using OpenTK.Mathematics;
using System.Windows;

namespace Minecraft
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
                .BuildServiceProvider()
                );
        }
    }
}
