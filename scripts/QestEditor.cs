using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quest))]
public class QestEditor : Editor
{
    #region vars
    
        #region serializedProperties
            
            private SerializedProperty questName;
            private SerializedProperty questDescription;
            private SerializedProperty questType;

            #region moveVars

                private SerializedProperty waypoints;
                private SerializedProperty colorGizmos;
                private SerializedProperty drawGizmos;

            #endregion

            #region talkVars

                private SerializedProperty talkTo;          

            #endregion
        
        #endregion

        #region splitsPart
        
            bool showGeneral = false;
            bool showEdit = false;
            
        #endregion
    #endregion

    private void OnEnable()
    {
        questName = serializedObject.FindProperty("questName");
        questDescription = serializedObject.FindProperty("questDescription");
        questType = serializedObject.FindProperty("questType");
        
        // moveVars
        waypoints = serializedObject.FindProperty("waypoints");
        colorGizmos = serializedObject.FindProperty("gizmosColor");
        drawGizmos = serializedObject.FindProperty("displayOnEditor");
        
        // talkVars
        talkTo = serializedObject.FindProperty("talkTo");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Quest quest = (Quest)target;
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
        EditorGUILayout.Space(5);
        showEdit = EditorGUILayout.BeginFoldoutHeaderGroup(showEdit, "Edit Quest");
        if (showEdit)
        {
            switch (quest.GetQuestType())
            {
                case QuestType.Collect:
                    DisplayCollectEditor();
                    break;
                case QuestType.Destruct:
                    DisplayDestructEditor();
                    break;
                case QuestType.Kill:
                    DisplayKillEditor();
                    break;
                case QuestType.Move:
                    DisplayMoveEditor(quest);
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
    
    private void DisplayCollectEditor() {
        
    }
    
    private void DisplayKillEditor() {
        
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
    
    private void DisplayMoveEditor(Quest quest)
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
        };
        EditorGUILayout.LabelField("Moving", style, GUILayout.ExpandWidth(true));
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

        serializedObject.ApplyModifiedProperties();
    }
    
    private void DisplayOtherEditor() {
        
    }
}
