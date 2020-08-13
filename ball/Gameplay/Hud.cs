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
using ball.Managers;

namespace ball.Gameplay
{
    public class Hud : Scene
    {
        public MouseManager Mouse;
        public GameManager GameManager;

        public Reload Reload;
        public Home Home;
        public Resize Resize;
        public BackBTN Back;
        public World World;

        public bool ReloadShow = true;
        public bool ResizeShow = true;
        public bool HomeShow = true;
        public bool BackShow = false;

        public Stage CurrentLevel;

        public void Start(ContentManager Content, MouseManager Mouse, ScreemController ScreemController)
        {
            this.Content = Content;
            this.Mouse = Mouse;
            this.Screem = ScreemController;

            if (this.ReloadShow)
            {
                this.Reload = new Reload();
                this.Reload.Start(Content, World, Mouse, ScreemController);
                this.UI.Add(this.Reload);
            }

            if (this.HomeShow)
            {
                this.Home = new Home();
                this.Home.GameManager = this.GameManager;
                this.Home.Hud = this;
                this.Home.Start(Content, World, Mouse, ScreemController);
                this.UI.Add(this.Home);
            }

            if (this.ResizeShow && this.Resize == null)
            {
                this.Resize = new Resize();
                this.Resize.Start(Content, World, Mouse, ScreemController);
                this.UI.Add(this.Resize);
            }

            
            if (this.BackShow)
            {
                if(this.Back == null) this.Back = new BackBTN();
                this.Back.Start(Content, World, Mouse, ScreemController);
                this.UI.Add(this.Back);
            }
            
            this.SetBackgroundColor = Color.Transparent;
            this.LevelReady = true;
        }
        
        public void Destroy()
        {
            this.LevelReady = false;
            if (this.Resize.CBody != null) this.Resize.CBody.World.Remove(this.Resize.CBody);
            if (this.Home.CBody != null) this.Home.CBody.World.Remove(this.Home.CBody);
            if (this.Reload.CBody != null) this.Reload.CBody.World.Remove(this.Reload.CBody);
            if (this.BackShow && this.Back.CBody != null) this.Back.CBody.World.Remove(this.Back.CBody);

            this.Home = null;
            this.Resize = null;
            this.Reload = null;
            this.Back = null;

            this.UI = new List<GameObject>();
        }

        public void SetLevel(Stage stage)
        {
            this.CurrentLevel = stage;
            this.Reload.CurrentLevel = stage;
        }
    }

    #region Back
    public class BackBTN : GameObject
    {
        public void Start(ContentManager Content, World World, MouseManager Mouse, ScreemController ScreemController)
        {
            this._Screem = ScreemController;
            this.Sprite = Content.Load<Texture2D>("Sprites/UI/arrowLeft");
            this._bodySize = new Vector2(this.Sprite.Height, this.Sprite.Width);
            this.TextureSize = this._bodySize;
            this._Mouse = Mouse;
            this.SetBoxCollision(World);
            this.CBody.Tag = "Back";
        }

        bool _pressedLeftButton;
        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && this._MouseOver && !_pressedLeftButton)
            {
                _pressedLeftButton = true;
                this.Click();
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released) _pressedLeftButton = false;

            Vector2 _position = new Vector2(this.Sprite.Width, this.Sprite.Height);
            if(!_pressedLeftButton)this.CBody.SetTransform(ref _position, this.CBody.Rotation);
            
