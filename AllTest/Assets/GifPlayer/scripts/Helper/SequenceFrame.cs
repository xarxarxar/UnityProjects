/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System;
using UnityEngine;

namespace GifPlayer
{
    [Serializable]
    public class SequenceFrame
    {
        public Texture2D Texture;


        private Sprite _sprite;

        private static Vector2 _pivotCenter = new Vector2(0.5f, 0.5f);

        public Sprite Sprite
        {
            get
            {
                if (!_sprite)
                {
                    _sprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), _pivotCenter);
                    _sprite.name = Texture.name;
                }
                return _sprite;
            }
        }


        public float DelaySeconds;


        private WaitForSeconds _waitForSeconds;

        public static WaitForSeconds DefaultWaitForSeconds = new WaitForSeconds(0.16f);

        public WaitForSeconds WaitForSeconds
        {
            get
            {
                if (_waitForSeconds == null)
                {
                    if (DelaySeconds > 0)
                        _waitForSeconds = new WaitForSeconds(DelaySeconds);
                    else
                        _waitForSeconds = DefaultWaitForSeconds;
                }
                return _waitForSeconds;
            }
        }


        public SequenceFrame(Texture2D texture, float delaySeconds)
        {
            Texture = texture;
            DelaySeconds = delaySeconds;
        }
    }
}