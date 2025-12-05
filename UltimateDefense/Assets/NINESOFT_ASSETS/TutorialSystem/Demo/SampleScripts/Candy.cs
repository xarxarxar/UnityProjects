using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM.Samples
{
    public class Candy : MonoBehaviour
    {
        public enum CandyType { None, Blue, Red, Green, Purple }

        public CandyType MyCandyType;
        private Vector2 startPos;
        private void OnMouseDown()
        {
            startPos = Input.mousePosition;
        }
        private void OnMouseUp()
        {
            Vector2 direction = ((Vector2)Input.mousePosition - startPos).normalized;
            Move(direction);
        }

        private void Move(Vector2 direction)
        {
            Vector2 dir = Vector3.zero;
            if (Mathf.Abs(direction.x) >= .5f)
            {
                dir = new Vector2(direction.x < 0 ? -1 : 1, 0);
            }
            else
            {
                dir = new Vector2(0, direction.y < 0 ? -1 : 1);
            }

            Debug.DrawRay(transform.position + (Vector3)(dir * 2.5f), dir *2f, Color.red, 5f);
            var hit = Physics2D.Raycast(transform.position + (Vector3)(dir * 2.5f), dir, 2f);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Candy"))
                {
                    Vector3 curPos = transform.position;
                    StartCoroutine(MoveToPos(transform, hit.collider.transform.position));
                    StartCoroutine(MoveToPos(hit.collider.transform, curPos));
                }
            }
        }
        private IEnumerator MoveToPos(Transform transform, Vector3 pos)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 5;
                transform.position = Vector3.Lerp(transform.position, pos, t);
                yield return new WaitForEndOfFrame();
            }
            CandyManager.Instance.CheckMatch(pos.x, pos.y);
        }



        public void DestroyMe()
        {
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}
