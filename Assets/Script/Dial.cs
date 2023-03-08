using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ���̾� Ŭ����
/// </summary>
public class Dial : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// ������Ʈ�� �߾� ��ǥ
    /// </summary>
    private Vector2 center;
    /// <summary>
    /// ���� ��ġ
    /// </summary>
    private Vector2 priorPosition;

    /// <summary>
    /// ���̾��� ȸ�� ������ �ٲ���� ���� �ݹ� �Լ��� ��������Ʈ
    /// </summary>
    /// <param name="angle">ȸ�� ����</param>
    public delegate void OnAngleChanged(float angle);
    /// <summary>
    /// ���̾��� ȸ�� ������ �ٲ���� ���� �ݹ� �Լ�
    /// </summary>
    public OnAngleChanged onAngleChanged;

    /// <summary>
    /// ����ڰ� ���̾� ���� �����ϰ� ���� ����
    /// </summary>
    public bool interactable = true;

    private void Start()
    {
        // �߾� ��ǥ �ʱ�ȭ
        center = transform.position;
    }

    /// <summary>
    /// �÷��̾ ���̾� �巡�׸� �������� ���� �̺�Ʈ
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ���� ��ġ ��ġ �ʱ����
        if(interactable)
            priorPosition = eventData.position;
    }

    /// <summary>
    /// �÷��̾ ���̾� �巡�� ���� ���� �̺�Ʈ
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            // �߾����� �������� ��ġ ��ġ�� ���� ��ġ ��ġ�� ���� ���� ����Ѵ�.
            // ��ġ�� ������ ����Ͽ� ȸ���� ������ ����Ѵ�.
            Vector2 touchPos = eventData.position;
            Vector2 v1 = center - priorPosition;
            Vector2 v2 = center - touchPos;
            float angle = Vector2.SignedAngle(v1, v2);
            priorPosition = touchPos;
            onAngleChanged(angle);

            // ���̾��� ���� ������ŭ ȸ���Ѵ�.
            transform.Rotate(new Vector3(0, 0, angle));
        }
    }

    /// <summary>
    /// Ư�� ������ ���̾� ȸ��
    /// </summary>
    /// <param name="angle">����</param>
    public void Rotate(float angle)
    {
        transform.Rotate(new Vector3(0, 0, angle));
    }
}
