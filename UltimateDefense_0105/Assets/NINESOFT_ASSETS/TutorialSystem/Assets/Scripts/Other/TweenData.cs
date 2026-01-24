using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{

    [System.Serializable]
    public class TweenData
    {
        [SerializeField] public Transform Obj;

        [Space(10)]
        public float Speed = .5f;

        [Space(10)]
        [SerializeField] public bool rotate;
        [SerializeField] public bool scale;
        [SerializeField] public bool yoyo;

        [Space(15)]

        [SerializeField] public bool lookAtCamera;
        [SerializeField] [Tooltip("Default: Main Camera")] public Camera targetCamera;

        [Space(15)]

        private Coroutine rotateCoroutine;
        private Coroutine scaleCoroutine;
        private Coroutine yoyoCoroutine;
        private Coroutine lookAtCameraCoroutine;

        public void TweenAnimation(MonoBehaviour behaviour)
        {
            if (rotateCoroutine != null) behaviour.StopCoroutine(rotateCoroutine);
            if (scaleCoroutine != null) behaviour.StopCoroutine(scaleCoroutine);
            if (yoyoCoroutine != null) behaviour.StopCoroutine(yoyoCoroutine);
            if (lookAtCameraCoroutine != null) behaviour.StopCoroutine(lookAtCameraCoroutine);

            if (rotate)
            {
                rotateCoroutine = behaviour.StartCoroutine(Rotate());
            }
            if (scale)
            {
                scaleCoroutine = behaviour.StartCoroutine(Scale());
            }
            if (yoyo)
            {
                yoyoCoroutine = behaviour.StartCoroutine(Yoyo());
            }
            if (lookAtCamera)
            {
                lookAtCameraCoroutine = behaviour.StartCoroutine(LookAtTheCamera());
            }
        }

        private IEnumerator Rotate()
        {
            while (true)
            {
                Obj.Rotate(new Vector3(0, 360 * Time.deltaTime * Speed, 0));
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Scale()
        {
            Vector3 defaultScale = Obj.localScale;
            while (true)
            {
                float t = 0;
                while (t < 1f)
                {
                    t += Time.deltaTime;
                    Obj.localScale = Vector3.Slerp(Obj.localScale, defaultScale * .75f, t * Speed * 0.01f);
                    yield return new WaitForEndOfFrame();
                }

                t = 0;
                while (t < 1f)
                {
                    t += Time.deltaTime;
                    Obj.localScale = Vector3.Slerp(Obj.localScale, defaultScale * 1f, t * Speed * 0.01f);
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Yoyo()
        {
            float t = 0f;

            Vector3 maxPos = Obj.localPosition + new Vector3(0, 1f, 0);

            Vector3 curPos = Obj.localPosition;
            while (true)
            {
                t += Time.deltaTime * Speed;

                curPos.y = Mathf.PingPong(t, maxPos.y);
                Obj.localPosition = Vector3.Lerp(Obj.localPosition, curPos, Time.deltaTime * 10f);

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LookAtTheCamera()
        {
            if (rotate)
            {
                TutorialManager.Instance.DebugLog(Obj.name + ": LookAt mode will not work because rotate mode is on", Obj.gameObject, DebugType.Error);
                yield break;
            }

            if (targetCamera == null) targetCamera = Camera.main;

            while (true)
            {
                Vector3 pos = targetCamera.transform.position - Obj.position;
                Obj.rotation = Quaternion.Lerp(Obj.rotation, Quaternion.LookRotation(pos), Time.deltaTime * Speed);
                yield return new WaitForEndOfFrame();
            }
        }
    }

}
