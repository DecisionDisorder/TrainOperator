using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// 진행 방향(V/H) 선택 열거형
/// </summary>
public enum Orientation
{
    Vertical = 0,
    Horizontal = 1
}
/// <summary>
/// 진행 방향(Up/Down) 선택 열거형
/// </summary>
public enum Direction
{ 
    Down = 0,
    Up = 1
}

/// <summary>
/// 오브젝트 위치 일괄 조정 도우미 클래스
/// </summary>
[Serializable]
public class Reposition : EditorWindow
{
    /// <summary>
    /// Hierachy상에서 선택된 오브젝트 배열
    /// </summary>
    UnityEngine.Object[] selecteObjects = new UnityEngine.Object[0];
    /// <summary>
    /// 선택된 오브젝트들의 RectTrnasform 배열
    /// </summary>
    RectTransform[] selectedRectTransforms = new RectTransform[0];

    /// <summary>
    /// 설정된 간격
    /// </summary>
    float interval;
    /// <summary>
    /// 설정된 V/H 방향
    /// </summary>
    Orientation orientation;
    /// <summary>
    /// 설정된 상하 방향
    /// </summary>
    Direction direction;

    /// <summary>
    /// 유니티 툴바에 등록
    /// </summary>
    [MenuItem("Tools/Reposition")]
    public static void ShowWindow()
    {
        var window = GetWindow(typeof(Reposition));
        window.minSize = new Vector2(512, 128);
    }

    /// <summary>
    /// 유니티 에디터에서의 GUI 구성
    /// </summary>
    private void OnGUI()
    {
        // 아래 방향으로 추가
        EditorGUILayout.BeginVertical();
        // 타이틀 라벨 추가
        GUILayout.Label("Reposition", EditorStyles.boldLabel);
        // 공간 띄우기
        EditorGUILayout.Space();
        // 오른쪽으로 추가
        EditorGUILayout.BeginHorizontal();
        // 간격 정보를 받을 입력 필드 추가
        interval = EditorGUILayout.FloatField("Interval", interval);
        // 오른쪽 방향 추가 종료
        EditorGUILayout.EndHorizontal();

        // 공간 띄우기 및 V/H 방향 Enum 선택
        EditorGUILayout.Space();
        orientation = (Orientation)EditorGUILayout.EnumPopup(new GUIContent("Orientation"), orientation);

        // 공간 띄우기 및 상하 방향 Enum 선택
        EditorGUILayout.Space();
        direction = (Direction)EditorGUILayout.EnumPopup(new GUIContent("Direction(+/-)"), direction);

        // 공간 띄우기 및 reposition 시작 버튼 생성
        EditorGUILayout.Space();
        if (GUILayout.Button(new GUIContent("Reposition", "Reposition the gameobject in same interval"))) { Repositioning(); }
        EditorGUILayout.EndVertical();

        // 선택된 오브젝트가 1개 이상일 때
        if (selectedRectTransforms.Length > 0)
        {
            // Box 단위 UI 생성
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.Space();
            // 선택 중인 오브젝트의 이름과 위치 정보를 표시
            for (int i = 0; i < selecteObjects.Length; i++)
            {
                EditorGUILayout.LabelField(selectedRectTransforms[i].name + " (" + selectedRectTransforms[i].anchoredPosition.x + ", " + selectedRectTransforms[i].anchoredPosition.y + ")");
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }
    }

    // 매 프레임 마다 선택중인 오브젝트 업데이트
    private void Update()
    {
        selecteObjects = Selection.objects;
        SetGameObject();
    }

    /// <summary>
    /// 오브젝트 위치 재배치 적용
    /// </summary>
    private void Repositioning()
    {
        SetGameObject();

        // 상하 방향 값 설정
        float dir = 1;
        if (direction == Direction.Down)
            dir = -1;

        for (int i = 1; i < selectedRectTransforms.Length; i++)
        {
            // Vertical/Horizontal 에 따라서 x축 혹은 y축으로 연속적으로 계산
            if (orientation == Orientation.Vertical)
                selectedRectTransforms[i].anchoredPosition = new Vector3(selectedRectTransforms[i].anchoredPosition.x, selectedRectTransforms[0].anchoredPosition.y + interval * i * dir);
            else
                selectedRectTransforms[i].anchoredPosition = new Vector3(selectedRectTransforms[0].anchoredPosition.x + interval * i * dir, selectedRectTransforms[i].anchoredPosition.y);
        }
    }

    /// <summary>
    /// 선택된 게임 오브젝트의 속성 추출
    /// </summary>
    private void SetGameObject()
    {
        // 선택중인 오브젝트가 1개 이상일 때
        if (selecteObjects.Length > 0)
        {
            // 해당 오브젝트의 RectTransform 컴포넌트 캐싱
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
