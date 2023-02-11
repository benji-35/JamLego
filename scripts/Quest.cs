using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quest : MonoBehaviour {
    [Header("GENERAL")]
    [SerializeField] private string questName;
    [SerializeField] private string questDescription;
    [SerializeField] private QuestType questType;
    [Header("Others")]
    [SerializeField] private UnityEvent eventsOnFinish;

    [SerializeField] private GameObject QuestMarker;
    [SerializeField] private QuestObject[] questObjects;
    private QuestState state = QuestState.NotStarted;
    private GameObject player;
    [SerializeField] private TMPro.TextMeshProUGUI distanceText;
    [SerializeField] private GameObject refDistance;

    [Header("Move Quest")]
    [SerializeField] private List<QuestWaypoint> waypoints;
    [SerializeField] private bool displayOnEditor = true;
    [SerializeField] private Color gizmosColor = Color.yellow;
    [SerializeField] private DiscussManager talkTo = null;

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
        if (state != QuestState.NotStarted)
            return;
        state = QuestState.InProgress;
        QuestMarker.SetActive(true);
        if (questType == QuestType.Talk && talkTo != null)
        {
            QuestMarker.transform.position = talkTo.transform.position;
            talkTo.AddEventOnFinish(FinishQuest);
        }
    }
    
    public QuestState GetState()
    {
        return state;
    }
    
    public QuestType GetQuestType()
    {
        return questType;
    }

    private void OnDrawGizmos()
    {
        if (questType == QuestType.Move && displayOnEditor) {
            Gizmos.color = gizmosColor;
            //draw sphere detection in waypoint
            foreach (var waypoint in waypoints) {
                if (waypoint.position != null)
                    Gizmos.DrawSphere(waypoint.position.position, waypoint.radius);
            }
            //draw line between waypoints
            for (int i = 0; i < waypoints.Count - 1; i++) {
                if (waypoints[i].position != null && waypoints[i + 1].position != null)
                    Gizmos.DrawLine(waypoints[i].position.position, waypoints[i + 1].position.position);
            }
        }
    }

    public List<QuestWaypoint> GetWaypoints()
    {
        if (questType == QuestType.Move)
            return waypoints;
        return new List<QuestWaypoint>();
    }
    
    public void RemoveWaypoint(int index)
    {
        waypoints.RemoveAt(index);
    }
    
    public void SetTransformWaypoint(int index, Transform transform)
    {
        waypoints[index].position = transform;
    }
    
    public void SetRadiusWaypoint(int index, float radius)
    {
        waypoints[index].radius = radius;
    }
}

[System.Serializable]
public enum QuestState {
    NotStarted,
    InProgress,
    Finished
}

[System.Serializable]
public enum QuestType {
    Collect,
    Kill,
    Talk,
    Destruct,
    Move,
    Other
}

[System.Serializable]
public enum QuestObjectState {
    ACTIVE,
    INACTIVE,
    DESTROYED
}

[System.Serializable]
public class QuestObject {
    public GameObject _object;
    public QuestObjectState _stateWanted;
    public bool done = false;
}

[System.Serializable]
public class QuestWaypoint
{
    public Transform position;
    public float radius;
}