using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NINESOFT.CORE
{
#if UNITY_EDITOR
    public class NSPackageInfo
    {
        public string ID; //CG : complete games, TK : tool kits, GUI : ui or gui
        public string PackageName;

        public string Version;
        public string LastUpdateDate;

        public string DocLink;

        public string EditorWindowPath;

    }


    public class NSPackageManager : MonoBehaviour
    {
        private static NSPackageInfo[] Packages;

        public static void InitPackageInfos()
        {
            string[] datas = Directory.GetFiles(Application.dataPath, "nspackageinfo.json", SearchOption.AllDirectories);
            Packages = new NSPackageInfo[datas.Length];
            for (int i = 0; i < Packages.Length; i++)
            {
                string data = File.ReadAllText(datas[i]);
                Packages[i] = JsonUtility.FromJson<NSPackageInfo>(data);
            }
        }

        public static NSPackageInfo GetPackageInfo(string packageIdOrName)
        {
            if (Packages == null || Packages.Length == 0) InitPackageInfos();
            NSPackageInfo p = new NSPackageInfo { PackageName = "Not Found" };
            for (int i = 0; i < Packages.Length; i++)
            {
                if (Packages[i].ID == packageIdOrName || Packages[i].PackageName == packageIdOrName)
                {
                    p = Packages[i];
                    break;
                }
            }
            return p;
        }

        public static NSPackageInfo[] GetPackages()
        {
            return Packages;
        }
    }
#endif
}
