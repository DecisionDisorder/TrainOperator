using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 다이얼 클래스
/// </summary>
public class Dial : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    /// <summary>
    /// 메인 카메라
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// 오브젝트의 중앙 좌표
    /// </summary>
    private Vector2 center;
    /// <summary>
    /// 이전 위치
    /// </summary>
    private Vector2 priorPosition;

    /// <summary>
    /// 다이얼의 회전 각도가 바뀌었을 떄의 콜백 함수의 델리게이트
    /// </summary>
    /// <param name="angle">회전 각도</param>
    public delegate void OnAngleChanged(float angle);
    /// <summary>
    /// 다이얼의 회전 각도가 바뀌었을 떄의 콜백 함수
    /// </summary>
    public OnAngleChanged onAngleChanged;

    /// <summary>
    /// 사용자가 다이얼 조작 가능하게 할지 여부
    /// </summary>
    public bool interactable = true;

    private void Start()
    {
        // 중앙 좌표 초기화
        center = transform.position;
    }

    /// <summary>
    /// 플레이어가 다이얼 드래그를 시작했을 때의 이벤트
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 직전 터치 위치 초기ㅗ하
        if(interactable)
            priorPosition = eventData.position;
    }

    /// <summary>
    /// 플레이어가 다이얼 드래그 중일 때의 이벤트
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            // 중앙점을 기준으로 터치 위치와 직전 터치 위치의 벡터 값을 계산한다.
            // 터치의 방향을 계산하여 회전한 각도를 계산한다.
            Vector2 touchPos = eventData.position;
            Vector2 v1 = center - priorPosition;
            Vector2 v2 = center - touchPos;
            float angle = Vector2.SignedAngle(v1, v2);
            priorPosition = touchPos;
            onAngleChanged(angle);

            // 다이얼을 계산된 각도만큼 회전한다.
            transform.Rotate(new Vector3(0, 0, angle));
        }
    }

    /// <summary>
    /// 특정 각도로 다이얼 회전
    /// </summary>
    /// <param name="angle">각도</param>
    public void Rotate(float angle)
    {
        transform.Rotate(new Vector3(0, 0, angle));
    }
}
