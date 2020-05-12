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
using ball.Gameplay.MemoryGame;

namespace ball.Gameplay.Levels.Level_03
{
    public class Level : Stage
    {
        List<WhiteCircle> Circle = new List<WhiteCircle>();
        MemoryGame.MemoryGame memoryGame;

        public override void Start(ContentManager Content, World World, MouseManager mouse)
        {
            this.World = World;
            memoryGame = new MemoryGame.MemoryGame();
            memoryGame.WhiteCirclesList = new List<MemoryGameWhiteCircle>();
            for (int i = 0; i < 2; i++)
            {
                this.Circle.Add(new WhiteCircle());
                this.Circle[i].Id = i;
                this.Circle[i]._Mouse = mouse;
                this.Circle[i].Sprite = Content.Load<Texture2D>("Sprites/white_circle");
                this.Circle[i].BlackCircle = new GameObject();
                this.Circle[i].BlackCircle.Sprite = Content.Load<Texture2D>("Sprites/black_circle");
                this.Circle[i].Start(World);
                this.Players.Add(this.Circle[i]);
                memoryGame.WhiteCirclesList.Add(this.Circle[i]);
            }
            this.Players.Add(memoryGame);
            this.SetBackgroundColor = Color.White;
            this.LevelReady = true;
        }

        public override void Destroy()
        {
            for (int i = 0; i < this.Players.Count(); i++)
            {
                if(this.Players[i].CBody != null) this.World.Remove(this.Players[i].CBody);
            }
            this.Circle.Clear();
            this.Players.Clear();
            this.LevelReady = false;
        }

        public override void ResetLevel(ContentManager Content, World World, MouseManager mouse)
        {
            this.Finished = false;
            this.LevelReady = true;
        }

        public override void UpdateLevel(GameTime gameTime)
        {
            if (this.LevelReady)
            {
                for (int i = 0; i < 2; i++) this.Circle[i]._Screem = this.Screem;
                this.Update(gameTime);
                this.Finished = memoryGame.Finished;
            }
            
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.Draw(spriteBatch, graphicsDevice);
        }

    }

    public class WhiteCircle : MemoryGameWhiteCircle
    {

        public override void SetSequence()
        {
            // part 1
            this.Sequence.Add(new List<int>());
            this.Sequence[0].Add(0);
            this.Sequence[0].Add(1);
            // part 2
            this.Sequence.Add(new List<int>());
            this.Sequence[0].Add(0);
            this.Sequence[0].Add(0);
            this.Sequence[0].Add(1);
            this.Sequence[0].Add(0);
            // part 3
            this.Sequence.Add(new List<int>());
            this.Sequence[2].Add(1);
            this.Sequence[2].Add(0);
            this.Sequence[2].Add(1);
            this.Sequence[2].Add(0);
            this.Sequence[2].Add(1);
            this.Sequence[2].Add(1);
        }

        public override void DefaultPostion()
        {
            switch (this.Id)
            {
                case 0:
                    _position.X -= _position.X / 2f;
                    break;
                case 1:
                    _position.X += _position.X / 2f;
                    break;
            }
        }

        public override void SequenceAnimationUpdate()
        {
            switch (this.Id)
            {
                case 0:
                    if (this.Sequence[SequenceNumPart][SequenceNum] == 0) this.SetTransparent(true);
                    else this.SetTransparent(false);
                    break;
                case 1:
                    if (this.Sequence[SequenceNumPart][SequenceNum] == 1) this.SetTransparent(true);
                    else this.SetTransparent(false);
                    break;
            }
        }

        public override void InitialAnimationUpdate()
        {
            lock (getrandom)
            {
                int randomX = getrandom.Next(5);
                int randomY = getrandom.Next(5);


                _shakePosition = new Vector2(
                    (randomX * _shakeMagnitude) + _InitialPosition.X,
                    (randomY * _shakeMagnitude) + _InitialPosition.Y
                );

            }
        }

        public override void SeparationAnimationUpdade()
        {
            switch (this.Id)
            {
                case 0:
                    _positionAfterExposion.X = (((_position.X / 2f) / ((20 - _positionAfterExposionFrame) * 2f))) + (_position.X / 2f);
                    break;
                case 1:
                    _positionAfterExposion.X = ((_position.X * 2) - (_position.X / ((20 - _positionAfterExposionFrame) * 2f))) - (_position.X / 2f);
                    break;
            }
        }
    }
}
