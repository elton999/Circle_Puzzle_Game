using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UmbrellaToolKit;
using tainicom.Aether.Physics2D.Dynamics;

namespace ball.Gameplay.MemoryGame
{
    public class MemoryGameWhiteCircle : GameObject
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
            this.CBody.BodyType = BodyType.Static;
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


        public virtual void InitialAnimationUpdate() { }
        public virtual void SeparationAnimationUpdade () { }
        public virtual void SequenceAnimationUpdate() { }
        public virtual void DefaultPostion() { }

        // initial animation
        private float _time;
        private float _timeInitialAnimation;
        private bool _ignoreGravity = true;
        private bool _animation = true;
        private bool _explosionAnimation = false;
        private int _timeExeposionAnimation;


        public Vector2 _InitialPosition;
        public  Vector2 _shakePosition;
        public Vector2 _positionAfterExposion;
        public Vector2 _position;
        public float _positionAfterExposionFrame = 20;
        public float _shakeMagnitude = 2f;
        public static readonly Random getrandom = new Random();

        // gameplay
        public enum GameStatus { NONE, SEQUENCE, PLAY, LOSE, WIN }
        public GameStatus _CurrentStatus;

        public override void Update(GameTime gameTime)
        {
            _position = this._Screem.getCenterScreem;
            _InitialPosition = this._Screem.getCenterScreem;

            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeInitialAnimation += (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseClickSequence = false;

            #region initial animation
            if (_animation)
            {
                if (_time % 0.2f >= 0.032f && _timeExeposionAnimation != 4)
                {
                    this.InitialAnimationUpdate();
                   
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
                }
                else if (_timeInitialAnimation % 2f >= (0.032f * 15) && _explosionAnimation && _timeExeposionAnimation != 4)
                {
                    _timeExeposionAnimation++;
                    _timeInitialAnimation = 0;
                    if (_timeExeposionAnimation == 4) _positionAfterExposion = _position;
                }

                if (_timeExeposionAnimation == 4 && _timeInitialAnimation % 2f >= 0.032f && _positionAfterExposionFrame != 0)
                {
                    this.BlackCircle.Scale = 1;

                    this.SeparationAnimationUpdade();
                    

                    _positionAfterExposionFrame--;
                    _timeInitialAnimation = 0;

                    if (_positionAfterExposionFrame == 0) this._animation = false;
                }
                if (this._positionAfterExposion != Vector2.Zero) this.CBody.SetTransformIgnoreContacts(ref _positionAfterExposion, 0);


            }
            #endregion

            #region gameplay
            else
            {
                this.DefaultPostion();
                
                if (_time % 0.2f >= (0.032f * 3) && _CurrentStatus == GameStatus.NONE)
                {
                    _time = 0;
                    _CurrentStatus = GameStatus.LOSE;
                }


                if (_CurrentStatus == GameStatus.SEQUENCE)
                {
                    if (this.SequenceNumPart < this.Sequence.Count())
                    {
                        if (_time % 2f >= 0.032f)
                        {
                            this.SequenceAnimationUpdate();

                            _time = 0;
                        }
                        this.NextStep(gameTime);
                    } else Finished = true;
                    
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
                    }
                    else if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        MouseClick = false;
                        this.Transparent = 1;
                    }
                }
                else if (_CurrentStatus == GameStatus.WIN)
                {
                    this.SightWin(gameTime);
                    if (SequenceNumPart == this.Sequence.Count()) this.Finished = true;
                }
                #endregion

                this.CBody.SetTransform(ref _position, 0f);
            }

            this.BlackCircle.Position = this.CBody.Position;
            this._mouseOver = false;
        }

        private float _transparentValue = 1f;
        public void SetTransparent(bool more)
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
            if (!_IsSightLose) this.Transparent = 0f;
            _SightLoseWait += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_SightLoseWait > 1f && !_IsSightLose)
            {
                this.Transparent = 1f;
                _SightLoseWait = 0;
                _IsSightLose = true;
            }
            if (_SightLoseWait > 1f && _IsSightLose)
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
        public virtual void SetSequence() { }

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
}
