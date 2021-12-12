using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity<EnemyData>
{
    public int score;

    private float despawnTimer = -1;

    public enum EnemyType
    {
        Default,
        Train,
        Fly,
        Airplane,
        Fridge,
        Item,
        HWave,
        VWave,
        HUTurn,
        VUTurn,
        Quadcopter,
        RoomRoom,
        Balloon,
        LaserPointer,
        Trainpiece,
    }

    private Dictionary<EnemyType, Action> _enemyUpdateBehaviours;
    private Dictionary<EnemyType, Action> _enemyDeathBehaviours;

    public EnemyType type;
    public Vector2 facingDirection = Vector2.down;
    public bool shootsProjectiles;
    public bool facesPlayer;
    public float shotInterval = 1;
    internal int _numericState = 0;

    internal float shotTimer = 0;

    private AudioSource _persistentSound;

    internal Vector2 _aiVector1;
    internal float _aiFloat1;
    internal Animator _animator;

    public override void Awake()
    {
        base.Awake();

        _enemyUpdateBehaviours = new Dictionary<EnemyType, Action>()
        {
        };

        _enemyDeathBehaviours = new Dictionary<EnemyType, Action>()
        {
        };
        despawnTimer = 0;
    }

    public void OverrideType(EnemyType e)
    {
        type = e;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        _animator = GetComponent<Animator>();
        base.Start();
        _vulnerableToLayers = LayerMask.GetMask("Player");

        switch (type)
        {
            default:
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        if (facesPlayer)
        {
            facingDirection = Manager.instance.playerInstance.transform.position - transform.position;
            facingDirection.Normalize();
        }

        if (_enemyUpdateBehaviours.ContainsKey(type))
        {
            _enemyUpdateBehaviours[type]();
        }

        if (despawnTimer >= 0)
        {
            despawnTimer += Manager.deltaTime;
            if (despawnTimer >= 6)
            {
                Despawn();
            }
        }
    }

    public override void Kill()
    {
        GetComponent<OnscreenTriggerable>().Unregister();
        if (_enemyDeathBehaviours.ContainsKey(type))
        {
            _enemyDeathBehaviours[type]();
        }
        else
        {
            base.Kill();
        }

        Manager.instance.SpawnEffect(EffectType.EnemyDeath, transform.position);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (_persistentSound != null)
        {
            _persistentSound.Stop();
        }
    }

    public void InitiateDespawn()
    {
        despawnTimer = 4;
    }

    public void DeinitiateDespawn()
    {
        despawnTimer = -1;
    }

    public override Vector2 GetFacingDirection()
    {
        if (facesPlayer)
        {
            return facingDirection;
        }
        else
        {
            return base.GetFacingDirection();
        }
    }

    public void SetState(int state)
    {
        _numericState = state;
    }
}