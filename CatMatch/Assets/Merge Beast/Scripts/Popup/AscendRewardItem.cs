using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MergeBeast
{
    public class AscendRewardItem : MonoBehaviour
    {
        public Image ImgLight;
        public Image ImgLight2;
        public Image ImgSelected;
        public Text TxtSl;
        private bool _isSeleced;
        private string _myDes;
        public UnityAction<string> _showToolTip;
        public UnityAction _hideTooltip;


        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnClickShowTooltip()
        {
            _showToolTip?.Invoke(_myDes);
        }

        public void OnClickCloseTooltip()
        {
            _hideTooltip?.Invoke();
        }

        public void ResetState()
        {
            ImgLight.color = ImgLight2.color = new Color(1f, 1f, 1f, 0f);
            ImgSelected.color = new Color(1f, 1f, 1f, 0f);
            
            _isSeleced = false;
        }

        public void SetDescription(string des)
        {
            _myDes = des;
        }

        public void SetSelected()
        {
            _isSeleced = true;
            StartCoroutine(IENhapNhay());
        }

        private void Update()
        {
            if (_isSeleced)
            {
                ImgLight.transform.Rotate(Vector3.forward * 20 * Time.deltaTime);
                ImgLight2.transform.Rotate(Vector3.forward * -20f * Time.deltaTime);
            }
        }

        private IEnumerator IENhapNhay()
        {
            for(int i = 0; i < 15; i++)
            {
                ImgSelected.gameObject.SetActive(!ImgSelected.gameObject.activeInHierarchy);
                yield return new WaitForSeconds(0.1f);
            }
            ImgSelected.gameObject.SetActive(true);
        }
    }
}
