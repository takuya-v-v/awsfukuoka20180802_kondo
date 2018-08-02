using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour {

    private IEnumerator Start()
	{
        ETHManager ethManager = ETHManager.Instance;
        SceneManager.Instance.ReplaceUIScene("Scenes/Title");
        yield return null;
        Debug.Log(ETHManager.Instance.SaveData.PrivateKey);
        Debug.Log(ETHManager.Instance.SaveData.Address);
	}
}
