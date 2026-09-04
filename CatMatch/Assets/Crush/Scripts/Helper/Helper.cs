

using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

public class Helper
{
    static CultureInfo ci = new CultureInfo("en-us");

    public static string WordFilt(string mystr, out int numSpace)
    {
        string[] split = Regex.Split(mystr, @"(?<!^)(?=[A-Z])");

        string str = "";

        numSpace = 0;

        for (int i = 0; i < split.Length; i++)
        {
            str += split[i] + " ";
            numSpace++;
        }
        str.TrimEnd(' ');
        return str;
    }

    public static string TimeFormat(int totalSec)
    {
        var min = totalSec / 60;
        var sec = totalSec - (min * 60);
        if (min < 10)
        {
            if (sec < 10)
            {
                return "0" + min + ":0" + sec;
            }
            else
            {
                return "0" + min + ":" + sec;
            }
        }
        else
        {
            if (sec < 10)
            {
                return min + ":0" + sec;
            }
            else
            {
                return min + ":" + sec;
            }
        }
    }
    
    private static string SubNumber(string number, int pow)
    {
        string sub = number.Substring(0, number.Length - pow + 2);
        string strRound = sub.Insert(sub.Length - 2, ".");
        return strRound;
    }

    public static string FormatNumber(BigInteger number)
    {
        string str = number.ToString();

        if (str.Length > 51)
        {
            str = $"{SubNumber(str, 51)}mm";
        }
        else
        if (str.Length > 48)
        {
            str = $"{SubNumber(str, 48)}ll";
        }
        else
        if (str.Length > 45)
        {
            str = $"{SubNumber(str, 45)}kk";
        }
        else
        if (str.Length > 42)
        {
            str = $"{SubNumber(str, 42)}jj";
        }
        else
        if (str.Length > 39)
        {
            str = $"{SubNumber(str, 39)}ii";
        }
        else
        if (str.Length > 36)
        {
            str = $"{SubNumber(str, 36)}hh";
        }
        else
        if (str.Length > 33)
        {
            str = $"{SubNumber(str, 33)}gg";
        }
        else
        if (str.Length > 30)
        {
            str = $"{SubNumber(str, 30)}ff";
        }
        else
        if (str.Length > 27)
        {
            str = $"{SubNumber(str, 27)}ee";
        }
        else
        if (str.Length > 24)
        {
            str = $"{SubNumber(str, 24)}dd";
        }
        else
        if (str.Length > 21)
        {
            str = $"{SubNumber(str, 21)}cc";
        }
        else
        if (str.Length > 18)
        {
            str = $"{SubNumber(str, 18)}bb";
        }
        else
        if (str.Length > 15)
        {
            str = $"{SubNumber(str, 15)}aa";
        }
        else
        if (str.Length > 12)
        {
            str = $"{SubNumber(str, 12)}T";
        }
        else
        if (str.Length > 9)
        {
            str = $"{SubNumber(str, 9)}B";
        }
        else
        if (str.Length > 6)
        {
            str = $"{SubNumber(str, 6)}M";
        }
        else
        if (str.Length > 3)
        {
            str = $"{SubNumber(str, 3)}K";
        }

        return str;
    }

    // public static string DecimalFormat(decimal a)
    // {
    //     return a.ToString("F02", ci);
    // }

    public static string FloatFormat(float a, bool showZero)
    {
        if (!showZero && a == 0) return "";

        return a.ToString("F02", ci);
    }

    // public static string DecimalFormat(BigInteger a, BigInteger b)
    // {
    //     decimal de = (decimal)a / (decimal)b;

    //     return de > 0 ? de.ToString("F02", ci) : "";
    // }

    // public static string DecimalPercentFormat(BigInteger a, BigInteger b)
    // {
    //     decimal de = (decimal)a / (decimal)b;
    //     de = de * 100;
    //     return de > 0 ? de.ToString("F02", ci) + "%" : "";
    // }

    // public static string DecimalPercentFormat(decimal a)
    // {
    //     a = a * 100;
    //     return a > 0 ? a.ToString("F02", ci) + "%" : "";
    // }

    public static string FloatPercentFormat(float a)
    {
        return a > 0 ? a.ToString("F02", ci) + "%" : "";
    }

    public static BigInteger GetMoney(CurrencyType currencyType)
    {
        //   playAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.PlayStageAmount);
        // gemValueTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());
        // sweepAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.SweepAmount);
        // tileValueTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.TileTicketAmount);
        if (currencyType == CurrencyType.Gem)
        {
            return MergeBeast.Utils.GetCurrentRubyMoney();
        }
        else if (currencyType == CurrencyType.Tile)
        {
            return PlayerData.instance.TileTicketAmount;
        }
        else if (currencyType == CurrencyType.PlayAmount)
        {
            return PlayerData.instance.PlayStageAmount;
        }
        else if (currencyType == CurrencyType.SweepAmount)
        {
            return PlayerData.instance.SweepAmount;
        }
        return 0;
    }

    public static void AddMoney(CurrencyType currencyType, int value)
    {
        //   playAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.PlayStageAmount);
        // gemValueTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());
        // sweepAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.SweepAmount);
        // tileValueTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.TileTicketAmount);
        if (currencyType == CurrencyType.Gem)
        {
            MergeBeast.Utils.AddRubyCoin(value);
        }
        else if (currencyType == CurrencyType.Tile)
        {
            PlayerData.instance.TileTicketAmount += value;
        }
        else if (currencyType == CurrencyType.PlayAmount)
        {
            PlayerData.instance.PlayStageAmount += value;
        }
        else if (currencyType == CurrencyType.SweepAmount)
        {
            PlayerData.instance.SweepAmount += value;
        }
    }
}