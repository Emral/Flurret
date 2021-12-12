using UnityEngine;

public class Projectile : Entity<ProjectileData>
{
    private float _lifespan = 0;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        rb.mass = data.mass;
    }

    // Update is called once per frame
    public override void Update()
    {
        _lifespan += Time.deltaTime;
        if (_lifespan >= data.maxLifespan)
        {
            Destroy(gameObject);
        }
    }

    public override void SetSpeed(Vector2 speed)
    {
        rb.velocity = data.throwSpeed * speed;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (Manager.instance.GetIsPaused())
        {
            return;
        }

        if (data.destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }

    public void Impact(bool isEnemy)
    {
        Destroy(gameObject);
    }
}