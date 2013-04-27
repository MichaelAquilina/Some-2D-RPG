using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;

namespace Some2DRPG.GameObjects.Misc
{
    public class CoinArea : Entity, ISizedEntity
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int CoinPadding { get; set; }

        public CoinType CoinType { get; set; }

        public int CoinValue { get; set; }

        public CoinArea()
        {
            this.CoinValue = 0;
            this.CoinType = CoinType.Copper;
            this.CoinPadding = 8;
        }

        public override bool PreInitialize(GameTime gameTime, TeeEngine engine)
        {
            int coinx = (Width / CoinPadding);
            int coiny = (Height / CoinPadding);

            for (int i = 0; i < coinx; i++)
            {
                for (int j = 0; j < coiny; j++)
                {
                    Coin coin = new Coin();
                    coin.CoinType = CoinType;
                    coin.CoinValue = CoinValue;
                    coin.Pos = new Vector2(Pos.X + i * CoinPadding, Pos.Y + j * CoinPadding);

                    engine.AddEntity(coin);
                }
            }

            return false;   // Destory self when ready.
        }
    }
}
