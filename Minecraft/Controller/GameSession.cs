using Minecraft.Game;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Controller
{
    class GameSession
    {
        public IPlayer Player { get; private set; }
        public IWorld World { get; private set; }

        public GameSession()
        {
            Player = new Player(new Vector3(0, 40, 0));
            World = new World();
        }
        public GameSession(string folderPath)
        {

        }
        public void Save()
        {

        }
    }
}
