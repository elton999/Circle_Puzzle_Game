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
using UmbrellaToolKit.Storage;
using UmbrellaToolKit.Localization;

namespace ball.Menu
{
    public class MainMenu : Scene
    {
        public Load Storage;
        public SpriteFont Font;
        public SpriteFont FontBold;
        public List<MenuItem> MenuItems = new List<MenuItem>();
        public List<MenuItemLevel> LevelsSelect = new List<MenuItemLevel>();
        public string[] MenuItemsString = { "PLAY", "SETTINGS", "CREDITS", "QUIT" };
        public enum MenuItens { PLAY, SETTINGS, CREDITS, QUIT, NONE };
        public MenuItens ItemOver;
        public MouseManager Mouse;
        public Game1 Game;
        public LocalizationDefinitions Localization;

        public Managers.GameManager GameManager;

        public Hud Hud;
        public Title TitleMainMenu;
        public CreditsArea CreditsArea;
        public SelectLanguage SelectLanguage;

        public void Start()
        {
            this.Hud = new Hud();
            this.Hud.World = this.World;
            this.Hud.ResizeShow = false;
            this.Restart();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (this.Hud != null && this.LevelReady)  this.Hud.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            base.Draw(spriteBatch, graphicsDevice);
            if (this.Hud != null && this.LevelReady) this.Hud.Draw(spriteBatch, graphicsDevice);
        }
        
        public void CreateMenu()
        {
            //this.DestroyMenu();
            this.ItemOver = MenuItens.NONE;

            string lang = this.Storage.getItemsString("Language")[0];
            this.MenuItemsString[0] = this.Localization.Get(lang, "UI_MAIN_MENU_PLAY").ToUpper();
            this.MenuItemsString[1] = this.Localization.Get(lang, "UI_MAIN_MENU_SETTINGS").ToUpper();
            this.MenuItemsString[2] = this.Localization.Get(lang, "UI_MAIN_MENU_CREDITS").ToUpper();
            this.MenuItemsString[3] = this.Localization.Get(lang, "UI_MAIN_MENU_QUIT").ToUpper();
            
            this.Hud.ReloadShow = false;
            this.Hud.HomeShow = false;
            this.Hud.BackShow = false;
            this.Hud.ResizeShow = true;
            this.Hud.Start(this.Content, this.Mouse, this.Screem);

            this.TitleMainMenu = new Title();
            this.TitleMainMenu.FontBold = this.FontBold;
            this.TitleMainMenu.Screem = this.Screem;
            this.TitleMainMenu.Start();
            this.TitleMainMenu.Show = true;
            this.UI.Add(this.TitleMainMenu);

            for (int i = 0; i < 4; i++)
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
                MenuItem.MainMenu = this;
                MenuItem.Show = true;
                MenuItems.Add(MenuItem);
                this.UI.Add(MenuItem);
            }
        }
        
        public void CreateSelectLevels()
        {
            this.DestroyMenu();

            this.Hud.ReloadShow = false;
            this.Hud.HomeShow = false;
            this.Hud.BackShow = true;
            BackHudBtn backHudBtn = new BackHudBtn();
            backHudBtn.mainMenu = this;
            this.Hud.Back = backHudBtn;
            this.Hud.Start(this.Content, this.Mouse, this.Screem);

            List<bool> levels = this.Storage.getItemsBool("Progress");
            this.LevelsSelect = new List<MenuItemLevel>();

            for (int i = 0; i < 8; i++)
            {
                MenuItemLevel MenuItemLevel = new MenuItemLevel();
                MenuItemLevel.Value = "0" + (i + 1).ToString();
                MenuItemLevel.Id = i;
                MenuItemLevel.Content = this.Content;
                MenuItemLevel.Font = this.Font;
                MenuItemLevel._Mouse = this.Mouse;
                MenuItemLevel.World = this.World;
                MenuItemLevel.Unlock = levels[i];
                MenuItemLevel.GameManager = this.GameManager;
                MenuItemLevel.Show = true;

                float xCenter = this.Screem.getCenterScreem.X - ((5 * 130 / 2f));

                if (i < 5) MenuItemLevel.Position = new Vector2(xCenter + 60 + (i * 130), 150);
                else MenuItemLevel.Position = new Vector2(xCenter + 60 + ((i - 5) * 130), 280);

                MenuItemLevel.Start();
                MenuItemLevel.MainMenu = this;
                LevelsSelect.Add(MenuItemLevel);
                this.UI.Add(MenuItemLevel);
            }
        }

