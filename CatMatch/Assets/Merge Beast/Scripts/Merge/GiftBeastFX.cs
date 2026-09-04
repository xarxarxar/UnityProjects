using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace MergeBeast
{
    public class GiftBeastFX : MonoBehaviour
    {
        [SerializeField] private Animator _myAnim;

        private int _slot, _id;
        private Vector2 _endPos;

        private UnityAction<GiftBeastFX> _takeToPool;

        // Use this for initialization
        void Start()
        {

        }

        public void SetData(int slot, int id,Vector2 endpos)
        {
            this._slot = slot;
            this._id = id;
            this._endPos = endpos;
            StartCoroutine(IEDropGift());
        }

        IEnumerator IEDropGift()
        {
            //transform.position = _endPos + Vector2.up * 5f;
            //transform.DOMove(_endPos, 0.5f);
            //_myAnim.Play("GiftBeast-Fall");

            //yield return new WaitForSeconds(0.5f);
            //_myAnim.Play("GiftBeast-Ground");
            //yield return new WaitForSeconds(Random.Range(0.5f,1f));
            //_myAnim.Play("GiftBeast-Show");
            transform.position = _endPos + new Vector2(0, 0.3f);
            yield return new WaitForSeconds(1.2f);
            ShowBeast();
            yield return new WaitForSeconds(0.5f);
            TakeToPool();

        }

        private void ShowBeast()
        {
            GameManager.Instance.SpawnBeastAtSlot(_id, _slot);
        }

        public void SetEvent(UnityAction<GiftBeastFX> take)
        {
            this._takeToPool = take;
        }

        private void TakeToPool()
        {
            this._takeToPool?.Invoke(this);
        }
    }
}
