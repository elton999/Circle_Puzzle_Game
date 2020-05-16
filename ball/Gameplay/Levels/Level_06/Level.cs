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
        Table Table;

        public override void Start(ContentManager Content, World World, MouseManager mouse)
        {
            this.Mouse = mouse;
            this.World = World;

            CircleAndBar = new CircleAndBar();
            CircleAndBar._Screem = this.Screem;
            CircleAndBar.Content = Content;
            CircleAndBar._Mouse = mouse;
            CircleAndBar.World = World;
            CircleAndBar.Start();
            
            Table = new Table();
            Table._Screem = this.Screem;
            Table._Mouse = mouse;
            Table.World = World;
            Table.Start();

            this.Players.Add(CircleAndBar);
            this.Backgrounds.Add(Table);

            this.LevelReady = true;
        }

        public override void ResetLevel(ContentManager Content, World World, MouseManager mouse)
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


    public class CircleAndBar : GameObject
    {

        GameObject Ball;
        GameObject Bar;

        public ContentManager Content;

        public void Start()
        {
            Vector2 _positionBall = new Vector2(this._Screem.getCenterScreem.X, this._Screem.getCenterScreem.Y -60f);

            this.Ball = new GameObject();
            this.Ball.Sprite = Content.Load<Texture2D>("Sprites/Level_6/ball");
            this.Ball.Radius = 35 / 2f;
            this.Ball.Origin = new Vector2(this.Radius, this.Radius);
            this.Ball.SetCircleCollision(this.World);
            this.Ball.CBody.BodyType = BodyType.Dynamic;
            this.Ball.CBody.SetTransform(_positionBall, 0f);


            this.Bar = new GameObject();
            this.Bar.Sprite = Content.Load<Texture2D>("Sprites/Level_6/bar");
            this.Bar._bodySize = new Vector2(100, 16);
            this.Bar.SetBoxCollision(this.World);
            this.Bar.CBody.BodyType = BodyType.Static;
            this.Bar.TextureSize = this._bodySize;
            this.Bar.CBody.SetTransform(this._Screem.getCenterScreem, 0f);
            this.Bar.CBody.SetRestitution(2);
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.Ball.DrawSprite(spriteBatch);
            this.Bar.DrawSprite(spriteBatch);
        }
    }

    public class Table : GameObject {
        
        GameObject _LeftBar;
        GameObject _RightBar;
        GameObject _TopBar;

        public void Start()
        {
            // left bar
            this._LeftBar = new GameObject();
            this._LeftBar._Mouse = this._Mouse;
            this._LeftBar._bodySize = new Vector2(7, this._Screem.getCurrentResolutionSize.Y);
            this._LeftBar.SetBoxCollision(this.World);
            this._LeftBar.CBody.BodyType = BodyType.Static;
            this._LeftBar.CBody.SetRestitution(2);
            this._LeftBar.CBody.Position = new Vector2(0, this._Screem.getCenterScreem.Y);
            this._LeftBar.Origin = new Vector2(4, this._Screem.getCenterScreem.Y);

            // right bar
            this._RightBar = new GameObject();
            this._RightBar._Mouse = this._Mouse;
            this._RightBar._bodySize = new Vector2(7, this._Screem.getCurrentResolutionSize.Y);
            this._RightBar.SetBoxCollision(this.World);
            this._RightBar.CBody.BodyType = BodyType.Static;
            this._RightBar.CBody.SetRestitution(2);
            this._RightBar.CBody.Position = new Vector2(this._Screem.getCurrentResolutionSize.X, this._Screem.getCenterScreem.Y);
            this._RightBar.Origin = new Vector2(4, this._Screem.getCenterScreem.Y);

            // top bar
            this._TopBar = new GameObject();
            this._TopBar._Mouse = this._Mouse;
            this._TopBar._bodySize = new Vector2(this._Screem.getCurrentResolutionSize.X, 7);
            this._TopBar.SetBoxCollision(this.World);
            this._TopBar.CBody.BodyType = BodyType.Static;
            this._TopBar.CBody.SetRestitution(2);
            this._TopBar.CBody.Position = new Vector2(this._Screem.getCenterScreem.X, 0);
            this._TopBar.Origin = new Vector2(this._Screem.getCenterScreem.X, 4);

        }


        public override void Update(GameTime gameTime)
        {
        }
        
    }

}
