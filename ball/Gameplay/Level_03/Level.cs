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

        public override void Start(ContentManager Content, World World, MouseManager mouse)
        {
            this.World = World;
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
            }
            this.SetBackgroundColor = Color.White;
            this.LevelReady = true;
        }

        public override void Destroy()
        {
            for (int i = 0; i < this.Players.Count(); i++)
            {
                this.World.Remove(this.Players[i].CBody);
            }
            this.Circle = new List<WhiteCircle>();
            this.Players = new List<GameObject>();
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
            }
            
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }

    }

    public class WhiteCircle : GameObject
    {
        public GameObject BlackCircle;
        public int Id;
        public void Start(World world)
        {
            this.Radius = this.Sprite.Width / 2f;
            this.Origin = new Vector2(this.Sprite.Width / 2f, this.Sprite.Height / 2f);
            this.SetCircleCollision(world);
            this.CBody.Tag = "Circle_level_3";
            this.CBody.IgnoreGravity = true;
            this.CBody.BodyType = BodyType.Dynamic;
            this.TextureSize = new Vector2(this.Sprite.Width, this.Sprite.Height);

            // set black circle
            this.BlackCircle.Radius = this.BlackCircle.Sprite.Width / 2f;
            this.BlackCircle.Origin = new Vector2(this.BlackCircle.Sprite.Width / 2f, this.Sprite.Height / 2f);
        }
        
        bool MouseClick = false;
        bool _mouseOver = false;
        public override void OnMouseOver()
        {
            this._mouseOver = true;
        }

        private float _time;
        private float _timeInitialAnimation;
        private bool _ignoreGravity = true;
        private bool _animation = true;
        private bool _explosionAnimation = false;
        private int _timeExeposionAnimation;
        

        private Vector2 _InitialPosition;
        private Vector2 _shakePosition;
        private Vector2 _positionAfterExposion;
        private float _positionAfterExposionFrame = 20;
        private float _shakeMagnitude = 2f;
        private static readonly Random getrandom = new Random();

        public override void Update(GameTime gameTime)
        {
            Vector2 _position = this._Screem.getCenterScreem;
            if (_InitialPosition == Vector2.Zero) _InitialPosition = this._Screem.getCenterScreem;

            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeInitialAnimation += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_animation)
            {
                if (_time % 0.2f >= 0.032f && _timeExeposionAnimation != 4)
                {
                    lock (getrandom)
                    {
                        int randomX = getrandom.Next(5);
                        int randomY = getrandom.Next(5);


                        _shakePosition = new Vector2(
                            (randomX * _shakeMagnitude) + _InitialPosition.X,
                            (randomY * _shakeMagnitude) + _InitialPosition.Y 
                        );
                        
                    }
                    _time = 0;
                }

                if (_timeExeposionAnimation != 4) this.CBody.SetTransformIgnoreContacts(ref _shakePosition, 0);

                if (_timeInitialAnimation % 2f >= (0.032f * 15) && !_explosionAnimation && _timeExeposionAnimation == 0)
                {
                    _shakeMagnitude += 0.5f;
                    _timeInitialAnimation = 0;
                    if (_shakeMagnitude > 5)
                    {
                        this.BlackCircle.Scale = 2;
                        _explosionAnimation = true;
                        _shakeMagnitude += 10f;
                    }
                } else if (_timeInitialAnimation % 2f >= (0.032f * 15) && _explosionAnimation && _timeExeposionAnimation != 4)
                {
                    _timeExeposionAnimation++;
                    _timeInitialAnimation = 0;
                    if (_timeExeposionAnimation == 4) _positionAfterExposion = _position;
                }

                if (_timeExeposionAnimation == 4 && _timeInitialAnimation % 2f >= 0.032f && _positionAfterExposionFrame != 0)
                {
                    this.BlackCircle.Scale = 1;
                    
                    
                    switch (this.Id)
                    {
                        case 0:
                            _positionAfterExposion.X = (((_position.X /2f) / ((20 -_positionAfterExposionFrame) * 2f))) + (_position.X / 2f);
                            break;
                        case 1:
                            _positionAfterExposion.X = ((_position.X * 2) - (_position.X / ((20 - _positionAfterExposionFrame) * 2f))) - (_position.X / 2f);
                            break;
                    }

                    _positionAfterExposionFrame--;
                    _timeInitialAnimation = 0;

                    if (_positionAfterExposionFrame == 0) this._animation = false;
                }
                if(this._positionAfterExposion != Vector2.Zero) this.CBody.SetTransformIgnoreContacts(ref _positionAfterExposion, 0);


            } else
            {
                switch (this.Id)
                {
                    case 0:
                        _position.X -= _position.X / 2f;
                        break;
                    case 1:
                        _position.X += _position.X / 2f;
                        break;
                }

                this.CBody.SetTransform(ref _position, 0f);
            }

            this.BlackCircle.Position = this.CBody.Position;
            this._mouseOver = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.BlackCircle.DrawSprite(spriteBatch);
            this.DrawSprite(spriteBatch);
        }
    }
}
