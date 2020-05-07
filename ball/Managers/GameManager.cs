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
using tainicom.Aether.Physics2D.Diagnostics;

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

        private int CurrentlyLevel;

        // All Levels
        private List<Stage> Levels = new List<Stage>();

        public MouseManager Mouse;
        public ScreemController Screem;

        public World World;


        public Texture2D MouseWhite;
        public Texture2D MouseBlack;

        public GameManager(ContentManager Content)
        {
            CurrentlyLevel = 0;
            this.CurrentlyStatus = GameStatus.PLAY;

            this.World = new World();
            this.World.Gravity = new Vector2(0, 20);

            this.Mouse = new MouseManager();
            this.MouseWhite = Content.Load<Texture2D>("Sprites/UI/upLeft_white");
            this.MouseBlack = Content.Load<Texture2D>("Sprites/UI/upLeft");
            this.Mouse.Sprite = this.MouseWhite;
            this.Mouse.SetPointMouse(this.World);
            this.Mouse.Show = true;

            this.SetAllLevels(Content);
            this.SceneLevel = this.Levels[this.CurrentlyLevel];
        }

        public void SetAllLevels(ContentManager Content)
        {
            this.Levels.Add(new Gameplay.Level_01.Level(Content, this.World, this.Mouse));
        }

        public void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (this.CurrentlyStatus == GameStatus.PLAY)
            {
                this.SceneLevel.Screem = this.Screem;
                this.SceneLevel.UpdateLevel(gameTime);

                //change Color Mouse
                if (this.SceneLevel.WhiteUI) this.Mouse.Sprite = this.MouseWhite;
                else this.Mouse.Sprite = this.MouseBlack;
            }
            this.Mouse.Update(gameTime);

        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            if (this.CurrentlyStatus == GameStatus.PLAY || this.CurrentlyStatus == GameStatus.PAUSE) this.SceneLevel.DrawLevel(spriteBatch, graphicsDevice);
            //spriteBatch.End();

            this.Mouse.Draw(spriteBatch);
        }

    }
}
