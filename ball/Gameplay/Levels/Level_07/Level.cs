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

namespace ball.Gameplay.Levels.Level_07
{
    public class Level : Stage
    {

        List<Number> Numbers;
        List<Btn> Btns;
        int[] CorrentSequence = { 3, 9, 1, 8, 3, 1, 2, 5 };

        public override void Start(ContentManager Content, World World, MouseManager mouse)
        {
            this.ResetLevel(Content, World, mouse);
            this.LevelReady = true;
        }

        public override void ResetLevel(ContentManager Content, World World, MouseManager mouse)
        {
            this.Content = Content;
            this.World = World;
            this.Mouse = mouse;
            Numbers = new List<Number>();
            Btns = new List<Btn>();
            float width = 0;

            for (int i = 0; i < 8; i++)
            {
                Number number = new Number();
                number.Font = this.FontBold;
               
                number.Start();
                if(i != 3 && i != 6) width += (60 + number.Origin.X);
                else width += (25 + number.Origin.X);
                this.Numbers.Add(number);
            }

            float newWidth = this.Screem.getCenterScreem.X - (width / 2f);
            for (int i = 0; i < 8; i++)
            {
                if (i != 3 && i != 6) newWidth += 60 + this.Numbers[i].Origin.X;
                else newWidth += 25 + this.Numbers[i].Origin.X;

                this.Numbers[i].Position = new Vector2(newWidth, this.Screem.getCenterScreem.Y);
                this.Players.Add(this.Numbers[i]);
                
                Btn BtnUp = new Btn();
                BtnUp.Content = Content;
                BtnUp.World = World;
                BtnUp._Mouse = mouse;
                BtnUp.Increment = true;
                BtnUp.Start();
                BtnUp.CBody.Position = new Vector2(newWidth, this.Screem.getCenterScreem.Y - this.Numbers[i].Origin.Y);
                BtnUp.Number = this.Numbers[i];

                Btns.Add(BtnUp);
                this.Players.Add(BtnUp);

                Btn BtnDown = new Btn();
                BtnDown.Content = Content;
                BtnDown.World = World;
                BtnDown._Mouse = mouse;
                BtnDown.Increment = false;
                BtnDown.Start();
                BtnDown.CBody.Position = new Vector2(newWidth, this.Screem.getCenterScreem.Y + this.Numbers[i].Origin.Y);
                BtnDown.Number = this.Numbers[i];

                Btns.Add(BtnDown);
                this.Players.Add(BtnDown);
            }
        }

        public override void Destroy()
        {
        }

        bool hasJusFinished = false;
        float _time = 0;
        public override void UpdateLevel(GameTime gameTime)
        {
            hasJusFinished = true;
            for (int i = 0; i < this.Numbers.Count(); i++)
            {
                if (this.Numbers[i].Value != CorrentSequence[i]) hasJusFinished = false;
            }

            if (hasJusFinished)
            {
                _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_time > 2f) this.Finished = true;
                for (int i = 0; i < this.Numbers.Count(); i++) this.Numbers[i].CanChangeValue = false;
            }

            this.Update(gameTime);
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }

    }

    public class Number : GameObject
    {
        public int Value = 0;
        public SpriteFont Font;
        public bool CanChangeValue = true;
        
        public void Start()
        {
            this.MeasureString();
        }


        public void MeasureString()
        {
            Vector2 size = Font.MeasureString(this.Value.ToString());
            this.Origin = new Vector2(size.X / 2f, size.Y / 2f);
        }

        public void Increment()
        {
            if (CanChangeValue)
            {
                if (this.Value < 9) this.Value += 1;
                else this.Value = 0;
                this.MeasureString();
            }
        }

        public void Decrement()
        {
            if (CanChangeValue)
            {
                if (this.Value > 0) this.Value -= 1;
                else this.Value = 9;
                this.MeasureString();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.Font, this.Value.ToString(), this.Position, Color.Black*this.Transparent, this.Rotation, this.Origin, this.Scale, SpriteEffects.None, 0);
        }
    }

    public class Btn : GameObject {
        public Number Number;
        public ContentManager Content;
        public bool Increment;

        public void Start()
        {
            if (this.Increment) this.Sprite = Content.Load<Texture2D>("Sprites/Up");
            else this.Sprite = Content.Load<Texture2D>("Sprites/Down");

            this._bodySize = new Vector2(this.Sprite.Height, this.Sprite.Width);
            this.TextureSize = this._bodySize;

            this.SetBoxCollision(this.World);
        }

        private bool _pressed = false;
        public override void Update(GameTime gameTime)
        {
            if (_isMouseOver && Mouse.GetState().LeftButton == ButtonState.Pressed && !this._pressed)
            {
                this._pressed = true;
                if (this.Increment) this.Number.Increment();
                else this.Number.Decrement();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released) this._pressed = false;

            this._isMouseOver = false;
        }

        private bool _isMouseOver;
        public override void OnMouseOver()
        {
            this._isMouseOver = true;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            this.DrawSprite(spriteBatch);
        }
    }
}
