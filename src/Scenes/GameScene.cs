using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DotFeather;

namespace HarapekoSnake
{
    public class GameScene : Scene
    {
        public override void OnStart(Router router, GameBase game, Dictionary<string, object> args)
        {
            BackgroundColor = Color.FromArgb(27, 94, 32);
            
            audio.Play(Resources.I.BgmMain, 605106);

            snake.Add(GenPart(true));
            snake.Add(GenPart());
            snake.Add(GenPart());
            snake.Add(GenPart());
            snake.Add(GenPart());

            trail.Insert(0, prevMouse);

            Root.Add(snakeContainer);
        }

        public override void OnUpdate(Router router, GameBase game, DFEventArgs e)
        {
            if (game.IsFocused)
            {
                UpdateTrail();
                RenderSnake();
            }
        }

        private SnakePart GenPart(bool isHead = false)
        {
            return new SnakePart
            {
                Sprite = new Sprite(isHead ? Resources.I.TextureSnakeHead : Resources.I.TextureSnakeBody),
            };
        }

        private void UpdateTrail()
        {
            var mouse = DFMouse.Position / 2;
            // if (mouse != prevMouse)
            // {
                trail.Insert(0, mouse);
            // }
            if (trail.Count > 256)
            {
                trail.RemoveAt(trail.Count - 1);
            }
            prevMouse = mouse / 2;
        }

        private void RenderSnake()
        {
            // Update snake's information
            foreach (var (part, i) in snake.Select((p, i) => (p, i)))
            {
                if (!snakeContainer.Contains(part.Sprite))
                    snakeContainer.Add(part.Sprite);
                part.Location = trail.Count - 1 < i * 8 ? trail[0] : trail[i * 8];
                part.Sprite.Location = part.Location;
            }

            // GC
            var removed = snakeContainer.Where(s => !snake.Select(p => p.Sprite).Contains(s)).ToList();
            foreach (var d in removed)
            {
                snakeContainer.Remove(d);
            }
        }

        private AudioPlayer audio = new AudioPlayer();

        private List<SnakePart> snake = new List<SnakePart>();
        private List<Vector> trail = new List<Vector>();
        private Container snakeContainer = new Container();
        private VectorInt prevMouse = DFMouse.Position / 2;

    }
}
