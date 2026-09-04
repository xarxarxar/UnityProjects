using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MergeBeast {
    public class GetGemEffect : MonoBehaviour {


        public void Move(Vector2 endPos, float timeShow, float timeMove) {
            StartCoroutine(_Move(endPos, timeShow, timeMove));

        }

        IEnumerator _Move(Vector2 endPos, float timeShow, float timeMove) {
            yield return new WaitForSeconds(timeShow);
            transform.DOMove(endPos, timeMove);
            yield return new WaitForSeconds(timeMove + 0.2f);
            gameObject.SetActive(false);
        }
        
    }
}