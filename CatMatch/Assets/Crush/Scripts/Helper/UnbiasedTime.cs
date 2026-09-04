//using UnityEngine;
//using System;
//using System.Collections;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Net;
//using System.Net.Sockets;

//public class UnbiasedTime : MonoBehaviour
//{

//    private static UnbiasedTime instance;
//    public static UnbiasedTime Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = FindObjectOfType<UnbiasedTime>();
//                if (instance != null)
//                    DontDestroyOnLoad(instance.gameObject);
//            }
//            return instance;
//        }
//    }

//    // Estimated difference in seconds between device time and real world time
//    // timeOffset = deviceTime - worldTime;
//    [HideInInspector]
//    public long timeOffset = 0;

//    [HideInInspector]
//    public long ntpTimeOffset = 0;

//    private const string TimeOffsetNativeAndNetworkKey = "TimeOffsetNativeAndNetworkKey";

//    private const string CURRENT_TIME_ZONE = "CURRENT_TIME_ZONE";

//    private static float TimeOffsetNativeAndNetwork
//    {
//        get
//        {
//            return PlayerPrefs.GetFloat(TimeOffsetNativeAndNetworkKey);
//        }

//        set
//        {
//            PlayerPrefs.SetFloat(TimeOffsetNativeAndNetworkKey, value);
//            PlayerPrefs.Save();
//        }
//    }

//    private float CurrentTimeZone
//    {
//        get
//        {
//            return PlayerPrefs.GetFloat(CURRENT_TIME_ZONE, 20);
//        }
//        set
//        {
//            PlayerPrefs.SetFloat(CURRENT_TIME_ZONE, value);
//            PlayerPrefs.Save();
//        }
//    }
//    private float currentTimeZone;

//    private Thread socketThread = null;
//    private UdpClient client;

//    private bool isNetworkTime = false;
//    private bool isUpdateTime = false;

//    void Awake()
//    {
//        if (instance)
//        {
//            DestroyImmediate(gameObject);
//            return;
//        }

//        instance = this;
//        DontDestroyOnLoad(gameObject);
//        isNetworkTime = false;
//        SessionStart();
//        if (CurrentTimeZone >= 19)
//            CurrentTimeZone = (float)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours;
//        currentTimeZone = CurrentTimeZone;
//    }

//    void OnEnable()
//    {
//        // var lastOnLineTimeStr = PlayerPrefs.GetString(CrushStringHelper.LastOnLineDateTime, "");
//        // if (string.IsNullOrEmpty(lastOnLineTimeStr))
//        // {
//        //     lastOnLineTimeStr = UnbiasedTime.Instance.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
//        //     Debug.Log("lastOnLineTimeStr:" + lastOnLineTimeStr);
//        //     PlayerPrefs.SetString(CrushStringHelper.LastOnLineDateTime, lastOnLineTimeStr);
//        // }
//    }

//    void OnApplicationPause(bool pause)
//    {
//        //Debug.Log("time Now " + Now + "  UtcNow " + UtcNow + "  timeOffset " + timeOffset + "  ntpTimeOffset " + ntpTimeOffset + "  " + (timeOffset + TimeOffsetNativeAndNetwork));
//        if (pause)
//        {
//            SessionEnd();
//        }
//        else
//        {
//            SessionStart();
//        }
//    }

//    private void OnDestroy()
//    {
//        //PlayerPrefs.SetString("LAST_TIME_OPEN_GAME", Now.ToBinary().ToString());
//    }

//    void OnApplicationQuit()
//    {
//        SessionEnd();
//        if (socketThread != null)
//        {
//            socketThread.Abort();
//        }
//    }

//    private void Update()
//    {
//        if (isUpdateTime == true)
//        {
//            isUpdateTime = false;
//            TimeOffsetNativeAndNetwork = ntpTimeOffset - timeOffset;
//        }
//    }

//    public DateTime UtcNow
//    {
//        get
//        {
//#if HACK_TIME
//            return DateTime.UtcNow;
//#endif
//            if (isNetworkTime)
//            {
//                return DateTime.UtcNow.AddSeconds(-1.0f * (double)ntpTimeOffset);
//            }
//            else
//            {
//                return DateTime.UtcNow.AddSeconds(-1.0f * (timeOffset + TimeOffsetNativeAndNetwork));
//            }
//        }
//    }

//    // Returns estimated DateTime value taking into account possible device time changes
//    public DateTime Now
//    {
//        get
//        {
//#if HACK_TIME
//            return DateTime.Now;
//#endif
//            return UtcNow.AddSeconds(currentTimeZone * 3600);
//        }
//    }

