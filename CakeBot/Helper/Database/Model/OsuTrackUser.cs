//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CakeBot.Helper.Database.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class OsuTrackUser
    {
        public long UserId { get; set; }
        public long ChannelId { get; set; }
        public Nullable<int> PpScore { get; set; }
        public Nullable<int> RankScore { get; set; }
    
        public virtual OsuUser OsuUser { get; set; }
        public virtual DiscordChannel DiscordChannel { get; set; }
    }
}
