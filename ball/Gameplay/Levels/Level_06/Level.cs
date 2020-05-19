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

namespace ball.Gameplay.Levels.Level_06
{
    public class Level : Stage
    {

        CircleAndBar CircleAndBar;
        Arena Arena;
        List<CircleWall> ballWalls = new List<CircleWall>();

        public override void Start(ContentManager Content, World World, MouseManager mouse)
        {
            this.Mouse = mouse;
            this.World = World;

            this.ResetLevel(Content, World, mouse);
            this.LevelReady = true;
        }

        public override void ResetLevel(ContentManager Content, World World, MouseManager mouse)
        {
            CircleAndBar = new CircleAndBar();
            CircleAndBar._Screem = this.Screem;
            CircleAndBar.Content = Content;
            CircleAndBar._Mouse = mouse;
            CircleAndBar.World = World;
            CircleAndBar.Start();

            Arena = new Arena();
            Arena._Screem = this.Screem;
            Arena._Mouse = mouse;
            Arena.World = World;
            Arena.Start();
            Arena._BottomBar.Ball = CircleAndBar;

            this.Players.Add(CircleAndBar);
            this.Backgrounds.Add(Arena);

            for (int i = 0; i < 5; i++)
            {
                CircleWall ballWall = new CircleWall();
                ballWall.Sprite = Content.Load<Texture2D>("Sprites/Level_6/SmallBallWall");
                ballWall.Radius = ballWall.Sprite.Width / 2f;
                ballWall.SetCircleCollision(this.World);
                ballWall.CBody.SetRestitution(4);
                ballWall.CBody.SetFriction(0);
                ballWall.CBody.BodyType = BodyType.Static;
                ballWall.TextureSize = new Vector2(ballWall.Sprite.Width, ballWall.Sprite.Height);
                ballWall.World = this.World;

                if (i > 0)
                {
                    if (i % 2 == 0) ballWall.CBody.Position = new Vector2(this.Screem.getCenterScreem.X - ((5 + (ballWall.Sprite.Width / 2f)) * i), this.Screem.getCenterScreem.Y - 200);
                    else ballWall.CBody.Position = new Vector2(this.Screem.getCenterScreem.X + (ballWall.Sprite.Width / 2f) + 5 + ((5 + (ballWall.Sprite.Width / 2f)) * i), this.Screem.getCenterScreem.Y - 200);
                }
                else ballWall.CBody.Position = new Vector2(this.Screem.getCenterScreem.X, this.Screem.getCenterScreem.Y - 200);
                this.ballWalls.Add(ballWall);
                this.Backgrounds.Add(ballWall);
            }
        }

        public override void Destroy()
        {
            this.Arena.Destroy();
            this.CircleAndBar.Destroy();
            for (int i = 0; i < this.ballWalls.Count(); i++)
            {
                this.World.Remove(this.ballWalls[i].CBody);
            }
            this.Backgrounds.Clear();
            this.ballWalls.Clear();
            this.Players.Clear();
        }

        bool _FinishedWait = false;
        float _time;
        public override void UpdateLevel(GameTime gameTime)
        {
            this._FinishedWait = true;
            for (int i = 0; i < this.ballWalls.Count(); i++)
            {
                if (this.ballWalls[i].show) this._FinishedWait = false;
            }

            this.Update(gameTime);

            if (this._FinishedWait)
            {
               
                _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if ((int)_time > 2)
                {
                    this.Finished = true;
                    Console.WriteLine((int)_time);
                }
            }
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }

    }


    public class CircleWall : GameObject
    {

        public bool show = true;
        
        public override void Update(GameTime gameTime)
        {
            if (!this.show && this.CBody != null)
            {
                this.World.Remove(this.CBody);
                this.CBody = null;
            }
        }

        public override void OnCollision(string tag)
        {
            if (tag == "ball")
            {
                this.show = false;
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this.show) this.DrawSprite(spriteBatch);
        }

    }

    public class CircleAndBar : GameObject
    {

        public GameObject Ball;
        public Bar Bar;

        public ContentManager Content;

