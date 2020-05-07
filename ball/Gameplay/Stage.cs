using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UmbrellaToolKit;
using UmbrellaToolKit.UI;

namespace ball.Gameplay
{
    public class Stage : Scene
    {
        public MouseManager Mouse;
        public ScreemController Screem;
        public bool WhiteUI { get; set; }
        public virtual void UpdateLevel(GameTime gameTime) { }
        public virtual void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) { }
    }
}
