using System;

namespace DivineRightConcept
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (DivineRightGame game = new DivineRightGame())
            {
                game.Run();
            }
        }
    }
#endif
}

