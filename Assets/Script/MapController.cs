using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 노선도 제어 클래스
/// </summary>
public class MapController : MonoBehaviour
{
    /// <summary>
    /// 노선도 이미지의 Transform
    /// </summary>
    public Transform imageTransform;
    /// <summary>
    /// 손가락 확대/축소 속도
    /// </summary>
    float zoomSpeed = 0.2f;
    /// <summary>
    /// 노선도 드래그 이동 속도
    /// </summary>
    float moveSpeed = 1f;
    /// <summary>
    /// 노선도 최소 크기
    /// </summary>
    public float minSize = 2.5f;
    /// <summary>
    /// 노선도 최대 크기
    /// </summary>
    public float maxSize = 7.5f;

    /// <summary>
    /// 현재 드래그로 노선도를 이동 중인지 여부
    /// </summary>
    private bool isMove = false;

    /// <summary>
    /// 노선도 조작 시작
    /// </summary>
    public void StartMove()
    {
        isMove = true;
        StartCoroutine(Move());
    }

    /// <summary>
    /// 노선도 조작 관리 코루틴
    /// </summary>
    IEnumerator Move()
    {
        yield return new WaitForEndOfFrame();

        // 손가락 1개 터치일 때 드래그 이동 처리
        if (Input.touchCount.Equals(1))
            MoveMap();
        // 손가락 2개 터치일 때 확대/축소 처리
        else if (Input.touchCount.Equals(2))
            ZoomInOut();

        if (isMove)
            StartCoroutine(Move());
    }

    /// <summary>
    /// 확대/축소 처리
    /// </summary>
    private void ZoomInOut()
    {
        // 각 손가락의 위치를 얻어온다.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // 손가락 1의 직전 위치
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        // 손가락 2의 직전 위치
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // 직전 손가락 위치에서의 두 점 사이의 거리 계산
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        // 현재 손가락 위치에서의 두 점 사이의 거리 계산
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // 거리 변화 차이 계산
        float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

        // 거리 변화 차이에 따라 스케일 값에 적용
        imageTransform.localScale += deltaMagnitudeDiff * zoomSpeed * Vector3.one * Time.deltaTime;
        imageTransform.localScale = new Vector3(Mathf.Clamp(imageTransform.localScale.x, minSize, maxSize), Mathf.Clamp(imageTransform.localScale.y, minSize, maxSize), 1);
    }

    /// <summary>
    /// 노선도 드래그 이동 처리
    /// </summary>
    private void MoveMap()
    {
        Touch touch = Input.GetTouch(0);
        // 터치가 이동된 경우에 (드래그 중인 경우)
        if (touch.phase == TouchPhase.Moved)
        {
            // 이동 방향을 읽고 노선도 위치 변경 처리
            Vector2 touchDeltaPosition = touch.deltaPosition;
            float x = imageTransform.position.x;
            float y = imageTransform.position.y;

            imageTransform.position = new Vector3(x + touchDeltaPosition.x * moveSpeed, y + touchDeltaPosition.y * moveSpeed, 0);
        }
    }

    /// <summary>
    /// 노선도 위치 및 크기 초기화
    /// </summary>
    public void ResetMap()
    {
        imageTransform.localPosition = Vector3.zero;
        imageTransform.localScale = new Vector3(minSize, minSize, 1);
    }

    /// <summary>
    /// 노선도 조작 종료 처리
    /// </summary>
    public void EndMove()
    {
        isMove = false;
    }
}
