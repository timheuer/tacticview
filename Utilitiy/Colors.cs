using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace TacticView.Utilitiy
{
    public class Colors
    {
        private const string COLOR_WHITE = "white";
        private const string COLOR_BLACK = "black";

        public static string GetReadableForeColorAsString(string backgroundColor)
        {
            // turn the background color into Color obj
            var c = ColorTranslator.FromHtml($"#{backgroundColor}");

            // calculate best foreground color
            return (((c.R + c.B + c.G) / 3) > 128) ? COLOR_BLACK : COLOR_WHITE;
        }
    }
}
