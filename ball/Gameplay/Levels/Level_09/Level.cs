using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using UmbrellaToolKit;
using UmbrellaToolKit.UI;

namespace ball.Gameplay.Levels.Level_09
{
    public class Level : Stage
    {
        public override void Start(ContentManager Content, World World, MouseManager Mouse)
        {
            this.Content = Content;
            this.World = World;
            this.Mouse = Mouse;

            this.ResetLevel();
        }

        public override void ResetLevel()
        {

        }

        public override void Destroy()
        {

        }


        public override void UpdateLevel(GameTime gameTime)
        {
            this.Update(gameTime);
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }
    }
}
