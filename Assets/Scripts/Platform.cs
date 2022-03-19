using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, ICollidable
{
    internal Vector2 _velocity;
    private List<Entity> _children = new List<Entity>();
    private Dictionary<Entity, Direction> _directionMap = new Dictionary<Entity, Direction>();

    private Collider2D _collider;

    public virtual void CollisionEnter(Entity other, Direction dir)
    {
        if (!_children.Contains(other))
        {
            _children.Add(other);
            _directionMap.Add(other, dir);
            Debug.Log("Adding player as child in direction " + dir);
        } else if (_directionMap[other] != dir)
        {
            _directionMap[other] = dir;
        }
    }

    public virtual void CollisionExit(Entity other, Direction dir)
    {
        if (_children.Contains(other))
        {
            _children.Remove(other);
            _directionMap.Remove(other);
        }
    }

    public virtual void CollisionStay(Entity other, Direction dir)
    {
    }

    private void MoveVelocity(Vector2 velocity)
    {
        bool isHorizontal = velocity.x != 0;
        //transform.Translate(velocity);
        ContactFilter2D filter = new ContactFilter2D();
        filter.useNormalAngle = true;
        if (isHorizontal)
        {
            if (velocity.x > 0)
            {
                filter.minNormalAngle = -45 + 180;
                filter.maxNormalAngle = 45 + 180;
            }
            else
            {
                filter.minNormalAngle = -45 + 0;
                filter.maxNormalAngle = 45 + 0;
            }
        }
        else
        {
            if (velocity.y > 0)
            {
                filter.minNormalAngle = -45 + 90;
                filter.maxNormalAngle = 45 + 90;
            }
            else
            {
                filter.minNormalAngle = -45 + 270;
                filter.maxNormalAngle = 45 + 270;
            }
        }
        List<Collider2D> overlaps = new List<Collider2D>();
        int overlapCount = _collider.OverlapCollider(filter, overlaps);
        foreach(Collider2D overlap in overlaps)
        {
            if (overlap.GetComponent<Entity>() is Entity e && overlap.attachedRigidbody.IsTouching(_collider, filter))
            {
                if (isHorizontal)
                {
                    e.TryMoveHorizontally(velocity.x);
                    "Moving player horizontally".Log("HLOG");
                } else
                {
                    e.TryMoveVertically(velocity.y);
                    "Moving player vertically".Log("VLOG");
                }
            }
        }
        //transform.Translate(-velocity);
    }

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    internal virtual void Update()
    {
        Vector3 thisVelocity = new Vector3(_velocity.x, _velocity.y) * Time.deltaTime;

        //transform.Translate(thisVelocity);

        MoveVelocity(new Vector2(thisVelocity.x, 0));
        MoveVelocity(new Vector2(0, thisVelocity.y));
        transform.Translate(thisVelocity);

        _children.ForEach(t =>
        {
            switch (_directionMap[t])
            {
                case Direction.Up:
                    if (thisVelocity.y < 0)
                    {
                        t.TryMoveVertically(thisVelocity.y, this);
                    }
                    break;
                case Direction.Down:
                    t.TryMoveVertically(thisVelocity.y, this);
                    t.TryMoveHorizontally(thisVelocity.x, this);
                    break;
                case Direction.Right:
                    if (thisVelocity.x < 0)
                    {
                        t.TryMoveVertically(thisVelocity.x, this);
                    }
                    break;
                case Direction.Left:
                    if (thisVelocity.x > 0)
                    {
                        t.TryMoveVertically(thisVelocity.x, this);
                    }
                    break;
            }
        });
    }
}
