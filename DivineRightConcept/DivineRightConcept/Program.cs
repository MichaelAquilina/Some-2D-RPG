using System;

namespace DivineRightConcept
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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

