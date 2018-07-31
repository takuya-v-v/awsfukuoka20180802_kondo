using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkText : MonoBehaviour {

    [SerializeField]
    private string linkURL;

    public void OnClick() {
        Application.OpenURL(linkURL);
    }
}
