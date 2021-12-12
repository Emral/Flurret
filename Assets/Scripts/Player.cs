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

public enum PlayerProjectileType { Normal, Bouncy, Bomb }

public class Player : Entity<PlayerData>
{
    public Collider2D boundingCollider;

    public Transform feetLocation;
    public BoxCollider2D collisionBox;

    private PlayerAnimationState _animState;

    private float _fireDelay = 0;
    private readonly float _fireDelayMax = 0.05f;
    private readonly bool _isShooting;
    private float _currentJumpTime = 0;
    private float _coyoteTime = 0;
    private bool __isGrounded;
    private int _direction = 1;

    private bool _isGrounded
    {
        get => __isGrounded;
        set
        {
            bool wasGrounded = __isGrounded;
            __isGrounded = value;
            if (_isGrounded)
            {
                _coyoteTime = 0;
            }
            else if (wasGrounded && rb.velocity.y <= 0)
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
        Vector2 newVelocity = rb.velocity;

        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            _direction = (int)Mathf.Sign(horizontalInput);
            Vector3 localScale = renderGroup.transform.localScale;
            localScale.x = _direction;
            renderGroup.transform.localScale = localScale;
        }

        if (rb.velocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.BoxCast(feetLocation.position, new Vector2(collisionBox.size.x, 0.01f), 0, Vector2.down, 0.01f, data.groundedLayerMask);
            RaycastHit2D hit2 = Physics2D.BoxCast(feetLocation.position + Vector3.up * 0.01f, new Vector2(collisionBox.size.x, 0.01f), 0, Vector2.up, 0.5f, data.groundedLayerMask);
            if (hit.collider != null && hit2.collider == null)
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }

        bool canJump = _isGrounded || _coyoteTime > 0;

        _coyoteTime -= Time.deltaTime;

        if (Input.GetButtonDown("Jump") && canJump)
        {
            _isGrounded = false;
            _currentJumpTime = 0;
            _coyoteTime = 0;
        }

        if (!_isGrounded && _currentJumpTime < data.maximumJumpTime && Input.GetButton("Jump"))
        {
            newVelocity.y = data.jumpForce;
            _currentJumpTime += Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            _currentJumpTime = data.maximumJumpTime;
        }

        horizontalInput = horizontalInput * (_isGrounded ? 1 : data.aerialMovementMultiplier) * (horizontalInput * rb.velocity.x > 0 ? data.moveAcceleration : data.turnDeceleration);

        newVelocity.x = rb.velocity.x * (_isGrounded ? data.groundedDeceleration : data.aerialDeceleration) + horizontalInput * data.moveSpeed;

        if (Mathf.Abs(newVelocity.x) > data.maxSpeed)
        {
            newVelocity.x = Mathf.SmoothStep(newVelocity.x, data.maxSpeed * Mathf.Sign(newVelocity.x), 0.9f);
        }

        if (!_isGrounded)
        {
            newVelocity.y += data.gravity * (newVelocity.y > 0 ? data.risingGravityMultiplier : data.fallingGravityMultiplier);

            if (newVelocity.y < -data.terminalVelocity)
            {
                newVelocity.y = Mathf.SmoothStep(newVelocity.y, -data.terminalVelocity, 0.9f);
            }
        }
        else
        {
            newVelocity.y = 0;
        }

        rb.velocity = newVelocity;

        if (_isGrounded)
        {
            if (Mathf.Abs(rb.velocity.x) < 1)
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
            if (rb.velocity.y > 0)
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