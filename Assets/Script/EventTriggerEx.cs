using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 이벤트 트리거 커스텀 클래스
/// </summary>
public class EventTriggerEx : EventTrigger
{
    /// <summary>
    /// 스크롤 중인지 여부
    /// </summary>
    public static bool isScroll = false;
    /// <summary>
    /// 드래그 시작 할 때 처음 위치
    /// </summary>
    private Vector2 initialPosition;

    /// <summary>
    /// 액션을 Hierarchy 상의 모든 부모 클래스에 전달
    /// </summary>
    private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            foreach (var component in parent.GetComponents<Component>())
            {
                if (component is T)
                    action((T)(IEventSystemHandler)component);
            }
            parent = parent.parent;
        }
    }

    /// <summary>
    /// 잠재적 드래그 이벤트를 부모에게 항상 라우팅
    /// </summary>
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
        base.OnInitializePotentialDrag(eventData);
    }

    /// <summary>
    /// 드래그 이벤트 처리
    /// </summary>
    public override void OnDrag(PointerEventData eventData)
    {
        DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        if (Vector2.Distance(initialPosition, eventData.position) > Screen.height * 0.05f)
            isScroll = true;
    }

    /// <summary>
    /// 드래그 시작 할 때의 이벤트 처리
    /// </summary>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        initialPosition = eventData.position;
    }

    /// <summary>
    /// 드래그 끝날 때의 이벤트 처리
    /// </summary>
    public override void OnEndDrag(PointerEventData eventData)
    {
        DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
        isScroll = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }
}
