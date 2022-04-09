using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Transform imageTransform;
    float zoomSpeed = 0.2f;
    float moveSpeed = 1f;
    public float minSize = 2.5f;
    public float maxSize = 7.5f;

    private bool isMove = false;

    public void StartMove()
    {
        isMove = true;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        yield return new WaitForEndOfFrame();

        if (Input.touchCount.Equals(1))
            MoveMap();
        else if (Input.touchCount.Equals(2))
            ZoomInOut();

        if (isMove)
            StartCoroutine(Move());
    }

    private void ZoomInOut()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

        imageTransform.localScale += deltaMagnitudeDiff * zoomSpeed * Vector3.one * Time.deltaTime;
        imageTransform.localScale = new Vector3(Mathf.Clamp(imageTransform.localScale.x, minSize, maxSize), Mathf.Clamp(imageTransform.localScale.y, minSize, maxSize), 1);
    }

    private void MoveMap()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = touch.deltaPosition;
            float x = imageTransform.position.x;
            float y = imageTransform.position.y;

            imageTransform.position = new Vector3(x + touchDeltaPosition.x * moveSpeed, y + touchDeltaPosition.y * moveSpeed, 0);
        }
    }

    public void ResetMap()
    {
        imageTransform.localPosition = Vector3.zero;
        imageTransform.localScale = new Vector3(minSize, minSize, 1);
    }

    public void EndMove()
    {
        isMove = false;
    }
}
