using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Camera))]
public class OnscreenTrigger : MonoBehaviour
{
    private Camera _camera;
    private BoxCollider2D _boxCollider;

    private static Dictionary<Collider2D, OnscreenTriggerable> Triggerables = new Dictionary<Collider2D, OnscreenTriggerable>();

    public static void RegisterTriggerable(Collider2D collider, OnscreenTriggerable triggerable)
    {
        if (!Triggerables.ContainsKey(collider))
        {
            Triggerables.Add(collider, triggerable);
        }
        else
        {
            Debug.Log("Triggerable " + collider.gameObject.name + " erroneously added twice.");
        }
    }

    public static void UnRegisterTriggerable(Collider2D collider)
    {
        if (Triggerables.ContainsKey(collider))
        {
            Triggerables.Remove(collider);
        }
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;
        SetColliderSize();
    }

    private void Update()
    {
        SetColliderSize();
    }

    private void SetColliderSize()
    {
        _boxCollider.size = _camera.GetCameraSize();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Triggerables.ContainsKey(collision))
        {
            if (Triggerables[collision].isOnScreen)
            {
                return;
            }

            Triggerables[collision].isOnScreen = true;

            Triggerables[collision].OnScreenEnterEvents.Invoke(_camera);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Triggerables.ContainsKey(collision))
        {
            if (!Triggerables[collision].isOnScreen)
            {
                return;
            }

            Triggerables[collision].isOnScreen = false;

            Triggerables[collision].OnScreenExitEvents.Invoke(_camera);
        }
    }
}