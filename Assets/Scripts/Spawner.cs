using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnBehaviour
{
    public Entity objectToSpawn;
    public Vector2 offset;
    public Enemy.EnemyType typeOverride;

    public Vector2 speed;
    public float count = 1;
    public float countMax = 9f;
    public float stopCountIf = 99f;

    public float startDelay = 0;
    public float interval = 0;
    public Vector2 offsetDelta;

    public float speedVelocityDelta;
    public float speedAngleDelta;

    public string customData;
}

public class Spawner : MonoBehaviour
{
    public List<SpawnBehaviour> spawns = new List<SpawnBehaviour>();
    public bool keepParent = false;
    public bool isLastOfWave = false;

    public void BeginSpawning()
    {
        if (!keepParent)
        {
            transform.SetParent(null);
        }
        foreach (SpawnBehaviour s in spawns)
        {
            StartCoroutine(SpawnFromBehaviour(s));
        }
    }

    private IEnumerator SpawnFromBehaviour(SpawnBehaviour behaviour)
    {
        float leftToSpawn = Mathf.Min(behaviour.count + Manager.enemyWaveCountAddition, behaviour.countMax);
        if (behaviour.stopCountIf > 90 || behaviour.count + Manager.enemyWaveCountAddition < behaviour.stopCountIf)
        {
            Vector2 spawnSpeed = behaviour.speed;
            if (keepParent && GetComponentInParent<Rigidbody2D>() is Rigidbody2D rb)
            {
                spawnSpeed = spawnSpeed + rb.velocity;
            }
            Vector2 spawnOffset = behaviour.offset;
            yield return new WaitForSeconds(behaviour.startDelay);
            while (leftToSpawn > 0)
            {
                Transform parent = null;
                if (keepParent)
                {
                    parent = transform.parent;
                }
                Entity newEntity = Instantiate(behaviour.objectToSpawn, parent);
                newEntity.transform.position = transform.position + new Vector3(spawnOffset.x, spawnOffset.y);
                newEntity.SetSpeed(spawnSpeed);
                //newEntity.hpMax = newEntity.hpMax + Manager.enemyHPAddition;
                newEntity.hp = newEntity.hp + Manager.enemyHPAddition;
                //newEntity.customData = behaviour.customData;
                if (newEntity is Enemy e && behaviour.typeOverride != Enemy.EnemyType.Default)
                {
                    e.OverrideType(behaviour.typeOverride);
                }

                spawnOffset = spawnOffset + behaviour.offsetDelta;
                spawnSpeed = spawnSpeed + spawnSpeed * behaviour.speedVelocityDelta;
                spawnSpeed = Quaternion.Euler(0, 0, behaviour.speedAngleDelta) * spawnSpeed;
                leftToSpawn = leftToSpawn - 1;
                yield return new WaitForSeconds(behaviour.interval);
            }
        }
    }
}