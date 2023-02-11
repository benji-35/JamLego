using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private GameObject camPlayer;
    [SerializeField] private TMPro.TextMeshProUGUI distanceText;
    [SerializeField] private GameObject refDistance;

    [Header("Move Quest")]
    [SerializeField] private List<QuestWaypoint> waypoints;
    [SerializeField] private bool displayOnEditor = true;
    [SerializeField] private Color gizmosColor = Color.yellow;
    private int currentWaypoint = 0;
    
    [Header("Talk Quest")]
    [SerializeField] private DiscussManager talkTo = null;
    
    [Header("Collect Quest")]
    [SerializeField] private List<CollectableObject> collectables;
    
    [Header("Kill Quest")]
    [SerializeField] private PnjLife enemy;

    private int nbCollected = 0;

    private void Start()
    {
        camPlayer = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");
        QuestMarker.SetActive(false);
        if (refDistance != null) {
            refDistance.SetActive(false);
        }
        if (state == QuestState.Finished)
            return;
    }

    // Update is called once per frame
    void Update() {
        if (state != QuestState.InProgress)
            return;
        if (camPlayer != null) {
            QuestMarker.transform.LookAt(2 * QuestMarker.transform.position - camPlayer.transform.position);
            if (refDistance == null) {
                distanceText.text = "Distance: N/A";
            } else {
                float distance = Vector3.Distance(camPlayer.transform.position, refDistance.transform.position);
                distanceText.text = "Distance: " + distance.ToString("0.00") + "cm";
            }
        }
        checkWaypoint();
        checkCollectables();
    }

    private void checkCollectables()
    {
        if (questType != QuestType.Collect)
            return;
        if (nbCollected >= collectables.Count)
            FinishQuest();
    }
    
    private void checkWaypoint() {
        if (questType != QuestType.Move)
            return;
        QuestWaypoint _waypoint = waypoints[currentWaypoint];
        Vector3 target = _waypoint.position.position;
        //check if player is in radius of target
        if (Vector3.Distance(target, player.transform.position) <= _waypoint.radius)
        {
            currentWaypoint++;
            if (currentWaypoint < waypoints.Count)
                QuestMarker.transform.position = waypoints[currentWaypoint].position.position;
        }
        if (currentWaypoint >= waypoints.Count)
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
    
    private void FinishQuest() {
        state = QuestState.Finished;
        eventsOnFinish.Invoke();
        QuestMarker.SetActive(false);
    }

    public void StartQuest() {
        if (state != QuestState.NotStarted)
            return;
        gameObject.SetActive(true);
        state = QuestState.InProgress;
        QuestMarker.SetActive(true);
        if (questType == QuestType.Talk && talkTo != null) {
            QuestMarker.transform.position = talkTo.transform.position;
            talkTo.AddEventOnFinish(FinishQuest, this);
        } else if (questType == QuestType.Move && waypoints.Count > 0) {
            QuestMarker.transform.position = waypoints[0].position.position;
            currentWaypoint = 0;
        } else if (questType == QuestType.Collect) {
            Debug.Log("collect quest : number of collectables = " + collectables.Count);
            for (int i = 0; i < collectables.Count; i++) {
                Debug.Log("add event for collectable");
                var col = collectables[i];
                col.AddEventOnCollect(setCollectableCollected);
            }
        } else if (questType == QuestType.Kill && enemy != null) {
            QuestMarker.transform.position = enemy.transform.position;
            enemy.AddEventOnDeath(FinishQuest);
        }
    }
    
    public void setCollectableCollected()
    {
        nbCollected++;
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
    
    public List<CollectableObject> GetCollectables() {
        if (questType == QuestType.Collect)
            return collectables;
        return new List<CollectableObject>();
    }
    
    public void RemoveCollectable(int index) {
        collectables.RemoveAt(index);
    }
    
    public void AddCollectable(CollectableObject collectable) {
        collectables.Add(collectable);
    }
    
    public void SetCollectable(int index, CollectableObject collectable)
    {
        collectables[index] = collectable;
    }

    public void SetListCollectable(List<CollectableObject> objs) {
        collectables = objs;
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