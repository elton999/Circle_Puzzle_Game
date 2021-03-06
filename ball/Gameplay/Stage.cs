﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using tainicom.Aether.Physics2D.Dynamics;
using UmbrellaToolKit;
using UmbrellaToolKit.UI;

namespace ball.Gameplay
{
    public class Stage : Scene
    {
        public MouseManager Mouse;
        public ScreemController Screem;
        public SpriteFont FontBold;
        public World World;
        public UmbrellaToolKit.Storage.Load Storage;
        public bool WhiteUI { get; set; }
        public bool UI { get; set; }
        public bool Finished { get; set; }
        public virtual void Start(ContentManager Content, World World, MouseManager Mouse) { }
        public virtual void UpdateLevel(GameTime gameTime) { }
        public virtual void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) { }
        public virtual void ResetLevel() { }
        public virtual void Destroy() { }
    }
}
