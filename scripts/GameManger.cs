using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool isDiscussing = false;

    private DiscussManager discussManager = null;
    // Start is called before the first frame update
    void Awake()
    {
        /*GameObject[] _objs = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < _objs.Length; i++)
        {
            if (_objs[i].tag == "Quest")
                objs.Add(_objs[i]);
        }*/
        
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenu.SetActive(true);
        }
        else if (numberOfPauseMenu <= 0 && pauseMenu.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenu.SetActive(false);
        }

        if (!pauseMenu.activeSelf && numberOfInteractables > 0 && !interactPanel.activeSelf)
            interactPanel.SetActive(true);
        else if ((pauseMenu.activeSelf && interactPanel.activeSelf) || (numberOfInteractables <= 0 && interactPanel.activeSelf))
            interactPanel.SetActive(false);
        if (!pauseMenu.activeSelf && isDiscussing && !discussPanel.activeSelf) {
            discussPanel.SetActive(true);
        } else if ((pauseMenu.activeSelf && discussPanel.activeSelf) || (!isDiscussing && discussPanel.activeSelf)) {
            discussPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isDiscussing) {
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
    
    public void OpenDiscuss(Discuss discuss, DiscussManager onFinish) {
        discussManager = onFinish;
        isDiscussing = true;
    }
    
    public void CloseDiscuss()
    {
        discussManager = null;
        isDiscussing = false;
    }

    private void NextDiscuss()
    {
        if (discussManager == null)
            return;
        discussManager.NextDiscuss();
    }
    
    public void SetInteractText(string talker, string text) {
        if (discussTalker == null || discussText == null)
            return;
        discussTalker.text = talker;
        discussText.text = text;
    }

    public bool IsInPauseMenu()
    {
        return numberOfPauseMenu > 0;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
