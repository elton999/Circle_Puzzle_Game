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

namespace ball.Gameplay.Levels.Level_02
{
    public class Level : Stage
    {
        private int _width;
        private int _height;

        List<CirclePart> CirclePart = new List<CirclePart>();

        public override void Start (ContentManager Content, World World, MouseManager mouse)
        {
            this.World = World;
            this.Mouse = mouse;
            for (int i = 0; i < 4; i++)
            {
                CirclePart.Add(new CirclePart());
                CirclePart[i].Sprite = Content.Load<Texture2D>("Sprites/white_circle");
                CirclePart[i]._Mouse = mouse;
                CirclePart[i].CreatePartOfCircle(i, World);
                this.Players.Add(CirclePart[i]);

                this._width = CirclePart[i].width;
                this._height = CirclePart[i].height;
            }
            this.SetBackgroundColor = Color.White;
            this.ResetLevel(Content, World, mouse);
            this.LevelReady = true;
            this.WhiteUI = false;
            Console.WriteLine(LevelReady);
        }

        public override void Destroy()
        {
            for (int i = 0; i < this.Players.Count(); i++)
            {
                this.World.Remove(this.Players[i].CBody);
            }
            this.CirclePart = new List<CirclePart>();
            this.Players = new List<GameObject>();
            this.Finished = false;
            this.LevelReady = false;
        }

        public override void ResetLevel(ContentManager Content, World World, MouseManager mouse)
        {
            this.Finished = false;
        }


        public override void UpdateLevel(GameTime gameTime)
        {
            Vector2 _center = this.Screem.getCenterScreem;

            this.Players[0].CBody.SetTransform(new Vector2(_center.X - (_width / 2f), _center.Y - (_height / 2f)), this.Players[0].Rotation);
            this.Players[1].CBody.SetTransform(new Vector2(_center.X + (_width / 2f), _center.Y - (_height / 2f)), this.Players[1].Rotation);
            this.Players[2].CBody.SetTransform(new Vector2(_center.X - (_width / 2f), _center.Y + (_height / 2f)), this.Players[2].Rotation);
            this.Players[3].CBody.SetTransform(new Vector2(_center.X + (_width / 2f), _center.Y + (_height / 2f)), this.Players[3].Rotation);

            if (this.Players[0].Rotation == 0 && this.Players[1].Rotation == 0 && this.Players[2].Rotation == 0  && this.Players[3].Rotation == 0)
            {
                this.Finished = true;
            }

            this.Update(gameTime);
        }


        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
           this.Draw(spriteBatch, graphicsDevice);
        }

    }


    public class CirclePart : GameObject
    {

        private List<Point> PositionsSprite = new List<Point>();
        public int width;
        public int height;
        public void CreatePartOfCircle(int position, World world)
        {
            width = this.Sprite.Width / 2;
            height = this.Sprite.Height / 2;
        
            this.PositionsSprite.Add(new Point(0, 0));
            this.PositionsSprite.Add(new Point(width, 0));
            this.PositionsSprite.Add(new Point(0, height));
            this.PositionsSprite.Add(new Point(width, height));

            this._bodySize = new Vector2(width, height);
            this.Body = new Rectangle(this.PositionsSprite[position], new Point(width, height));
            this.SetBoxCollision(world);
            this.CBody.IgnoreGravity = true;
            this.CBody.Tag = "Part_"+position.ToString();
            this.TextureSize = new Vector2(width, height);
            this.Rotation = 360/2f;
            this.CBody.Rotation = this.Rotation;
        }


        bool MouseClick = false;
        public override void Update(GameTime gameTime)
        {
            if (_mouseOver)
            {
                
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !this.MouseClick)
                {
                    this.MouseClick = true;
                    if (this.Rotation == -180) this.Rotation = 360 / 2f;
                    else this.Rotation -= 90;
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    this.MouseClick = false;
                }
            }

            _mouseOver = false;
            
        }

        bool _mouseOver = false;
        public override void OnMouseOver()
        {
            _mouseOver = true;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
        }

    }
}
