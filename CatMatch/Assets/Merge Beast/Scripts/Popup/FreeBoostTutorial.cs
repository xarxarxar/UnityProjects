using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MergeBeast
{
    public class FreeBoostTutorial : MonoBehaviour
    {
        [SerializeField] private UIButton _btnFreeboost;
        [SerializeField] private UIButton _btnOk;
        [SerializeField] private Button _btnBoostFake;
        [SerializeField] private UIButton _btnOkFake;

        [SerializeField] private Image _imgIconFreeBoost;
        [SerializeField] private Image _imgIconFake;


        // Start is called before the first frame update
        void Start()
        {
            _imgIconFake.sprite = _imgIconFreeBoost.sprite;
            _imgIconFake.SetNativeSize();

            _btnBoostFake?.onClick.AddListener(this.OnClickFreeBoost);
            _btnOkFake?.onClick.AddListener(this.OnClickOK);

            _btnBoostFake.transform.position = _btnFreeboost.transform.position;
            ScreenManager.Instance.DeActiveScreen();
        }

        private void OnClickFreeBoost()
        {
            _btnBoostFake.gameObject.SetActive(false);
            _btnFreeboost?.onClick.Invoke();
            StartCoroutine(IEDisplayButonOK());
        }

        private void OnClickOK()
        {
            _btnOk?.onClick.Invoke();
            Destroy(this.gameObject);
        }

        private IEnumerator IEDisplayButonOK()
        {
            yield return new WaitForSeconds(0.5f);
            _btnOkFake.transform.position = _btnOk.transform.position;
            _btnOkFake.gameObject.SetActive(true);
        }


    }
}
