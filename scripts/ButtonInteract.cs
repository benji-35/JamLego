using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : Interractor
{
    [SerializeField] private GameObject displayableState;
    [SerializeField] private  bool active = false;
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent onButtonActive;
    [SerializeField] private UnityEvent onButtonInactive;
    protected override void OnInteract()
    {
        animator.SetTrigger("Interact");
        active = !active;
        if (active) {
            onButtonActive.Invoke();
            if (displayableState != null)
                displayableState.GetComponent<Renderer>().material.color = Color.green;
        } else {
            onButtonInactive.Invoke();
            if (displayableState != null)
                displayableState.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public bool IsActiveButton() {
        return active;
    }
}
