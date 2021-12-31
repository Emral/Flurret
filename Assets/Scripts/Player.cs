using UnityEngine;

public enum PlayerAnimationState
{
    Idle = 0,
    Moving = 1,
    Jumping = 2,
    Landing = 3,
    Ducking = 4,
    LookingUp = 5
}

public enum PositioningState
{
    Grounded = 0,
    Aerial = 1
}

public enum PlayerProjectileType { Normal, Bouncy, Bomb }

public class Player : Entity<PlayerData>
{
    public Collider2D boundingCollider;

    private PlayerAnimationState _animState;
    private PositioningState __positionState;

    private float _fireDelay = 0;
    private readonly float _fireDelayMax = 0.05f;
    private readonly bool _isShooting;
    private float _currentJumpTime = 0;
    private float _coyoteTime = 0;
    private int _direction = 1;

    private PositioningState _positionState
    {
        get => __positionState;
        set
        {
            PositioningState lastState = __positionState;
            __positionState = value;
            if (__positionState == PositioningState.Grounded)
            {
                _coyoteTime = 0;
            }
            else if (lastState == PositioningState.Grounded && velocity.y <= 0)
            {
                _coyoteTime = data.coyoteTimeMax;
            }
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _vulnerableToLayers = LayerMask.GetMask("Enemy");
        _layerMasks[Direction.Down] = data.groundedLayerMask;
        _layerMasks[Direction.Left] = data.leftWallLayerMask;
        _layerMasks[Direction.Right] = data.rightWallLayerMask;
        _layerMasks[Direction.Up] = data.ceilingLayerMask;
        Manager.instance.playerInstance = this;
        Manager.instance.mainCam.GetComponent<CameraMovement>().AddTarget(this, true);
    }

    // Update is called once per frame
    public override void Update()
    {
        //if (Manager.instance.GetIsPaused())
        //{
        //    return;
        //}
        base.Update();
        _fireDelay = _fireDelay - Time.deltaTime;

        Move();
        Attack();
    }

    public void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (data.usingProjectile != null)
            {
                Projectile instance = Instantiate(data.usingProjectile, transform.position, Quaternion.identity);
                instance.renderGroup.localScale = renderGroup.localScale;
                instance.SetSpeed(new Vector2(instance.data.throwVector.x * _direction, instance.data.throwVector.y));
            }
        }
    }

    public void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector2 newVelocity = velocity;

        if (horizontalInput != 0)
        {
            _direction = (int)Mathf.Sign(horizontalInput);
            Vector3 localScale = renderGroup.transform.localScale;
            localScale.x = _direction;
            renderGroup.transform.localScale = localScale;
        }

        bool hasBottomCollision = HelperMaps.IsState(_collisionState[Direction.Down], ActionState.Stay);
        bool hasTopCollision = HelperMaps.IsState(_collisionState[Direction.Up], ActionState.Stay);

        _coyoteTime -= Time.deltaTime;

        bool canJump = hasBottomCollision || _coyoteTime > 0;

        canJump.Log("Can Jump");

        if (Input.GetButtonDown("Jump") && canJump)
        {
            _positionState = PositioningState.Aerial;
            _currentJumpTime = 0;
            _coyoteTime = 0;
        }

        if (_positionState == PositioningState.Aerial && _currentJumpTime < data.maximumJumpTime && Input.GetButton("Jump"))
        {
            newVelocity.y = data.jumpForce;
            _currentJumpTime += Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            _currentJumpTime = data.maximumJumpTime;
        }

        horizontalInput = horizontalInput * (_positionState == PositioningState.Grounded ? 1 : data.aerialMovementMultiplier) * (horizontalInput * velocity.x > 0 ? data.moveAcceleration : data.turnDeceleration);

        newVelocity.x = velocity.x * (_positionState == PositioningState.Grounded ? data.groundedDeceleration : data.aerialDeceleration) + horizontalInput * data.moveSpeed;

        if (Mathf.Abs(newVelocity.x) > data.maxSpeed)
        {
            newVelocity.x = Mathf.SmoothStep(newVelocity.x, data.maxSpeed * Mathf.Sign(newVelocity.x), 0.9f);
        }

        if (_positionState == PositioningState.Aerial)
        {
            newVelocity.y += data.gravity * (newVelocity.y > 0 ? data.risingGravityMultiplier : data.fallingGravityMultiplier) * Time.deltaTime;

            if (newVelocity.y < -data.terminalVelocity)
            {
                newVelocity.y = Mathf.SmoothStep(newVelocity.y, -data.terminalVelocity, 0.9f);
            }
        }
        else
        {
            newVelocity.y = 0;
        }

        velocity = newVelocity;

        if (canJump)
        {
            if (Mathf.Abs(velocity.x) < 1)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    _animState = PlayerAnimationState.LookingUp;
                }
                else if (Input.GetAxis("Vertical") < 0)
                {
                    _animState = PlayerAnimationState.Ducking;
                }
                else
                {
                    _animState = PlayerAnimationState.Idle;
                }
            }
            else
            {
                _animState = PlayerAnimationState.Moving;
            }
        }
        else
        {
            if (velocity.y > 0)
            {
                _animState = PlayerAnimationState.Jumping;
            }
            else
            {
                _animState = PlayerAnimationState.Landing;
            }
        }
        //boundingCollider.KeepOnScreen(Manager.instance.mainCam);
        an.SetInteger("State", (int)_animState);
    }

    public override void Collide(Direction dir, ActionState state, RaycastHit2D hit, Vector2 source)
    {
        base.Collide(dir, state, hit, source);

        bool isColliding = HelperMaps.IsState(state, ActionState.Stay);

        Vector2 vel = velocity;
        switch (dir)
        {
            case Direction.Down:
                _positionState = (isColliding && vel.y < 0) ? PositioningState.Grounded : PositioningState.Aerial;

                if (isColliding &&  vel.y < 0)
                {
                    vel.y = 0;
                }
                break;
            case Direction.Up:
                if (isColliding)
                {
                    vel.y = 0;
                    _currentJumpTime = data.maximumJumpTime;
                }
                break;
            case Direction.Left:
            case Direction.Right:
                if (isColliding)
                {
                    vel.x = 0;
                }
                break;
        }

        velocity = vel;
    }

    public override void Kill()
    {
        //mainRenderer.enabled = false;
        //AudioManager.PlaySFX(SFX.PlayerKillForever);
    }

    public override bool GetShouldUpdateCameraTargetPosition()
    {
        return true;//_isGrounded;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Manager.instance.GetIsPaused())
        {
            return;
        }
        base.OnTriggerEnter2D(collision);

        if (collision.GetComponent<Entity>() is Entity e)
        {
            if (_vulnerableToLayers.Contains(collision.gameObject.layer) && _iframes <= 0)
            {
                _iframes = data.iFramesMax;
                hp = hp - 1 / ((float)Manager.instance.Shields + 1);

                if (hp <= 0)
                {
                    Kill();
                }
                else
                {
                    AudioManager.PlaySFX(SFX.PlayerHarm);
                }
            }
        }
    }
}