        public void Start()
        {
            Vector2 _positionBall = new Vector2(this._Screem.getCenterScreem.X, this._Screem.getCenterScreem.Y -26f);

            this.Ball = new GameObject();
            this.Ball.Sprite = Content.Load<Texture2D>("Sprites/Level_6/ball");
            this.Ball.Radius = 35 / 2f;
            this.Ball.TextureSize = new Vector2(this.Ball.Sprite.Width, this.Ball.Sprite.Height);
            this.Ball.SetCircleCollision(this.World);
            this.Ball.CBody.BodyType = BodyType.Dynamic;
            this.Ball.CBody.SetTransform(_positionBall, 0f);
            this.Ball.CBody.Tag = "ball";
            this.Ball.CBody.AngularVelocity = 10;
            
            Vector2 _positionBar = new Vector2(this._Screem.getCenterScreem.X, this._Screem.getCenterScreem.Y + 200f);

            this.Bar = new Bar();
            this.Bar.Sprite = Content.Load<Texture2D>("Sprites/Level_6/bar");
            this.Bar._bodySize = new Vector2(170, 30);
            this.Bar.TextureSize = this.Bar._bodySize;
            this.Bar.SetBoxCollision(this.World);
            this.Bar.CBody.BodyType = BodyType.Kinematic;
            this.Bar.CBody.SetTransform(_positionBar, 0f);
            this.Bar.CBody.SetRestitution(0);
            this.Bar.CBody.SetFriction(0);
            this.Bar.Ball = this.Ball;
            this.Bar._Screem = this._Screem;
            this.Bar.CBody.Tag = "bar";

            this.World.Gravity = new Vector2(0, 1);
        }
        
        public void Restart()
        {
            this._keyPressed = false;
        }

        bool _keyPressed = false;
        float _MoveForce = 10f;
        public override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Vector2 _PositionBar = new Vector2(this.Bar.CBody.Position.X - _MoveForce, this.Bar.CBody.Position.Y);
                this.Bar.CBody.SetTransform(_PositionBar, 0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Vector2 _PositionBar = new Vector2(this.Bar.CBody.Position.X + _MoveForce, this.Bar.CBody.Position.Y);
                this.Bar.CBody.SetTransform(_PositionBar, 0f);
            }

            if (!this._keyPressed)
            {
                if (!this.World.IsLocked)
                {
                    Vector2 _positionBall = new Vector2(this._Screem.getCenterScreem.X, this._Screem.getCenterScreem.Y - 26f);
                    this.Ball.CBody.SetTransformIgnoreContacts(ref _positionBall, this.Ball.CBody.Rotation);
                    this._keyPressed = true;
                    Vector2 _force = new Vector2(1000000, -9000000);

                    this.Bar.CBody.SetRestitution(80);
                    this.Ball.CBody.ApplyForce(ref _force);
                }
                
            }
            this.Bar.Update(gameTime);
        }

