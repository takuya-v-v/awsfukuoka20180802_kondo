using System;
using System.Linq;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;
using Nethereum.KeyStore;

public class ETHManager : SingletonMonoBehaviour<ETHManager> {

    private static string url = "https://ropsten.infura.io/z0c62ab9Er9VhdaQRMYn";

    [SerializeField]
    public string PrivateKey;

    [SerializeField]
    public string Address;

    [SerializeField]
    public decimal Balance;

    [SerializeField]
    public string ContractAddress = "";


    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        PrivateKey = PlayerPrefs.GetString("PrivateKey");
        if (string.IsNullOrEmpty(PrivateKey))
        {
            EthECKey ecKey = EthECKey.GenerateKey();
            PrivateKey = ecKey.GetPrivateKey().Substring(2);
            Address = ecKey.GetPublicAddress();
        }
        else
        {
            EthECKey ecKey = new EthECKey(PrivateKey);
            Address = ecKey.GetPublicAddress();
        }
        ETHManager.Instance.RopstenFauset(Address, (arg0) => { });
        Save();
    }




    public void Save()
    {
        PlayerPrefs.SetString("PrivateKey", PrivateKey);
        PlayerPrefs.SetString("Balance", Balance.ToString());
        PlayerPrefs.Save();
    }

    public void GetBalance(UnityAction<decimal> callback)
    {
        Debug.Log("GetBalance");
        StartCoroutine(_GetBalance(callback));
    }

    public IEnumerator _GetBalance(UnityAction<decimal> callback)
    {
        EthGetBalanceUnityRequest getBalance = new EthGetBalanceUnityRequest(url);
        yield return getBalance.SendRequest(Address, BlockParameter.CreateLatest());
        HexBigInteger balance = getBalance.Result;
        decimal ret = ETHUtils.IntegerToDecimal(decimal.Parse(balance.Value.ToString()), 18);
        Debug.Log("GetBalance ret:" + ret.ToString());
        callback.Invoke(ret);
    }

    public void GetTokenBalance(UnityAction<decimal> callback)
    {
        Debug.Log("GetTokenBalance");
        StartCoroutine(_GetTokenBalance(callback));
    }

    public IEnumerator _GetTokenBalance(UnityAction<decimal> callback)
    {
        yield return null;
        EthCallUnityRequest ethBalanceCall = new EthCallUnityRequest(url);
        Contract contract = new Contract(null, @"[{ 'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'type':'function'}]", ContractAddress);
        Function balanceOf = contract.GetFunction("balanceOf");
        CallInput callInputBalance = balanceOf.CreateCallInput();
        callInputBalance.From = Address;
        yield return ethBalanceCall.SendRequest(callInputBalance, BlockParameter.CreateLatest());
        Debug.Log("_GetTokenBalance end");
        if (string.IsNullOrEmpty(ethBalanceCall.Result) || ethBalanceCall.Result == "0x")
        {
            callback(0);
        }
        else
        {
            EthCallUnityRequest ethDecimalsCall = new EthCallUnityRequest(url);
            Function decimals = contract.GetFunction("decimals");
            CallInput callInputDecimals = decimals.CreateCallInput();
            callInputDecimals.From = Address;
            yield return ethDecimalsCall.SendRequest(callInputDecimals, BlockParameter.CreateLatest());
            if (string.IsNullOrEmpty(ethDecimalsCall.Result) || ethDecimalsCall.Result == "0x")
            {
                callback(0);
            }
            else
            {
                decimal balance = decimal.Parse(ethBalanceCall.Result.Substring(2));
                decimal decimalsVal = decimal.Parse(ethDecimalsCall.Result.Substring(2));
                callback(ETHUtils.DecimalToInteger(balance, (int)decimalsVal));
            }
        }
    }

    public void Send(string privateKey, string from, string to, decimal amount, decimal gasPrice, decimal gasLimit, UnityAction<string> callback)
    {
        Debug.Log("Send");
        StartCoroutine(_Send(privateKey, from, to, amount, gasPrice, gasLimit, callback));
    }

    public IEnumerator _Send(string privateKey, string from, string to, decimal amount, decimal gasPrice, decimal gasLimit, UnityAction<string> callback)
    {
        TransactionSignedUnityRequest ethSend = new TransactionSignedUnityRequest(url, privateKey, from);
        TransactionInput input = new TransactionInput("", to, from,
                                                      new HexBigInteger(new BigInteger(gasPrice)),
                                                      new HexBigInteger(new BigInteger(gasLimit)),
                                                      new HexBigInteger(new BigInteger(amount)));
        yield return ethSend.SignAndSendTransaction(input);
        Debug.Log(ethSend.Result);
        callback(ethSend.Result);
    }

    public void SendToken(string privateKey, string contract, string from, string to, decimal amount, decimal gasPrice, decimal gasLimit, UnityAction<string> callback)
    {
        Debug.Log("Send");
        StartCoroutine(_SendToken(privateKey, contract, from, to, amount, gasPrice, gasLimit, callback));
    }

    public IEnumerator _SendToken(string privateKey, string contract, string from, string to, decimal amount, decimal gasPrice, decimal gasLimit, UnityAction<string> callback)
    {
        TransactionSignedUnityRequest ethSend = new TransactionSignedUnityRequest(url, privateKey, from);
        TransactionInput input = new TransactionInput("", to, from,
                                                      new HexBigInteger(new BigInteger(gasPrice)),
                                                      new HexBigInteger(new BigInteger(gasLimit)),
                                                      new HexBigInteger(new BigInteger(amount)));
        yield return ethSend.SignAndSendTransaction(input);
        Debug.Log(ethSend.Result);
        callback(ethSend.Result);
    }

    public void RopstenFauset(string address, UnityAction<string> callback)
    {
        Debug.Log("RopstenFauset");
        StartCoroutine(_RopstenFauset(address, callback));
    }

    private IEnumerator _RopstenFauset(string address, UnityAction<string> callback)
    {
        string url = "https://ropsten.faucet.b9lab.com/tap";
        string paramjson = "{\"toWhom\":\"" + address + "\"}";
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(paramjson);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
        callback.Invoke(request.downloadHandler.text);
    }
}