        public void CreateCreditsArea()
        {
            this.DestroyMenu();

            this.Hud.ReloadShow = false;
            this.Hud.HomeShow = false;
            this.Hud.BackShow = true;
            this.Hud.ResizeShow = true;
            BackHudBtn backHudBtn = new BackHudBtn();
            backHudBtn.mainMenu = this;
            this.Hud.Back = backHudBtn;
            this.Hud.Start(this.Content, this.Mouse, this.Screem);
            backHudBtn.CBody.Tag = "back";

            this.CreditsArea = new CreditsArea();
            this.CreditsArea.Localization = this.Localization;
            this.CreditsArea.Storage = this.Storage;
            this.CreditsArea.Screem = this.Screem;
            this.CreditsArea.Font = this.Font;
            this.CreditsArea.FontBold = this.FontBold;
            this.CreditsArea.Show = true;
            this.CreditsArea.Start();
            
            this.UI.Add(this.CreditsArea);
        }

        public void CreateLanguageSelect()
        {
            this.DestroyMenu();

            this.Hud.ReloadShow = false;
            this.Hud.HomeShow = false;
            this.Hud.BackShow = true;
            this.Hud.ResizeShow = true;
            BackHudBtn backHudBtn = new BackHudBtn();
            backHudBtn.mainMenu = this;
            this.Hud.Back = backHudBtn;
            this.Hud.Start(this.Content, this.Mouse, this.Screem);
            backHudBtn.CBody.Tag = "back";

            this.SelectLanguage = new SelectLanguage();
            this.SelectLanguage._Screem = this.Screem;
            this.SelectLanguage.World = this.World;
            this.SelectLanguage.Font = this.Font;
            this.SelectLanguage.Content = this.Content;
            this.SelectLanguage.Storage = this.Storage;
            this.SelectLanguage._Mouse = this.Mouse;
            this.SelectLanguage.MainMenu = this;
            this.SelectLanguage.Start();

            this.UI.Add(this.SelectLanguage);
        }

        
        public void Restart()
        {
            if (this.CheckLanguage())
                this.CreateMenu();
            else
                this.CreateLanguageSelect();

            this.SetBackgroundColor = Color.White;
            this.LevelReady = true;
        }

        public void DestroyHud()
        {
            if (this.Hud != null)
            {
                if (this.Hud.BackShow)
                {
                    this.Hud.Back.RemoveFromScene = true;
                    this.Hud.Back = null;
                    this.Hud.BackShow = false;
                }
            }
        }

        public void DestroyMenu()
        {
            this.DestroyHud();
            foreach (GameObject gameObject in this.UI)
                gameObject.RemoveFromScene = true;

            this.MenuItems.Clear();
        }

        public void DestroyLanguageSelect()
        {
            if (this.SelectLanguage != null)
            {
                this.DestroyHud();
                foreach (GameObject gameObject in this.UI)
                    gameObject.RemoveFromScene = true;
                this.SelectLanguage = null;
            }
        }

        public void DestroySelectLevel() {
            if (this.LevelsSelect.Count > 0)
            {
                this.DestroyHud();
                foreach (GameObject gameObject in this.UI)
                    gameObject.RemoveFromScene = true;
                this.LevelsSelect.Clear();
            }
        }

        public void DestroyAreas()
        {
            if (this.CreditsArea != null)
            {
                this.DestroyHud();
                this.CreditsArea.RemoveFromScene = true;
                this.CreditsArea = null;
            }
        }

        public void Destroy()
        {
            this.ItemOver = MenuItens.NONE;
            this.World.Clear();

            this.LevelReady = false;
        }

