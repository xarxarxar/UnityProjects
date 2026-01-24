using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace NINESOFT.CORE.EDITOR
{
#if UNITY_EDITOR
    [System.Serializable]
    public class EditorWebAssetItem
    {
        public string ID;
        public string Name;
        public string Description;
        public string ImageUri;
        public string WebUri;
    }

    public delegate void WebProgressAction(float percent);
    public delegate void WebCompleteAction();

    public class WebRequestManager : MonoBehaviour
    {

        private static WebRequestManager instance;
        private static GameObject thisObject;

        public static string FolderPath => (@"\NINESOFT_ASSETS\NineSoft_Core\Editor\OurAssets\Resources");
        public static string PATH => (Application.dataPath + FolderPath);

        public WebProgressAction OnProgress;
        public WebCompleteAction OnComplete;
        private int maxSteps;
        private int curSteps;

        public static WebRequestManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (thisObject != null) { instance = null; DestroyImmediate(thisObject); }
                    thisObject = new GameObject("NS_WebRequest");
                    instance = thisObject.AddComponent<WebRequestManager>();
                }
                return instance;
            }
        }

        public void GetData_OurOtherAssets()
        {
            StartCoroutine(GetData_OurOtherAssets_Enum());
        }
        private IEnumerator GetData_OurOtherAssets_Enum()
        {
            string url = "https://9ninesoft9.blogspot.com/p/data.html";
            string fileName = "data.txt";
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                DestroyMe();
            }
            else
            {

                string data = www.downloadHandler.text;
                data = ExtractSpecificText(data);

                string path = Path.Combine(PATH, fileName);
                File.WriteAllText(path, data);


                string[] splitItems = data.Split("[s;]");

                maxSteps = splitItems.Length + 1;
                UpdateProgress();


                for (int i = 0; i < splitItems.Length; i++)
                {
                    if (splitItems[i].Length < 5) continue;
                    EditorWebAssetItem item = JsonUtility.FromJson<EditorWebAssetItem>(splitItems[i]);
                    StartCoroutine(DownloadAndSaveImage(item));
                }

            }
        }


        IEnumerator DownloadAndSaveImage(EditorWebAssetItem item)
        {
            string imageUrl = item.ImageUri;
            string fileName = item.ID + ".png";

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                DestroyMe();
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                byte[] imageBytes = texture.EncodeToPNG();

                string path = Path.Combine(PATH, fileName);

                File.WriteAllBytes(path, imageBytes);
            }
           
            UpdateProgress();
        }

     

        private void UpdateProgress()
        {
            curSteps++;
            float percent = ((float)curSteps / (float)maxSteps);
            OnProgress?.Invoke(percent);

            if (percent >= .99f)
            {
                DestroyMe();
            }
        }
        private void DestroyMe()
        {
            AssetDatabase.Refresh();
            OnComplete?.Invoke();
            if (thisObject != null) { instance = null; DestroyImmediate(thisObject); }
        }

        private string ExtractSpecificText(string htmlContent)
        {
            htmlContent = htmlContent.Trim();

            string startTag = "<data>";
            string endTag = "</data>";

            int startIndex = htmlContent.IndexOf(startTag) + startTag.Length;
            int endIndex = htmlContent.IndexOf(endTag, startIndex);

            if (startIndex > -1 && endIndex > -1)
            {
                return htmlContent.Substring(startIndex, endIndex - startIndex);
            }

            return "Not found!";
        }
    }
#endif
}

