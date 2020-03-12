using System.Collections.Generic;
using System.Drawing;
using DotFeather;

namespace HarapekoSnake
{
    public class GameScene : Scene
    {
        public override void OnStart(Router router, GameBase game, Dictionary<string, object> args)
        {
            BackgroundColor = Color.FromArgb(27, 94, 32);
            
            audio.Play(Resources.I.BgmMain, 605106);
        }

        private AudioPlayer audio = new AudioPlayer();
    }
}
