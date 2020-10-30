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

        static readonly string[] bots =
        {
            "trampoline","asp.net","base","beach","Blazor","builder","chef","cloudapps","delivery-bot","doctor","facing-left","gamedevelopment","golf","grilling","guitar","handybot","hang-gliding","iot","jetpack-faceing-right","kayaking","lifting","machinelearning","magician","microservices","mic-drop","mobileapps-xamarin","on-the-phone","paragliding","presenting","razor-scooter","relaxing","scooter","skating","surfing"
        };

        public static string GetBot()
        {
            Random r = new();

            return bots[r.Next(0, bots.Length)];
        }
        public static string GetMotivator()
        {
            Random r = new();

            return motivators[r.Next(0, motivators.Length)];
        }
    }
}
