using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuidasBarItem : MonoBehaviour {

    [SerializeField]
    private Text Name;

    [SerializeField]
    private Text Score;

    [SerializeField]
    private Animator Animator;

    public void Load (string name, string score, UnityEditor.Animations.AnimatorController controller) {
        Name.text = name;
        Score.text = score;
        Animator.runtimeAnimatorController = controller;
	}
}
