using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MergeBeast
{
    public class UpgradeTutorialCtrl : MonoBehaviour
    {
        [SerializeField] private UIButton _btnOpenUpgradeTab;
        [SerializeField] private UIButton _btnUpgrade;
        [SerializeField] private Button _btnOpenFake;
        [SerializeField] private Button _btnUpgradeFake;
        [SerializeField] private Text _txtPrice;


        // Start is called before the first frame update
        void Start()
        {
            _btnOpenFake.onClick?.AddListener(this.OnClickOpenTabUpgrade);
            _btnUpgradeFake.onClick?.AddListener(this.OnClickUpgrade);
           
        }

        private void OnEnable()
        {
            Debug.LogError("zzzz");
            ScreenManager.Instance.DeActiveScreen();
        }

        private void OnClickOpenTabUpgrade()
        {
            _btnOpenFake.transform.parent.gameObject.SetActive(false);
            _btnUpgradeFake.transform.parent.gameObject.SetActive(true);
            _btnOpenUpgradeTab.onClick.Invoke();
            _txtPrice.text = _btnUpgrade.transform.GetChild(0).GetComponent<Text>().text;
        }

        private void OnClickUpgrade()
        {
            _btnUpgrade.onClick.Invoke();
            Destroy(this.gameObject);
        }
    }
}
