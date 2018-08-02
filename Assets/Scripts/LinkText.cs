using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkText : MonoBehaviour {

    [SerializeField]
    private string linkURL;

    public void OnClick()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalEval("window.open(\"" + linkURL + "\")");
        } else {
            Application.OpenURL(linkURL);
        }
    }
}
