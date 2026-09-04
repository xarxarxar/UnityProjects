using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;


namespace MergeBeast
{
    public class MergeEffect : MonoBehaviour
    {
        [SerializeField] private Animator _myAnim;
        [SerializeField] private Image _imgBeastRoot1;
        [SerializeField] private Image _imgBeastRoot2;
        [SerializeField] private Image _imgBeastUpgrade;        

        private UnityAction _callback;
        private UnityAction<MergeEffect> _takeToPool;


        public void ShowEffectMerge(int lv,UnityAction evt)
        {
            //var currentBeast = GameManager.Instance.GetBeast(lv);
            //_imgBeastRoot1.sprite = _imgBeastRoot2.sprite = currentBeast.Character;
            //_imgBeastRoot1.SetNativeSize();
            //_imgBeastRoot2.SetNativeSize();

        //    var nextBeast = GameManager.Instance.GetNextBeast(lv);
        //    _imgBeastUpgrade.sprite = nextBeast.Character;

            //_myAnim.SetTrigger("show");

            _callback = evt;

            StartCoroutine(_ShowBeastImage());
            //show effect


        }

        IEnumerator _ShowBeastImage() {

            yield return new WaitForSeconds(0.3f);
            //_imgBeastUpgrade.transform.DOScale(1, 0.3f);
            //yield return new WaitForSeconds(0.3f);
            OnEventAnim();
            yield return new WaitForSeconds(0.4f);
            OnEventEndAnim();
        }

        public void SetTakePool(UnityAction<MergeEffect> action)
        {
            _takeToPool = action;
        }

        void OnEventAnim()
        {
            _callback?.Invoke();
        }

        void OnEventEndAnim()
        {
            _takeToPool?.Invoke(this);
        }
    }
}
