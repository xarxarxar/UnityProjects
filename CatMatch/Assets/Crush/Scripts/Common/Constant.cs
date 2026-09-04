using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Constant
{
    public static Vector3 Vector3Default = new Vector3(10_000, 10_000, 0);

    public static Dictionary<int, List<string>> SkillPairs = new Dictionary<int, List<string>>()
    {
        {1, new List<string>(){"skill01_1", "skill01_2", "skill01_3"}},
        {2, new List<string>(){"skill02_1", "skill02_2", "skill02_3"}},
        {3, new List<string>(){"skill03_1", "skill03_2", "skill03_3"}},
    };

    public static string AnimGetHitName = "get_hit";
    public static string AnimMoveName = "move";
    public static string AnimDieName = "die";

    public static List<int> medalByStars = new List<int>() { 10, 20, 50, 200, 600 };
    public static Dictionary<int, int> slotPrices = new Dictionary<int, int>
    {
        {6, 50},
        {7, 100},
        {8, 200},
        {9, 400},
        {10, 800},
    };
    public static Dictionary<int, int> teamSlotPrices = new Dictionary<int, int>
    {
        {3, 100},
        {4, 200},
        {5, 400}
    };
    public static Dictionary<int, int> buyPlayTicketAmount = new Dictionary<int, int>
    {
        {1, 25},
        {2, 50},
        {3, 75},
        {4, 100}
    };

    public static Dictionary<int, float> damageByElementPair = new Dictionary<int, float>
    {
        {(int)Element.Dark * 100 + (int)Element.Light, 2},
        {(int)Element.Light * 100 + (int)Element.Dark, 2},
        {(int)Element.Fire * 100 + (int)Element.Grass, 4},
        {(int)Element.Fire * 100 + (int)Element.Water, 0.5f},
        {(int)Element.Grass * 100 + (int)Element.Water, 4f},
        {(int)Element.Grass * 100 + (int)Element.Fire, 0.5f},
        {(int)Element.Water * 100 + (int)Element.Fire, 4f},
        {(int)Element.Water * 100 + (int)Element.Grass, 0.5f},
    };
}