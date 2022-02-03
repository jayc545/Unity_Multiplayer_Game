using System;

namespace Unity_Multiplatyer___Game_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set the Tittle for the console.
            Console.Title = "Game Server";


            Server.Start(50, 26950);

            Console.ReadKey();
        }
    }
}
