using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscussManager : Interractor
{
    [SerializeField] private Discuss firstDiscuss;
    [SerializeField] private UnityEvent onDiscussionFinish;
    // Start is called before the first frame update
    void Start()
    {
        if (firstDiscuss == null)
        {
            Debug.LogWarning("No discuss found");
        }
    }

    protected override void onInteract() {
        Debug.Log("Interact");
        StartDiscuss();
    }
    
    private void StartDiscuss() {
        if (firstDiscuss == null)
            return;
        GameManger gameManager = GetGameManager();
        if (gameManager == null) {
            Debug.LogWarning("No game manager found");
            return;
        }

        gameManager.OpenDiscuss(firstDiscuss, onDiscussionFinish);
    }
}
