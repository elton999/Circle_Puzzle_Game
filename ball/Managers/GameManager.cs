using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using UmbrellaToolKit;
using UmbrellaToolKit.UI;
using UmbrellaToolKit.Localization;
using tainicom.Aether.Physics2D.Dynamics;
using ball.Menu;
using ball.Gameplay;

namespace ball.Managers
{
    public class GameManager
    {
        public enum GameStatus {
            MAIN_MENU,
            SETTINGS_MENU,
            PLAY,
            PAUSE,
            GAMEOVER,
            RESERT,
            EXIT,
        }
        public GameStatus CurrentlyStatus;

        public Stage SceneLevel;
        public Hud SceneUI;
        public MainMenu SceneMainMenu;
        public Scene CreditsScene;

        public int CurrentlyLevel;

        private bool _IntialCredits;
        public Credits Credits;

        // All Levels
        private List<Stage> Levels = new List<Stage>();

        public MouseManager Mouse;
        public ScreemController Screem;

        public World World;
        public World WorldUIMainMenu;
        
        public Texture2D MouseWhite;
        public Texture2D MouseBlack;

        private SoundEffect MusicSoundTrack;
        public SoundEffectInstance soundInstance;

        public SpriteFont FontBold;
        public SpriteFont FontRegular;

        public UmbrellaToolKit.Storage.Load Storage;

        ContentManager Content;
        LocalizationDefinitions Localization;
        Game1 Game;
        

        public GameManager(ContentManager Content, ScreemController ScreemController, UmbrellaToolKit.Storage.Load Storage, Game1 Game)
        {
            this.Storage = Storage;
            CurrentlyLevel = 0;

            this.Content = Content;
            this.Screem = ScreemController;
            this.Game = Game;
            
            this.FontBold = Content.Load<SpriteFont>("Fonts/Quicksand-Bold");
            this.FontRegular = Content.Load<SpriteFont>("Fonts/Quicksand-Regular");
            this.Localization = Content.Load<LocalizationDefinitions>("Languages");

            this.World = new World();
            this.World.Gravity = new Vector2(0, 10);

            this.MusicSoundTrack = Content.Load<SoundEffect>("Sound/kalimba-relaxation-music-by-kevin-macleod-from-filmmusic-io");
            this.soundInstance = this.MusicSoundTrack.CreateInstance();
            this.soundInstance.Volume = 0.1f;
            this.soundInstance.IsLooped = true;

            this.Mouse = new MouseManager();
            this.MouseWhite = Content.Load<Texture2D>("Sprites/UI/upLeft_white");
            this.MouseBlack = Content.Load<Texture2D>("Sprites/UI/upLeft");
            this.Mouse.Sprite = this.MouseWhite;
            this.Mouse.SetPointMouse(this.World);
            this.Mouse.Show = true;

            this.WorldUIMainMenu = new World();
            this.WorldUIMainMenu.Gravity = Vector2.Zero;
            this.Mouse.SetPointMouse(WorldUIMainMenu);
            
            this.SetAllLevels(Content);
            this.SetCreditsScene();

            this.SceneMainMenu = new MainMenu();
            this.SceneMainMenu.Content = this.Content;
            this.SceneMainMenu.Font = this.FontRegular;
            this.SceneMainMenu.FontBold = this.FontBold;
            this.SceneMainMenu.Screem = this.Screem;
            this.SceneMainMenu.World = this.World;
            this.SceneMainMenu.Mouse = this.Mouse;
            this.SceneMainMenu.Screem = this.Screem;
            this.SceneMainMenu.Game = this.Game;
            this.SceneMainMenu.Localization = this.Localization;
            this.SceneMainMenu.Storage = this.Storage;
            this.SceneMainMenu.GameManager = this;

            List<bool> levels = this.Storage.getItemsBool("Progress");
            IEnumerable<bool> _levels_progress = from level in levels where level == true select level;
            if (_levels_progress.ToList<bool>().Count == 0)
                this.CurrentlyStatus = GameStatus.PLAY;
            else
            {
                this.SceneMainMenu.Start();
                this.CurrentlyStatus = GameStatus.MAIN_MENU;
            }

        }

        public void SetCreditsScene()
        {
            // set Credits Game Object
            this._IntialCredits = false;
            this.Credits = new Credits();
            this.Credits.FontBold = this.FontBold;
            this.Credits.FontRegular = this.FontRegular;
            this.Credits.Start();

            // set Scene
            this.CreditsScene = new Scene();
            this.CreditsScene.SetBackgroundColor = Color.White;
            this.CreditsScene.UI.Add(this.Credits);
            this.CreditsScene.LevelReady = true;
        }

