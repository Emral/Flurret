using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget
{
    public Entity target;
    public Vector2 lastUpdatedPosition;

    public CameraTarget(Entity e)
    {
        target = e;
    }
}

public class CameraMovement : MonoBehaviour
{
    private Coroutine _currentRoutine;

    public static CameraMovement instance;

    private Bounds _currentBounds;

    private List<CameraTarget> _overrideTargets = null;

    private List<CameraTarget> _targets = new List<CameraTarget>();

    private float _targetsChangedLerp = 0;

    private Vector2 _positionBeforeTargetsChanged;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        _positionBeforeTargetsChanged = transform.position;
    }

    private void ResetLerp(bool skipReset)
    {
        if (!skipReset)
        {
            _targetsChangedLerp = 0;
            _positionBeforeTargetsChanged = transform.position;
        } else
        {
            _targetsChangedLerp = 1;
        }
    }

    public void SetBounds(BoxCollider2D boundsCollider, bool snap = false)
    {
        _currentBounds = boundsCollider.bounds;
        ResetLerp(snap);
    }

    public void OverrideTargets(List<Entity> overrides, bool snap = false)
    {
        _overrideTargets = new List<CameraTarget>();

        foreach (Entity target in overrides)
        {
            _overrideTargets.Add(new CameraTarget(target));
        }

        ResetLerp(snap);
    }

    public void RemoveOverride(bool snap = false)
    {
        _overrideTargets = null;
        ResetLerp(snap);
    }

    public void AddTarget(Entity e, bool snap = false)
    {
        _targets.Add(new CameraTarget(e));
        ResetLerp(snap);
    }

    public void RemoveTarget(Entity e, bool snap = false)
    {
        foreach (CameraTarget target in _targets)
        {
            if (target.target == e)
            {
                _targets.Remove(target);
            }
        }
        ResetLerp(snap);
    }

    private Vector2 GetPositionOfTarget(CameraTarget target)
    {
        if (target.target.GetShouldUpdateCameraTargetPosition())
        {
            target.lastUpdatedPosition = target.target.transform.position;
        }

        return target.lastUpdatedPosition;
    }

    private Vector2 GetMiddleOfAllTargets()
    {
        List<CameraTarget> targets = _overrideTargets != null ? _overrideTargets : _targets;

        Vector2 centerPosition = Vector2.zero;

        if (targets.Count > 0)
        {
            centerPosition = GetPositionOfTarget(targets[0]);

            for (int i=1; i<targets.Count; i++)
            {
                centerPosition += 0.5f * GetPositionOfTarget(targets[i]);
            }
        }

        return centerPosition;
    }

    public Vector2 FitToBounds(Vector2 currentTargetPosition)
    {
        Rect bounds = Manager.instance.mainCam.GetCameraBounds();
        Bounds targetBounds = _currentBounds;
        Vector2 camSize = Manager.instance.mainCam.GetCameraSize();
        targetBounds.size -= (Vector3) camSize;

        return new Vector2(Mathf.Clamp(currentTargetPosition.x, targetBounds.min.x, targetBounds.max.x), Mathf.Clamp(currentTargetPosition.y, targetBounds.min.y, targetBounds.max.y));
    }

    private void Update()
    {
        Vector2 targetPosition = GetMiddleOfAllTargets();
        if (_currentBounds != null)
        {
            targetPosition = FitToBounds(targetPosition);
        }

        _targetsChangedLerp = Mathf.Clamp01(_targetsChangedLerp + 2 * Manager.deltaTime);

        targetPosition.x = Mathf.SmoothStep(_positionBeforeTargetsChanged.x, targetPosition.x, _targetsChangedLerp);
        targetPosition.y = Mathf.SmoothStep(_positionBeforeTargetsChanged.y, targetPosition.y, _targetsChangedLerp);

        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }

    public void SetCameraBehaviour(IEnumerator newBehaviour)
    {
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
        }
        _currentRoutine = StartCoroutine(newBehaviour);
    }
}