using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace MergeBeast
{
    public class CoinRewardFX : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigid2D;
        [SerializeField] private Collider2D _collider;
        private Vector3 _positionTarget;
        private UnityAction<CoinRewardFX> _callback;


        // Use this for initialization
        
        public void ShowFX(Vector3 position,Vector3 target,UnityAction<CoinRewardFX> action)
        {
            this.transform.position = position;
            this._callback = action;
            this._collider.enabled = true;
            _rigid2D.gravityScale = 2f;
            _rigid2D.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(1.5f, 3f));
            _positionTarget = target;
            StartCoroutine(IEStopDown());
        }

        IEnumerator IEStopDown()
        {
            float time = Random.Range(1f, 1.5f);
            yield return new WaitForSeconds(time);
            this._collider.enabled = false;
            this.transform.DOMove(_positionTarget, 0.5f);
            yield return new WaitForSeconds(0.5f);
            _callback?.Invoke(this);
        }
    }
}
