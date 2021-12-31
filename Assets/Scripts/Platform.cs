using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, ICollidable
{
    internal Vector2 _velocity;
    private List<GameObject> _children = new List<GameObject>();

    public virtual void CollisionEnter(GameObject other, Direction dir)
    {
        if (dir != Direction.Down)
        {
            return;
        }

        if (!_children.Contains(other))
        {
            _children.Add(other);

            DebugExtensions.Log("Child Added");
        }
    }

    public virtual void CollisionExit(GameObject other, Direction dir)
    {
        if (dir != Direction.Down)
        {
            return;
        }

        if (_children.Contains(other))
        {
            _children.Remove(other);

            DebugExtensions.Log("Child Removed");
        }
    }

    public virtual void CollisionStay(GameObject other, Direction dir)
    {
    }

    // Update is called once per frame
    internal virtual void Update()
    {
        Vector3 thisVelocity = new Vector3(_velocity.x, _velocity.y) * Time.deltaTime;

        transform.Translate(thisVelocity);

        _children.ForEach(t => t.transform.Translate(thisVelocity));

        DebugExtensions.Log("Movement applied to children", _children.Count);
    }
}
