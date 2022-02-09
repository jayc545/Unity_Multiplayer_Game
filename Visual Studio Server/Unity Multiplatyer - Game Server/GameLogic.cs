using System;
using System.Collections.Generic;
using System.Text;

namespace Unity_Multiplatyer___Game_Server
{
    class GameLogic
    {
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }
    }
}
