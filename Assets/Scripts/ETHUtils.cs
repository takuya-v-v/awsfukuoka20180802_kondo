using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.JsonRpc.UnityClient;

public class ETHUtils
{

    public static string TrimForDecimal(decimal value)
    {
        string ret = value.ToString();
        if (ret.IndexOf('.') >= 0)
        {
            return ret.TrimEnd('0');
        }
        return ret;
    }

    public static decimal StringToDecimal(string value, int digits)
    {
        return IntegerToDecimal(decimal.Parse(value));
    }

    public static decimal IntegerToDecimal(decimal value, int digits = 18)
    {
        if (digits == 0)
        {
            digits = 18;
        }
        string zero = "1";
        for (int i = 0; i < digits; i++)
        {
            zero += "0";
        }
        return value / decimal.Parse(zero);
    }

    public static decimal DecimalToInteger(decimal value, int digits = 18)
    {
        if (digits == 0)
        {
            digits = 18;
        }
        string zero = "1";
        for (int i = 0; i < digits; i++)
        {
            zero += "0";
        }
        return value * decimal.Parse(zero);
    }

    public static decimal GWeiToInteger(decimal value)
    {
        string zero = "1000000000";
        return value / decimal.Parse(zero);
    }

    public static decimal IntegerToGWei(decimal value)
    {
        string zero = "1000000000";
        return value * decimal.Parse(zero);
    }
}
