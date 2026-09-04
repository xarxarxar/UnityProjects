using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

namespace MergeBeast
{
    public class TooltipEnemyScr : MonoBehaviour
    {
        [SerializeField] private Text _txtSoul;
        [SerializeField] private Text _txtGem;
        [SerializeField] private Text _txtMedal;
        [SerializeField] private Text _txtBoostChest;

        private void OnEnable()
        {
            int lvMonster = PlayerPrefs.GetInt(StringDefine.LEVEL_MONSTER, 0);
            string soul = DataConfig.ListSoulReward[lvMonster];
            _txtSoul.text = Utils.FormatNumber(BigInteger.Parse(soul));
            _txtGem.text = DataConfig.ListGemReward[lvMonster].AsInt.ToString();
            _txtMedal.text = DataConfig.ListMedalReward[lvMonster].AsInt.ToString();
            _txtBoostChest.text = DataConfig.ListChestReward[lvMonster].AsInt.ToString();
        }

    }
}
