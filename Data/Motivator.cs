using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TacticView.Data
{
    public class Motivator
    {
        static readonly string[] motivators = { 
            "🎉 Nothing for now! 🙌",
            "Amazing.  The future is yours. 🚀",
            "You have nothing, here have a pony 🐴",
            "Nada.  Therefore Steve has approved shrimp and weenies for everyone 🍤🌭",
            "All caught up, clear skies ahead ☀",
            "All caught up, what's next? ✌",
            "Don't you wish your inbox was this empty? 🤣🤣🤣",
            "Umm, yeah I'm gonna need those TPS reports 📃",
            "🎯 Boom, bullseye...not really, but nothing in queue",
            "🎉 Ta-da!  You've got nothing to see here.",
            "You are up-to-date.  Go forth and conquer",
            "🍠 Sweet potato! You're all done!",
            "All done, time for 🏈 #GoHawks!",
            "If you know of a good fish pun, let minnow...🐟 🤣",
            "🧮 Math is hard, but it looks to me like this is empty",
            "Method ~ of ~ failed...just kidding, you're all cleared up",
            "The seas are clear...RELEASE THE KRAKEN 🐙"
        };

        public static string GetMotivator()
        {
            var r = new Random();

            return motivators[r.Next(0, motivators.Length)];
        }
    }
}
