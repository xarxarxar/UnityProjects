using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MergeBeast {
    public class BoostChestItem : MonoBehaviour {

        [SerializeField] private Sprite boostDamageSpr;
        [SerializeField] private Sprite spawnFasterSpr;
        [SerializeField] private Sprite autoMergeSpr;
        [SerializeField] private Sprite medalMergeSpr;

        [SerializeField] private Image icon;
        [SerializeField] private Text title;
        [SerializeField] private Text desc;
        [SerializeField] private UIButton btnUse;
        //color #D37100FF

        private EnumDefine.BOOST _currentBoost;
        private int amount;
        private BoostScreen boostScreen;

        private void Awake() {
            boostScreen = FindObjectOfType<BoostScreen>();
            btnUse.onClick.AddListener(() => Use());
        }
        public void SetInfo(EnumDefine.BOOST boostType, int amount) {           
            _currentBoost = boostType;
            this.amount = amount;
            if(amount <= 0) {
                gameObject.SetActive(false);
            }
            switch (boostType) {
                case EnumDefine.BOOST.DAMAGE_BOOST_1:
                icon.sprite = boostDamageSpr;
                icon.SetNativeSize();
                title.text = "伤害增益";
                desc.text = "怪物的伤害提升100%，持续<color=#D37100FF>5分钟</color>";
                break;
                case EnumDefine.BOOST.SPAWN_FASTER_1:
                icon.sprite = spawnFasterSpr;
                icon.SetNativeSize();
                title.text = "生成加速";
                desc.text = "新魔兽生成速度提升<color=#D37100FF>2倍</color>，持续<color=#D37100FF>5分钟</color>";
                break;
                case EnumDefine.BOOST.AUTO_MERGE_1:
                icon.sprite = autoMergeSpr;
                icon.SetNativeSize();
                title.text = "自动合成";
                desc.text = "自动合成魔兽，最长<color=#D37100FF>5分钟</color>";
                break;
                case EnumDefine.BOOST.SEVERAL_MERGE_MEDAL:
                icon.sprite = medalMergeSpr;
                icon.SetNativeSize();
                title.text = "合成勋章堆";
                desc.text = "获得<color=#D37100FF>200合成勋章</color>";
                break;
            }

            btnUse.GetComponentInChildren<Text>().text = $"使用\n({amount})";
        }

        private void Use() {

            var boost = BoostManager.Instance.Boost(_currentBoost);

            if (_currentBoost == EnumDefine.BOOST.AUTO_MERGE_1) {
                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.AUTO_MERGE, Utils.GetBuffMinute(), 0);                
            } else {
                BoostManager.Instance.AddBoost(_currentBoost, Utils.GetBuffMinute());                
            }

            UIManager.Instance?.ShowNotify($"使用成功{boost.Name}");



            amount -= 1;
            boostScreen.UpdateBoostChest((int)_currentBoost, -1);
            
        }
    }
}