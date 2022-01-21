using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISC_Simulator
{
    static class Graphics
    {
        /// <summary>
        /// Draws a pixel with the desired color at X, Y - X starts at 0 towards the right side, Y starts at the top.
        /// The real width is halved because of a double █ character making up a more realistic square.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">Color list based on ConsoleColor order</param>
        public static void DrawPixel(short x, short y, short color, bool retainPrevPosition = false)
        {
            (int x, int y) prevPos = (0,0);
            if (color > Enum.GetValues(typeof(ConsoleColor)).Length) throw new ArgumentOutOfRangeException($"Invalid color number {color}");
            if (retainPrevPosition) prevPos = Console.GetCursorPosition();
            Console.SetCursorPosition(x, y*2);
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = (ConsoleColor)color;
            Console.Write("██");
            Console.ForegroundColor = prevColor;
            if (retainPrevPosition) Console.SetCursorPosition(prevPos.x, prevPos.y);
        }
    }
}
