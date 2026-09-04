
using System;
using System.Numerics;

public class BigIntegerHelper
{
    private static (BigInteger numerator, BigInteger denominator) Fraction(decimal d)
    {
        int[] bits = decimal.GetBits(d);
        BigInteger numerator = (1 - ((bits[3] >> 30) & 2)) *
                               unchecked(((BigInteger)(uint)bits[2] << 64) |
                                         ((BigInteger)(uint)bits[1] << 32) |
                                          (BigInteger)(uint)bits[0]);
        BigInteger denominator = BigInteger.Pow(10, (bits[3] >> 16) & 0xff);
        return (numerator, denominator);
    }


    public static BigInteger BigMultiplyFloat(BigInteger bi, float f)
    {
        var de = Fraction((Decimal)f);
        return (BigInteger)(bi * de.numerator / de.denominator);
    }

    // public static BigInteger BigMultiplyDecimal(BigInteger bi, decimal f)
    // {
    //     var de = Fraction(f);
    //     return (BigInteger)(bi * de.numerator / de.denominator);
    // }

    // public static BigInteger BigMultiplyIntDivInt(BigInteger bi, int m, int d)
    // {
    //     return bi * m / d;
    // }

    // public static decimal Pow(decimal coso, BigInteger mu)
    // {
    //     var muTmp = BigInteger.Abs(mu);

    //     decimal re = 1;
    //     for (int k = 0; k < muTmp; k++)
    //     {
    //         re *= coso;
    //     }

    //     if (mu < 0) return 1 / re;

    //     return re;
    // }

    public static (BigInteger numerator, BigInteger denominator) Pow2(decimal coso, BigInteger mu)
    {
        var fr = Fraction(coso);

        var muTmp = BigInteger.Abs(mu);

        BigInteger numerator = 1;
        for (int k = 0; k < muTmp; k++)
        {
            numerator *= fr.numerator;
        }

        BigInteger denominator = 1;
        for (int k = 0; k < muTmp; k++)
        {
            denominator *= fr.denominator;
        }

        if(muTmp > 0) return (numerator, denominator);
        else return (denominator, numerator);
    }
}