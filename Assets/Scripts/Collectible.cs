using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : Entity<EntityData>
{
    public int score = 1000;

    private int _bounces = 0;

    public override void Awake()
    {
        base.Awake();
        _vulnerableToLayers = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    public override void Update()
    {
        Rect cameraBounds = Manager.instance.mainCam.GetCameraBounds();
        if (transform.position.x < cameraBounds.xMin || transform.position.x > cameraBounds.xMax)
        {
            transform.position = transform.position - Vector3.right * Mathf.Sign(transform.position.x - cameraBounds.center.x) * 0.1f;
            SetSpeed(new Vector2(-rb.velocity.x * 0.8f, rb.velocity.y));
        }
        if (transform.position.y < cameraBounds.yMin && _bounces < 2)
        {
            transform.position = transform.position - Vector3.up * Mathf.Sign(transform.position.y - cameraBounds.center.y) * 0.1f;
            SetSpeed(new Vector2(rb.velocity.x, -rb.velocity.y));
            _bounces++;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity>() is Entity e)
        {
            if (_vulnerableToLayers.Contains(collision.gameObject.layer))
            {
                Kill();
            }
        }
    }
}