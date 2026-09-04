using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    public static class DevPanelEnabler
    {
        private const string MenuName = "Actions/Show Dev Panel";
        private const string SettingName = "IsDevPanelDisplayed";

        private static GameObject panel;

        public static bool IsDevPanelDisplayed()
        {
#if UNITY_EDITOR
            return IsDevPanelDisplayedPrefs;
#else
            return false;
#endif
        }

        public static void RegisterPanel(GameObject panel)
        {
            DevPanelEnabler.panel = panel;
            panel.SetActive(IsDevPanelDisplayed());
        }

#if UNITY_EDITOR
        private static bool IsDevPanelDisplayedPrefs
        {
            get { return EditorPrefs.GetBool(SettingName, false); }
            set { EditorPrefs.SetBool(SettingName, value); }
        }

        [MenuItem(MenuName, priority = 201)]
        private static void ToggleAction()
        {
            bool devPanelState = IsDevPanelDisplayedPrefs;
            IsDevPanelDisplayedPrefs = !devPanelState;

            if(panel != null)
            {
                panel.SetActive(IsDevPanelDisplayedPrefs);
            }
        }

        [MenuItem(MenuName, true, priority = 201)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(MenuName, IsDevPanelDisplayedPrefs);

            return true;
        }
#endif
    }
}