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

namespace ball.Gameplay.Level_01
{
    public class Level : Stage
    {
        public Circle WhiteCircle;
        
        public override void Start (ContentManager Content, World World, MouseManager mouse)
        {
            this.World = World;
            this.ResetLevel(Content, World, mouse);
        }

        public override void ResetLevel(ContentManager Content, World World, MouseManager mouse)
        {
            this.Mouse = mouse;
            this.WhiteCircle = new Circle();
            this.WhiteCircle._Mouse = this.Mouse;
            this.WhiteCircle.setWhiteCircle(Content, World);
            this.Players.Add(this.WhiteCircle);

            this.SetBackgroundColor = Color.Black;
            this.LevelReady = true;
            this.Finished = false;
        }

        public override void Destroy()
        {
            for (int i = 0; i < this.Players.Count(); i++)
            {
                this.World.Remove(this.Players[i].CBody);
            }
            this.Players.Clear();
            this.WhiteCircle = null;
            this.LevelReady = false;
        }

        private float _MaxSize = 6f;

        public override void UpdateLevel(GameTime gameTime)
        {
            this.WhiteCircle.CBody.Position = this.Screem.getCenterScreem;
            this.WhiteUI = this.WhiteCircle.WhiteUI;

            if (this.WhiteCircle.Scale > this._MaxSize) this.Finished = true;

            this.Update(gameTime);
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }
    }

    public class Circle : GameObject
    {

        public bool WhiteUI { get; set; }
        private bool MouseOver;
        private bool MouseClick;

        public void setWhiteCircle(ContentManager Content, World World)
        {
            this.World = World;
            this.Sprite = Content.Load<Texture2D>("Sprites/white_circle");
            this.Radius = this.Sprite.Width / 2f;
            this.Origin = new Vector2(this.Position.X + (this.Sprite.Width / 2f), this.Position.Y + (this.Sprite.Height / 2f));
            this.SetCircleCollision(World);
            this.CBody.Tag = "Circle";
            this.CBody.IgnoreGravity = true;
            this.CBody.BodyType = BodyType.Dynamic;
            this.TextureSize = new Vector2(this.Sprite.Width, this.Sprite.Height);

            this.SetGrowUpAnimation();
            this.SetFollowTroughtAnimation();
            this.IsGrowingUp = false;
        }

        public override void OnMouseOver()
        {
            this.MouseOver = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.MouseOver)
            {
                this.WhiteUI = false;

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !this.MouseClick && !this.IsGrowingUp)
                {
                    this.IsGrowingUp = true;
                    this.MouseClick = true;
                    this._MoreBiggerFrame = 0;
                    this._FollowTroughtFrame = 0;
                } else if (Mouse.GetState().LeftButton == ButtonState.Released) this.MouseClick = false;

            } else this.WhiteUI = true;

            this.GrowUp(gameTime);

            this.MouseOver = false;
        }
        

        private bool IsGrowingUp { get; set; }
        private float _time;
        public void GrowUp(GameTime gameTime)
        {
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsGrowingUp)
            {
                if (this._MoreBiggerFrame < this._MoreBigger.Count())
                {
                    if (_time % 0.2f >= 0.032f)
                    {
                        this.Scale += this._MoreBigger[this._MoreBiggerFrame];
                        this._MoreBiggerFrame++;
                        _time = 0;
                    }
                }
            }
            else if (this._FollowTroughtFrame >= this._FollowTrought.Count())
            {
                if (this.Scale > 1f && _time % 0.2f >= 0.032f)
                {
                    this.Scale -= 0.2f;
                    _time = 0;   
                }
            }

            if (this._MoreBiggerFrame == this._MoreBigger.Count() && this._FollowTroughtFrame < this._FollowTrought.Count())
            {
                if (_time % 0.2f >= 0.032f)
                {
                    this.Scale += this._FollowTrought[this._FollowTroughtFrame];
                    this._FollowTroughtFrame++;
                    _time = 0;

                    if (this._FollowTroughtFrame > 15) this.IsGrowingUp = false;
                }
            }
                
        }

        private List<float> _MoreBigger = new List<float>();
        private int _MoreBiggerFrame;
        private void SetGrowUpAnimation()
        {
            float _MaxForce = 0.8f;
            this._MoreBigger.Add(_MaxForce/64f);
            this._MoreBigger.Add(_MaxForce/32f);
            this._MoreBigger.Add(_MaxForce/16f);
            this._MoreBigger.Add(_MaxForce/8f);
            this._MoreBigger.Add(_MaxForce/4f);
            this._MoreBigger.Add(_MaxForce/2f);
        }

        private List<float> _FollowTrought = new List<float>();
        private int _FollowTroughtFrame = 0;
        private void SetFollowTroughtAnimation() {
            float _MaxForce = 0.2f;

            this._FollowTrought.Add(_MaxForce);

            this._FollowTrought.Add(-_MaxForce/2f);
            this._FollowTrought.Add(-_MaxForce/2f);

            this._FollowTrought.Add(-_MaxForce);

            this._FollowTrought.Add(_MaxForce/4f);
            this._FollowTrought.Add(_MaxForce/4f);

            this._FollowTrought.Add(_MaxForce/4f);

            this._FollowTrought.Add(-_MaxForce/4f);
            this._FollowTrought.Add(-_MaxForce/4f);

            this._FollowTrought.Add(-_MaxForce/4f);

            this._FollowTrought.Add(_MaxForce / 8f);
            this._FollowTrought.Add(_MaxForce / 8f);

            this._FollowTrought.Add(_MaxForce / 8f);

            this._FollowTrought.Add(-_MaxForce / 16f);
            this._FollowTrought.Add(-_MaxForce / 16f);

            this._FollowTrought.Add(-_MaxForce / 8f);

            this._FollowTrought.Add(_MaxForce / 16f);
            this._FollowTrought.Add(_MaxForce / 16f);

            this._FollowTrought.Add(_MaxForce / 16f);
            this._FollowTrought.Add(-_MaxForce / 16f);
            this._FollowTrought.Add(-_MaxForce / 16f);
            this._FollowTrought.Add(-_MaxForce / 16f);
            this._FollowTrought.Add(_MaxForce / 16f);
            this._FollowTrought.Add(-_MaxForce / 16f);

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
        }
    }
}
