/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System.Collections;
using UnityEngine;

namespace GifPlayer
{
    public class UnityGif : SequenceFrames
    {
        [Header("[如果你的图像有异常，请用PhotoShop打开GIF，文件-导出-Web格式-gif]")]
        [Header("[If your image is abnormal, please open GIF with PhotoShop, file-export-WebFormat-gif]")]
        [Header("[遵循GIF89A标准]")]
        [Header("[Follow GIF89A standard]")]
        [Header("[GIF的文件流：将 .gif 变为 .bytes 后便可拖拽]")]
        [Header("[Gif Serialize Field: Copy and rename .gif to .bytes]")]
        [Header("[Priority Low]")]
        public TextAsset GifAsset;

        [Header("[预加载状态 通过序列帧赋值实现]")]
        [Header("[Preload State,Completed by valuing Frames]")]
        [DisplayOnly]
        public bool IsPreloaded;

        IEnumerator DecodeAndPlay()
        {
            if (!GifAsset)
            {
                Debug.LogError("UnityGif@" + name + ": Gif asset is null, please check gif asset SerializeField !");
                yield break;
            }

            var gifData = new GifData(GifAsset.bytes);
            yield return GifHelper.GetFramesEnumerator(gifData);
            if (gifData.Frames == null)
            {
                Debug.Log("Gif decoding by Coroutine does not effect, use no-Coroutine instead !");
                GifHelper.GetFramesVoid(gifData);
            }
            Frames = gifData.Frames;
            StartCoroutine(PlayNextFrame());
        }

        protected override void OnEnable()
        {
            if (Frames.Length == 0)
                StartCoroutine(DecodeAndPlay());
            else
                base.OnEnable();
        }
    }
}