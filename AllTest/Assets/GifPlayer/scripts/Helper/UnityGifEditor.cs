/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GifPlayer
{
    [CustomEditor(typeof(UnityGif))]
    public class UnityGifEditor : Editor
    {
        private const string _savePath = "Assets/GifPlayer/preload/{0}/{1}";

        public static void FileWrite(string path, byte[] bytes)
        {
            var stream = File.OpenWrite(path);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
        }

        public static SequenceFrame[] Preload(TextAsset gifAsset)
        {
            //Create folder
            var saveFolder = string.Format(_savePath, gifAsset.name, "");
            Directory.CreateDirectory(saveFolder);

            //Decode
            var memoryFrames = GifHelper.GetFrames(gifAsset);
            var delaySecondsArray = string.Join(",", memoryFrames.Select(m => m.DelaySeconds.ToString()).ToArray());
            for (var index = 0; index < memoryFrames.Length; index++)
                FileWrite(string.Format(_savePath, gifAsset.name, index + ".png"), memoryFrames[index].Sprite.texture.EncodeToPNG());
            FileWrite(string.Format(_savePath, gifAsset.name, "delays.txt"), Encoding.UTF8.GetBytes(delaySecondsArray));

            //Refresh Adb
            AssetDatabase.Refresh();

            //Load Adb
            var delaySecondsAsset = AssetDatabase.LoadAssetAtPath(string.Format(_savePath, gifAsset.name, "delays.txt"), typeof(TextAsset)) as TextAsset;
            var delaySeconds = delaySecondsAsset.text.Split(',');
            var adbFrames = new SequenceFrame[delaySeconds.Length];
            for (var index = 0; index < adbFrames.Length; index++)
            {
                var texture = AssetDatabase.LoadAssetAtPath(string.Format(_savePath, gifAsset.name, index + ".png"), typeof(Texture2D)) as Texture2D;
                adbFrames[index] = new SequenceFrame(texture, float.Parse(delaySeconds[index]));
            }
            return adbFrames;
        }

        public static void DeletePreload(TextAsset gifAsset)
        {
            var saveFolder = string.Format(_savePath, gifAsset.name, "");
            FileUtil.DeleteFileOrDirectory(saveFolder);
            AssetDatabase.Refresh();
        }

        UnityGif _target;

        private void OnEnable()
        {
            _target = (UnityGif)target;
        }

        void SaveScene(string message)
        {
            _target.gameObject.SetActive(false);
            EditorUtility.DisplayDialog("Important!!!", message + "\r\n\r\nThe picture has been hidden, please manual check to show, in order to trigger the storage !"
                + "\r\n\r\n图片已经隐藏，请手动勾选显示，以便触发存储！", "OK");
            //EditorUtility.SetDirty(_target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Preload 预加载"))
            {
                _target.Frames = Preload(_target.GifAsset);
                _target.IsPreloaded = true;
                SaveScene("Preloaded ! Frames Valued !\r\n\r\n预加载完毕！序列帧已赋值！");
            }

            if (GUILayout.Button("Delete Preload 删除预加载"))
            {
                _target.Frames = null;
                _target.IsPreloaded = false;
                DeletePreload(_target.GifAsset);
                SaveScene("Preload Deleted ! Frames Valued Null !\r\n\r\n预加载已删除！序列帧已清空！");
            }
        }
    }
}
#endif