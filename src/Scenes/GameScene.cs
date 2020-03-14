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
        public const int scoreAdditionByApple = 10;
        public const int scoreAdditionByTick = 1;
        public const int scoreAdditionByLevelUp = 50;
        public const int scoreAdditionByGoldApple = 100;

        public int Length => snake.Count;

        public Vector Forward => new Vector(
            MathF.Cos(DFMath.ToRadian(currentDegree)),
            MathF.Sin(DFMath.ToRadian(currentDegree))
        );

        public override void OnStart(Router router, GameBase game, Dictionary<string, object> args)
        {
            dotfeather = game;
            BackgroundColor = Color.FromArgb(27, 94, 32);

            audio.Play(Resources.I.BgmMain, 605106);

            snake.Add(GenPart(true));

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
            Root.Add(hud);
        }

        public override void OnUpdate(Router router, GameBase game, DFEventArgs e)
        {
            UpdateTrail();
            RenderSnake();

            hud.Text = $"{Time.Fps}FPS SCORE:{score} Life:{life} Level{level}";

            if (introDone)
            {
                HandleKey();
                if (!prevIntroDone)
                {
                    PopulateApple();
                }
                tickCount++;
                if (tickCount > 2)
                {
                    IncrementScore(scoreAdditionByTick, false);
                    tickCount = 0;
                }
                prevIntroDone = introDone;
            }

            if (IntersectWithApple())
            {
                if (currentAppleType == AppleType.Normal)
                {
                    snake.Add(GenPart());
                    if (Length > 10)
                    {
                        level++;
                        snake.RemoveRange(1, Length - 1);
                        IncrementScore(scoreAdditionByLevelUp, false);
                        DisplayTextParticle("LEVEL UP!", snake.First().Location * 2);
                    }
                    else
                    {
                        IncrementScore(scoreAdditionByApple);
                    }
                }
                else
                {
                    life++;
                    IncrementScore(scoreAdditionByGoldApple);
                }
                if (currentApple != null)
                {
                    stageContainer.Remove(currentApple);
                }
                PopulateApple();
            }
        }

        private void IncrementScore(int addition, bool display = true)
        {
            score += addition;
            if (display) DisplayTextParticle("+" + addition, snake.First().Location * 2);
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

        private void DisplayTextParticle(string text, Vector position)
        {
            CoroutineRunner.Start(DisplayTextParticleCoroutine(text, position));
        }


        private IEnumerator DisplayTextParticleCoroutine(string text, Vector position)
        {
            var drawable = new TextDrawable(text) { Location = position };
            Root.Add(drawable);

            var time = 0f;
            while (time < 2)
            {
                drawable.Location += Vector.Up * 16 * Time.DeltaTime;
                drawable.Color = Color.FromArgb((int)DFMath.Lerp(time / 2, 255, 0), 255, 255, 255);
                time += Time.DeltaTime;
                yield return null;
            }
            Root.Remove(drawable);
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
                s && d ? 45 :
                w ? 270 :
                a ? 180 :
                s ? 90 :
                d ? 0 : -1;

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

        private void PopulateApple()
        {
            // 2% の確率でゴールデンリンゴ
            const int goldenRate = 2;
            currentAppleType = random.Next(100) < goldenRate ? AppleType.Golden : AppleType.Normal;
            var apple = currentAppleType == AppleType.Golden ? Resources.I.TextureFruitGold : Resources.I.TextureFruit;

            currentApple = new Sprite(apple);

            // ランダムな位置にりんごを置く
            currentApple.Location = Random.NextVector(
                (int)(dotfeather.Width - currentApple.Width),
                (int)(dotfeather.Height - currentApple.Height)
            ) / 2;

            stageContainer.Add(currentApple);
        }

        private bool IntersectWithApple()
            => currentApple == null ? false : snake.Select(s => s.Sprite).Any(body => Intersect(body, currentApple));

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

        private bool Intersect(Sprite s1, Sprite s2) =>
            Math.Abs(s2.Location.X - s1.Location.X) < (s1.Width + s2.Width) / 2 &&
            Math.Abs(s2.Location.Y - s1.Location.Y) < (s1.Height + s2.Height) / 2;

        private AudioPlayer audio = new AudioPlayer();

        private List<SnakePart> snake = new List<SnakePart>();
        private List<Vector> trail = new List<Vector>();
        private Container snakeContainer = new Container();
        private Container stageContainer = new Container();
        private TextDrawable hud = new TextDrawable("", Resources.I.GetFont(24), Color.White);

        private Vector cursor;
        private Vector prevCursor;
        private float currentDegree = 270;
        private bool introDone;
        private bool prevIntroDone;
        private Sprite? currentApple;
        private AppleType currentAppleType;

        private int score;
        private int life = 3;
        private int level = 1;

        // スコア付与するための変数
        private int tickCount = 0;

        #pragma warning disable CS8618 // OnStart で必ず初期化するので例外
        private GameBase dotfeather;
        private Random random = new Random();
    }

    public enum AppleType
    {
        Normal, Golden,
    }
}