        public void Destroy()
        {
            this.World.Remove(this.Bar.CBody);
            this.World.Remove(this.Ball.CBody);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.Ball.DrawSprite(spriteBatch);
            this.Bar.DrawSprite(spriteBatch);
        }
    }

    public class Bar : GameObject
    {
        public GameObject Ball;
        Vector2 _OldPositionBar = Vector2.Zero;
        float _time;

        public override void Update(GameTime gameTime)
        {
            if (this._OldPositionBar != Vector2.Zero)
            {
                _time += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_time > 0.05f)
                {
                    if (this._OldPositionBar.X > this._Screem.getCenterScreem.X)
                    {
                        this.Ball.CBody.ApplyLinearImpulse(new Vector2(this.CBody.LinearVelocity.X - ((this._OldPositionBar.X - this._Screem.getCenterScreem.X) * 0.9f), this.CBody.LinearVelocity.Y - 200));
                        this._OldPositionBar = Vector2.Zero;
                    }
                    else if (this._OldPositionBar.X < this._Screem.getCenterScreem.X)
                    {
                        this.Ball.CBody.ApplyLinearImpulse(new Vector2(this.CBody.LinearVelocity.X + ((this._Screem.getCenterScreem.X - this._OldPositionBar.X) * 0.9f), this.CBody.LinearVelocity.Y - 200));
                        this._OldPositionBar = Vector2.Zero;
                    }
                    _time = 0;
                }
                
            }
        }

        public override void OnCollision(string tag)
        {
            if (tag == "ball")
            {
                this._OldPositionBar = this.CBody.Position;
            }
        }
    }

    public class Arena : GameObject {
        
        GameObject _LeftBar;
        GameObject _RightBar;
        GameObject _TopBar;
        public BottomBar _BottomBar;

        float Resititution = 80;

        public void Start()
        {
            // left bar
            this._LeftBar = new GameObject();
            this._LeftBar._Mouse = this._Mouse;
            this._LeftBar._bodySize = new Vector2(7, this._Screem.getCurrentResolutionSize.Y);
            this._LeftBar.SetBoxCollision(this.World);
            this._LeftBar.CBody.BodyType = BodyType.Static;
            this._LeftBar.CBody.SetRestitution(Resititution);
            this._LeftBar.CBody.SetFriction(0);
            this._LeftBar.CBody.Position = new Vector2(0, this._Screem.getCenterScreem.Y);
            this._LeftBar.Origin = new Vector2(4, this._Screem.getCenterScreem.Y);

            // right bar
            this._RightBar = new GameObject();
            this._RightBar._Mouse = this._Mouse;
            this._RightBar._bodySize = new Vector2(7, this._Screem.getCurrentResolutionSize.Y);
            this._RightBar.SetBoxCollision(this.World);
            this._RightBar.CBody.BodyType = BodyType.Static;
            this._RightBar.CBody.SetRestitution(Resititution);
            this._RightBar.CBody.SetFriction(0);
            this._RightBar.CBody.Position = new Vector2(this._Screem.getCurrentResolutionSize.X, this._Screem.getCenterScreem.Y);
            this._RightBar.Origin = new Vector2(4, this._Screem.getCenterScreem.Y);

            // top bar
            this._TopBar = new GameObject();
            this._TopBar._Mouse = this._Mouse;
            this._TopBar._bodySize = new Vector2(this._Screem.getCurrentResolutionSize.X, 7);
            this._TopBar.SetBoxCollision(this.World);
            this._TopBar.CBody.BodyType = BodyType.Static;
            this._TopBar.CBody.SetRestitution(Resititution);
            this._TopBar.CBody.SetFriction(0);
            this._TopBar.CBody.Position = new Vector2(this._Screem.getCenterScreem.X, 0);
            this._TopBar.Origin = new Vector2(this._Screem.getCenterScreem.X, 4);

            // bottom bar
            this._BottomBar = new BottomBar();
            this._BottomBar.World = this.World;
            this._BottomBar._Mouse = this._Mouse;
            this._BottomBar._Screem = this._Screem;
            this._BottomBar.Start(this.Resititution);

        }

        public void Destroy()
        {
            this.World.Remove(this._LeftBar.CBody);
            this._LeftBar.CBody = null;
            this.World.Remove(this._RightBar.CBody);
            this._RightBar.CBody = null;
            this.World.Remove(this._TopBar.CBody);
            this._TopBar.CBody = null;
            this.World.Remove(this._BottomBar.CBody);
            this._BottomBar.CBody = null;
        }


        public override void Update(GameTime gameTime)
        {
        }
        
    }


    public class BottomBar : GameObject
    {

        public CircleAndBar Ball;

        public void Start(float Resititution)
        {
            this._bodySize = new Vector2(this._Screem.getCurrentResolutionSize.X, 7);
            this.SetBoxCollision(this.World);
            this.CBody.BodyType = BodyType.Static;
            this.CBody.SetRestitution(Resititution);
            this.CBody.SetFriction(0);
            this.CBody.Position = new Vector2(this._Screem.getCenterScreem.X, this._Screem.getCurrentResolutionSize.Y);
            this.Origin = new Vector2(this._Screem.getCenterScreem.X, 4);
        }

        public override void OnCollision(string tag)
        {
            if (tag == "ball") this.Ball.Restart();
        }

    }

}
