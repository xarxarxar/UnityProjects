using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NINESOFT.TUTORIAL_SYSTEM;

namespace NINESOFT.TUTORIAL_SYSTEM.Samples
{
    public class CandyManager : MonoBehaviour
    {
        public static CandyManager Instance;
        private int score;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Candy[] Candies;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void CheckMatch(float x, float y)
        {
            RaycastHit2D[][] raycastHits = new RaycastHit2D[2][];
            raycastHits[0] = Physics2D.RaycastAll(new Vector2(-15f, y), Vector2.right, 40f);
            raycastHits[1] = Physics2D.RaycastAll(new Vector2(x, 15f), Vector2.down, 40f);

            List<Candy> candies = new List<Candy>();
            for (int a = 0; a < raycastHits.Length; a++)
            {
                candies.Clear();
                for (int i = 0; i < raycastHits[a].Length; i++)
                {
                    if (raycastHits[a][i].collider.CompareTag("Candy"))
                    {
                        candies.Add(raycastHits[a][i].collider.GetComponent<Candy>());
                    }
                }

                for (int i = 0; i < candies.Count; i++)
                {
                    if (i > 1)
                    {

                        if (candies[i - 2].TryGetComponent<Candy>(out var c1) &&
                            candies[i - 1].TryGetComponent<Candy>(out var c2) &&
                            candies[i].TryGetComponent<Candy>(out var c3))
                        {
                            if (c1.MyCandyType == c2.MyCandyType && c1.MyCandyType == c3.MyCandyType)
                            {
                                Matched(c1, c2, c3);
                            }
                        }
                    }
                }

            }
        }

        private void Matched(Candy c1, Candy c2, Candy c3)
        {

            if (TutorialManager.Instance.StageCompleted(1, 0))
            {

            }
            else if (TutorialManager.Instance.StageCompleted(1, 1))
            {

            }


            score += 10;
            scoreText.SetText("Your score: " + score.ToString());

            StartCoroutine(DestroyCandy(c1));
            StartCoroutine(DestroyCandy(c2));
            StartCoroutine(DestroyCandy(c3));

            StartCoroutine(SpawnNewRandomCandy(c1.transform.position));
            StartCoroutine(SpawnNewRandomCandy(c2.transform.position));
            StartCoroutine(SpawnNewRandomCandy(c3.transform.position));

        }

        private IEnumerator SpawnNewRandomCandy(Vector2 pos)
        {
            yield return new WaitForSeconds(Random.Range(.4f, .6f));

            var col = Physics2D.OverlapBox(pos, new Vector2(.015f, .015f), 0);
            if (col != null)
            {
                if (col.CompareTag("Candy"))
                {
                    yield break;
                }
            }

            Candy createdCandy = Instantiate(Candies[Random.Range(0, Candies.Length - 1)]);
            BoxCollider2D collider = createdCandy.GetComponent<BoxCollider2D>();
            collider.enabled = false;

            createdCandy.transform.position = new Vector3(pos.x, pos.y, 1);
            createdCandy.transform.localScale = Vector3.one * .0001f;


            Vector3 candyScale =Vector3.one;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 2f;
                createdCandy.transform.localScale = Vector3.Lerp(createdCandy.transform.localScale, candyScale, t);
                yield return new WaitForEndOfFrame();
            }

            collider.enabled = true;

        }

        private IEnumerator DestroyCandy(Candy candy)
        {
            candy.DestroyMe();
            Vector3 minScale = Vector3.one * .0001f;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 2f;
                candy.transform.localScale = Vector3.Lerp(candy.transform.localScale, minScale, t);
                yield return new WaitForEndOfFrame();
            }
            candy.gameObject.SetActive(false);
        }

    }
}
