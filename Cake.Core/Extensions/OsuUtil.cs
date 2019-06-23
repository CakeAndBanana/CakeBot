﻿namespace Cake.Core.Extensions
{
    public class OsuUtil
    {
        public const string OsuApiBaseUrl = "https://osu.ppy.sh/api/";
        public const string OsuDownload = "https://osu.ppy.sh/d/";
        public const string OsuUserUrl = "https://osu.ppy.sh/u/";
        public const string OsuAvatarUrl = "https://a.ppy.sh/";
        public const string OsuFlagUrl = "https://osu.ppy.sh/images/flags/";
        public const string OsuOldFlagUrl = "https://s.ppy.sh/images/flags/";

        //Osu game links
        public const string OsuDirect = "osu://s/";
        public const string OsuSpectate = "osu://spectate/";

        //Thirdparty urls
        public const string Bloodcat = "https://bloodcat.com/osu/s/";
        public const string OsuTrack = "https://ameobea.me/osutrack/user/";
        public const string OsuStats = "https://osustats.ppy.sh/u/";
        public const string OsuSkills = "http://osuskills.tk/user/";
        public const string OsuChan = "https://syrin.me/osuchan/u/";

        public const string EmoteX = "<:miss:486635818224713739>";
        public const string Emote50 = "<:hit50:486637577286189056>";
        public const string Emote100 = "<:hit100:486637577181593610>";
        public const string Emote300 = "<:hit300:486637577202302976>";
    }

    public static class OsuEmotes
    {
        public static string LevelEmotes(this string rank)
        {
            switch (rank)
            {
                case "F":
                    return "<:rankingF:487313612289998868>";
                case "D":
                    return "<:rankingD:487304280827494410>";
                case "C":
                    return "<:rankingC:487304280496406528>";
                case "B":
                    return "<:rankingB:487304280680693766>";
                case "A":
                    return "<:rankingA:487304280534155267>";
                case "S":
                    return "<:rankingS:487304280827494420>";
                case "SH":
                    return "<:rankingSH:487304280517115904>";
                case "X":
                    return "<:rankingX:487304280844533770>";
                case "XH":
                    return "<:rankingXH:487304281070764063>";
                default:
                    return "No valid rank given";
            }
        }
    }

    internal class OsuMods
    {
        public static string Modnames(int mods)
        {
            string modString;
            if (mods > 0)
            {
                modString = "+";
            }
            else
            {
                modString = "";
            }


            if (IsBitSet(mods, 0))
                modString += "NF";
            if (IsBitSet(mods, 1))
                modString += "EZ";
            if (IsBitSet(mods, 8))
                modString += "HT";

            if (IsBitSet(mods, 3))
                modString += "HD";
            if (IsBitSet(mods, 4))
                modString += "HR";
            if (IsBitSet(mods, 6) && !IsBitSet(mods, 9))
                modString += "DT";
            if (IsBitSet(mods, 9))
                modString += "NC";
            if (IsBitSet(mods, 10))
                modString += "FL";

            if (IsBitSet(mods, 5))
                modString += "SD";
            if (IsBitSet(mods, 14))
                modString += "PF";

            if (IsBitSet(mods, 7))
                modString += "RX";
            if (IsBitSet(mods, 11))
                modString += "AT";
            if (IsBitSet(mods, 12))
                modString += "SO";
            return modString;
        }

        private static bool IsBitSet(int mods, int pos) =>
            (mods & (1 << pos)) != 0;
    }
}
