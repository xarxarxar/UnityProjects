using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localpush : MonoBehaviour
{



    string title = "Break the brick with joy!";
    string contents = "Play right now!";


    void Start()
    {
        Init_Notification();
    }

    public void Init_Notification()
    {
#if UNITY_IOS
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(
         UnityEngine.iOS.NotificationType.Alert |
         UnityEngine.iOS.NotificationType.Badge |
         UnityEngine.iOS.NotificationType.Sound
         );
#elif UNITY_ANDROID


#endif

    }

    //로컬푸쉬 설정하기
    public void SetNotification(string title, string body, int second)
    {
#if UNITY_IOS
        UnityEngine.iOS.LocalNotification noti = new UnityEngine.iOS.LocalNotification();
        noti.fireDate = System.DateTime.Now.AddSeconds(second);
        noti.alertAction = title;
        noti.alertTitle = title;
        noti.alertBody = body;
        noti.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
        //noti.applicationIconBadgeNumber = 1;

        //Every day
        noti.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;

        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(noti);

#elif UNITY_ANDROID

#endif
    }

    //Canceling local push
    public void CancelAllNotification()
    {
#if UNITY_IOS
        UnityEngine.iOS.LocalNotification noti = new UnityEngine.iOS.LocalNotification();
        noti.fireDate = System.DateTime.Now;
        //noti.applicationIconBadgeNumber = -1;
        noti.hasAction = false;
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(noti);

        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();

#elif UNITY_ANDROID

#endif
    }

    void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus == false)
        {
            //Local push reset
            CancelAllNotification();

            //Time calculation
            double dtime = GetLeftTimeDay().TotalSeconds;
            int time = (int) dtime;

            ///Local push reset
            SetNotification(title, contents, time);
        }
        else
        {
            //Cancel local push
            CancelAllNotification();
        }
    }

    void OnApplicationQuit()
    {
        //Cancel local push
        CancelAllNotification();

        //Time calculation
        double dtime = GetLeftTimeDay().TotalSeconds;
        int time = (int) dtime;

        //Local push reset
        SetNotification(title, contents, time);
    }


    //Time calculation
    public System.TimeSpan GetLeftTimeDay()
    {
        System.DateTime _time = new System.DateTime(
            System.DateTime.UtcNow.Year, System.DateTime.UtcNow.Month, System.DateTime.UtcNow.Day + 1, 0, 0, 0);
        System.TimeSpan _dateOnlyDifference = _time.Date.Subtract(System.DateTime.UtcNow);
        return _dateOnlyDifference;
    }
}
