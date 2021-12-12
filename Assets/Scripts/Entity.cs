using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : MonoBehaviour, IFacingDirection
{
    internal float _iframes = 0;
    internal float hp;

    public Transform renderGroup;

    public UnityEvent<Entity> OnDestroyEvent;

    internal LayerMask _vulnerableToLayers;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator an;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
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

    public virtual void SetSpeed(Vector2 speed)
    {
        rb.velocity = speed;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
    }

    public virtual bool GetShouldUpdateCameraTargetPosition()
    {
        return true;
    }

    public virtual void Kill()
    {
        Destroy(gameObject);
    }

    public virtual Vector2 GetFacingDirection()
    {
        return rb.velocity;
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