﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cake.Core.Discord.Attributes;
using Cake.Core.Discord.Services;
using Cake.Core.Exceptions;
using Cake.Core.Extensions.Osu;
using Cake.Core.Logging;
using Discord;
using Discord.Commands;
using static System.Int32;

namespace Cake.Core.Discord.Modules
{
    public class OsuArg
    {
        private readonly bool _userUsername;
        private readonly bool _recent;
        private readonly int? _play;
        private readonly string _userId;

        public OsuArg(string arg)
        {
            if (arg.Contains("-id"))
            {
                _userId = Regex.Match(arg, @"\d+").Value;
                _userUsername = false;
            }
            else
            {
                _userId = arg;
                _userUsername = true;
            }
        }

        public OsuArg(string arg, bool best)
        {
            if (arg.Contains("-r"))
            {
                _recent = true;
                arg.Replace("-r", "");
            }
            else if (arg.Contains("-p"))
            {
                _play = Parse(Regex.Match(arg, @"\d+").Value);
                if (_play > 100 || _play == 0)
                {
                    throw new CakeException("``Play number has to be between 1 and 100``");
                }
                arg.Replace("-p", "");
            }
            _userId = arg;
            _userUsername = true;

            if (arg.Contains("-id"))
            {
                _userId = Regex.Match(arg, @"\d+").Value;
                _userUsername = false;
                arg.Replace("-p", "");
            }
        }

        public bool UseUsername()
        {
            return _userUsername;
        }

        public bool IsRecent()
        {
            return _recent;
        }

        public int? GetPlayNumber()
        {
            return _play;
        }

        public string GetUserId()
        {
            return _userId;
        }
    }

    [Name("osu!")]
    [Group("osu")]
    [Alias("o")]
    public class OsuModule : CustomBaseModule
    {
        private readonly OsuService _service;
        private static int? _mapId;
        private readonly Logger _logger = Logger.Get() as Logger;

        public static int? GetMapId()
        {
            return _mapId;
        }

        public static void SetMapId(int? value)
        {
            _mapId = value;
        }

        public OsuModule(OsuService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("set", RunMode = RunMode.Async)]
        [Summary("osu set (?username)")]
        [Alias("s")]
        [Remarks("Binds your osu account to your discord user.")]
        public async Task SetAccount([Remainder]string userName)
        {
            await _service.SetAccount(userName);
        }

        [Alias("m")]
        [Command("mode", RunMode = RunMode.Async)]
        [Summary(">osu mode (0-3)")]
        [Remarks("Changes the mode of your username.")]
        public async Task SetMode(string mode)
        {
            try
            {
                var modeNumber = (int)OsuMode.GetOsuMode(mode);
                await _service.SetMode(modeNumber);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }
        }

        [Command("profile", RunMode = RunMode.Async)]
        [Summary("osu profile (?user)")]
        [Alias("u", "p")]
        [Remarks("Gets profile of current user or given user.")]
        public async Task GetProfile([Remainder]string args = "")
        {
            OsuArg osuDiscordArg = null;

            try
            {
                osuDiscordArg = new OsuArg(args);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }

            await _service.GetUserProfile(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());
        }

        [Command("profile", RunMode = RunMode.Async)]
        [Hide]
        [Alias("u", "p")]
        public async Task GetProfileDUser(IGuildUser user)
        {
            await _service.GetUserProfile("", false, true, user.Id);
        }

        [Command("best", RunMode = RunMode.Async)]
        [Alias("b")]
        [Summary(">osu best (mode) (username)")]
        [Remarks("Returns someones best plays.")]
        public async Task GetUserBest([Remainder]string arg = "")
        {
            OsuArg osuDiscordArg = null;

            try
            {
                osuDiscordArg = new OsuArg(arg, true);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }

            if (osuDiscordArg != null)
            {
                await _service.GetUserBest(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername(), osuDiscordArg.IsRecent(), osuDiscordArg.GetPlayNumber());
            }
        }

        [Command("best", RunMode = RunMode.Async)]
        [Hide]
        [Alias("b")]
        public async Task GetUserBestDUser(IGuildUser user, [Remainder]string arg = "")
        {
            OsuArg osuDiscordArg = null;

            try
            {
                osuDiscordArg = new OsuArg(arg, true);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }

            if (osuDiscordArg != null)
            {
                await _service.GetUserBest("", false, osuDiscordArg.IsRecent(), osuDiscordArg.GetPlayNumber(), true, user.Id);
            }
        }

        [Command("recent", RunMode = RunMode.Async)]
        [Alias("r")]
        [Summary(">osu recent (username)")]
        [Remarks("Returns someones recently played maps")]
        public async Task Recent([Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;

            try
            {
                osuDiscordArg = new OsuArg(arg);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }

            await _service.GetUserRecent(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());

            var mapId = GetMapId();
            if (mapId != null)
            {
                await SaveMapId(Context.Channel.Id, Context.Guild.Id, (int)mapId);
            }
        }

        [Command("recent", RunMode = RunMode.Async)]
        [Hide]
        [Alias("r")]
        public async Task RecentDUser(IGuildUser user)
        {
            await _service.GetUserRecent("", false, true, user.Id);

            var mapId = GetMapId();
            if (mapId != null)
            {
                await SaveMapId(Context.Channel.Id, Context.Guild.Id, (int)mapId);
            }
        }

        [Command("compare", RunMode = RunMode.Async)]
        [Alias("c")]
        [Summary(">osu compare")]
        [Remarks("Compare your score on a recently played map")]
        public async Task GetCompare([Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;

            try
            {
                osuDiscordArg = new OsuArg(arg);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }

            await _service.GetCompare(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());
        }

        [Command("compare", RunMode = RunMode.Async)]
        [Hide]
        [Alias("c")]
        public async Task GetCompareDUser(IGuildUser user)
        {
            await _service.GetCompare("", false, true, user.Id);
        }

        private async Task SaveMapId(ulong channelId, ulong guildId, int mapId)
        {
            var channel = await Database.Queries.ChannelQueries.FindOrCreateChannel(channelId, guildId);
            channel.OsuMapId = mapId;

            await Database.Queries.ChannelQueries.Update(channel);
        }
    }
}
