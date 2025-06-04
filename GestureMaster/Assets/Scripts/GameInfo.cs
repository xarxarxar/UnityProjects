using System;
using System.Collections.Generic;
using System.Diagnostics;

[Serializable]
public class GameInfo
{
    public DateTime lastLoginDate;//上一次登录的时间，精确到日期
    public string playerName;//玩家昵称
    public string avatarUrl;//玩家头像列表
    public string playerProvince;//玩家所在省份
    public int level;//当前关卡
    public bool isMusicOn;//音乐是否开启
    public bool isSfxfOn;//音效是否开启
    public List<SkinInfo> meijiaDecals=new List<SkinInfo>();
    public List<SkinInfo> tiehuaDecals=new List<SkinInfo>();

    public List<LoginRewardInfo> loginRewardInfos=new List<LoginRewardInfo>();//一周的登录领取皮肤详情

    public void Init()
    {
        lastLoginDate = GameEntrance.TodayDate;//今天
        playerName = "游客";
        avatarUrl = "";
        playerProvince = "其他";

        level = 1;
        isMusicOn = true;
        isSfxfOn = true;

        //填充三个list
        meijiaDecals.Clear();
        tiehuaDecals.Clear();
        if (GameEntrance.instance.decalDatabase_meijia != null)
        {
            foreach (var decal in GameEntrance.instance.decalDatabase_meijia.decalList)
            {
                meijiaDecals.Add(new SkinInfo
                {
                    id = decal.id,
                    isOwned = decal.isOwned,
                    isEquip = decal.isEquip
                });
            }
        }
        if (GameEntrance.instance.decalDatabase_tiehua != null)
        {
            foreach (var decal in GameEntrance.instance.decalDatabase_tiehua.decalList)
            {
                tiehuaDecals.Add(new SkinInfo
                {
                    id = decal.id,
                    isOwned = decal.isOwned,
                    isEquip = decal.isEquip
                });
            }
        }
        GameEntrance.instance.SetGameInfoLoginRewardInfos(loginRewardInfos);
    }


}

[Serializable]
public class SkinInfo
{
    public string id;
    public bool isOwned;
    public bool isEquip;
}
