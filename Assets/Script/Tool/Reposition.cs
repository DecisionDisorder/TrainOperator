using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum Orientation
{
    Vertical = 0,
    Horizontal = 1
}
public enum Direction
{ 
    Down = 0,
    Up = 1
}

[Serializable]
public class Reposition : EditorWindow
{
    UnityEngine.Object[] selecteObjects = new UnityEngine.Object[0];
    RectTransform[] selectedRectTransforms = new RectTransform[0];

    float interval;
    Orientation orientation;
    Direction direction;

    [MenuItem("Tools/Reposition")]
    public static void ShowWindow()
    {
        var window = GetWindow(typeof(Reposition));
        window.minSize = new Vector2(512, 128);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Reposition", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        interval = EditorGUILayout.FloatField("Interval", interval);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();
        orientation = (Orientation)EditorGUILayout.EnumPopup(new GUIContent("Orientation"), orientation);

        EditorGUILayout.Space();
        direction = (Direction)EditorGUILayout.EnumPopup(new GUIContent("Direction(+/-)"), direction);

        EditorGUILayout.Space();
        if (GUILayout.Button(new GUIContent("Reposition", "Reposition the gameobject in same interval"))) { Repositioning(); }
        EditorGUILayout.EndVertical();

        if (selectedRectTransforms.Length > 0)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.Space();
            for (int i = 0; i < selecteObjects.Length; i++)
            {
                EditorGUILayout.LabelField(selectedRectTransforms[i].name + " (" + selectedRectTransforms[i].anchoredPosition.x + ", " + selectedRectTransforms[i].anchoredPosition.y + ")");
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void Update()
    {
        selecteObjects = Selection.objects;
        SetGameObject();
    }

    private void Repositioning()
    {
        SetGameObject();

        float dir = 1;
        if (direction == Direction.Down)
            dir = -1;

        for (int i = 1; i < selectedRectTransforms.Length; i++)
        {


            if (orientation == Orientation.Vertical)
                selectedRectTransforms[i].anchoredPosition = new Vector3(selectedRectTransforms[i].anchoredPosition.x, selectedRectTransforms[0].anchoredPosition.y + interval * i * dir);
            else
                selectedRectTransforms[i].anchoredPosition = new Vector3(selectedRectTransforms[0].anchoredPosition.x + interval * i * dir, selectedRectTransforms[i].anchoredPosition.y);
        }
    }

    private void SetGameObject()
    {
        if (selecteObjects.Length > 0)
        {
            GameObject[] gameObjects = Selection.gameObjects;
            selectedRectTransforms = new RectTransform[gameObjects.Length]; 
            for (int i = 0; i < selecteObjects.Length; i++)
            {
                for (int j = 0; j < gameObjects.Length; j++)
                {
                    if (selecteObjects[i].name.Equals(gameObjects[j].name))
                    {
                        selectedRectTransforms[i] = gameObjects[j].GetComponent<RectTransform>();
                        break;
                    }
                }
            }
        }
    }
}
