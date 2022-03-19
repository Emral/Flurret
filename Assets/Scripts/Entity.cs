using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public abstract class Entity : MonoBehaviour, IFacingDirection
{
    internal float _iframes = 0;
    internal float hp;

    public Transform renderGroup;

    public UnityEvent<Entity> OnDestroyEvent;

    internal LayerMask _vulnerableToLayers;

    private Dictionary<Direction, ICollidable> _lastCollisions = new Dictionary<Direction, ICollidable>()
    {
        [Direction.Left] = null,
        [Direction.Right] = null,
        [Direction.Up] = null,
        [Direction.Down] = null
    };

    private Dictionary<Direction, ICollidable> _nextCollisions = new Dictionary<Direction, ICollidable>()
    {
        [Direction.Left] = null,
        [Direction.Right] = null,
        [Direction.Up] = null,
        [Direction.Down] = null
    };

    internal Dictionary<Direction, LayerMask> _layerMasks = new Dictionary<Direction, LayerMask>()
    {
        [Direction.Left] = 0,
        [Direction.Right] = 0,
        [Direction.Up] = 0,
        [Direction.Down] = 0
    };

    internal Vector2 velocity;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator an;
    [HideInInspector] public BoxCollider2D col;

    internal Dictionary<Direction, CollisionAction> _collisionType = new Dictionary<Direction, CollisionAction>() {
        [Direction.Left] = CollisionAction.None,
        [Direction.Right] = CollisionAction.None,
        [Direction.Up] = CollisionAction.None,
        [Direction.Down] = CollisionAction.None
    };

    internal Dictionary<Direction, ActionState> _collisionState = new Dictionary<Direction, ActionState>()
    {
        [Direction.Left] = ActionState.None,
        [Direction.Right] = ActionState.None,
        [Direction.Up] = ActionState.None,
        [Direction.Down] = ActionState.None
    };

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
        if (_iframes > 0)
        {
            _iframes = _iframes - Manager.deltaTime;
        }

    }

    public virtual void LateUpdate()
    {
        foreach (Direction dir in _nextCollisions.Keys)
        {
            if (_nextCollisions[dir] == null && _lastCollisions[dir] != null)
            {
                _lastCollisions[dir].CollisionExit(this, dir);
                _lastCollisions[dir] = _nextCollisions[dir];
                _nextCollisions[dir] = null;
            }
        }

        if (col != null)
        {
            TryMoveVertically(velocity.y * Time.deltaTime);
            TryMoveHorizontally(velocity.x * Time.deltaTime);
        }

        //transform.Translate(new Vector3(velocity.x, velocity.y) * Time.deltaTime);
    }

    public virtual void SetSpeed(Vector2 speed)
    {
        velocity = speed;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
    }

    public virtual bool GetShouldUpdateCameraTargetPosition()
    {
        return true;
    }

    public virtual void Collide(Direction dir, ActionState state, RaycastHit2D hit, Vector2 source)
    {

    }

    public virtual void Kill()
    {
        Destroy(gameObject);
    }

    public virtual Vector2 GetFacingDirection()
    {
        return velocity;
    }

    public virtual void OnDestroy()
    {
        if (!Manager.instance.GetIsPaused())
        {
            if (OnDestroyEvent != null)
            {
                OnDestroyEvent.Invoke(this);
            }
        }
    }

    private Rect GetColliderEdge(Direction dir)
    {
        return new Rect(
            col.bounds.center + new Vector3(HelperMaps.DirectionVectorMap[dir].x * col.bounds.extents.x, HelperMaps.DirectionVectorMap[dir].y * col.bounds.extents.y),
            new Vector2(Mathf.Max(HelperMaps.DirectionPerpendicularMap[dir].x * col.bounds.size.x, 0.01f), Mathf.Max(HelperMaps.DirectionPerpendicularMap[dir].y * col.bounds.size.y, 0.01f)));
    }

    private void ResetDirectionalCollision(Direction dir)
    {
        if (_nextCollisions[dir] != null)
        {
            _nextCollisions[dir] = null;
        }
    }

    private bool TryMoveInternal(float desiredDistance, Direction dir, Vector2 movementVector, ICollidable parent)
    {
        if (Mathf.Abs(desiredDistance) < 0.0001f)
        {
            return true;
        }

        float movedDistance = desiredDistance;
        float sign = Mathf.Sign(desiredDistance);
        if (desiredDistance == 0)
        {
            sign = 1;
        }

        float colliderSizeMultiplier = 0.5f;
        float skinWidth = 0.02f;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position.To2D() + col.offset, col.size * colliderSizeMultiplier * 2 + movementVector * 2 * skinWidth, 0, movementVector, desiredDistance, _layerMasks[dir]);

        if (hit && (movementVector == Vector2.right ? sign * hit.normal.x < 0 : sign * hit.normal.y < 0)
            &&      ((dir == Direction.Right && hit.normal.x < 0 && hit.point.x > transform.position.x + col.offset.x + colliderSizeMultiplier * col.size.x)
                 || (dir == Direction.Left  && hit.normal.x > 0 && hit.point.x < transform.position.x + col.offset.x - colliderSizeMultiplier * col.size.x)
                 || (dir == Direction.Up    && hit.normal.y < 0 && hit.point.y > transform.position.y + col.offset.y + colliderSizeMultiplier * col.size.y)
                 || (dir == Direction.Down  && hit.normal.y > 0 && hit.point.y < transform.position.y + col.offset.y - colliderSizeMultiplier * col.size.y)))
        {

            movedDistance = movementVector == Vector2.right ? hit.point.x - (transform.position.To2D().x + col.offset.x + (col.size.x * 0.5f + skinWidth) * sign)
                                                            : hit.point.y - (transform.position.To2D().y + col.offset.y + (col.size.y * 0.5f + skinWidth) * sign);

            _collisionType[dir] = CollisionAction.Solid;

            _collisionState[dir] = _collisionState[dir] == ActionState.Hit ? ActionState.Stay : ActionState.Hit;

            if (hit.transform.GetComponent<ICollidable>() is ICollidable collidable)
            {
                if (parent == null)
                {
                    if (_collisionState[dir] == ActionState.Hit)
                    {
                        ResetDirectionalCollision(dir);

                        collidable.CollisionEnter(this, dir);
                        _nextCollisions[dir] = collidable;
                    }
                    collidable.CollisionStay(this, dir);
                }
            }
            else
            {
                ResetDirectionalCollision(dir);
            }
        }
        else
        {
            _collisionType[dir] = CollisionAction.None;

            _collisionState[dir] = _collisionState[dir] == ActionState.Leave ? ActionState.None : ActionState.Leave;

            ResetDirectionalCollision(dir);
        }

        Collide(dir, _collisionState[dir], hit, hit.point);

        transform.Translate(movementVector * movedDistance);
        //Vector3 position = transform.position;
        //position.x = Mathf.Round(position.x * 24) / 24;
        //position.y = Mathf.Round(position.y * 24) / 24;
        //transform.position = position;

        Vector2 edge = movementVector == Vector2.right ? (transform.position.To2D() + movementVector * (col.offset + (col.size * 0.5f) * sign))
                                                       : (transform.position.To2D() + movementVector * (col.offset + (col.size * 0.5f) * sign));
        Debug.DrawRay(edge, movementVector * movedDistance, Color.yellow);
        //DebugExtensions.LogMany("Moving", dir.ToString(), "At speed", movedDistance, "wanting", desiredDistance);

        return movedDistance == desiredDistance;
    }

    public bool TryMoveHorizontally(float desiredDistance, ICollidable parent = null)
    {
        return TryMoveInternal(desiredDistance, Mathf.Sign(desiredDistance) == 1 ? Direction.Right : Direction.Left, Vector2.right, parent);
    }

    public bool TryMoveVertically(float desiredDistance, ICollidable parent = null)
    {
        return TryMoveInternal(desiredDistance, Mathf.Sign(desiredDistance) == 1 ? Direction.Up : Direction.Down, Vector2.up, parent);
    }
}

public class Entity<T> : Entity where T : EntityData
{
    public T data;
    public override void Awake()
    {
        base.Awake();
        hp = data.hpMax;

        if (rb.gravityScale > 0)
        {
            rb.gravityScale = data.gravity / Physics.gravity.y;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Manager.instance.GetIsPaused())
        {
            return;
        }
        if (hp <= 0)
        {
            return;
        }

        if (collision.GetComponent<Projectile>() is Projectile p)
        {
            if (_vulnerableToLayers.Contains(collision.gameObject.layer))
            {
                p.Impact(true);
                if (_iframes > 0)
                {
                    return;
                }
                // iframes are damage block
                float damage = 1;
                _iframes = data.iFramesMax;

                hp = hp - damage;

                if (hp <= 0)
                {
                    Kill();
                }
            }
        }
    }
}