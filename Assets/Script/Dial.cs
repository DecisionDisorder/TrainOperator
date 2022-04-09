using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dial : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public Camera mainCamera;

    private Vector2 center;
    private Vector2 priorPosition;

    public delegate void OnAngleChanged(float angle);
    public OnAngleChanged onAngleChanged;

    public bool interactable = true;

    private void Start()
    {
        center = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(interactable)
            priorPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            Vector2 touchPos = eventData.position;
            Vector2 v1 = center - priorPosition;
            Vector2 v2 = center - touchPos;
            float angle = Vector2.SignedAngle(v1, v2);
            priorPosition = touchPos;
            onAngleChanged(angle);

            transform.Rotate(new Vector3(0, 0, angle));
        }
    }

    public void Rotate(float angle)
    {
        transform.Rotate(new Vector3(0, 0, angle));
    }
}
