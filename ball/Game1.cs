using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UmbrellaToolKit;
using UmbrellaToolKit.Storage;

namespace ball
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        UmbrellaToolKit.Storage.Load Storage;

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

            GameManager = new Managers.GameManager(Content, ScreemController, Storage);
            GameManager.Screem = ScreemController;
            
            _spriteBatchEffect = new BasicEffect(graphics.GraphicsDevice);
            _spriteBatchEffect.TextureEnabled = true;
            
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            this.GameManager.Update(gameTime);
            this.ScreemController.Update(gameTime);
            
            base.Update(gameTime);
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
