﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpaceTaxi_1 {
    internal class Program {
        
        public static void Main(string[] args)
        {
//            GraphicsGenerator gene = new GraphicsGenerator("short-n-sweet",500,500);

            Game newGame = new Game();
            newGame.GameLoop();
        }
    }
}