        public void SetAllLevels(ContentManager Content)
        {
            this.Levels.Add(new Gameplay.Levels.Level_01.Level());
            this.Levels.Add(new Gameplay.Levels.Level_02.Level());
            this.Levels.Add(new Gameplay.Levels.Level_03.Level());
            this.Levels.Add(new Gameplay.Levels.Level_04.Level());
            this.Levels.Add(new Gameplay.Levels.Level_05.Level());
            this.Levels.Add(new Gameplay.Levels.Level_06.Level());
            this.Levels.Add(new Gameplay.Levels.Level_07.Level());
            this.Levels.Add(new Gameplay.Levels.Level_08.Level());
        }

        public void StartLevel()
        {
            if(this.CurrentlyLevel > 0)
                this.StartSound();
            if (this.SceneUI == null) {
                this.SceneUI = new Hud();
                this.SceneUI.World = this.WorldUIMainMenu;
                this.SceneUI.GameManager = this;
                this.SceneUI.Start(this.Content, this.Mouse, this.Screem);
            }
            
            this.SceneLevel = this.Levels[this.CurrentlyLevel];
            this.SceneLevel.Screem = this.Screem;
            this.SceneLevel.Storage = this.Storage;
            this.SceneLevel.FontBold = this.FontBold;
            this.SceneLevel.Start(Content, World, Mouse);
            
            this.SceneUI.SetLevel(this.SceneLevel);

            this.SaveProgress(this.CurrentlyLevel);
        }

        public void StartSound()
        {
            if (this.soundInstance.State == SoundState.Stopped)
                this.soundInstance.Play();
        }

        public void GoToMenu()
        {
            this.SceneUI.Home.RemoveFromScene = true;
            this.SceneUI.Home = null;
            this.SceneUI.HomeShow = false;

            this.SceneUI = null;
            this.SceneMainMenu.Start();

            this.Levels[this.CurrentlyLevel].Destroy();
            this.CurrentlyStatus = GameStatus.MAIN_MENU;
        }

        public void GoToCreditsArea()
        {
            this.GoToMenu();
            this.SceneMainMenu.CreateCreditsArea();
        }

        public void SaveProgress(int _level)
        {
            List<bool> _levels = this.Storage.getItemsBool("Progress");
            _levels[_level] = true;
            this.Storage.AddItemBool("Progress", _levels);
            this.Storage.Save();
        }

        public void Update(GameTime gameTime)
        {
            for(int i = 0; i < 4; i++) this.World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds, (1f / 30f)));
            
            if (!this._IntialCredits)
            {
                this.Mouse.Show = true;
                switch (this.CurrentlyStatus)
                {
                    case GameStatus.PLAY:
                        if (this.Levels[this.CurrentlyLevel].LevelReady == false) this.StartLevel();
                        this.SceneLevel.UpdateLevel(gameTime);
                        this.SceneUI.Update(gameTime);

                        //change Color Mouse
                        if (this.SceneLevel.WhiteUI) this.Mouse.Sprite = this.MouseWhite;
                        else this.Mouse.Sprite = this.MouseBlack;

                        if (this.SceneLevel.Finished && this.CurrentlyLevel == 0) this._IntialCredits = true;
                        else if (this.SceneLevel.Finished && this.CurrentlyLevel < this.Levels.Count() - 1)
                        {
                            this.Levels[this.CurrentlyLevel].Destroy();
                            this.CurrentlyLevel++;
                            this.StartLevel();
                        } else if (this.SceneLevel.Finished && this.CurrentlyLevel == this.Levels.Count() - 1)
                        {
                            this.GoToCreditsArea();
                        }
                        break;
                    case GameStatus.MAIN_MENU:
                        this.Mouse.Sprite = this.MouseBlack;
                        this.SceneMainMenu.Update(gameTime);
                        break;
                }
            }
            else
            {
                this.Mouse.Show = false;
                this.Credits.Screem = this.Screem;
                this.CreditsScene.Update(gameTime);
                if (this.Credits.Finished)
                {
                    this._IntialCredits = false;
                    this.SceneLevel.Destroy();
                    this.CurrentlyLevel++;
                    this.SceneLevel = this.Levels[this.CurrentlyLevel];
                    this.StartLevel();
                }
            }
            this.Mouse.Update(gameTime);

        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            //spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            if (!this._IntialCredits)
            {
                switch (this.CurrentlyStatus)
                {
                    case GameStatus.PLAY:
                        this.SceneLevel.DrawLevel(spriteBatch, graphicsDevice);
                        this.SceneUI.Draw(spriteBatch, graphicsDevice);
                        break;
                    case GameStatus.PAUSE:
                        this.SceneLevel.DrawLevel(spriteBatch, graphicsDevice);
                        this.SceneUI.Draw(spriteBatch, graphicsDevice);
                        break;
                    case GameStatus.MAIN_MENU:
                        this.SceneMainMenu.Draw(spriteBatch, graphicsDevice);
                        break;
                }
            }
            else this.CreditsScene.Draw(spriteBatch, graphicsDevice);
            //spriteBatch.End();

            this.Mouse.Draw(spriteBatch);
        }

    }
}
