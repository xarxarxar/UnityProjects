using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class TutorialModule_DynamicHand : TutorialModule
    {

        [SerializeField] private TransformSpaceType TransformSpace;

        public List<HandPointStruct> Points = new List<HandPointStruct>();

        [SerializeField] private float speed = 1;
        [SerializeField] private float waitingTime = .5f;
        [Space(10f)]
        [SerializeField] private float waitTimeForReplay = 1f;
        [SerializeField] private bool hideBeforeReplay;

        [SerializeField] private Sprite normalHand;
        [SerializeField] private Sprite clickHand;
        [SerializeField] private SpriteRenderer hand;
        [SerializeField] private Image handImage;

        private Coroutine loopCoroutine;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Points.Count > 0 && Points[0].Point != null)
            {
                transform.position = Points[0].Point.position + Points[0].Offset;
            }
        }
#endif

        public override IEnumerator ActiveTheModuleEnum()
        {
            if (loopCoroutine != null) StopCoroutine(loopCoroutine);
            loopCoroutine = StartCoroutine(StartLoop());

            yield return new WaitForEndOfFrame();
        }

        private IEnumerator StartLoop()
        {

            if (Points.Count > 0 && Points[0].Point != null)
            {
                transform.position = Points[0].Point.position + Points[0].Offset;
            }
            else
            {
                TutorialManager.Instance.DebugLog("Point list is null!", gameObject, DebugType.Error);
                yield break;
            }

            Vector3 handScale = TransformSpace == TransformSpaceType.ThreeD ? hand.transform.localScale : handImage.transform.localScale;
            if (hideBeforeReplay)
            {
                if (TransformSpace == TransformSpaceType.ThreeD)
                    hand.transform.localScale = Vector3.one * .001f;
                else
                    handImage.transform.localScale = Vector3.one * .001f;

                float t0 = 0;
                while (t0 < 1)
                {
                    t0 += Time.deltaTime * (2f);
                    if (transform == null) break;

                    if (TransformSpace == TransformSpaceType.ThreeD)
                        hand.transform.localScale = Vector3.Lerp(hand.transform.localScale, handScale, t0);
                    else
                        handImage.transform.localScale = Vector3.Lerp(handImage.transform.localScale, handScale, t0);

                    yield return new WaitForEndOfFrame();
                }
            }

            if (TransformSpace == TransformSpaceType.ThreeD)
                hand.sprite = normalHand;
            else
                handImage.sprite = normalHand;


            while (true)
            {
                for (int i = 0; i < Points.Count; i++)
                {

                    float t = 0;
                    while (t < 1)
                    {
                        t += Time.deltaTime * (speed / 20f);
                        transform.position = Vector3.Lerp(transform.position, Points[i].Point.position + Points[i].Offset, t);
                        if (Vector3.Distance(transform.position, Points[i].Point.position + Points[i].Offset) < .025f) break;
                        yield return new WaitForEndOfFrame();
                    }

                    switch (Points[i].HandEventType)
                    {
                        case HandEventType.Normal:
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = normalHand;
                            else
                                handImage.sprite = normalHand;
                            break;
                        case HandEventType.Holding:
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = clickHand;
                            else
                                handImage.sprite = clickHand;
                            break;
                        case HandEventType.Click:
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = clickHand;
                            else
                                handImage.sprite = clickHand;
                            yield return new WaitForSeconds(.25f);
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = normalHand;
                            else
                                handImage.sprite = normalHand;

                            break;
                        case HandEventType.DoubleClick:
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = clickHand;
                            else
                                handImage.sprite = clickHand;
                            yield return new WaitForSeconds(.25f);
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = normalHand;
                            else
                                handImage.sprite = normalHand;
                            yield return new WaitForSeconds(.15f);
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = clickHand;
                            else
                                handImage.sprite = clickHand;
                            yield return new WaitForSeconds(.25f);
                            if (TransformSpace == TransformSpaceType.ThreeD)
                                hand.sprite = normalHand;
                            else
                                handImage.sprite = normalHand;
                            break;
                        default:
                            break;
                    }

                    yield return new WaitForSeconds(waitingTime);

                }


                float t1 = 0;
                if (hideBeforeReplay)
                {
                    while (t1 < 1)
                    {
                        t1 += Time.deltaTime * (2f);
                        if (transform == null) break;
                        if (TransformSpace == TransformSpaceType.ThreeD)
                            hand.transform.localScale = Vector3.Lerp(hand.transform.localScale, handScale * .01f, t1);
                        else
                            handImage.transform.localScale = Vector3.Lerp(handImage.transform.localScale, handScale * .01f, t1);
                        yield return new WaitForEndOfFrame();
                    }

                    transform.position = Points[0].Point.position + Points[0].Offset;
                }

                if (TransformSpace == TransformSpaceType.ThreeD)
                    hand.sprite = normalHand;
                else
                    handImage.sprite = normalHand;
                yield return new WaitForSeconds(waitTimeForReplay);
                if (TransformSpace == TransformSpaceType.ThreeD) hand.gameObject.SetActive(true); else handImage.gameObject.SetActive(true);

                t1 = 0;
                if (hideBeforeReplay)
                {

                    while (t1 < 1)
                    {
                        t1 += Time.deltaTime * (2f);
                        if (transform == null) break;

                        if (TransformSpace == TransformSpaceType.ThreeD)
                            hand.transform.localScale = Vector3.Lerp(hand.transform.localScale, handScale, t1);
                        else
                            handImage.transform.localScale = Vector3.Lerp(handImage.transform.localScale, handScale, t1);

                        yield return new WaitForEndOfFrame();
                    }

                }
                else
                {
                    while (t1 < 1)
                    {
                        t1 += Time.deltaTime * (speed );
                        transform.position = Vector3.Lerp(transform.position, Points[0].Point.position + Points[0].Offset, t1); print("a");
                        if (Vector3.Distance(transform.position, Points[0].Point.position + Points[0].Offset) < .025f) break;
                        yield return new WaitForEndOfFrame();
                    }
                }


                yield return new WaitForEndOfFrame();
            }





        }
    }
}