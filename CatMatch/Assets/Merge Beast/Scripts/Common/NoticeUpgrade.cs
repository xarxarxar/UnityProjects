using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

namespace MergeBeast {
    public class NoticeUpgrade : MonoBehaviour {
        [SerializeField] private GameObject _objNotice;
        [SerializeField] private UIButton _btnUpgrade;
        [SerializeField] private UIManager _uiMgr;
        [SerializeField] private GameObject _pnUpgradeTutorial;

        private bool _lockStage, _hasTutorial;
        private BigInteger _priceBeast;
        private BigInteger _priceSpam;
        private BigInteger _priceDoubleSpawn;
        private BigInteger _priceLevelMerge;

        private float timeToCheckNotice = 2f;
        private float timeCountNotice = 2f;

        // Start is called before the first frame update
        void Start() {

            int _currentIDBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            var beast = GameManager.Instance.GetBeast(_currentIDBeast);
            int lvBeast = beast.Level;
            int _currentLvSpam = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM, 1);
            int _currentLvDoubleSpawn = PlayerPrefs.GetInt(StringDefine.LEVEL_DOUBLE_SPAWN, 0);
            int _currentLvLevelMerge = PlayerPrefs.GetInt(StringDefine.LEVEL_LEVEL_MERGE, 0);

            BigInteger prLevel = Utils.PriceUpgradeBeast(lvBeast);
            BigInteger prSpam = Utils.PriceUpgradeSpam(_currentLvSpam);
            BigInteger prDoubleSpawn = Utils.PriceUpgradeDoubleSpawn(_currentLvDoubleSpawn);
            BigInteger prLevelMerge = Utils.PriceUpgradeLevelMerge(_currentLvLevelMerge);

            this.UpdatePrice(prLevel, prSpam, prDoubleSpawn, prLevelMerge);

            _btnUpgrade?.onClick.AddListener(OnClickOpenNotice);
            _hasTutorial = PlayerPrefs.GetInt(StringDefine.HAS_TUTORIAL_UPGRADE, 0) != 0;
            if (_hasTutorial) {
                Destroy(_pnUpgradeTutorial);
            }
        }

        // Update is called once per frame
        void Update() {
            timeCountNotice += Time.deltaTime;
            if (timeCountNotice >= timeToCheckNotice) {
                timeCountNotice = 0;

                _objNotice.SetActive(UIManager.Instance.CurrentCoin >= _priceBeast || UIManager.Instance.CurrentCoin >= _priceSpam 
                || UIManager.Instance.CurrentCoin >= _priceDoubleSpawn || UIManager.Instance.CurrentCoin >= _priceLevelMerge);

            }
            //if (!_lockStage)
            //{
            //    _objNotice.SetActive(_uiMgr.CurrentCoin >= _priceBeast || _uiMgr.CurrentCoin >= _priceSpam);

            //    if (!_hasTutorial)
            //    {
            //        if(_uiMgr.CurrentCoin >= _priceBeast)
            //        {
            //            _pnUpgradeTutorial.SetActive(true);
            //            _hasTutorial = true;
            //            PlayerPrefs.SetInt(StringDefine.HAS_TUTORIAL_UPGRADE, 1);
            //        }
            //    }
            //}
        }

        public void UpdatePrice(BigInteger priceBeast, BigInteger priceSpam,
         BigInteger priceDoubleSpawn, BigInteger priceLevelMerge) {
            if(priceBeast > 0) this._priceBeast = priceBeast;
            if(priceSpam > 0) this._priceSpam = priceSpam;
            if(priceDoubleSpawn > 0) this._priceDoubleSpawn = priceDoubleSpawn;
            if(priceLevelMerge > 0) this._priceLevelMerge = priceLevelMerge;
        }

        private void OnClickOpenNotice() {
            if (_objNotice.activeInHierarchy) {
                _lockStage = true;
                _objNotice.SetActive(false);
            }
        }

        public void SetNextStage() {
            _lockStage = false;
        }
    }
}
