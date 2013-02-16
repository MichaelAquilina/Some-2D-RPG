using System;

namespace ShadowKill
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (ShadowKillGame game = new ShadowKillGame())
            {
                game.Run();
            }
        }
    }
#endif
}

