using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using UmbrellaToolKit;
using UmbrellaToolKit.UI;
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
        public Scene SceneUI;
        public Scene SceneMainMenu;
        public Scene CreditsScene;

        private int CurrentlyLevel;

        private bool _IntialCredits;
        public Credits Credits;

        // All Levels
        private List<Stage> Levels = new List<Stage>();

        public MouseManager Mouse;
        public ScreemController Screem;

        public World World;
        
        public Texture2D MouseWhite;
        public Texture2D MouseBlack;

        public SpriteFont FontBold;
        public SpriteFont FontRegular;
        
        ContentManager Content;

        public GameManager(ContentManager Content)
        {
            CurrentlyLevel = 2;
            this.Content = Content;
            this.CurrentlyStatus = GameStatus.PLAY;
            this.FontBold = Content.Load<SpriteFont>("Fonts/Quicksand-Bold");
            this.FontRegular = Content.Load<SpriteFont>("Fonts/Quicksand-Regular");

            this.World = new World();

            this.Mouse = new MouseManager();
            this.MouseWhite = Content.Load<Texture2D>("Sprites/UI/upLeft_white");
            this.MouseBlack = Content.Load<Texture2D>("Sprites/UI/upLeft");
            this.Mouse.Sprite = this.MouseWhite;
            this.Mouse.SetPointMouse(this.World);
            this.Mouse.Show = true;

            this.SetAllLevels(Content);

            this.SetCreditsScene();
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
            this.Levels.Add(new Gameplay.Level_01.Level());
            this.Levels.Add(new Gameplay.Level_02.Level());
            this.Levels.Add(new Gameplay.Level_03.Level());
        }

        public void StartLevel()
        {
            this.SceneLevel = this.Levels[this.CurrentlyLevel];
            this.SceneLevel.Start(Content, World, Mouse);
            this.SceneLevel.Screem = this.Screem;
        }

        public void Update(GameTime gameTime)
        {
            this.World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

            if (this.Levels[this.CurrentlyLevel].LevelReady == false) this.StartLevel();

            if (!this._IntialCredits)
            {
                this.Mouse.Show = true;
                switch (this.CurrentlyStatus)
                {
                    case GameStatus.PLAY:
                        this.SceneLevel.UpdateLevel(gameTime);
                        //change Color Mouse
                        if (this.SceneLevel.WhiteUI) this.Mouse.Sprite = this.MouseWhite;
                        else this.Mouse.Sprite = this.MouseBlack;

                        if (this.SceneLevel.Finished && this.CurrentlyLevel == 0) this._IntialCredits = true;
                        else if (this.SceneLevel.Finished && this.CurrentlyLevel < this.Levels.Count() - 1)
                        {
                            this.Levels[this.CurrentlyLevel].Destroy();
                            this.CurrentlyLevel++;
                            this.StartLevel();
                        }
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
                        break;
                    case GameStatus.PAUSE:
                        this.SceneLevel.DrawLevel(spriteBatch, graphicsDevice);
                        break;
                }
            }
            else this.CreditsScene.Draw(spriteBatch, graphicsDevice);
            //spriteBatch.End();

            this.Mouse.Draw(spriteBatch);
        }

    }
}
