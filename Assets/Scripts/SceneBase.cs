using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneBase : MonoBehaviour {


    [SerializeField]
    protected Canvas canvas;
    public Canvas Canvas {
        get {
            if (canvas == null) {
                canvas = GetComponent<Canvas>();
            }
            return canvas;
        }
    }

    [SerializeField]
    protected Animator transition;
    public Animator Transition
    {
        get
        {
            if (transition == null)
            {
                transition = GetComponent<Animator>();
            }
            return transition;
        }
    }

    [SerializeField]
    private string nextScene;

    [SerializeField]
    private GameObject[] sequenceObject;

	private void Start()
	{
        if (sequenceObject != null)
        {
            foreach (GameObject obj in sequenceObject) {
                obj.SetActive(false);
            }
        }
	}

	public void OnNextScene()
    {
        if (sequenceObject != null) {
            foreach (GameObject obj in sequenceObject)
            {
                if (!obj.activeSelf) {
                    obj.SetActive(true);
                    return;
                }
            }
        }
        SceneManager.Instance.PushUIScene("Scenes/" + nextScene);
    }

    public void OnBeforeScene()
    {
        if (SceneManager.Instance.CurrentSceneBase == this)
        {
            SceneManager.Instance.PopUIScene();
        }
    }

    public virtual void SetSortingOrderCanvas(int index)
    {
        if (canvas != null)
        {
            canvas.sortingOrder = index;
        }
    }

    public virtual IEnumerator In()
    {
        if (Transition == null)
        {
            yield break;
        }
        Transition.Play("In");
        yield return null;
        yield return new WaitForAnimation(Transition);
    }

    public virtual IEnumerator Out()
    {
        if (Transition == null)
        {
            yield break;
        }
        Transition.Play("Out");
        yield return null;
        yield return new WaitForAnimation(Transition);
    }

    public virtual IEnumerator PushIn()
    {
        if (Transition == null)
        {
            yield break;
        }
        Transition.Play("PushIn");
        yield return null;
        yield return new WaitForAnimation(Transition);
    }

    public virtual IEnumerator PushOut()
    {
        if (Transition == null)
        {
            yield break;
        }
        Transition.Play("PushOut");
        yield return null;
        yield return new WaitForAnimation(Transition);
    }

    public virtual IEnumerator PopIn()
    {
        if (Transition == null)
        {
            yield break;
        }
        Transition.Play("PopIn");
        yield return null;
        yield return new WaitForAnimation(Transition);
    }

    public virtual IEnumerator PopOut()
    {
        if (Transition == null)
        {
            yield break;
        }
        Transition.Play("PopOut");
        yield return null;
        yield return new WaitForAnimation(Transition);
    }
}
