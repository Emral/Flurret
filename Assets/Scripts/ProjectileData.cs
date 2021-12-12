using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectileData")]
[System.Serializable]
public class ProjectileData : EntityData
{
    public float maxLifespan;
    public bool destroyOnImpact;
    public float mass;
    public float throwSpeed;
    public Vector2 throwVector;
}