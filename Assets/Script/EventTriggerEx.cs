using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class EventTriggerEx : EventTrigger
{
    public static bool isScroll = false;
    private Vector2 initialPosition;
    /// <summary>
    /// Do action for all parents
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
    /// Always route initialize potential drag event to parents
    /// </summary>
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
        base.OnInitializePotentialDrag(eventData);
    }

    /// <summary>
    /// Drag Event
    /// </summary>
    public override void OnDrag(PointerEventData eventData)
    {
        DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        if (Vector2.Distance(initialPosition, eventData.position) > Screen.height * 0.05f)
            isScroll = true;
    }

    /// <summary>
    /// Begin Drag event
    /// </summary>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        initialPosition = eventData.position;
    }

    /// <summary>
    /// End drag event
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
