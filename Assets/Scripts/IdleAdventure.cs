using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdleAdventure : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Image image;
	
	void Update () {
        animator.Play("idle");
        image.sprite = spriteRenderer.sprite;
	}
}
