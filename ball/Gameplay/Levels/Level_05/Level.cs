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

namespace ball.Gameplay.Levels.Level_05
{
    #region Level
    public class Level : Stage
    {
        public YinYang YinYang;
        public override void Start(ContentManager Content, World World, MouseManager Mouse)
        {
            this.Content = Content;
            this.World = World;
            this.Mouse = Mouse;
            this.ResetLevel();
        }

        public override void ResetLevel()
        {
            this.SetBackgroundColor = Color.White;
            this.YinYang = new YinYang();
            this.YinYang.Content = Content;
            this.YinYang.Start();
            this.Players.Add(this.YinYang);
            this.Finished = false;
            this.LevelReady = true;
        }

        public override void Destroy()
        {
            this.Players.Clear();
            this.Finished = false;
            this.LevelReady = false;
        }

        public override void UpdateLevel(GameTime gameTime)
        {
            this.YinYang.Position = this.Screem.getCenterScreem;
            this.YinYang.SmallCircles.Position = this.Screem.getCenterScreem;
            if (this.YinYang.Finished) this.Finished = true;
            this.Update(gameTime);
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }
    }
    #endregion

    #region YinYang
    public class YinYang : GameObject
    {

        public ContentManager Content;
        
        public Texture2D BlackCircleSprite;
        public Texture2D YinYangSprite;

        public GameObject SmallCircles;

        private float RotationNum = 5;

        public bool Finished = false;
        
        public void Start()
        {
            this.BlackCircleSprite = Content.Load<Texture2D>("Sprites/Level_5/black_circle");
            this.YinYangSprite = Content.Load<Texture2D>("Sprites/Level_5/black_white_circle");
            this.Origin = new Vector2(this.BlackCircleSprite.Width / 2f, this.BlackCircleSprite.Height / 2f);
            this.Sprite = this.BlackCircleSprite;

            this.SmallCircles = new GameObject();
            this.SmallCircles.Sprite = Content.Load<Texture2D>("Sprites/Level_5/black_white_small_circle");
            this.SmallCircles.Origin = new Vector2(this.SmallCircles.Sprite.Width / 2f, this.SmallCircles.Sprite.Height / 2f);
        }

        public float _time;
        private bool _ChangeSprite = true;
        private bool _Stoping = false;
        private bool _SetRotation = true;
        public bool InitialAnimation = true;

        private Vector2 _LastClick;

        public override void Update(GameTime gameTime)
        {
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (InitialAnimation)
            {
                if (_time > 2)
                {
                    if (this._ChangeSprite)
                    {
                        this.Sprite = this.YinYangSprite;
                        this._ChangeSprite = false;
                    }
                    if (this.RotationNum > 0) this.RotationNum -= 0.1f;
                    else this._Stoping = true;

                    if (!this._Stoping)
                    {
                        if (this.Rotation >= 360) this.Rotation = 0;
                        else this.Rotation += this.RotationNum;
                    }
                    else
                    {
                        if (this.Rotation >= 135) this.InitialAnimation = false;
                        else this.Rotation += 0.5f;
                    }
                }
            }
            else
            {
                if (this._SetRotation)
                {
                    this.Rotation = 3;
                    this._SetRotation = false;
                }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    Vector2 _ClickPosition = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
                    if (this._LastClick == Vector2.Zero)
                    {
                        this._LastClick = _ClickPosition;
                    }
                    else
                    {
                        if (this._LastClick.Y < _ClickPosition.Y)
                        {
                            this.Rotation += 0.05f;
                        }
                        else
                        {
                            this.Rotation -= 0.05f;
                        }
                    }
                } else if (Mouse.GetState().LeftButton == ButtonState.Released) this._LastClick = Vector2.Zero;
                
                if ((this.Rotation < 0.5f && this.Rotation > -0.3f) || (this.Rotation > 5.8f && this.Rotation < 6.3)) this.Finished = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
            if (!this.InitialAnimation) this.SmallCircles.DrawSprite(spriteBatch);
        }
    }
    #endregion
}