//    // This method updates network time offset asynchronously. 
//    // It is called automatically on each session start event. 
//    // The Now() method will use network offset if it is available, otherwise 
//    // it will fallback to estimated offline real time from native plugin. 
//    public void UpdateNetworkTimeOffset()
//    {
//        if (socketThread != null && socketThread.IsAlive) return;

//        socketThread = new Thread(new ThreadStart(NtpImpl));
//        socketThread.IsBackground = true;
//        socketThread.Start();
//    }

//    // timeOffset value is cached for performance reasons (calls to native plugins can be expensive). 
//    // This method is used to update offset value in cases if you think device time was changed by user. 
//    // 
//    // However, time offset is updated automatically when app gets backgrounded or foregrounded. 
//    // 
//    public void UpdateTimeOffset()
//    {
//#if UNITY_ANDROID
//        UpdateTimeOffsetAndroid();
//#elif UNITY_IPHONE
//			UpdateTimeOffsetIOS();
//#endif
//    }

//    private void SessionStart()
//    {


//#if UNITY_ANDROID
//        StartAndroid();
//#elif UNITY_IPHONE
//			StartIOS();
//#endif
//        UpdateNetworkTimeOffset();
//    }

//    private void SessionEnd()
//    {
//#if UNITY_ANDROID
//        EndAndroid();
//#elif UNITY_IPHONE
//			EndIOS();
//#endif
//    }

//    private void NtpImpl()
//    {
//        const string ntpServer = "time.google.com";
//        var ntpData = new byte[48];
//        ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

//        try
//        {
//            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
//            var ipEndPoint = new IPEndPoint(addresses[0], 123);
//            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

//            socket.ReceiveTimeout = 5000; // milliseconds
//            socket.SendTimeout = 5000;
//            socket.Connect(ipEndPoint);
//            socket.Send(ntpData);
//            socket.Receive(ntpData);
//            socket.Close();

//            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
//            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

//            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
//            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

//            isNetworkTime = true;
//            ntpTimeOffset = (long)(DateTime.UtcNow - networkDateTime).TotalSeconds;
//            isUpdateTime = true;
//            //print("ntpTimeOffset " + ntpTimeOffset);
//        }
//        catch
//        {
//            Debug.Log("Get network time error " + ntpTimeOffset);
//        }
//    }

//    // Platform specific code
//    // 

//#if UNITY_IPHONE
//	[DllImport ("__Internal")]
//	private static extern void _vtcOnSessionStart();

//	[DllImport ("__Internal")]
//	private static extern void _vtcOnSessionEnd();
	
//	[DllImport ("__Internal")]
//	private static extern int _vtcTimestampOffset();


//	private void UpdateTimeOffsetIOS() {
//		if (Application.platform != RuntimePlatform.IPhonePlayer) {
//			return;
//		}

//		timeOffset = _vtcTimestampOffset();
//	}

//	private void StartIOS() {
//		if (Application.platform != RuntimePlatform.IPhonePlayer) {
//			return;
//		}

//		_vtcOnSessionStart();
//		timeOffset = _vtcTimestampOffset();
//	}

//	private void EndIOS() {
//		if (Application.platform != RuntimePlatform.IPhonePlayer) {
//			return;
//		}

//		_vtcOnSessionEnd();
//	}
//#endif


//#if UNITY_ANDROID
//    private void UpdateTimeOffsetAndroid()
//    {
//        if (Application.platform != RuntimePlatform.Android)
//        {
//            return;
//        }

//        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//        using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime"))
//        {
//            var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
//            if (playerActivityContext != null && unbiasedTimeClass != null)
//            {
//                timeOffset = unbiasedTimeClass.CallStatic<long>("vtcTimestampOffset", playerActivityContext);
//            }
//        }
//    }

//    private void StartAndroid()
//    {
//        if (Application.platform != RuntimePlatform.Android)
//        {
//            return;
//        }

//        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//        using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime"))
//        {
//            var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
//            if (playerActivityContext != null && unbiasedTimeClass != null)
//            {
//                unbiasedTimeClass.CallStatic("vtcOnSessionStart", playerActivityContext);
//                timeOffset = unbiasedTimeClass.CallStatic<long>("vtcTimestampOffset");
//            }
//        }
//    }

//    private void EndAndroid()
//    {
//        if (Application.platform != RuntimePlatform.Android)
//        {
//            return;
//        }

//        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//        using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime"))
//        {
//            var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
//            if (playerActivityContext != null && unbiasedTimeClass != null)
//            {
//                unbiasedTimeClass.CallStatic("vtcOnSessionEnd", playerActivityContext);
//            }
//        }
//    }
//#endif

//}