using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DotFeather;

namespace HarapekoSnake
{
    public class GameScene : Scene
    {
        public Vector Forward => new Vector(
            MathF.Cos(DFMath.ToRadian(currentDegree)),
            MathF.Sin(DFMath.ToRadian(currentDegree))
        );

        public override void OnStart(Router router, GameBase game, Dictionary<string, object> args)
        {
            BackgroundColor = Color.FromArgb(27, 94, 32);
            
            audio.Play(Resources.I.BgmMain, 605106);

            snake.Add(GenPart(true));
            snake.Add(GenPart());
            snake.Add(GenPart());
            snake.Add(GenPart());
            snake.Add(GenPart());

            trail.Insert(0, prevCursor);

            cursor = new VectorInt(game.Width / 4 - 8, game.Height / 2);
            prevCursor = cursor;
            trail.AddRange(Enumerable.Repeat(cursor, 256));

            game.StartCoroutine(Intro(game));

            UpdateTrail();
            RenderSnake();

            stageContainer.Scale *= 2;
            stageContainer.Add(snakeContainer);

            Root.Add(stageContainer);
        }

        public override void OnUpdate(Router router, GameBase game, DFEventArgs e)
        {
            UpdateTrail();
            RenderSnake();

            game.Print(currentDegree);

            if (introDone)
            {
                HandleKey();
            }
        }

        private IEnumerator Intro(GameBase game)
        {
            var duration = 1.5f;
            var tick = 0f;
            var initial = cursor;
            var target = new VectorInt(game.Width, game.Height) / 4;
            while (tick < duration)
            {
                cursor = DFMath.EaseOut(tick / duration, initial, target);
                yield return null;
                tick += Time.DeltaTime;
            }
            cursor = target;
            introDone = true;
        }

        private void HandleKey()
        {
            var w = DFKeyboard.W;
            var a = DFKeyboard.A;
            var s = DFKeyboard.S;
            var d = DFKeyboard.D;

            var target = 
                w && a ? 225 :
                w && d ? 315 :
                s && a ? 135 :
                s && d ?  45 :
                w ? 270 :
                a ? 180 :
                s ?  90 :
                d ?   0 : -1;

            if (w || a || s || d)
            {
                currentDegree = DFMath.Lerp(0.3f, currentDegree, target);
                cursor += Forward * 128 * Time.DeltaTime;
            }
        }

        private void UpdateTrail()
        {
            if (cursor != prevCursor)
            {
                trail.Insert(0, cursor);
            }
            while (trail.Count > 256)
            {
                trail.RemoveAt(trail.Count - 1);
            }
            prevCursor = cursor;
        }

        private void RenderSnake()
        {
            // Update snake's information
            Vector offset = trail[0];
            snake.First().Location = offset;
            foreach (var (body, i) in snake.Select((p, i) => (p, i)))
            {
                if (!snakeContainer.Contains(body.Sprite))
                    snakeContainer.Add(body.Sprite);
                
                if (i > 0)
                {
                    var target = trail[(i + 1) * 16];
                    var angle = Vector.Angle(offset, target);
                    offset = body.Location = offset + new Vector(MathF.Cos(angle), MathF.Sin(angle)) * 16;
                }

                body.Sprite.ZOrder = -i;
                body.Sprite.Location = body.Location;
            }

            // GC
            var removed = snakeContainer.Where(s => !snake.Select(p => p.Sprite).Contains(s)).ToList();
            foreach (var d in removed)
            {
                snakeContainer.Remove(d);
            }
        }

        private SnakePart GenPart(bool isHead = false)
        {
            return new SnakePart
            {
                Sprite = new Sprite(isHead ? Resources.I.TextureSnakeHead : Resources.I.TextureSnakeBody),
            };
        }

        private AudioPlayer audio = new AudioPlayer();

        private List<SnakePart> snake = new List<SnakePart>();
        private List<Vector> trail = new List<Vector>();
        private Container snakeContainer = new Container();
        private Container stageContainer = new Container();
        private Vector prevCursor;
        private Vector cursor;
        private bool introDone;
        private float currentDegree = 270;

    }
}
