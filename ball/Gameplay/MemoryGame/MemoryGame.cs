using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using UmbrellaToolKit;

namespace ball.Gameplay.MemoryGame
{
    public class MemoryGame : GameObject
    {
        public List<MemoryGameWhiteCircle> WhiteCirclesList = new List<MemoryGameWhiteCircle>();
        public List<int> ClickSquence = new List<int>();
        public bool Finished = false;

        public override void Update(GameTime gameTime)
        {
            if (WhiteCirclesList.Count() > 0)
            {
                bool _isPlaying = true;
                foreach (MemoryGameWhiteCircle whiteCircle in WhiteCirclesList)
                {
                    if (whiteCircle._CurrentStatus != MemoryGameWhiteCircle.GameStatus.PLAY) _isPlaying = false;
                }


                //Collet sequence and validade
                if (_isPlaying)
                {
                    int Id = 0;
                    foreach (MemoryGameWhiteCircle whiteCircle in WhiteCirclesList)
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
                        foreach (MemoryGameWhiteCircle whiteCircle in WhiteCirclesList)
                        {
                            whiteCircle._CurrentStatus = MemoryGameWhiteCircle.GameStatus.WIN;
                            whiteCircle.Transparent = 1f;
                        }
                        this.ClickSquence.Clear();
                    }
                }
                if (WhiteCirclesList[0].Finished) this.Finished = true;
            }
        }

        public void tryAgainGame()
        {
            this.ClickSquence.Clear();
            foreach (MemoryGameWhiteCircle whiteCircle in WhiteCirclesList)
            {
                whiteCircle.Transparent = 1f;
                whiteCircle._CurrentStatus = MemoryGameWhiteCircle.GameStatus.LOSE;
            }
        }
    }
}
