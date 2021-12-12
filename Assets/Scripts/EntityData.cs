using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityData : ScriptableObject
{
    public float hpMax = 1;
    public float iFramesMax;

    public float gravity;
    public float terminalVelocity;
    public LayerMask groundedLayerMask;

    public Projectile usingProjectile;
}