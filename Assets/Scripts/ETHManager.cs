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

    [Serializable]
    public class SaveDataModel {
        public string PrivateKey;
        public string Address;
        public decimal Balance;
        public AdventureModel[] topAdventures;
        public AdventureModel adventure;
    }

    [FunctionOutput]
    public class Adventure
    {
        [Parameter("address", "addr", 0)]
        public string Address { get; set; }
        [Parameter("int256", "score", 1)]
        public int Score { get; set; }
        [Parameter("string", "name", 2)]
        public string Name { get; set; }
        [Parameter("int256", "charaId", 3)]
        public int CharaId { get; set; }
    }

    private static string url = "https://ropsten.infura.io/z0c62ab9Er9VhdaQRMYn";

    [SerializeField]
    public SaveDataModel SaveData;

    [SerializeField]
    public string ContractAddress = "0x1F875F7161F3b8F521dE0B9C11f6f56345eDbf2B";

    [SerializeField]
    private string ABI = @"[{'constant':false,'inputs':[{'name':'addr','type':'address'},{'name':'score','type':'int256'},{'name':'name','type':'string'},{'name':'charaId','type':'int256'}],'name':'setAdventure','outputs':[{'name':'','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[{'name':'','type':'uint256'}],'name':'topAdventures','outputs':[{'name':'addr','type':'address'},{'name':'score','type':'int256'},{'name':'name','type':'string'},{'name':'charaId','type':'int256'}],'type':'function'},{'constant':false,'inputs':[],'name':'getCountTopAdventures','outputs':[{'name':'','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'userAdventures','outputs':[{'name':'addr','type':'address'},{'name':'score','type':'int256'},{'name':'name','type':'string'},{'name':'charaId','type':'int256'}],'type':'function'}]";

    [SerializeField]
    public bool IsGetBalance;

    [SerializeField]
    public bool IsGetAdventure;

    [SerializeField]
    public bool IsSetAdventure;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        SaveData = JsonUtility.FromJson<SaveDataModel>(PlayerPrefs.GetString("SaveData"));
        if (SaveData == null) {
            SaveData = new SaveDataModel();
        }
        if (string.IsNullOrEmpty(SaveData.PrivateKey))
        {
            EthECKey ecKey = EthECKey.GenerateKey();
            SaveData.PrivateKey = ecKey.GetPrivateKey().Substring(2);
            SaveData.Address = ecKey.GetPublicAddress().ToLower().Trim();
        }
        else
        {
            EthECKey ecKey = new EthECKey(SaveData.PrivateKey);
            SaveData.Address = ecKey.GetPublicAddress().ToLower().Trim();
        }

        if (SaveData.Balance <= (decimal)1)
        {
            ETHManager.Instance.RopstenFauset(SaveData.Address, (arg0) => { });
        }
        Save();
        GetBalance();
    }




    public void Save()
    {
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(SaveData));
        PlayerPrefs.Save();
    }

    public void GetBalance()
    {
        if (IsGetBalance) {
            return;
        }
        IsGetBalance = true;
        Debug.Log("GetBalance");
        StartCoroutine(_GetBalance());
    }

    public IEnumerator _GetBalance()
    {
        EthGetBalanceUnityRequest getBalance = new EthGetBalanceUnityRequest(url);
        yield return getBalance.SendRequest(SaveData.Address, BlockParameter.CreateLatest());
        HexBigInteger balance = getBalance.Result;
        SaveData.Balance = ETHUtils.IntegerToDecimal(decimal.Parse(balance.Value.ToString()), 18);
        Debug.Log("GetBalance ret:" + SaveData.Balance.ToString());
        GetAdventureAndTop100();
        IsGetBalance = false;
        Save();
    }

    public void GetAdventureAndTop100()
    {
        if (IsGetAdventure) {
            return;
        }
        IsGetAdventure = true;
        Debug.Log("GetAdventureAndTop100");
        StartCoroutine(_GetAdventureAndTop100());
    }

    public IEnumerator _GetAdventureAndTop100()
    {
        yield return null;
        EthCallUnityRequest ethCallUserAdventures = new EthCallUnityRequest(url);
        Contract contract = new Contract(null, ABI, ContractAddress);
        Function userAdventures = contract.GetFunction("userAdventures");
        CallInput callInputUserAdventures = userAdventures.CreateCallInput();
        callInputUserAdventures.Data = userAdventures.GetData(SaveData.Address);
        // TODO::なぜかアドレスが一位識別しない
        yield return ethCallUserAdventures.SendRequest(callInputUserAdventures, BlockParameter.CreateLatest());
        Adventure adventure = userAdventures.DecodeDTOTypeOutput<Adventure>(ethCallUserAdventures.Result);
        if (!string.IsNullOrEmpty(adventure.Address) && !string.IsNullOrEmpty(adventure.Name))
        {
            AdventureModel adventureModel = new AdventureModel(adventure);
            SaveData.adventure = adventureModel;
        }
        Debug.Log("_GetAdventureAndTop100 userAdventures:" + ethCallUserAdventures.Result);
        EthCallUnityRequest ethCallGetCountTopAdventures = new EthCallUnityRequest(url);
        Function getCountTopAdventures = contract.GetFunction("getCountTopAdventures");
        CallInput callInputGetCountTopAdventures = getCountTopAdventures.CreateCallInput();
        //callInputGetCountTopAdventures.Data = getCountTopAdventures.GetData();
        yield return ethCallGetCountTopAdventures.SendRequest(callInputGetCountTopAdventures, BlockParameter.CreateLatest());
        string retCount = ethCallGetCountTopAdventures.Result;
        int count = int.Parse(retCount.Substring(2));
        Debug.Log("_GetAdventureAndTop100 getCountTopAdventures:" + count.ToString());
        List<AdventureModel> list = new List<AdventureModel>();
        for (int i = 0; i < count; i++)
        {
            // TODO::なぜか重複登録されてしまう。
            EthCallUnityRequest ethCallTopAdventures = new EthCallUnityRequest(url);
            Function topAdventures = contract.GetFunction("topAdventures");
            CallInput callInputTopAdventures = topAdventures.CreateCallInput();
            callInputTopAdventures.Data = topAdventures.GetData(i);
            yield return ethCallTopAdventures.SendRequest(callInputTopAdventures, BlockParameter.CreateLatest());
            Debug.Log("_GetAdventureAndTop100 ethCallTopAdventures:" + ethCallTopAdventures.Result);
            Adventure topadventure = topAdventures.DecodeDTOTypeOutput<Adventure>(ethCallTopAdventures.Result);
            if (!string.IsNullOrEmpty(topadventure.Address) && !string.IsNullOrEmpty(topadventure.Name))
            {
                AdventureModel adventureModel = new AdventureModel(topadventure);
                if (SaveData.Address.ToLower().Trim().Equals(adventureModel.address.ToLower().Trim()))
                {
                    if (SaveData.adventure == null || string.IsNullOrEmpty(SaveData.adventure.name) || SaveData.adventure.score < adventureModel.score)
                    {
                        SaveData.adventure = adventureModel;
                    }
                }
                bool unique = true;
                for (int index = 0; index < list.Count; index++)
                {
                    AdventureModel item = list[index];
                    if (item.address.ToLower().Trim().Equals(adventureModel.address.ToLower().Trim())) {
                        unique = false;
                        if (adventureModel.score > item.score) {
                            list[index] = adventureModel;
                        }
                    }
                }
                if (unique)
                {
                    list.Add(adventureModel);
                }
            }
            list.Sort((a, b) => { if (a.score < b.score) { return 1; } else if (a.score > b.score) { return - 1; } else { return 0; } });
            Debug.Log("_GetAdventureAndTop100 i:" + i.ToString());
        }
        SaveData.topAdventures = list.ToArray();
        Debug.Log("_GetAdventureAndTop100 list count:" + list.Count.ToString());
        IsGetAdventure = false;
        Save();
    }

    public void SetAdventure(AdventureModel adventure)
    {
        if (IsSetAdventure)
        {
            return;
        }
        IsSetAdventure = true;
        Debug.Log("SetAdventure");
        StartCoroutine(_SetAdventure(adventure));
    }

    public IEnumerator _SetAdventure(AdventureModel adventure)
    {
        decimal amount = new decimal(0.00);
        decimal gasPrice = 10000000;
        decimal gasLimit = 300000;
        Contract contract = new Contract(null, ABI, ContractAddress);
        Function func = contract.GetFunction("setAdventure");
        // TODO::なぜかアドレスが一位識別しない
        string data = func.GetData(adventure.address, adventure.score, adventure.name, adventure.charaId);
        TransactionSignedUnityRequest ethSend = new TransactionSignedUnityRequest(url, SaveData.PrivateKey, SaveData.Address);
        TransactionInput input = new TransactionInput(data, ContractAddress, SaveData.Address,
                                                      new HexBigInteger(new BigInteger(gasPrice)),
                                                      new HexBigInteger(new BigInteger(gasLimit)),
                                                      new HexBigInteger(new BigInteger(amount)));
        yield return ethSend.SignAndSendTransaction(input);
        Debug.Log(ethSend.Result);
        Save();
        yield return new WaitForSeconds(3);
        GetBalance();
        IsSetAdventure = false;
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
