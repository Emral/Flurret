using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_WeightedHover : Platform
{
    public Vector3 maxOffset = Vector2.one;

    public float amplitude;
    public float frequency;

    public Axis axis = Axis.Y;

    private bool beingWeighted = false;

    private Vector2 targetPosition;

    private Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.localPosition;
    }

    // Update is called once per frame
    internal override void Update()
    {
        if (beingWeighted)
        {
            switch (axis)
            {
                case Axis.Y:
                    offset.y = Mathf.SmoothStep(offset.y + 1 * Time.deltaTime, -maxOffset.y, 0.6f);
                    break;
                case Axis.X:
                    offset.y = Mathf.SmoothStep(offset.y + 1 * Time.deltaTime, -maxOffset.y, 0.6f);
                    offset.x = Mathf.SmoothStep(offset.y, Mathf.Sin(Time.time * frequency) * amplitude, 0.6f);
                    break;
            }
        } else
        {
            switch (axis)
            {
                case Axis.Y:
                    offset.y = Mathf.SmoothStep(offset.y, Mathf.Sin(Time.time * frequency) * amplitude, 0.6f);
                    break;
                case Axis.X:
                    offset.y = Mathf.SmoothStep(offset.y, 0, 0.6f);
                    offset.x = Mathf.SmoothStep(offset.y, Mathf.Sin(Time.time * frequency) * amplitude, 0.6f);
                    break;
            }
        }
        beingWeighted = false;

        Vector2 current = targetPosition + offset;
        _velocity = new Vector2(current.x - transform.localPosition.x, current.y - transform.localPosition.y);

        base.Update();
    }

    public override void CollisionStay(Entity other, Direction dir)
    {
        if (dir != Direction.Down)
        {
            return;
        }

        if (other.gameObject.layer == 6)
        {
            beingWeighted = true;
        }
    }
}
