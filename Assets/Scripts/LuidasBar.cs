using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuidasBar : MonoBehaviour {

    [SerializeField]
    private GameObject Loading;

    [SerializeField]
    private Animator MyAdventure;

    [SerializeField]
    private GameObject ListContent;

    [SerializeField]
    private GameObject ListItemPrefab;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Text Balance;

    [SerializeField]
    private Text Score;

    [SerializeField]
    private Text Name;

    [SerializeField]
    private Button BuyButton;

    [SerializeField]
    private UnityEditor.Animations.AnimatorController[] animators;

    private static string[] GenName = new string[] { "バスチェル", "セオドリー", "ジェリオット", "ジャレオン", "グレアラン", "アルヴィス", "ジャレイ", "ジャレッド", "モーガイ", "ジェフィー", "リックリス", "ブラッドニー", "ジャイアス", "ジェローズ", "エグバート", "クスタイン", "ジェラルフ", "アーロブ", "ゴードルフ", "エグバート", "チェルシー", "ドレイミー", "チェルード", "オーフィー", "クトリノア", "クトリーナ", "カーラ", "ジョイヴ", "ブライディ", "ティルシー", "ダーリアル", "キャサラ", "ドライリー", "コール", "ワンダ", "レスティナ", "シャローラ", "アメリーン", "イレイティ", "モイラ"};


    private void Start () {
        BuyButton.gameObject.SetActive(true);
        MyAdventure.gameObject.SetActive(false);
        Name.text = "";
        Score.text = "";

        Loading.SetActive(true);
        ETHManager.Instance.GetBalance();
        StartCoroutine(Load());
	}

    public void OnReloadButton() {
        Loading.SetActive(true);
        ETHManager.Instance.GetBalance();
        StartCoroutine(Load());
    }

    private IEnumerator Load() {
        
        while (ETHManager.Instance.IsGetBalance || ETHManager.Instance.IsGetAdventure || ETHManager.Instance.IsSetAdventure)
        {
            yield return null;
        }
        yield return null;
        Loading.SetActive(false);

        Balance.text = ETHUtils.TrimForDecimal(ETHManager.Instance.SaveData.Balance);
        ListContent.transform.DetachChildren();

        AdventureModel adventure = ETHManager.Instance.SaveData.adventure;
        if (adventure == null || string.IsNullOrEmpty(adventure.address))
        {
            BuyButton.gameObject.SetActive(true);
            MyAdventure.gameObject.SetActive(false);
            Name.text = "";
            Score.text = "";
        }
        else
        {
            BuyButton.gameObject.SetActive(false);
            MyAdventure.gameObject.SetActive(true);
            MyAdventure.runtimeAnimatorController = animators[adventure.charaId];
            Name.text = adventure.name;
            Score.text = adventure.score.ToString();
        }

        ListUpdate();
    }

    private void ListUpdate() {
        if (ETHManager.Instance.SaveData.topAdventures == null) {
            return;
        }
        ListContent.transform.DetachChildren();
        foreach (AdventureModel ranking in ETHManager.Instance.SaveData.topAdventures)
        {
            GameObject listItem = Instantiate<GameObject>(ListItemPrefab, ListContent.transform);
            LuidasBarItem item = listItem.GetComponent<LuidasBarItem>();
            item.Load(ranking.name, ranking.score.ToString(), animators[ranking.charaId]);
        }
    }

    private string RandomName() {
        int index = Random.Range(0, GenName.Length - 1);
        return GenName[index];
    }

    public void OnBuyButton() {
        Loading.SetActive(true);
        AdventureModel adventure = new AdventureModel();
        adventure.address = ETHManager.Instance.SaveData.Address;
        adventure.name = RandomName();
        adventure.charaId = Random.Range(0, 3);
        adventure.score = 0;

        ETHManager.Instance.SetAdventure(adventure);
        StartCoroutine(Load());
    }

}
