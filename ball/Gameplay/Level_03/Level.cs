using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using UmbrellaToolKit;
using UmbrellaToolKit.UI;
using tainicom.Aether.Physics2D.Dynamics;


namespace ball.Gameplay.Level_03
{
    public class Level : Stage
    {
        List<WhiteCircle> Circle = new List<WhiteCircle>();
        MemoryGame memoryGame;

        public override void Start(ContentManager Content, World World, MouseManager mouse)
        {
            this.World = World;
            memoryGame = new MemoryGame();
            memoryGame.WhiteCirclesList = new List<WhiteCircle>();
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
                this.World.Remove(this.Players[i].CBody);
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

    public class WhiteCircle : GameObject
    {
        public GameObject BlackCircle;
        public int Id;
        public void Start(World world)
        {
            this.Radius = this.Sprite.Width / 2f;
            this.Origin = new Vector2(this.Sprite.Width / 2f, this.Sprite.Height / 2f);
            this.SetCircleCollision(world);
            this.CBody.Tag = "Circle_level_3";
            this.CBody.IgnoreGravity = true;
            this.CBody.BodyType = BodyType.Dynamic;
            this.TextureSize = new Vector2(this.Sprite.Width, this.Sprite.Height);

            // set black circle
            this.BlackCircle.Radius = this.BlackCircle.Sprite.Width / 2f;
            this.BlackCircle.Origin = new Vector2(this.BlackCircle.Sprite.Width / 2f, this.Sprite.Height / 2f);
            this.SetSequence();
        }
        
        bool MouseClick = false;
        public bool MouseClickSequence = false;
        bool _mouseOver = false;
        public override void OnMouseOver()
        {
            this._mouseOver = true;
        }

        // initial animation
        private float _time;
        private float _timeInitialAnimation;
        private bool _ignoreGravity = true;
        private bool _animation = true;
        private bool _explosionAnimation = false;
        private int _timeExeposionAnimation;
        

        private Vector2 _InitialPosition;
        private Vector2 _shakePosition;
        private Vector2 _positionAfterExposion;
        private float _positionAfterExposionFrame = 20;
        private float _shakeMagnitude = 2f;
        private static readonly Random getrandom = new Random();


        // gameplay
        public enum GameStatus { NONE, SEQUENCE, PLAY, LOSE, WIN }
        public GameStatus _CurrentStatus;

        public override void Update(GameTime gameTime)
        {
            Vector2 _position = this._Screem.getCenterScreem;
            if (_InitialPosition == Vector2.Zero) _InitialPosition = this._Screem.getCenterScreem;

            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeInitialAnimation += (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseClickSequence = false;

            #region initial animation
            if (_animation)
            {
                if (_time % 0.2f >= 0.032f && _timeExeposionAnimation != 4)
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
                    _time = 0;
                }

                if (_timeExeposionAnimation != 4) this.CBody.SetTransformIgnoreContacts(ref _shakePosition, 0);

                if (_timeInitialAnimation % 2f >= (0.032f * 15) && !_explosionAnimation && _timeExeposionAnimation == 0)
                {
                    _shakeMagnitude += 0.5f;
                    _timeInitialAnimation = 0;
                    if (_shakeMagnitude > 5)
                    {
                        this.BlackCircle.Scale = 2;
                        _explosionAnimation = true;
                        _shakeMagnitude += 10f;
                    }
                } else if (_timeInitialAnimation % 2f >= (0.032f * 15) && _explosionAnimation && _timeExeposionAnimation != 4)
                {
                    _timeExeposionAnimation++;
                    _timeInitialAnimation = 0;
                    if (_timeExeposionAnimation == 4) _positionAfterExposion = _position;
                }

                if (_timeExeposionAnimation == 4 && _timeInitialAnimation % 2f >= 0.032f && _positionAfterExposionFrame != 0)
                {
                    this.BlackCircle.Scale = 1;
                    
                    
                    switch (this.Id)
                    {
                        case 0:
                            _positionAfterExposion.X = (((_position.X /2f) / ((20 -_positionAfterExposionFrame) * 2f))) + (_position.X / 2f);
                            break;
                        case 1:
                            _positionAfterExposion.X = ((_position.X * 2) - (_position.X / ((20 - _positionAfterExposionFrame) * 2f))) - (_position.X / 2f);
                            break;
                    }

                    _positionAfterExposionFrame--;
                    _timeInitialAnimation = 0;

                    if (_positionAfterExposionFrame == 0) this._animation = false;
                }
                if(this._positionAfterExposion != Vector2.Zero) this.CBody.SetTransformIgnoreContacts(ref _positionAfterExposion, 0);


            }
            #endregion

            #region gameplay
            else
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


                if(_time % 0.2f >= (0.032f * 3) && _CurrentStatus == GameStatus.NONE)
                {
                    _time = 0;
                    _CurrentStatus = GameStatus.LOSE;
                }


                if (_CurrentStatus == GameStatus.SEQUENCE)
                {
                    if(_time % 2f >= 0.032f)
                    {
                        switch (this.Id)
                        {
                            case 0:
                                if (this.Sequence[SequenceNumPart][SequenceNum] == 0) this.SetTransparent(true);
                                else this.SetTransparent(false);
                                break;
                            case 1:
                                if (this.Sequence[SequenceNumPart][SequenceNum] == 1)this.SetTransparent(true);
                                else this.SetTransparent(false);
                                break;
                        }
                        _time = 0;
                    }
                    this.NextStep(gameTime);
                }
                else if (_CurrentStatus == GameStatus.LOSE) this.SightLose(gameTime);
                else if (_CurrentStatus == GameStatus.PLAY)
                {
                    _SightWinWaitTime = 0;

                    // validate click
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !MouseClick && _mouseOver)
                    {
                        MouseClickSequence = true;
                        MouseClick = true;
                        this.Transparent = 0;
                    } else if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        MouseClick = false;
                        this.Transparent = 1;
                    }
                }
                else if (_CurrentStatus == GameStatus.WIN)
                {
                    this.SightWin(gameTime);
                }
                #endregion

                this.CBody.SetTransform(ref _position, 0f);
            }

