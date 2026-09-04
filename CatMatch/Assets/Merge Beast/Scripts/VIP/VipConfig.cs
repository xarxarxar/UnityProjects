using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MergeBeast {
    [CreateAssetMenu(fileName ="VipConfig", menuName = "ConfigData/VipConfig")]
    public class VipConfig : ScriptableObject {
        public int min;
        public int max;
        public string status;
        public float dpsBuffAscend; //dps bonus khi ascend
        public int timeRefreshStarShop; //thoi gian lam moi star shop
        public int levelUpgradeAscend;//tat ca cac muc trong button upgrade 
        public int levelStageAscend;//cap quai sau khi ascend
        public int buffMinute; //so phut moi lan su dung boost
        public bool quickBuy; //dung tien mua nhanh
        public bool disableVideo;
        public bool disableBanner;
        public float percentIgnoreReward;
        
        [Header("== login 30 days==")]
        //30 day
        public int moreMedalMerge;
        public int moreStar;
        public int moreAutoMerge;
        public int rateMoreGem;
        public int moreBoostChest;
        [Header("== one time==")]
        //reward 1 time
        public int boostChest;
        public int timeJump2;
        public int timeJump4;
        [Header("===code==")]
        public int index;

    }
}