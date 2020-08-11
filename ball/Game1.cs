using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UmbrellaToolKit;
using UmbrellaToolKit.Storage;
using UmbrellaToolKit.Localization;

namespace ball
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        UmbrellaToolKit.Storage.Load Storage;
        LocalizationDefinitions Location;

        private BasicEffect _spriteBatchEffect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            Storage = new Load();
        }
        
        protected override void Initialize()
        {
            base.Initialize();
        }

        private Managers.GameManager GameManager;
        private ScreemController ScreemController;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreemController = new ScreemController(graphics, graphics.GraphicsDevice.Adapter, GraphicsDevice, 0);

            // check game progress
            this.CheckFirstSettings();

            GameManager = new Managers.GameManager(Content, ScreemController, Storage, this);
            GameManager.Screem = ScreemController;
            
            _spriteBatchEffect = new BasicEffect(graphics.GraphicsDevice);
            _spriteBatchEffect.TextureEnabled = true;

            Location = Content.Load<LocalizationDefinitions>("Languages");
            
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Exit();

            this.GameManager.Update(gameTime);
            this.ScreemController.Update(gameTime);
            
            base.Update(gameTime);
        }

        public void CheckFirstSettings()
        {
            List<string> levels = this.Storage.getItemsString("Progress");
            if (levels.Count == 0)
                for(int i = 0; i < 8; i++) levels.Add("False");
                this.Storage.AddItemString("Progress", levels);

            if (this.Storage.getItemsString("Language").Count == 0)
            {
                List<string> language = new List<string>();
                language.Add("English");
                this.Storage.AddItemString("Language", language);
            }

            this.Storage.Save();
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            this.ScreemController.BeginDraw(GraphicsDevice, spriteBatch);
            this.GameManager.Draw(spriteBatch, GraphicsDevice);
            this.ScreemController.EndDraw(GraphicsDevice, spriteBatch);
            base.Draw(gameTime);
        }
    }
}
