using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// ���� ����(V/H) ���� ������
/// </summary>
public enum Orientation
{
    Vertical = 0,
    Horizontal = 1
}
/// <summary>
/// ���� ����(Up/Down) ���� ������
/// </summary>
public enum Direction
{ 
    Down = 0,
    Up = 1
}

/// <summary>
/// ������Ʈ ��ġ �ϰ� ���� ����� Ŭ����
/// </summary>
[Serializable]
public class Reposition : EditorWindow
{
    /// <summary>
    /// Hierachy�󿡼� ���õ� ������Ʈ �迭
    /// </summary>
    UnityEngine.Object[] selecteObjects = new UnityEngine.Object[0];
    /// <summary>
    /// ���õ� ������Ʈ���� RectTrnasform �迭
    /// </summary>
    RectTransform[] selectedRectTransforms = new RectTransform[0];

    /// <summary>
    /// ������ ����
    /// </summary>
    float interval;
    /// <summary>
    /// ������ V/H ����
    /// </summary>
    Orientation orientation;
    /// <summary>
    /// ������ ���� ����
    /// </summary>
    Direction direction;

    /// <summary>
    /// ����Ƽ ���ٿ� ���
    /// </summary>
    [MenuItem("Tools/Reposition")]
    public static void ShowWindow()
    {
        var window = GetWindow(typeof(Reposition));
        window.minSize = new Vector2(512, 128);
    }

    /// <summary>
    /// ����Ƽ �����Ϳ����� GUI ����
    /// </summary>
    private void OnGUI()
    {
        // �Ʒ� �������� �߰�
        EditorGUILayout.BeginVertical();
        // Ÿ��Ʋ �� �߰�
        GUILayout.Label("Reposition", EditorStyles.boldLabel);
        // ���� ����
        EditorGUILayout.Space();
        // ���������� �߰�
        EditorGUILayout.BeginHorizontal();
        // ���� ������ ���� �Է� �ʵ� �߰�
        interval = EditorGUILayout.FloatField("Interval", interval);
        // ������ ���� �߰� ����
        EditorGUILayout.EndHorizontal();

        // ���� ���� �� V/H ���� Enum ����
        EditorGUILayout.Space();
        orientation = (Orientation)EditorGUILayout.EnumPopup(new GUIContent("Orientation"), orientation);

        // ���� ���� �� ���� ���� Enum ����
        EditorGUILayout.Space();
        direction = (Direction)EditorGUILayout.EnumPopup(new GUIContent("Direction(+/-)"), direction);

        // ���� ���� �� reposition ���� ��ư ����
        EditorGUILayout.Space();
        if (GUILayout.Button(new GUIContent("Reposition", "Reposition the gameobject in same interval"))) { Repositioning(); }
        EditorGUILayout.EndVertical();

        // ���õ� ������Ʈ�� 1�� �̻��� ��
        if (selectedRectTransforms.Length > 0)
        {
            // Box ���� UI ����
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.Space();
            // ���� ���� ������Ʈ�� �̸��� ��ġ ������ ǥ��
            for (int i = 0; i < selecteObjects.Length; i++)
            {
                EditorGUILayout.LabelField(selectedRectTransforms[i].name + " (" + selectedRectTransforms[i].anchoredPosition.x + ", " + selectedRectTransforms[i].anchoredPosition.y + ")");
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }
    }

    // �� ������ ���� �������� ������Ʈ ������Ʈ
    private void Update()
    {
        selecteObjects = Selection.objects;
        SetGameObject();
    }

    /// <summary>
    /// ������Ʈ ��ġ ���ġ ����
    /// </summary>
    private void Repositioning()
    {
        SetGameObject();

        // ���� ���� �� ����
        float dir = 1;
        if (direction == Direction.Down)
            dir = -1;

        for (int i = 1; i < selectedRectTransforms.Length; i++)
        {
            // Vertical/Horizontal �� ���� x�� Ȥ�� y������ ���������� ���
            if (orientation == Orientation.Vertical)
                selectedRectTransforms[i].anchoredPosition = new Vector3(selectedRectTransforms[i].anchoredPosition.x, selectedRectTransforms[0].anchoredPosition.y + interval * i * dir);
            else
                selectedRectTransforms[i].anchoredPosition = new Vector3(selectedRectTransforms[0].anchoredPosition.x + interval * i * dir, selectedRectTransforms[i].anchoredPosition.y);
        }
    }

    /// <summary>
    /// ���õ� ���� ������Ʈ�� �Ӽ� ����
    /// </summary>
    private void SetGameObject()
    {
        // �������� ������Ʈ�� 1�� �̻��� ��
        if (selecteObjects.Length > 0)
        {
            // �ش� ������Ʈ�� RectTransform ������Ʈ ĳ��
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
