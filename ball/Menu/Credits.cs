using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UmbrellaToolKit;

namespace ball.Menu
{
    public class Credits : GameObject
    {
        public SpriteFont FontBold;
        public SpriteFont FontRegular;
        public ScreemController Screem;
        
        public float AddicionalCreditsTransparence;

        public Vector2 TitleSize;
        public List<Vector2> AddicionalCreditsPosition = new List<Vector2>();
        public List<Vector2> AddicionalCredits = new List<Vector2>();
        
        public void Start()
        {
            this.SetSizes();
            this.Transparent = 0;
            this.AddicionalCreditsTransparence = 0;
            this.SpriteColor = Color.Black;
        }

        public void SetSizes()
        {
            this.TitleSize = this.FontBold.MeasureString("Ball");
            this.AddicionalCredits.Add(this.FontRegular.MeasureString("a game by Elton Silva"));
            this.AddicionalCreditsPosition.Add(Vector2.One);
        }

        public bool Finished;

        private float _time;
        public override void Update(GameTime gameTime)
        {
            Vector2 _center = new Vector2(this.Screem.getCenterScreem.X, this.Screem.getCenterScreem.Y);

            this.Position.X = _center.X - (this.TitleSize.Y / 2f);
            this.Position.Y = _center.Y - (this.TitleSize.X / 2f);

            this.AddicionalCreditsPosition[0] = new Vector2(_center.X - (this.AddicionalCredits[0].X / 2f) + 18f, _center.Y - (this.AddicionalCredits[0].Y / 2f) + 60f);

            _time += (float)gameTime.TotalGameTime.TotalSeconds;
            if (this.Transparent < 1f && _time % 0.2f >= 0.032f)
            {
                this.Transparent += 0.01f;
                _time = 0;
            } else if (this.AddicionalCreditsTransparence < 1f &&  _time % 0.2f >= 0.032f)
            {
                this.AddicionalCreditsTransparence += 0.01f;
                _time = 0;
            } else if (this.AddicionalCreditsTransparence >= 1f && this.Transparent >= 1f)
            {
                if (_time % 0.2f >= (0.032f))
                {
                    this.Finished = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.FontBold, "Ball", this.Position, this.SpriteColor * this.Transparent);
            spriteBatch.DrawString(this.FontRegular, "a game by Elton Silva", this.AddicionalCreditsPosition[0], this.SpriteColor * this.AddicionalCreditsTransparence);
        }
    }
}