            this._MouseOver = false;
        }

        public virtual void Click() { }

        bool _MouseOver;
        public override void OnMouseOver()
        {
            this._MouseOver = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
        }
    }

    #endregion

    #region Reload
    public class Reload : GameObject
    {
        public Stage CurrentLevel;
        public void Start(ContentManager Content, World World, MouseManager Mouse, ScreemController ScreemController)
        {
            this._Screem = ScreemController;
            this.Sprite = Content.Load<Texture2D>("Sprites/UI/return");
            this._bodySize = new Vector2(this.Sprite.Height, this.Sprite.Width);
            this.TextureSize = this._bodySize;
            this._Mouse = Mouse;
            this.SetBoxCollision(World);
            this.CBody.Tag = "Reload";
        }

        bool _pressedLeftButton;
        public override void Update(GameTime gameTime)
        {
            if (_MouseOver && Mouse.GetState().LeftButton == ButtonState.Pressed && !_pressedLeftButton)
            {
                CurrentLevel.Destroy();
                CurrentLevel.ResetLevel();
                _pressedLeftButton = true;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released) _pressedLeftButton = false;

            Vector2 _position = new Vector2(this._Screem.getCenterScreem.X, this.Sprite.Height);
            this.CBody.SetTransform(ref _position, this.CBody.Rotation);

            _MouseOver = false;
        }
        
        bool _MouseOver;
        public override void OnMouseOver()
        {
            _MouseOver = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
        }
    }
    #endregion

    #region Resize
    public class Resize : GameObject
    {
        Texture2D SpriteFull;
        Texture2D SpriteWindowed;
        public bool CanResize = true;

        public void Start(ContentManager Content, World World, MouseManager Mouse, ScreemController ScreemController)
        {
            this._Screem = ScreemController;
            this.SpriteFull = Content.Load<Texture2D>("Sprites/UI/larger");
            this.SpriteWindowed = Content.Load<Texture2D>("Sprites/UI/smaller");
            this._bodySize = new Vector2(this.SpriteFull.Height, this.SpriteFull.Width);
            this.TextureSize = this._bodySize;
            this._Mouse = Mouse;
            this.SetBoxCollision(World);
            this.CBody.Tag = "Resize";
            this.CBody.BodyType = BodyType.Static;
        }

        bool _pressedLeftButton;
        public override void Update(GameTime gameTime)
        {
            if (this._Screem.graphics.IsFullScreen) this.Sprite = this.SpriteWindowed;
            else this.Sprite = this.SpriteFull;


            if (Mouse.GetState().LeftButton == ButtonState.Pressed && this._MouseOver && !_pressedLeftButton && !this.RemoveFromScene)
            {
                _pressedLeftButton = true;
                this._Screem.graphics.IsFullScreen = !this._Screem.graphics.IsFullScreen;
                this._Screem.graphics.ApplyChanges();
            } else if (Mouse.GetState().LeftButton == ButtonState.Released) _pressedLeftButton = false;

            Vector2 _position = new Vector2(this._Screem.getCurrentResolutionSize.X - this.Sprite.Width, this.Sprite.Height);
            this.CBody.SetTransform(ref _position, this.CBody.Rotation);

            this._MouseOver = false;
        }

        bool _MouseOver;
        public override void OnMouseOver()
        {
            if (CanResize)
                this._MouseOver = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
        }
    }
    #endregion

    #region Home
    public class Home : GameObject
    {
        public GameManager GameManager;
        public Hud Hud;
        public void Start(ContentManager Content, World World, MouseManager Mouse, ScreemController ScreemController)
        {
            this._Screem = ScreemController;
            this.Sprite = Content.Load<Texture2D>("Sprites/UI/home");
            this._bodySize = new Vector2(this.Sprite.Height, this.Sprite.Width);
            this.TextureSize = this._bodySize;
            this._Mouse = Mouse;
            this.SetBoxCollision(World);
            this.CBody.Tag = "Home";
            this.CBody.BodyType = BodyType.Static;
        }

        bool _pressedLeftButton;
        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && this._MouseOver && !_pressedLeftButton)
            {
                this.Hud.Resize._Mouse = null;
                GameManager.GoToMenu();
                _pressedLeftButton = true;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released) _pressedLeftButton = false;

            Vector2 _position = new Vector2(this.Sprite.Width, this.Sprite.Height);
            this.CBody.SetTransform(ref _position, this.CBody.Rotation);

            this._MouseOver = false;
        }

        bool _MouseOver;
        public override void OnMouseOver()
        {
            this._MouseOver = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
        }
    }
    #endregion
}
