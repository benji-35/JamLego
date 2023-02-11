using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GameManger : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject interactPanel;
    [SerializeField] private GameObject pauseMenu;
    [Header("Quests")]
    [SerializeField] private List<Quest> quests;
    [SerializeField] private TMPro.TextMeshProUGUI questText;
    [Header("Discussion")]
    [SerializeField] private GameObject discussPanel;
    [SerializeField] private TMPro.TextMeshProUGUI discussTalker;
    [SerializeField] private TMPro.TextMeshProUGUI discussText;
    private bool gameIsReady = false;
    private int numberOfInteractables = 0;
    private int numberOfPauseMenu = 0;
    private int numberOfDiscuss = 0;
    private Discuss currentDiscuss = null;

    private UnityEvent onDiscussionFinish = null;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Quest");
        quests.Clear();
        foreach (var obj in objs)
        {
            Quest quest = obj.GetComponent<Quest>();
            if (quest != null) {
                quests.Add(quest);
            }
        }
        if (player.GetComponent<PlayerController>() != null && interactPanel != null && pauseMenu != null && discussPanel != null)
            gameIsReady = true;
        HideInteract();
        HidePauseMenu();
        CloseDiscuss();
    }

    private void Update()
    {
        if (!gameIsReady)
            return;
        questText.text = "Quests: " + GetNbQuestsDone() + " / " + quests.Count + "";
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pauseMenu.activeSelf)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }
        if (numberOfPauseMenu > 0 && !pauseMenu.activeSelf)
            pauseMenu.SetActive(true);
        else if (numberOfPauseMenu <= 0 && pauseMenu.activeSelf)
            pauseMenu.SetActive(false);
        if (!pauseMenu.activeSelf && numberOfInteractables > 0 && !interactPanel.activeSelf)
            interactPanel.SetActive(true);
        else if ((pauseMenu.activeSelf && interactPanel.activeSelf) || (numberOfInteractables <= 0 && interactPanel.activeSelf))
            interactPanel.SetActive(false);
        if (!pauseMenu.activeSelf && numberOfDiscuss > 0 && !discussPanel.activeSelf) {
            discussPanel.SetActive(true);
        } else if ((pauseMenu.activeSelf && discussPanel.activeSelf) || (numberOfDiscuss <= 0 && discussPanel.activeSelf)) {
            discussPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (numberOfDiscuss > 0) {
                NextDiscuss();
            }
        }
    }

    private int GetNbQuestsDone()
    {
        int result = 0;
        foreach (var quest in quests)
        {
            if (quest.GetState() == QuestState.Finished)
                result++;
        }

        return result;
    }

    public bool IsGameReady() {
        return gameIsReady;
    }

    public void DisablePlayerController()
    {
        if (!gameIsReady)
        {
            Debug.LogError("Game is not ready");
            return;
        }
        player.GetComponent<PlayerController>().enabled = false;
    }

    public void EnablePlayerController()
    {
        if (!gameIsReady)
        {
            Debug.LogError("Game is not ready");
            return;
        }
        player.GetComponent<PlayerController>().enabled = true;
    }

    public void HidePlayer()
    {
        player.SetActive(false);
    }
    
    public void ShowPlayer()
    {
        player.SetActive(true);        
    }
    
    public void TeleportPlayer(Vector3 position)
    {
        player.transform.position = position;
    }
    
    public void DebugSomething(string message)
    {
        Debug.Log(message);
    }

    public void ShowInteract() {
        numberOfInteractables++;
    }

    public void HideInteract() {
        numberOfInteractables--;
        if (numberOfInteractables < 0)
            numberOfInteractables = 0;
    }

    public void ShowPauseMenu() {
        numberOfPauseMenu++;
    }
    
    public void HidePauseMenu() {
        numberOfPauseMenu--;
        if (numberOfPauseMenu < 0)
            numberOfPauseMenu = 0;
    }
    
    public void OpenDiscuss(Discuss discuss, UnityEvent onFinish) {
        numberOfDiscuss++;
        currentDiscuss = discuss;
        onDiscussionFinish = onFinish;
        SetInteractText(currentDiscuss.GetTalker(), currentDiscuss.GetText());
        DisablePlayerController();
    }
    
    public void CloseDiscuss() {
        numberOfDiscuss--;
        if (numberOfDiscuss < 0)
            numberOfDiscuss = 0;
        if (numberOfDiscuss == 0)
            EnablePlayerController();
    }

    private void NextDiscuss() {
        if (currentDiscuss == null)
        {
            if (onDiscussionFinish != null)
                onDiscussionFinish.Invoke();
            CloseDiscuss();
            return;
        }
        currentDiscuss = currentDiscuss.GetNextDiscussion();
        if (currentDiscuss == null) {
            if (onDiscussionFinish != null)
                onDiscussionFinish.Invoke();
            CloseDiscuss();
        } else {
            SetInteractText(currentDiscuss.GetTalker(), currentDiscuss.GetText());
        }
    }
    
    public void SetInteractText(string talker, string text) {
        if (discussTalker == null || discussText == null)
            return;
        discussTalker.text = talker;
        discussText.text = text;
    }
}
