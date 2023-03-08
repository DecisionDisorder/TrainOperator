using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �뼱�� ���� Ŭ����
/// </summary>
public class MapController : MonoBehaviour
{
    /// <summary>
    /// �뼱�� �̹����� Transform
    /// </summary>
    public Transform imageTransform;
    /// <summary>
    /// �հ��� Ȯ��/��� �ӵ�
    /// </summary>
    float zoomSpeed = 0.2f;
    /// <summary>
    /// �뼱�� �巡�� �̵� �ӵ�
    /// </summary>
    float moveSpeed = 1f;
    /// <summary>
    /// �뼱�� �ּ� ũ��
    /// </summary>
    public float minSize = 2.5f;
    /// <summary>
    /// �뼱�� �ִ� ũ��
    /// </summary>
    public float maxSize = 7.5f;

    /// <summary>
    /// ���� �巡�׷� �뼱���� �̵� ������ ����
    /// </summary>
    private bool isMove = false;

    /// <summary>
    /// �뼱�� ���� ����
    /// </summary>
    public void StartMove()
    {
        isMove = true;
        StartCoroutine(Move());
    }

    /// <summary>
    /// �뼱�� ���� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator Move()
    {
        yield return new WaitForEndOfFrame();

        // �հ��� 1�� ��ġ�� �� �巡�� �̵� ó��
        if (Input.touchCount.Equals(1))
            MoveMap();
        // �հ��� 2�� ��ġ�� �� Ȯ��/��� ó��
        else if (Input.touchCount.Equals(2))
            ZoomInOut();

        if (isMove)
            StartCoroutine(Move());
    }

    /// <summary>
    /// Ȯ��/��� ó��
    /// </summary>
    private void ZoomInOut()
    {
        // �� �հ����� ��ġ�� ���´�.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // �հ��� 1�� ���� ��ġ
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        // �հ��� 2�� ���� ��ġ
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // ���� �հ��� ��ġ������ �� �� ������ �Ÿ� ���
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        // ���� �հ��� ��ġ������ �� �� ������ �Ÿ� ���
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // �Ÿ� ��ȭ ���� ���
        float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

        // �Ÿ� ��ȭ ���̿� ���� ������ ���� ����
        imageTransform.localScale += deltaMagnitudeDiff * zoomSpeed * Vector3.one * Time.deltaTime;
        imageTransform.localScale = new Vector3(Mathf.Clamp(imageTransform.localScale.x, minSize, maxSize), Mathf.Clamp(imageTransform.localScale.y, minSize, maxSize), 1);
    }

    /// <summary>
    /// �뼱�� �巡�� �̵� ó��
    /// </summary>
    private void MoveMap()
    {
        Touch touch = Input.GetTouch(0);
        // ��ġ�� �̵��� ��쿡 (�巡�� ���� ���)
        if (touch.phase == TouchPhase.Moved)
        {
            // �̵� ������ �а� �뼱�� ��ġ ���� ó��
            Vector2 touchDeltaPosition = touch.deltaPosition;
            float x = imageTransform.position.x;
            float y = imageTransform.position.y;

            imageTransform.position = new Vector3(x + touchDeltaPosition.x * moveSpeed, y + touchDeltaPosition.y * moveSpeed, 0);
        }
    }

    /// <summary>
    /// �뼱�� ��ġ �� ũ�� �ʱ�ȭ
    /// </summary>
    public void ResetMap()
    {
        imageTransform.localPosition = Vector3.zero;
        imageTransform.localScale = new Vector3(minSize, minSize, 1);
    }

    /// <summary>
    /// �뼱�� ���� ���� ó��
    /// </summary>
    public void EndMove()
    {
        isMove = false;
    }
}
