using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProjectileRef
{
    public PlayerProjectileType type;
    public Projectile prefab;
}

[CreateAssetMenu(menuName = "PlayerData")]
[System.Serializable]
public class PlayerData : EntityData
{
    public float moveSpeed;
    public float maxSpeed;

    public float groundedDeceleration;
    public float aerialDeceleration;
    public float turnDeceleration;
    public float moveAcceleration;

    public float jumpForce;
    public float maximumJumpTime;
    public float coyoteTimeMax;
    public float risingGravityMultiplier;
    public float fallingGravityMultiplier;
    public float aerialMovementMultiplier;

    public float projectileShootCooldown;

    public List<PlayerProjectileRef> possibleProjectiles;
}