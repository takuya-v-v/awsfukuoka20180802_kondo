using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyAdventure : MonoBehaviour {

    public static FlappyAdventure Instance;

    [SerializeField]
    private GameObject FlappyGame;

    private GameObject FlappyPlayable;

    [SerializeField]
    private GameObject Loading;

    [SerializeField]
    private Transform RootTransform;

	private IEnumerator Start()
	{
        Instance = this;
        yield return new WaitForSeconds(2);
        Restart();

	}

    public void Restart() {
        StartCoroutine(_Restart());
    }

    public IEnumerator _Restart()
    {
        if (FlappyPlayable != null)
        {
            Destroy(FlappyPlayable);
        }
        yield return new WaitForEndOfFrame();
        FlappyPlayable = Instantiate<GameObject>(FlappyGame, RootTransform);
    }

	public void OnEndGame(int score)
	{
        AdventureModel adventure = ETHManager.Instance.SaveData.adventure;
        if (adventure != null)
        {
            if (adventure.score < score)
            {
                Loading.SetActive(true);
                adventure.score = score;
                ETHManager.Instance.SetAdventure(adventure);
                StartCoroutine(Load());
            }
        }

	}
    private IEnumerator Load()
    {
        while (ETHManager.Instance.IsGetBalance || ETHManager.Instance.IsGetAdventure || ETHManager.Instance.IsSetAdventure)
        {
            yield return null;
        }
        yield return null;
        Loading.SetActive(false);
    }
}
