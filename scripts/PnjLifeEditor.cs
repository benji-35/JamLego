using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PnjLife))]
[CanEditMultipleObjects]
public class PnjLifeEditor : Editor
{
    #region vars

    #region Pnj

        SerializedProperty maxLife;
        SerializedProperty respawnable;
        SerializedProperty fgLifeBar;
        SerializedProperty onDeath;
        
        SerializedProperty coin;
        SerializedProperty yellowCoins;
        SerializedProperty blueCoins;
        SerializedProperty purpleCoins;

        SerializedProperty renderers;
        SerializedProperty colliders;
    
    #endregion
    
    #region parts
    
        bool pnjPart = true;
        bool coinsPart = false;
        bool renderersPart = false;
    
    #endregion

    #endregion

    private void OnEnable()
    {
        maxLife = serializedObject.FindProperty("maxLife");
        respawnable = serializedObject.FindProperty("respawnable");
        fgLifeBar = serializedObject.FindProperty("fgLifeBar");
        onDeath = serializedObject.FindProperty("onDeath");
        
        coin = serializedObject.FindProperty("coin");
        yellowCoins = serializedObject.FindProperty("yellowCoins");
        blueCoins = serializedObject.FindProperty("blueCoins");
        purpleCoins = serializedObject.FindProperty("purpleCoins");

        renderers = serializedObject.FindProperty("renderers");
        colliders = serializedObject.FindProperty("colliders");
    }


    public override void OnInspectorGUI()
    {
        PnjLife pnj = (PnjLife)target;
        serializedObject.Update();
        
        pnjPart = EditorGUILayout.BeginFoldoutHeaderGroup(pnjPart, "PNJ");
        if (pnjPart) {
            EditorGUILayout.PropertyField(maxLife, new GUIContent("Max life"));
            EditorGUILayout.PropertyField(respawnable, new GUIContent("Respawnable"));
            EditorGUILayout.PropertyField(fgLifeBar, new GUIContent("Foreground life bar"));
            EditorGUILayout.PropertyField(onDeath, new GUIContent("On death"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        coinsPart = EditorGUILayout.BeginFoldoutHeaderGroup(coinsPart, "Coins");
        if (coinsPart)
        {
            EditorGUILayout.PropertyField(coin, new GUIContent("Coin"));
            if (pnj.GetCoin() != null)
            {
                EditorGUILayout.PropertyField(yellowCoins, new GUIContent("Yellow coins"));
                EditorGUILayout.PropertyField(blueCoins, new GUIContent("Blue coins"));
                EditorGUILayout.PropertyField(purpleCoins, new GUIContent("Purple coins"));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        renderersPart = EditorGUILayout.BeginFoldoutHeaderGroup(renderersPart, "Renderers");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (renderersPart)
        {
            if (pnj.IsRespawnable())
            {
                EditorGUILayout.LabelField("Renderers", TitleStyle());
                EditorGUILayout.PropertyField(renderers, new GUIContent("Renderers"));

                EditorGUILayout.LabelField("Colliders", TitleStyle());
                EditorGUILayout.PropertyField(colliders, new GUIContent("Colliders"));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
    
    private GUIStyle TitleStyle()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        return style;
    }
}
