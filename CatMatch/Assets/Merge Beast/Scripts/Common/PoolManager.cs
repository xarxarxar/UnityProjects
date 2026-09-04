using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Spine.Unity;

namespace MergeBeast
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;
        public static PoolManager Instance => _instance;

        public List<FXDefine> _poolData;

        private Dictionary<EnumDefine.FXTYPE, Dictionary<int, List<ParticleSystem>>> _pool;

        public UnityAction<float, int> ActionLoading;

        [SerializeField] private List< SkeletonAnimation> _prfHit;
        private Dictionary<int,List< SkeletonAnimation>> _poolHits;


        // Use this for initialization
        private void Awake()
        {
            if (_instance == null) _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        IEnumerator Start()
        {
            _pool = new Dictionary<EnumDefine.FXTYPE, Dictionary<int, List<ParticleSystem>>>();
            int total = 0;
            for (int t = 0; t < _poolData.Count; t++)
            {
                total += _poolData[t].Particles.Count * _poolData[t].Amount;
            }

            float count = 0f;
            for (int t = 0; t < _poolData.Count; t++)
            {
                Dictionary<int, List<ParticleSystem>> pol = new Dictionary<int, List<ParticleSystem>>();
                GameObject obj = new GameObject($"{_poolData[t].Name}");
                obj.transform.SetParent(this.transform);
                obj.transform.localPosition = Vector3.zero;

                for (int i = 0; i < _poolData[t].Particles.Count; i++)
                {
                    List<ParticleSystem> listParticle = new List<ParticleSystem>();
                    for (int j = 0; j < _poolData[t].Amount; j++)
                    {
                        ParticleSystem particle = Instantiate(_poolData[t].Particles[i], obj.transform);
                        listParticle.Add(particle);
                        particle.gameObject.SetActive(false);

                        count++;
                        ActionLoading?.Invoke(count, total);
                            yield return null;
                    }
                    pol.Add(i, listParticle);
                }

                _pool.Add(_poolData[t].FxType, pol);
            }

            GameObject poolHit = new GameObject("Pool Hit");
            poolHit.transform.SetParent(this.transform);
            _poolHits = new Dictionary<int,List< SkeletonAnimation>>();
            for(int i = 0; i < _prfHit.Count; i++)
            {
                List<SkeletonAnimation> fxs = new List<SkeletonAnimation>();
                for (int j = 0; j < 5; j++)
                {
                    SkeletonAnimation skel = Instantiate(_prfHit[i], poolHit.transform);
                    fxs.Add(skel);
                    skel.gameObject.SetActive(false);
                    yield return new WaitForEndOfFrame();
                }
                _poolHits.Add(i, fxs);
            }

            float timeCountAds = 0f;
            while(timeCountAds < 3f )
            {
                timeCountAds += Time.deltaTime;
                if (AdsManager.Instance.IsLoaded())
                {
                    timeCountAds = 10;
                }
                yield return null;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(StringDefine.SCENE_MAIN, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = true;
            yield return asyncLoad;

        }

        public void PlayFXGetHit(int id, Vector2 position)
        {
            //    var pool = _pool[EnumDefine.FXTYPE.GET_HIT];
            //    this.PlayFX(id, position, pool);

            List<SkeletonAnimation> fxs = _poolHits[Random.Range(0, _poolHits.Count)];
            for(int i = 0; i < fxs.Count; i++)
            {
                if (!fxs[i].gameObject.activeInHierarchy)
                {
                    fxs[i].gameObject.SetActive(true);
                    fxs[i].transform.position = position;

                    fxs[i].AnimationState.Complete += delegate {
                        fxs[i].gameObject.SetActive(false);
                    };

                    return;
                }
            }
        }


        public void PlayFXHealHP( Vector2 position)
        {
            var pool = _pool[EnumDefine.FXTYPE.HEALING_HP];
            this.PlayFX(0, position, pool);
        }

        public void PlayFXEnemyDead(Vector3 postion)
        {
            var pool = _pool[EnumDefine.FXTYPE.ENEMY_DEAD];
            this.PlayFX(0, postion, pool);
        }

        public void PlayFXBeastMerge(int id, Vector2 postion)
        {
            var pool = _pool[EnumDefine.FXTYPE.BEAST_MERGE];
            this.PlayFX(id, postion, pool);
        }

        public void PlayFXNewBeast(int id)
        {
            var pool = _pool[EnumDefine.FXTYPE.NEW_BEAST];
            this.PlayFX(id, Vector2.zero, pool);
        }

        private void PlayFX(int id, Vector3 position, Dictionary<int, List<ParticleSystem>> pool)
        {
            int n = id % pool.Count;
            var fxs = pool[n];

            for (int i = 0; i < fxs.Count; i++)
            {
                if (!fxs[i].gameObject.activeInHierarchy)
                {
                    fxs[i].gameObject.SetActive(true);
                    fxs[i].transform.position = position;
                    fxs[i].Play();
                    //fxs[i].enableEmission = true;
                    if(gameObject.activeSelf)
                        StartCoroutine(IETakeFX(fxs[i].main.duration, fxs[i].gameObject));
                    return;
                }
            }
        }

        private IEnumerator IETakeFX(float time,GameObject fx)
        {
            yield return new WaitForSeconds(time);
            fx.SetActive(false);
        }
    }

    [System.Serializable]
    public class FXDefine
    {
        public string Name;
        public int Amount;
        public EnumDefine.FXTYPE FxType;
        public List<ParticleSystem> Particles;
    }
}
