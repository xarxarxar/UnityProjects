/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GifPlayer
{
    public class SequenceFrames : MonoBehaviour
    {
        [Header("[Priority High]")]
        public SequenceFrame[] Frames;

        [SerializeField]
        [Header("[Priority Middle]")]
        Texture2D[] _simpleFrames;

        [SerializeField]
        float _simpleFramesIntervalSeconds;

        [SerializeField]
        bool _loop = true;

        SpriteRenderer _rendererCanvas;
        Image _imageCanvas;
        RawImage _rawCanvas;

        //★★★★★ awake before enable , start after enable
        protected virtual void Awake()
        {
            if (Frames.Length == 0 && _simpleFrames.Length > 0)
            {
                Frames = new SequenceFrame[_simpleFrames.Length];
                for (int index = 0; index < _simpleFrames.Length; index++)
                    Frames[index] = new SequenceFrame(_simpleFrames[index], _simpleFramesIntervalSeconds);
            }

            _rendererCanvas = GetComponent<SpriteRenderer>();
            _imageCanvas = GetComponent<Image>();
            _rawCanvas = GetComponent<RawImage>();
        }

        int _frameIndex = 0;

        protected IEnumerator PlayNextFrame()
        {
            _frameIndex %= Frames.Length;

            if (_rendererCanvas)
                _rendererCanvas.sprite = Frames[_frameIndex].Sprite;

            if (_imageCanvas)
                _imageCanvas.sprite = Frames[_frameIndex].Sprite;

            if (_rawCanvas)
                _rawCanvas.texture = Frames[_frameIndex].Texture;

            yield return Frames[_frameIndex].WaitForSeconds;

            _frameIndex++;

            if (!_loop && _frameIndex == Frames.Length)
                yield break;

            StartCoroutine(PlayNextFrame());
        }

        protected virtual void OnEnable()
        {
            if (Frames != null && Frames.Length > 0)
                StartCoroutine(PlayNextFrame());
        }

        void OnDisable()
        {
            _frameIndex = 0;
        }
    }
}