            this.BlackCircle.Position = this.CBody.Position;
            this._mouseOver = false;
        }

        private float _transparentValue = 1f;
        private void SetTransparent(bool more)
        {
            if (!_IsWaiting)
            {
                this._transparentValue -= 0.2f;
                if (this.Transparent > 0 && more) this.Transparent -= 0.2f;
                else if (this.Transparent < 1f && !more) this.Transparent += 0.2f;
            }
        }

        // step wait
        private float _NextSepWait;
        private float _NextSepWaitingBetween;
        private bool _IsWaiting;
        private void NextStep(GameTime gameTime)
        {
            if (this._transparentValue <= 0 && SequenceNum < this.CurrentSequence.Count())
            {
                _IsWaiting = true;
                _NextSepWait += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _NextSepWaitingBetween += (float)gameTime.ElapsedGameTime.TotalSeconds;

                
                if (_NextSepWait > 2f)
                {
                    this._transparentValue = 1f;
                    _NextSepWait = 0;
                    SequenceNum++;
                    this.Transparent = 1f;
                    _IsWaiting = false;

                    if (SequenceNum == this.Sequence[SequenceNumPart].Count())
                    {
                        SequenceNum = 0;
                        _CurrentStatus = GameStatus.PLAY;
                    }
                }
                if (_NextSepWaitingBetween > 1f)
                {
                    this.Transparent = 1f;
                    _NextSepWaitingBetween = 0;
                }

            }
            //else if (this.Transparent == 0 && SequenceNum == this.Sequence[SequenceNumPart].Count()) _CurrentStatus = GameStatus.PLAY;
        }

