using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class ToolsMenu : MonoBehaviour
{
    [MenuItem("Tools/Ultimate Slider/Add Slider Manager Object")]
    static void uGUIAnchorAroundObject()
    {
        GameObject newObject = new GameObject();
        newObject.name = "Slider Manager";
        newObject.AddComponent<UltimateSlider.SliderManager>();
        Selection.activeObject = newObject.transform;
    }

    [MenuItem("Tools/Ultimate Slider/Documents")]
    static void Documents()
    {
        Application.OpenURL("");
    }

    [MenuItem("Tools/Ultimate Slider/API")]
    static void API()
    {
        Application.OpenURL("");
    }
}
