using System;

namespace ShadowKill
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (Some2DRPG game = new Some2DRPG())
            {
                game.Run();
            }
        }
    }
#endif
}

