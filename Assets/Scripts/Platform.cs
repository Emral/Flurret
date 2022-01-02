using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, ICollidable
{
    internal Vector2 _velocity;
    private List<GameObject> _children = new List<GameObject>();
    private Dictionary<GameObject, Direction> _directionMap = new Dictionary<GameObject, Direction>();

    public virtual void CollisionEnter(GameObject other, Direction dir)
    {
        if (!_children.Contains(other))
        {
            _children.Add(other);
            _directionMap.Add(other, dir);
        } else if (_directionMap[other] != dir)
        {
            _directionMap[other] = dir;
        }
    }

    public virtual void CollisionExit(GameObject other, Direction dir)
    {
        if (_children.Contains(other))
        {
            _children.Remove(other);
            _directionMap.Remove(other);
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

        _children.ForEach(t => {
            switch(_directionMap[t])
            {
                case Direction.Up:
                    if (thisVelocity.y < 0)
                    {
                        t.transform.Translate(Vector2.up * thisVelocity);
                    }
                    break;
                case Direction.Down:
                    t.transform.Translate(thisVelocity);
                    break;
                case Direction.Right:
                    if (thisVelocity.x < 0)
                    {
                        t.transform.Translate(Vector2.right * thisVelocity);
                    }
                    break;
                case Direction.Left:
                    if (thisVelocity.x > 0)
                    {
                        t.transform.Translate(Vector2.right * thisVelocity);
                    }
                    break;
            }
        });
    }
}