        public bool CheckLanguage()
        {
            List<string> _lang = this.Storage.getItemsString("Language");
            if (_lang.Count == 0)
                _lang.Add("English");
            this.Storage.AddItemString("Language", _lang);
            this.Storage.Save();

            if (_lang.Count == 0)
                return false;

            return true;
        }
    }

    public class BackHudBtn : BackBTN
    {
        public MainMenu mainMenu;
        public override void Click()
        {
            this.mainMenu.DestroyAreas();
            this.mainMenu.DestroyLanguageSelect();
            this.mainMenu.DestroySelectLevel();
            this.mainMenu.CreateMenu();
        }
    }

    public class MenuItem : GameObject
    {
        public int Id;
        public SpriteFont Font;
        public string Value;
        public bool Show = true;
        public MainMenu MainMenu;
        public ContentManager Content;
        private MenuItemBox MenuItemBox;

        public void Start()
        {
            this.SpriteColor = Color.Black;
            this.Origin = this.Font.MeasureString(this.Value) / 2f;
            this.MenuItemBox = new MenuItemBox();
            this.MenuItemBox.Position = new Vector2(this.Position.X - 105, this.Position.Y - 20);
            this.MenuItemBox.ContentSize = new Vector2(190, 20);
            this.MenuItemBox.Content = this.Content;
            this.MenuItemBox.Start();
            this._bodySize = new Vector2(210, 40);
            this.SetBoxCollision(this.World);
            this.CBody.SetTransform(new Vector2(this.Position.X -10, this.Position.Y - 15), this.Rotation);
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
            if (this.Show) { 
                if (this._MouseOver)
                {
                    this.MenuItemBox.Sprite = this.MenuItemBox.SpriteOver;
                    this.SpriteColor = Color.White;
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_click)
                    {
                        this._click = true;
                        switch (this.Id) {
                            case 0:
                                this.MainMenu.CreateSelectLevels();
                                this.MainMenu.TitleMainMenu.Show = false;
                                break;
                            case 1:
                                this.MainMenu.CreateLanguageSelect();
                                break;
                            case 2:
                                this.MainMenu.CreateCreditsArea();
                                break;
                            case 3:
                                this.MainMenu.Game.Exit();
                                break;
                        }
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
            }
            this._MouseOver = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this.Show) this.MenuItemBox.Draw(spriteBatch);
            if(this.Show) spriteBatch.DrawString(this.Font, this.Value, this.Position, this.SpriteColor*this.Transparent, this.Rotation, this.Origin, this.Scale, this.spriteEffect, 0f);
        }
    }


    public class Title : GameObject
    {
        public SpriteFont FontBold;
        public List<Vector2> AddicionalCreditsPosition = new List<Vector2>();
        public Vector2 TitleSize;
        public ScreemController Screem;

        public bool Show { get; set; }

        public void SetSizes()
        {
            this.TitleSize = this.FontBold.MeasureString("Circle");
            this.AddicionalCreditsPosition.Add(Vector2.One);
        }

        public override void Start()
        {
            this.SetSizes();
            this.Origin = new Vector2(this.TitleSize.X / 2f, this.TitleSize.Y / 2f);
            this.Position = new Vector2(this.Screem.getCenterScreem.X - this.Origin.X, 150f);
            this.SpriteColor = Color.Black;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.Show) spriteBatch.DrawString(this.FontBold, "Circle", this.Position, this.SpriteColor * this.Transparent);
        }
    }


    public class SelectLanguage : GameObject
    {
        public SpriteFont Font;
        public Language English;
        public Language Portugues;
        public MainMenu MainMenu;
        public Load Storage;

        public override void Start()
        {
            List<string> _langStorage = this.Storage.getItemsString("Language");
            string _lang = "";
            if (_langStorage.Count > 0)
                _lang = _langStorage[0];

            this.English = new Language();
            this.English.Font = this.Font;
            this.English.Content = this.Content;
            this.English.World = this.World;
            this.English.Value = "ENGLISH";
            this.English.Id = "English";
            this.English.Position = new Vector2(this._Screem.getCenterScreem.X - 140, this._Screem.getCenterScreem.Y);
            if (_lang == "English") this.English.Checked = true;
            this.English._Mouse = this._Mouse;
            this.English.Start();

            this.Portugues = new Language();
            this.Portugues.Font = this.Font;
            this.Portugues.Content = this.Content;
            this.Portugues.World = this.World;
            this.Portugues.Value = "PORTUGUES";
            this.Portugues.Id = "Portugues";
            this.Portugues.Position = new Vector2(this._Screem.getCenterScreem.X + 140, this._Screem.getCenterScreem.Y);
            if (_lang == "Portugues") this.Portugues.Checked = true;
            this.Portugues._Mouse = this._Mouse;
            this.Portugues.Start();

            this.MainMenu.UI.Add(this.English);
            this.MainMenu.UI.Add(this.Portugues);
        }

        public bool _isSaving = false;
        public override void Update(GameTime gameTime)
        {

            if((this.English._click || this.Portugues._click) && !this._isSaving)
            {
                this._isSaving = true;
                List<string> _lang = new List<string>();

                if (this.English._click)
                {
                    this.English.Checked = true;
                    this.Portugues.Checked = false;
                    _lang.Add("English");
                } else if(this.Portugues._click)
                {
                    this.English.Checked = false;
                    this.Portugues.Checked = true;
                    _lang.Add("Portugues");
                }

                this.Storage.AddItemString("Language", _lang);
                this.Storage.Save();
                this._isSaving = false;
            }
        }
    }

    public class Language : BoxSprite
    {
        public string Id;
        public SpriteFont Font;
        public string Value;
        public MainMenu MainMenu;
        public MenuItemBox MenuItemBox;
        public bool Checked;

        public void Start()
        {
            this.SpriteColor = Color.Black;
            this.Origin = this.Font.MeasureString(this.Value) / 2f;
            this.MenuItemBox = new MenuItemBox();
            this.MenuItemBox.Position = new Vector2(this.Position.X - 80, this.Position.Y - 20);
            this.MenuItemBox.ContentSize = new Vector2(140, 20);
            this.MenuItemBox.Content = this.Content;
            this.MenuItemBox.Start();
            this._bodySize = new Vector2(120, 120);
            this.SetBoxCollision(this.World);
            this.CBody.SetTransform(new Vector2(this.Position.X - 10, this.Position.Y - 15), this.Rotation);
            this.CBody.Tag = this.Id;
        }

        bool _MouseOver;
        public override void OnMouseOver()
        {
            this._MouseOver = true;
        }

        public bool _click;
        public override void Update(GameTime gameTime)
        {
            this.MenuItemBox.Transparent = this.Transparent;
            if (this.Checked)
            {
                this.MenuItemBox.Sprite = this.MenuItemBox.SpriteOver;
                this.SpriteColor = Color.White;
            }
            else
            {
                this.MenuItemBox.Sprite = this.MenuItemBox.SpriteBorder;
                this.SpriteColor = Color.Black;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_click && this._MouseOver)
            {
                this._click = true;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released && _click)
            {
                this._click = false;
            }

            this._MouseOver = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.MenuItemBox.Draw(spriteBatch);
            spriteBatch.DrawString(this.Font, this.Value, this.Position, this.SpriteColor * this.Transparent, this.Rotation, this.Origin, this.Scale, this.spriteEffect, 0f);
        }

    }

    public class MenuItemLevel : BoxSprite
    {
        public int Id;
        public SpriteFont Font;
        public string Value;
        public bool Show;
        public MainMenu MainMenu;
        public ContentManager Content;
        public bool Unlock;
        public Managers.GameManager GameManager;
        private MenuItemLevelBox MenuItemBox;

        public void Start()
        {
            this.SpriteColor = Color.Black;
            this.Origin = this.Font.MeasureString(this.Value) / 2f;
            this.MenuItemBox = new MenuItemLevelBox();
            this.MenuItemBox.Position = new Vector2(this.Position.X - 60, this.Position.Y - 60);
            this.MenuItemBox.ContentSize = new Vector2(100, 100);
            this.MenuItemBox.Content = this.Content;
            this.MenuItemBox.Start();
            this._bodySize = new Vector2(120, 120);
            this.SetBoxCollision(this.World);
            this.CBody.SetTransform(new Vector2(this.Position.X - 10, this.Position.Y - 15), this.Rotation);
            this.CBody.Tag = "Level_" + this.Id;
            if (!this.Unlock) this.Transparent = 0.5f;
        }

        bool _MouseOver;
        public override void OnMouseOver()
        {
            this._MouseOver = true;
        }

        bool _click;
        public override void Update(GameTime gameTime)
        {
            if (this.Show)
            {
                this.MenuItemBox.Transparent = this.Transparent;
                if (this._MouseOver)
                {
                    this.MenuItemBox.Sprite = this.MenuItemBox.SpriteOver;
                    this.SpriteColor = Color.White;
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_click)
                    {
                        this._click = true;
                        if (this.Unlock)
                        {
                            this.MainMenu.DestroySelectLevel();
                            this.GameManager.CurrentlyLevel = this.Id;
                            this.GameManager.StartLevel();
                            this.GameManager.CurrentlyStatus = Managers.GameManager.GameStatus.PLAY;
                        }
                    }
                    else if (Mouse.GetState().LeftButton == ButtonState.Released && _click)
                    {
                        this._click = false;
                    }
                }
                else
                {
                    this.MenuItemBox.Sprite = this.MenuItemBox.SpriteBorder;
                    this.SpriteColor = Color.Black;
                }
            }

            this._MouseOver = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this.Show) this.MenuItemBox.Draw(spriteBatch);
            if(this.Show) spriteBatch.DrawString(this.Font, this.Value, this.Position, this.SpriteColor * this.Transparent, this.Rotation, this.Origin, this.Scale, this.spriteEffect, 0f);
        }
    }

    public class CreditsArea : GameObject
    {
        public SpriteFont FontBold;
        public SpriteFont Font;
        public string[] CreditsString = {
            "CREDITS_UI_PRODUCTED_BY",
            "Elton Silva",
            "CREDITS_UI_MUSIC",
            "music credits",
            "CREDITS_UI_SPECIAL_THANKS",
            "Mom    &    Dad",
            "Mayra Carvalho",
            "Pedro Souza",
            "Josue Côrtes",
            "Gustavo Albuquerque",
            "CREDITS_UI_THANKS",
        };
        public List<Vector2> CreditsPostion = new List<Vector2>();
        public Vector2 TitleSize;
        public ScreemController Screem;
        public LocalizationDefinitions Localization;
        public Load Storage;

        public bool Show { get; set; }

        public void SetSizes()
        {
            this.TitleSize = this.FontBold.MeasureString("Circle");
            this.CreditsPostion.Clear();
            int i = 1;
            foreach (string Credit in CreditsString)
            {
                Vector2 size = this.Font.MeasureString(Credit);

                if(i == 1) CreditsPostion.Add(new Vector2(this.Screem.getCenterScreem.X - (size.X / 2f),  280));
                else if(i == 3  || i == 5 || i == 11) CreditsPostion.Add(new Vector2(this.Screem.getCenterScreem.X - (size.X / 2f), CreditsPostion[CreditsPostion.Count - 1].Y + 50 ) );
                else CreditsPostion.Add(new Vector2(this.Screem.getCenterScreem.X - (size.X / 2f), CreditsPostion[CreditsPostion.Count - 1].Y + 30));
                i++;
            }
        }

        public override void Start()
        {
            string lang = this.Storage.getItemsString("Language")[0];
            this.CreditsString[0] = this.Localization.Get(lang, "CREDITS_UI_PRODUCTED_BY").ToUpper();
            this.CreditsString[2] = this.Localization.Get(lang, "CREDITS_UI_MUSIC").ToUpper();
            this.CreditsString[4] = this.Localization.Get(lang, "CREDITS_UI_SPECIAL_THANKS").ToUpper();
            this.CreditsString[10] = this.Localization.Get(lang, "CREDITS_UI_THANKS").ToUpper();

            this.SetSizes();
            this.Origin = new Vector2(this.TitleSize.X / 2f, this.TitleSize.Y / 2f);
            this.Position = new Vector2(this.Screem.getCenterScreem.X - this.Origin.X, 150f);
            this.SpriteColor = Color.Black;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.Show)
            {
                spriteBatch.DrawString(this.FontBold, "Circle", this.Position, this.SpriteColor * this.Transparent);

                spriteBatch.DrawString(this.Font, this.CreditsString[0], CreditsPostion[0], Color.Gray * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[1], CreditsPostion[1], this.SpriteColor * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[2], CreditsPostion[2], Color.Gray * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[3], CreditsPostion[3], this.SpriteColor * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[4], CreditsPostion[4], Color.Gray * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[5], CreditsPostion[5], this.SpriteColor * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[6], CreditsPostion[6], this.SpriteColor * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[7], CreditsPostion[7], this.SpriteColor * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[8], CreditsPostion[8], this.SpriteColor * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[9], CreditsPostion[9], this.SpriteColor * this.Transparent);
                spriteBatch.DrawString(this.Font, this.CreditsString[10], CreditsPostion[10], this.SpriteColor * this.Transparent);
            }
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


    public class MenuItemLevelBox : BoxSprite
    {
        public ContentManager Content;
        public Texture2D SpriteBorder;
        public Texture2D SpriteOver;

        public void Start()
        {
            this.SpriteOver = Content.Load<Texture2D>("Sprites/UI/menu_item_over");
            this.SpriteBorder = Content.Load<Texture2D>("Sprites/UI/menu_item");

            this.Sprite = this.SpriteBorder;

            this.ContentBoxSprite = new Rectangle(new Point(10, 10), new Point(10, 10));

            this.LeftTopBoxSprite = new Rectangle(new Point(0, 0), new Point(10, 10));
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
