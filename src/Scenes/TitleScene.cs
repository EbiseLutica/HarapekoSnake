using System.Collections.Generic;
using System.Drawing;
using DotFeather;
using DFFont = DotFeather.Font;

namespace HarapekoSnake
{
    public class TitleScene : Scene
    {
        public override void OnStart(Router router, GameBase game, Dictionary<string, object> args)
        {
            BackgroundColor = Color.FromArgb(27, 94, 32);

            var title = new TextDrawable("はらぺこスネーク", Resources.I.GetFont(32), Color.Orange);
            title.Location = new Vector(game.Width / 4 - title.Width / 2, 24);

            var label = new TextDrawable("がめんを クリックして スタート", Resources.I.GetFont(12), Color.White);
            label.Location = new Vector(game.Width / 4 - label.Width / 2, 120);

            var copyright = new TextDrawable("(C)2020 Xeltica", Resources.I.GetFont(12), Color.White);
            copyright.Location = new Vector(4, 224);

            Root.Add(title);
            Root.Add(label);
            Root.Add(copyright);
            
            audio.Play(Resources.I.BgmTitle, 0);
        }

        public override void OnUpdate(Router router, GameBase game, DFEventArgs e)
        {
            if (DFMouse.IsLeft && game.IsFocused)
            {
                router.ChangeScene<GameScene>();
            }
        }

        public override void OnDestroy(Router router)
        {
            audio.Stop();
            audio.Dispose();
        }

        private AudioPlayer audio = new AudioPlayer();
    }
}
