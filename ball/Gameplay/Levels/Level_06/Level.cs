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
        String[] CorrentSequence = { "C", "I", "R", "C", "L", "E" };
        List<Box> Boxs;

        public override void Start(ContentManager Content, World World, MouseManager Mouse)
        {
            this.Content = Content;
            this.World = World;
            this.Mouse = Mouse;

            this.ResetLevel();
        }

        public override void ResetLevel()
        {
            Boxs = new List<Box>();
            float width = 0;

            for (int i = 0; i < 6; i++)
            {
                Box box = new Box();
                box.World = this.World;
                box._Mouse = this.Mouse;
                box._Screem = this.Screem;
                box.Content = this.Content;
                box.Font = this.FontBold;
                box.Id = i;
                box.Start();

                if(i != 0)width += (10 + box._bodySize.X);

                this.Boxs.Add(box);
                this.Players.Add(box);
            }

            float newWidth = this.Screem.getCenterScreem.X - (width / 2f);

            for (int i = 0; i < 6; i++)
            {
                if (i != 0)  newWidth += (10 + this.Boxs[i]._bodySize.X);
                this.Boxs[i].CBody.Position = new Vector2(newWidth, this.Screem.getCenterScreem.Y);
            }

            this.SetBackgroundColor = Color.White;
            this.LevelReady = true;
            this.Finished = false;
        }

        public override void Destroy()
        {
            for (int i = 0; i < this.Boxs.Count(); i++)
            {
                this.World.Remove(this.Boxs[i].CBody);
            }
            this.Boxs.Clear();
            this.Players.Clear();

            this.LevelReady = false;
            this.Finished = false;
        }

        bool HaveFinished = false;
        float _time;
        public override void UpdateLevel(GameTime gameTime)
        {
            HaveFinished = true;

            for (int i = 0; i < this.Boxs.Count(); i++)
            {
                if (this.Boxs[i].Letters[this.Boxs[i].Value] != this.CorrentSequence[i]) HaveFinished = false;
            }

            if (HaveFinished)
            {
                _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_time > 2f)
                {
                    this.Finished = true;
                }
                for (int i = 0; i < this.Boxs.Count(); i++)
                {
                    this.Boxs[i].CanChange = false;
                }
            }

            this.Update(gameTime);
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }

    }

    public class Box : GameObject
    {
        public ContentManager Content;
        public String[] Letters = { "A", "I", "C", "L", "O", "E", "R" };
        public int Value = 0;
        public int Id;
        public SpriteFont Font;
        public Vector2 FontPosition;
        public Vector2 FontOrigin;

        public void Start()
        {
            this.Sprite = this.Content.Load<Texture2D>("Sprites/Level_8/Box");
            this._bodySize = new Vector2(this.Sprite.Width, this.Sprite.Height);
            this.SetBoxCollision(this.World);
            this.Origin = this._bodySize / 2f;
            this.CBody.Tag = "box_"+this.Id.ToString();
            this.MeasureString();
        }

        public bool CanChange = true;

        public void MeasureString()
        {
            Vector2 size = Font.MeasureString(this.Letters[this.Value]);
            this.FontOrigin = new Vector2(size.X / 2f, size.Y / 2f);
        }

        public void Change()
        {
            if (Letters.Length > this.Value + 1) this.Value++;
            else this.Value = 0;
            this.MeasureString();
        }

        bool _pressed = false;
        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_pressed && this._mouseOver && CanChange)
            {
                this._pressed = true;
                this.Change();
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released) this._pressed = false;

            this.FontPosition = this.CBody.Position;

            this._mouseOver = false;
        }

        bool _mouseOver;
        public override void OnMouseOver()
        {
            this._mouseOver = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
            spriteBatch.DrawString(this.Font, this.Letters[this.Value], this.FontPosition, Color.Black, 0f, this.FontOrigin, this.Scale, SpriteEffects.None, 0f);
        }
    }
}
