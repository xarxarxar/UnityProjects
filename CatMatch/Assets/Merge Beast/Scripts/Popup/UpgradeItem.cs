using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace MergeBeast {
    public class UpgradeItem : MonoBehaviour {
        [SerializeField] EnumDefine.UpgradeItemType itemType;
        [SerializeField] Text txtLevel;
        [SerializeField] Text txtDesc;
        [SerializeField] Text txtPrice;
        [SerializeField] Button btnUpgrade;

        [SerializeField] private NoticeUpgrade _notice;
        [SerializeField] private Sprite _spriteBuy;
        [SerializeField] private Sprite _spriteDisable;

        [SerializeField] ParticleSystem particle;

        private int currentId;
        private BigInteger price;

        private void Start() {
            btnUpgrade.onClick.AddListener(() => Upgrade());
        }

        private void OnEnable() {
            particle.gameObject.SetActive(false);
        }

        public void UpdateData() {
            
            if (itemType == EnumDefine.UpgradeItemType.LevelSpawn) {
                currentId = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
                var beast = GameManager.Instance.GetBeast(currentId);
                int lvBeast = UIManager.Instance.IsLockSpawn() ? beast.Level - GameManager.Instance.boostSpawnLevel : beast.Level;
                SetButtonActive((int)Config.MAX_BEAST);
                txtLevel.text = $"等级{lvBeast}";
                txtDesc.text = $"你的猫出生等级：<color=#D07000FF>{lvBeast}</color>\n下一级：<color=#D07000FF>+1</color>";

                price = Utils.PriceUpgradeBeast(lvBeast);
                txtPrice.text = Utils.FormatNumber(price);

                _notice.UpdatePrice(price, 0, 0, 0);

            } else if (itemType == EnumDefine.UpgradeItemType.TimeSpawn) {
                currentId = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM, 1);
                SetButtonActive(50);
                txtLevel.text = $"等级{currentId}";
                txtDesc.text = $"你的猫出生间隔：<color=#D07000FF>{6f - currentId * 0.1f}秒</color>\n下一级：<color=#D07000FF>-0.1秒</color>";

                price = Utils.PriceUpgradeSpam(currentId);
                txtPrice.text = Utils.FormatNumber(price);

                _notice.UpdatePrice(0, price, 0, 0);
            } else if (itemType == EnumDefine.UpgradeItemType.DoubleSpawn) {
                currentId = PlayerPrefs.GetInt(StringDefine.LEVEL_DOUBLE_SPAWN, 0);
                SetButtonActive(50);
                txtLevel.text = $"等级{currentId}";
                txtDesc.text = $"有<color=#D07000FF>{currentId}%</color>几率每次从蛋中生成的猫高于1级\n下一级：<color=#D07000FF>+1%</color>";

                price = Utils.PriceUpgradeDoubleSpawn(currentId);
                txtPrice.text = Utils.FormatNumber(price);

                _notice.UpdatePrice(0, 0, price, 0);
            } else if (itemType == EnumDefine.UpgradeItemType.LevelMerge) {
                currentId = PlayerPrefs.GetInt(StringDefine.LEVEL_LEVEL_MERGE, 0);

                SetButtonActive(10);

                txtLevel.text = $"等级{currentId}";
                txtDesc.text = $"有<color=#D07000FF>{currentId}%</color>几率猫合成提升1级";
                if (currentId <= 10) {
                    txtDesc.text += "\n下一级：<color=#D07000FF>+1%</color>";
                }
                price = Utils.PriceUpgradeLevelMerge(currentId);
                txtPrice.text = Utils.FormatNumber(price);

                _notice.UpdatePrice(0, 0, 0, price);
            }
        }

        private void CheckCanUpgrade() {
            if (itemType == EnumDefine.UpgradeItemType.LevelMerge && currentId >= 10) {
                btnUpgrade.image.sprite = _spriteDisable;
                btnUpgrade.interactable = false;
                return;
            }
            bool canUp = UIManager.Instance.CurrentCoin >= price;
            btnUpgrade.image.sprite = canUp ? _spriteBuy : _spriteDisable;
            btnUpgrade.interactable = canUp;
        }
        private void Update() {
            CheckCanUpgrade();
        }

        void Upgrade() {

            particle.gameObject.SetActive(true);
            particle.transform.position = transform.position;
            particle.Play();

            UIManager.Instance.MinusCoin(price);
            ++currentId;
            if (itemType == EnumDefine.UpgradeItemType.LevelSpawn) {
                PlayerPrefs.SetInt(StringDefine.LEVEL_BEAST, currentId);
                var beast = GameManager.Instance.GetBeast(currentId);
                UIManager.Instance.SetPriceBeast(beast);
                GameManager.Instance.UpgradeBeast(currentId, true);
                UIManager.Instance.ShowNotify("升级成功");
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.Upgrade, EnumDefine.Mission.UpgradeLevelSpawn);
            } else if (itemType == EnumDefine.UpgradeItemType.TimeSpawn) {
                PlayerPrefs.SetInt(StringDefine.LEVEL_SPAM, currentId);
                GameManager.Instance.UpgradeTimeSpam(currentId);
                UIManager.Instance.ShowNotify("生成时间升级成功");
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.Upgrade, EnumDefine.Mission.UpgradeTimeSpawn);
            } else if (itemType == EnumDefine.UpgradeItemType.DoubleSpawn) {
                PlayerPrefs.SetInt(StringDefine.LEVEL_DOUBLE_SPAWN, currentId);
                UIManager.Instance.ShowNotify("升级成功");
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.Upgrade, EnumDefine.Mission.None);
            } else if (itemType == EnumDefine.UpgradeItemType.LevelMerge) {
                currentId = Mathf.Min(10, currentId);
                PlayerPrefs.SetInt(StringDefine.LEVEL_LEVEL_MERGE, currentId);
                UIManager.Instance.ShowNotify("升级成功");
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.Upgrade, EnumDefine.Mission.None);
            }

            UpdateData();

            //Firebase.Analytics.FirebaseAnalytics.LogEvent(
            //  "upgrade_cat",
            //  new Firebase.Analytics.Parameter(
            //    "item_type", itemType.ToString()),
            //  new Firebase.Analytics.Parameter(
            //    "item_level", currentId)
            //  );
           
        }

        void SetButtonActive(int level) {
            btnUpgrade.gameObject.SetActive(currentId < level);
            txtPrice.gameObject.SetActive(currentId < level);
        }
    }
}