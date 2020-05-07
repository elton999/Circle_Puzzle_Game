using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UmbrellaToolKit;

namespace ball
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private BasicEffect _spriteBatchEffect;
        // Simple camera controls
        private Vector3 _cameraPosition = new Vector3(0, 1.70f, 0); // camera is 1.7 meters above the ground
        float cameraViewWidth = 12.5f; // camera is 12.5 meters wide.

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
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
            GameManager = new Managers.GameManager(Content);
            ScreemController = new ScreemController(graphics, graphics.GraphicsDevice.Adapter, GraphicsDevice, 0);
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

            var vp = GraphicsDevice.Viewport;
            _spriteBatchEffect.View = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + Vector3.Forward, Vector3.Up);
            _spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0f, -1f);

            //GraphicsDevice.Clear(Color.CornflowerBlue);
            this.ScreemController.BeginDraw(GraphicsDevice, spriteBatch);
            this.GameManager.Draw(spriteBatch, GraphicsDevice);
            this.ScreemController.EndDraw(GraphicsDevice, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
