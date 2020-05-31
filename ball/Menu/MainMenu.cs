using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ball.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using UmbrellaToolKit;
using UmbrellaToolKit.Sprite;
using UmbrellaToolKit.UI;

namespace ball.Menu
{
    public class MainMenu : Scene
    {
        public UmbrellaToolKit.Storage.Load Storage;
        public SpriteFont Font;
        public List<MenuItem> MenuItems = new List<MenuItem>();
        public string[] MenuItemsString = { "PLAY", "SETTINGS", "QUIT" };
        public enum MenuItens { PLAY, SETTINGS, QUIT, NONE };
        public MenuItens ItemOver;
        public World World;
        public MouseManager Mouse;
        public Hud Hud;

        public void Start()
        {
            this.Restart();
        }

        public void Restart()
        {
            this.ItemOver = MenuItens.NONE;

            this.Hud = new Hud();
            this.Hud.World = this.World;
            this.Hud.ReloadShow = false;
            this.Hud.HomeShow = false;
            this.Hud.Start(this.Content, this.Mouse, this.Screem);

            for (int i = 0; i < 3; i++)
            {
                MenuItem MenuItem = new MenuItem();
                MenuItem.Value = MenuItemsString[i];
                MenuItem.Content = this.Content;
                MenuItem.Font = this.Font;
                MenuItem._Mouse = this.Mouse;
                MenuItem.Position = new Vector2(this.Screem.getCenterScreem.X, this.Screem.getCenterScreem.Y + (50 * i));
                MenuItem.World = this.World;
                MenuItem.Id = i;
                MenuItem.Start();
                MenuItems.Add(MenuItem);
                this.UI.Add(MenuItem);
            }
            this.SetBackgroundColor = Color.White;
            this.LevelReady = true;
        }

        public void Destroy()
        {
            this.ItemOver = MenuItens.NONE;
            foreach (MenuItem MenuItem in MenuItems) {
                this.World.Remove(MenuItem.CBody);                
            }
            this.MenuItems.Clear();
            this.UI.Clear();

            this.Hud.Destroy();

            this.LevelReady = false;
        }

    }


    public class MenuItem : GameObject
    {
        public int Id;
        public SpriteFont Font;
        public string Value;
        public MainMenu MainMenu;
        public ContentManager Content;
        private MenuItemBox MenuItemBox;

        public void Start()
        {
            this.SpriteColor = Color.Black;
            this.Origin = this.Font.MeasureString(this.Value) / 2f;
            this.MenuItemBox = new MenuItemBox();
            this.MenuItemBox.Position = new Vector2(this.Position.X - 80, this.Position.Y - 20);
            this.MenuItemBox.ContentSize = new Vector2(140, 20);
            this.MenuItemBox.Content = this.Content;
            this.MenuItemBox.Start();
            this._bodySize = new Vector2(160, 40);
            this.SetBoxCollision(this.World);
            this.CBody.SetTransform(new Vector2(this.Position.X  - 10, this.Position.Y - 15), this.Rotation);
            this.CBody.Tag = "Menu_Item_"+this.Id;
        }

        bool _MouseOver;
        public override void OnMouseOver()
        {
            this._MouseOver = true;
        }

        bool _click;
        public override void Update(GameTime gameTime)
        {
            if (this._MouseOver)
            {
                this.MenuItemBox.Sprite = this.MenuItemBox.SpriteOver;
                this.SpriteColor = Color.White;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_click)
                {
                    this._click = true;
                    MainMenu.ItemOver = (MainMenu.MenuItens)this.Id;
                } else if (Mouse.GetState().LeftButton == ButtonState.Released && _click)
                {
                    this._click = false;
                }
            }
            else
            {
                this.MenuItemBox.Sprite = this.MenuItemBox.SpriteBorder;
                this.SpriteColor = Color.Black;
            }

            this._MouseOver = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.MenuItemBox.Draw(spriteBatch);
            spriteBatch.DrawString(this.Font, this.Value, this.Position, this.SpriteColor*this.Transparent, this.Rotation, this.Origin, this.Scale, this.spriteEffect, 0f);
        }
    }

    public class MenuItemBox : BoxSprite
    {
        public ContentManager Content;
        public Texture2D SpriteBorder;
        public Texture2D SpriteOver;

        public void Start()
        {
            this.SpriteOver = Content.Load<Texture2D>("Sprites/UI/menu_item_over");
            this.SpriteBorder = Content.Load<Texture2D>("Sprites/UI/menu_item");

            this.Sprite = this.SpriteBorder;

            this.ContentBoxSprite = new Rectangle(new Point(10,10), new Point(10, 10));

            this.LeftTopBoxSprite = new Rectangle(new Point(0,0), new Point(10,10));
            this.RightTopBoxSprite = new Rectangle(new Point(33, 0), new Point(10, 10));
            this.LeftBottomBoxSprite = new Rectangle(new Point(0, 33), new Point(10, 10));
            this.RightBottomBoxSprite = new Rectangle(new Point(33, 33), new Point(10, 10));

            this.TopBoxSprite = new Rectangle(new Point(10, 0), new Point(10, 10));
            this.BottomBoxSprite = new Rectangle(new Point(10, 33), new Point(10, 10));

            this.LeftBoxSprite = new Rectangle(new Point(0, 10), new Point(10, 10));
            this.RightBoxSprite = new Rectangle(new Point(33, 10), new Point(10, 10));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawRectangle(spriteBatch);
        }
    }
}
