using UnityEngine;

namespace GifPlayer
{
    public class GifLoading : MonoBehaviour
    {
        [SerializeField]
        UnityGif _target;

        void Update()
        {
            if (_target.Frames.Length > 0)
                gameObject.SetActive(false);
        }
    }
}
