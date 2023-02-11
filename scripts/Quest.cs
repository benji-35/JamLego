using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quest : MonoBehaviour {
    [SerializeField] private UnityEvent eventsOnFinish;

    [SerializeField] private GameObject QuestMarker;
    [SerializeField] private QuestObject[] questObjects;
    private QuestState state = QuestState.NotStarted;
    private GameObject player;
    [SerializeField] private TMPro.TextMeshProUGUI distanceText;
    [SerializeField] private GameObject refDistance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera");
        QuestMarker.SetActive(false);
        if (refDistance != null) {
            refDistance.SetActive(false);
        }
        if (state == QuestState.Finished)
            return;
        QuestMarker.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (state != QuestState.InProgress)
            return;
        if (player != null) {
            QuestMarker.transform.LookAt(2 * QuestMarker.transform.position - player.transform.position);
            if (refDistance == null) {
                distanceText.text = "Distance: N/A";
            } else {
                float distance = Vector3.Distance(player.transform.position, refDistance.transform.position);
                distanceText.text = "Distance: " + distance.ToString("0.00") + "cm";
            }
        }
        checkAllDone();
        if (allDone())
            FinishQuest();
    }

    private void checkAllDone()
    {
        foreach (var obj in questObjects) {
            if (obj.done)
                continue;
            switch (obj._stateWanted) {
                case QuestObjectState.ACTIVE:
                    if (obj._object.activeSelf)
                        obj.done = true;
                    break;
                case QuestObjectState.INACTIVE:
                    if (!obj._object.activeSelf)
                        obj.done = true;
                    break;
                case QuestObjectState.DESTROYED:
                    if (obj._object == null)
                        obj.done = true;
                    break;
                default:
                    break;
            }
        }
    }
    
    private bool allDone()
    {
        foreach (var obj in questObjects) {
            if (!obj.done)
                return false;
        }
        return true;
    }
    
    private void FinishQuest()
    {
        state = QuestState.Finished;
        eventsOnFinish.Invoke();
        QuestMarker.SetActive(false);
    }

    public void StartQuest()
    {
        state = QuestState.InProgress;
        QuestMarker.SetActive(true);
    }
    
    public QuestState GetState()
    {
        return state;
    }
}

[System.Serializable]
public enum QuestState
{
    NotStarted,
    InProgress,
    Finished
}

[System.Serializable]
public enum QuestObjectState {
    ACTIVE,
    INACTIVE,
    DESTROYED
}

[System.Serializable]
public class QuestObject
{
    public GameObject _object;
    public QuestObjectState _stateWanted;
    public bool done = false;
}
