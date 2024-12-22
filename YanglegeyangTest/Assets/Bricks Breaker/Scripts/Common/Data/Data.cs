using UnityEngine;

public class Data
{
    public static string version = "1.0.0";

    public static float VolumeEffect
    {
        get { return PlayerPrefs.GetFloat("VolumeEffect", 1f); }
        set { PlayerPrefs.SetFloat("VolumeEffect", value); }
    }

    public static float VolumeMusic
    {
        get { return PlayerPrefs.GetFloat("VolumeMusic", 1f); }
        set { PlayerPrefs.SetFloat("VolumeMusic", value); }
    }

    //Sound File Path
    public static string path_sound = "Sounds/";

    //SceneLoad
    public static string scene_splash = "0_Splash";
    public static string scene_title = "1_Title";
    public static string scene_home = "2_Home";
    public static string scene_play = "3_Play";
    public static string scene_result = "4_Result";

    //Product Web Url
    public const string url_facebook = "https://www.facebook.com/layerlab/";



#if UNITY_ANDROID
    public const string url_app = "https://play.google.com/store/apps/details?id=com.layerlab.cartoonbricks";
#elif UNITY_IOS || UNITY_EDITOR
    public const string url_app = "https://itunes.apple.com/app/cartoon-bricks/id1465573332?l=ko&ls=1&mt=8";
#endif


#if UNITY_ANDROID
    public const string url_othergame = "https://play.google.com/store/apps/dev?id=5635426190248131344&hl=ko";
#elif UNITY_IOS || UNITY_EDITOR
    public const string url_othergame = "https://itunes.apple.com/kr/developer/layer/id731338523";
#endif

    public static string GetSystemLanguageLetter()
    {
        SystemLanguage lang = Application.systemLanguage;
        string res = "US";
        
        switch (lang)
        {
            case SystemLanguage.English:
                res = "us";
                break;
            case SystemLanguage.Korean:
                res = "kr";
                break;
            case SystemLanguage.Chinese:
                res = "zh";
                break;
            case SystemLanguage.Thai:
                res = "th";
                break;
            case SystemLanguage.Japanese:
                res = "jp";
                break;
            case SystemLanguage.Spanish:
                res = "es";
                break;
            case SystemLanguage.Portuguese:
                res = "pt";
                break;
            case SystemLanguage.German:
                res = "de";
                break;
            case SystemLanguage.Russian:
                res = "ru";
                break;
            case SystemLanguage.French:
                res = "fr";
                break;
            case SystemLanguage.Italian:
                res = "it";
                break;
            case SystemLanguage.Turkish:
                res = "tr";
                break;
            case SystemLanguage.Greek:
                res = "el";
                break;
            case SystemLanguage.Swedish:
                res = "sv";
                break;
            case SystemLanguage.Indonesian:
                res = "in";
                break;
            case SystemLanguage.Ukrainian:
                res = "uk";
                break;
            case SystemLanguage.Polish:
                res = "pl";
                break;
            case SystemLanguage.Romanian:
                res = "ro";
                break;
            case SystemLanguage.Dutch:
                res = "nl";
                break;
            case SystemLanguage.Afrikaans:
                res = "af";
                break;
            case SystemLanguage.Arabic:
                res = "ar";
                break;
            case SystemLanguage.Basque:
                res = "eu";
                break;
            case SystemLanguage.Belarusian:
                res = "by";
                break;
            case SystemLanguage.Bulgarian:
                res = "bg";
                break;
            case SystemLanguage.Catalan:
                res = "ca";
                break;
            case SystemLanguage.Czech:
                res = "cs";
                break;
            case SystemLanguage.Danish:
                res = "da";
                break;
            case SystemLanguage.Estonian:
                res = "et";
                break;
            case SystemLanguage.Faroese:
                res = "fo";
                break;
            case SystemLanguage.Finnish:
                res = "fi";
                break;
            case SystemLanguage.Hebrew:
                res = "iw";
                break;
            case SystemLanguage.Hungarian:
                res = "hu";
                break;
            case SystemLanguage.Icelandic:
                res = "is";
                break;
            case SystemLanguage.Latvian:
                res = "lv";
                break;
            case SystemLanguage.Lithuanian:
                res = "lt";
                break;
            case SystemLanguage.Norwegian:
                res = "no";
                break;
            case SystemLanguage.SerboCroatian:
                res = "sh";
                break;
            case SystemLanguage.Slovak:
                res = "sk";
                break;
            case SystemLanguage.Slovenian:
                res = "sl";
                break;
            case SystemLanguage.Unknown:
                res = "un";
                break;
            case SystemLanguage.Vietnamese:
                res = "vi";
                break;
        }

        return res;
    }



    
}
