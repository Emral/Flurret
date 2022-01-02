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
        if (col != null)
        {
            DoCollisions();
        }

        transform.Translate(new Vector3(velocity.x, velocity.y) * Time.deltaTime);
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

    private void DoCollisions()
    {
        for (int i=0; i < _collisionState.Count; i++)
        {
            Direction dir = _collisionState.Keys.ElementAt(i);
            Rect colliderEdge = GetColliderEdge(dir);
            Vector2 rayDir = HelperMaps.DirectionVectorMap[dir];
            float val = i < 2 ? velocity.x : velocity.y;
            if (val >= -0.1 && val <= 0.1f)
            {
                val = 0.1f;
            }
            else
            {
                rayDir = i < 2 ? Vector2.right : Vector2.up;
            }
            val.Log("Value for " + dir.ToString());
            float length = Mathf.Abs(val * Time.deltaTime) * Mathf.Sign(val);
            RaycastHit2D hit = Physics2D.BoxCast(colliderEdge.position, colliderEdge.size * 0.99f, 0, rayDir, length, _layerMasks[dir]);
            RaycastHit2D hit2 = Physics2D.BoxCast(colliderEdge.position - 0.05f * HelperMaps.DirectionVectorMap[dir], colliderEdge.size * 0.99f, 0, -HelperMaps.DirectionVectorMap[dir], Mathf.Abs(length), _layerMasks[dir]);
            
            if (hit && !hit2)
            {
                _collisionType[dir] = CollisionAction.Solid;
                if (_collisionState[dir] == ActionState.Hit)
                {
                    _collisionState[dir] = ActionState.Stay;
                }
                else
                {
                    _collisionState[dir] = ActionState.Hit;
                }

                if (hit.transform.GetComponent<ICollidable>() is ICollidable collidable)
                {

                    if (_collisionState[dir] == ActionState.Hit)
                    {
                        if (_lastCollisions[dir] != null)
                        {
                            _lastCollisions[dir].CollisionExit(gameObject, dir);
                            _lastCollisions[dir] = null;
                        }

                        collidable.CollisionEnter(gameObject, dir);
                        _lastCollisions[dir] = collidable;
                    }
                    collidable.CollisionStay(gameObject, dir);
                } else
                {
                    if (_lastCollisions[dir] != null)
                    {
                        _lastCollisions[dir].CollisionExit(gameObject, dir);
                        _lastCollisions[dir] = null;
                    }
                }
            }
            else
            {
                _collisionType[dir] = CollisionAction.None;
                if (_collisionState[dir] == ActionState.Leave)
                {
                    _collisionState[dir] = ActionState.None;
                }
                else
                {
                    _collisionState[dir] = ActionState.Leave;
                }

                if (_lastCollisions[dir] != null)
                {
                    _lastCollisions[dir].CollisionExit(gameObject, dir);
                    _lastCollisions[dir] = null;
                }
            }

            Collide(dir, _collisionState[dir], hit, colliderEdge.position);

            if (dir == Direction.Down && _collisionState[dir] == ActionState.Hit)
            {
                Vector3 pos = transform.position;
                //if (i < 2)
                //{
                //    pos.x = pos.x + (hit.point.x - colliderEdge.x);
                //    velocity.x = 0;
                //}
                //else
                //{
                pos.y = hit.point.y + (transform.position.y - colliderEdge.y) + 0.005f;
                velocity.y = 0;

                //}
                transform.position = pos;
            }

            Debug.DrawRay(colliderEdge.position + velocity * Time.deltaTime, rayDir * length, Color.yellow);
            Debug.DrawRay(colliderEdge.position + velocity * Time.deltaTime - 0.05f * HelperMaps.DirectionVectorMap[dir] + 0.1f * HelperMaps.DirectionPerpendicularMap[dir], - HelperMaps.DirectionVectorMap[dir] * Mathf.Abs(length), Color.red);

        }
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