using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

public class OnscreenTriggerable : MonoBehaviour
{
    public Collider2D triggerCollider;

    public UnityEvent<Camera> OnScreenEnterEvents;
    public UnityEvent<Camera> OnScreenExitEvents;

    [HideInInspector] public bool isOnScreen = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider2D>();
        }
        OnscreenTrigger.RegisterTriggerable(triggerCollider, this);
    }

    private void OnDestroy()
    {
        Unregister();
    }

    public void Unregister()
    {
        OnscreenTrigger.UnRegisterTriggerable(triggerCollider);
    }
}