        // lose or start
        private float _SightLoseWait;
        private bool _IsSightLose;
        private void SightLose(GameTime gameTime)
        {
            if(!_IsSightLose) this.Transparent = 0f;
            _SightLoseWait += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_SightLoseWait > 1f && !_IsSightLose)
            {
                this.Transparent = 1f;
                _SightLoseWait = 0;
                _IsSightLose = true;
            }
            if(_SightLoseWait > 1f && _IsSightLose)
            {
                _IsSightLose = false;
                _SightLoseWait = 0;
                this.Transparent = 1f;
                SequenceNum = 0;
                _CurrentStatus = GameStatus.SEQUENCE;
            }
        }


        // win sight
        private float _SightWinWait;
        private int _SightWinWaitTime;
        public bool Finished;
        private void SightWin(GameTime gameTime)
        {
            if (_SightWinWaitTime == 10)
            {
                _SightWinWaitTime = 0;
                _CurrentStatus = GameStatus.SEQUENCE;
                SequenceNumPart++;
                if (SequenceNumPart < this.Sequence.Count()) _SightWinWaitTime++;
                else Finished = true;
            }

            _SightWinWait += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_SightWinWait > 0.3f && _SightWinWaitTime < 10)
            {
                if (this.Transparent == 1f) this.Transparent = 0;
                else this.Transparent = 1f;
                _SightWinWaitTime++;
                _SightWinWait = 0;
            }
        }

        public List<List<int>> Sequence = new List<List<int>>();
        public int SequenceNum = 0;
        public int SequenceNumPart = 0;
        public void SetSequence()
        {
            // part 1
            this.Sequence.Add(new List<int>());
            this.Sequence[0].Add(0);
            this.Sequence[0].Add(0);
            this.Sequence[0].Add(1);
            this.Sequence[0].Add(0);
            // part 2
            this.Sequence.Add(new List<int>());
            this.Sequence[1].Add(1);
            this.Sequence[1].Add(0);
            this.Sequence[1].Add(1);
            this.Sequence[1].Add(0);
            this.Sequence[1].Add(1);
            this.Sequence[1].Add(1);
            // part 3
            this.Sequence.Add(new List<int>());
            this.Sequence[2].Add(1);
            this.Sequence[2].Add(1);
            this.Sequence[2].Add(0);
            this.Sequence[2].Add(1);
            this.Sequence[2].Add(0);
            this.Sequence[2].Add(0);
        }

        public List<int> CurrentSequence
        {
            get => this.Sequence[SequenceNumPart];
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.BlackCircle.DrawSprite(spriteBatch);
            this.DrawSprite(spriteBatch);
        }
    }


    public class MemoryGame : GameObject
    {
        public List<WhiteCircle> WhiteCirclesList = new List<WhiteCircle>();
        public List<int> ClickSquence = new List<int>();
        public bool Finished = false;

        public override void Update(GameTime gameTime)
        {
            if (WhiteCirclesList.Count() > 0)
            {
                bool _isPlaying = true;
                foreach(WhiteCircle whiteCircle in WhiteCirclesList)
                {
                    if (whiteCircle._CurrentStatus != WhiteCircle.GameStatus.PLAY) _isPlaying = false;
                }


                //Collet sequence and validade
                if (_isPlaying)
                {
                    int Id = 0;
                    foreach (WhiteCircle whiteCircle in WhiteCirclesList)
                    {
                        if (whiteCircle.MouseClickSequence)
                        {
                            if (whiteCircle.CurrentSequence[ClickSquence.Count()] == Id) this.ClickSquence.Add(Id);
                            else
                            {
                                tryAgainGame();
                                break;
                            }
                        }
                        Id++;
                    }

                    //win
                    if (WhiteCirclesList[0].CurrentSequence.Count() == ClickSquence.Count())
                    {
                        foreach (WhiteCircle whiteCircle in WhiteCirclesList)
                        {
                            whiteCircle._CurrentStatus = WhiteCircle.GameStatus.WIN;
                            whiteCircle.Transparent = 1f;
                        }
                        this.ClickSquence.Clear();
                    }

                    if (WhiteCirclesList[0].Finished)
                    {
                        this.Finished = true;
                    }
                }
                
            }
        }

        public void tryAgainGame()
        {
            this.ClickSquence.Clear();
            foreach (WhiteCircle whiteCircle in WhiteCirclesList)
            {
                whiteCircle.Transparent = 1f;
                whiteCircle._CurrentStatus = WhiteCircle.GameStatus.LOSE;
            }
        }
        
    }
}
