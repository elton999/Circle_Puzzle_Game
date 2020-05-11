using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using UmbrellaToolKit;
using UmbrellaToolKit.UI;
using tainicom.Aether.Physics2D.Dynamics;


namespace ball.Gameplay.Level_03
{
    public class Level : Stage
    {
        List<WhiteCircle> Circle = new List<WhiteCircle>();
        MemoryGame memoryGame;

        public override void Start(ContentManager Content, World World, MouseManager mouse)
        {
            this.World = World;
            memoryGame = new MemoryGame();
            memoryGame.WhiteCirclesList = new List<MemoryGameWhiteCircle>();
            for (int i = 0; i < 2; i++)
            {
                this.Circle.Add(new WhiteCircle());
                this.Circle[i].Id = i;
                this.Circle[i]._Mouse = mouse;
                this.Circle[i].Sprite = Content.Load<Texture2D>("Sprites/white_circle");
                this.Circle[i].BlackCircle = new GameObject();
                this.Circle[i].BlackCircle.Sprite = Content.Load<Texture2D>("Sprites/black_circle");
                this.Circle[i].Start(World);
                this.Players.Add(this.Circle[i]);
                memoryGame.WhiteCirclesList.Add(this.Circle[i]);
            }
            this.Players.Add(memoryGame);
            this.SetBackgroundColor = Color.White;
            this.LevelReady = true;
        }

        public override void Destroy()
        {
            for (int i = 0; i < this.Players.Count(); i++)
            {
                this.World.Remove(this.Players[i].CBody);
            }
            this.Circle.Clear();
            this.Players.Clear();
            this.LevelReady = false;
        }

        public override void ResetLevel(ContentManager Content, World World, MouseManager mouse)
        {
            this.Finished = false;
            this.LevelReady = true;
        }

        public override void UpdateLevel(GameTime gameTime)
        {
            if (this.LevelReady)
            {
                for (int i = 0; i < 2; i++) this.Circle[i]._Screem = this.Screem;
                this.Update(gameTime);
                this.Finished = memoryGame.Finished;
            }
            
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }

    }

    public class WhiteCircle : MemoryGameWhiteCircle
    {
        
    }
}
