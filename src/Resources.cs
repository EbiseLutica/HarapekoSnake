using DotFeather;

namespace HarapekoSnake
{
    public sealed class Resources
    {
        public static Resources I { get; } = new Resources();

        public Texture2D TextureSnakeHead { get; }
        public Texture2D TextureSnakeBody { get; }
        public Texture2D TextureFruit { get; }
        public Texture2D TextureFruitGold { get; }
        public Texture2D TextureBug1 { get; }
        public Texture2D TextureBug2 { get; }

        public IAudioSource BgmMain { get;}
        public IAudioSource BgmTitle { get;}

        public DotFeather.Font GetFont(float size = 16, FontStyle style = FontStyle.Normal)
        {
            return new DotFeather.Font("./resources/font.ttf", size, style);
        }

        private Resources()
        {
            var sprites = Texture2D.LoadAndSplitFrom("./resources/sprites.png", 6, 1, VectorInt.One * 8);
            TextureSnakeHead = sprites[0];
            TextureSnakeBody  = sprites[1];
            TextureFruit = sprites[2];
            TextureFruitGold  = sprites[3];
            TextureBug1 = sprites[4];
            TextureBug2 = sprites[5];

            BgmMain = new VorbisAudioSource("./resources/bgm_main.ogg");
            BgmTitle = new VorbisAudioSource("./resources/bgm_title.ogg");
        }
    }
}