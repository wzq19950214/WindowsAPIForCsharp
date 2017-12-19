using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace GTR
{
    class JosnHelper
    {
        /// <summary>
        /// 召唤师名字
        /// </summary>
        public string Summoner
        { get; set; }
        /// <summary>
        /// 游戏等级
        /// </summary>
        public string Level
        { get; set; }
        /// <summary>
        /// 游戏金币
        /// </summary>
        public string IpBalance
        { get; set; }
        /// <summary>
        /// 游戏点券
        /// </summary>
        public string RpBalance
        { get; set; }
        /// <summary>
        /// 符文页
        /// </summary>
        public string RunePages
        { get; set; }
        /// <summary>
        /// 组队排位
        /// </summary>
        public string PreviousSeasonRank
        { get; set; }
        /// <summary>
        /// 最后比赛时间
        /// </summary>
        public string LastPlay
        { get; set; }
        /// <summary>
        /// 单人排位
        /// </summary>
        public string SoloQRank
        { get; set; }
        /// <summary>
        /// 皮肤数组
        /// </summary>
        public List<SkinList> SkinList;//皮肤数组     
        public List<ChampionList> ChampionList;//英雄数组
    }
    class SkinList
    {
        /// <summary>
        /// 皮肤ID
        /// </summary>
        public string Id
        { get; set; }
        /// <summary>
        /// 皮肤名字
        /// </summary>
        public string Name
        { get; set; }
    }
    class ChampionList
    {
        /// <summary>
        /// 皮肤ID
        /// </summary>
        public string Id
        { get; set; }
        /// <summary>
        /// 皮肤名字
        /// </summary>
        public string Name
        { get; set; }
    }
}
