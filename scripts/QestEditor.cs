using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quest))]
[CanEditMultipleObjects]
public class QestEditor : Editor
{
    #region vars
    
        #region serializedProperties
            
            private SerializedProperty questName;
            private SerializedProperty questDescription;
            private SerializedProperty questType;
            private SerializedProperty onFinish;

            #region moveVars

                private SerializedProperty waypoints;
                private SerializedProperty colorGizmos;
                private SerializedProperty drawGizmos;

            #endregion

            #region talkVars

                private SerializedProperty talkTo;          

            #endregion

            #region EnemyVars

                private SerializedProperty enemy;

            #endregion
        
        #endregion

        #region splitsPart
        
            bool showGeneral = false;
            bool showEdit = false;
            bool events = false;
            
        #endregion
    #endregion

    private void OnEnable() {
        questName = serializedObject.FindProperty("questName");
        questDescription = serializedObject.FindProperty("questDescription");
        questType = serializedObject.FindProperty("questType");
        onFinish = serializedObject.FindProperty("eventsOnFinish");
        
        // moveVars
        waypoints = serializedObject.FindProperty("waypoints");
        colorGizmos = serializedObject.FindProperty("gizmosColor");
        drawGizmos = serializedObject.FindProperty("displayOnEditor");
        
        // talkVars
        talkTo = serializedObject.FindProperty("talkTo");
        
        // collectVars
        enemy = serializedObject.FindProperty("enemy");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Quest quest = (Quest)target;
        if (quest.GetQuestType() == QuestType.Collect || quest.GetQuestType() == QuestType.Move)
        {
            DrawDefaultInspector();
            return;
        }
        EditorGUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 20
        };
        EditorGUILayout.LabelField("Quest Editor", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);
        showGeneral = EditorGUILayout.BeginFoldoutHeaderGroup(showGeneral, "General Information");
        if (showGeneral) {
            EditorGUILayout.PropertyField(questName);
            EditorGUILayout.PropertyField(questDescription);
            EditorGUILayout.PropertyField(questType);
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUI.EndFoldoutHeaderGroup();

        events = EditorGUILayout.BeginFoldoutHeaderGroup(events, "Events");
        if (events) {
            EditorGUILayout.PropertyField(onFinish);
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUI.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);
        showEdit = EditorGUILayout.BeginFoldoutHeaderGroup(showEdit, "Edit Quest");
        if (showEdit) {
            switch (quest.GetQuestType())
            {
                case QuestType.Collect:
                    break;
                case QuestType.Destruct:
                    DisplayDestructEditor();
                    break;
                case QuestType.Kill:
                    DisplayKillEditor();
                    break;
                case QuestType.Move:
                    break;
                case QuestType.Talk:
                    DisplayTalkEditor();
                    break;
                default:
                    DisplayOtherEditor();
                    break;
            }
        }
        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5);
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DisplayCollectEditor(Quest quest)
    {
        List<CollectableObject> collectables = quest.GetCollectables();
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
        };
        EditorGUILayout.LabelField("cCollect (" + collectables.Count + ")", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(15);
        for (int i = 0; i < collectables.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Collectable " + (i + 1), GUILayout.Width(100));
            CollectableObject col = (CollectableObject)EditorGUILayout.ObjectField(collectables[i], typeof(CollectableObject), true);
            quest.SetCollectable(i, col);
            if (GUILayout.Button("X"))
            {
                quest.RemoveCollectable(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Collectable"))
        {
            quest.AddCollectable(null);
        }
    }
    
    private void DisplayKillEditor() {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
        };
        EditorGUILayout.LabelField("Kill", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(enemy);
        EditorGUILayout.EndHorizontal();
    }
    
    private void DisplayTalkEditor() {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
        };
        EditorGUILayout.LabelField("Talk", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("You", GUILayout.Width(100));
        EditorGUILayout.PropertyField(talkTo);
        EditorGUILayout.EndHorizontal();
    }
    
    private void DisplayDestructEditor() {
        
    }
    
    private void DisplayMoveEditor(Quest quest) {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
        };
        EditorGUILayout.LabelField("Moving (" + quest.GetWaypoints().Count + ")", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(15);
        List<QuestWaypoint> waypointsList = quest.GetWaypoints();
        //display waypoints
        for (int i = 0; i < waypointsList.Count; i++)
        {
            var _waypoint = waypointsList[i];
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Waypoint", GUILayout.Width(100));
            Transform tr = (Transform)EditorGUILayout.ObjectField(_waypoint.position, typeof(Transform), true);
            quest.SetTransformWaypoint(i, tr);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Radius", GUILayout.Width(100));
            float radius = EditorGUILayout.FloatField(_waypoint.radius);
            quest.SetRadiusWaypoint(i, radius);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            if (GUILayout.Button( "Remove"))
            {
                quest.RemoveWaypoint(i);
            }
        }
        if (GUILayout.Button( "Add"))
        {
            waypointsList.Add(new QuestWaypoint());
        }
    }
    
    private void DisplayOtherEditor() {
        
    }
}
