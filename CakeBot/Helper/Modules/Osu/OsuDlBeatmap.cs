﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using CakeBot.Helper.Logging;
using static System.IO.Directory;

namespace CakeBot.Helper.Modules.Osu
{
    public static class OsuDlBeatmap
    {
        private static readonly string _path = AppDomain.CurrentDomain.BaseDirectory + @"\osu";

        public static byte[] FindMap(int beatmapId)
        {
            CreateFolder();
            try
            {
                return GetMap(beatmapId);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

            return null;
        }

        private static void CreateFolder()
        {
            if (Exists(_path))
            {
                return;
            }

            CreateDirectory(_path);
        }

        private static byte[] DownloadData(string filename, int beatmapId)
        {
            var data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{beatmapId}");
            if (!File.Exists(filename))
                File.WriteAllBytes($@"osu\{beatmapId}.txt", data.ToArray());
            return data;
        }

        private static byte[] GetMap(int beatmapId)
        {
            var filename = _path + $@"\{beatmapId}.txt";
            if (File.Exists(filename))
            {
                return File.ReadAllBytes(filename);
            }
            return DownloadData(filename, beatmapId);
        }
    }
}
