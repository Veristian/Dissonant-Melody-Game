using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    [Header("References")]
    public PlayerMovementStats moveStats;
    private Collider2D triggerColl;
    [SerializeField] private Collider2D bodyColl;
    [SerializeField] private Collider2D feetColl;

    [Header("Settings")]
    [SerializeField] private bool reverseRun = true;
    //rb
    private Rigidbody2D rb;

    //movement
    
    private Vector2 moveVelocity;
    private bool isFacingRight;
    private bool WasRunning;
    private bool isWalking;
    private bool isIdle;

    public float MaxFallSpeed {private get; set;}
    //collchecks
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isGrounded;
    private bool isBumpedHead;
    private RaycastHit2D wallHitLeft;
    private RaycastHit2D wallHitRight;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;

    //matrix
    [SerializeField] private float matrixTime;
    private float matrixTimer;
    [SerializeField] private AnimationCurve matrixSlowDownGraph;

    public bool usingMatrix {private get; set;}

    //jump
    public float VerticalVelocity {get; private set;}
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;
    
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    private float jumpBufferTime;
    private bool jumpReleaseDuringBuffer;

    private float coyoteTimer;
    
    //Ability
    public bool usingAbility {private get; set;}
    private Vector2 direction;

    //Dash
    public bool isDashing{private get; set;}
    public float dashTime {private get; set;}
    private Vector2 rbVelocity;
    //Hopping(super jump)
    public bool isHopping {private get; set;}
    public bool hasHopped {private get; set;}
    [Header("Teleport")]
    //teleport
    [SerializeField] private LayerMask teleportAllowLayer;
    public bool isTeleporting {private get; set;}
    public bool WasTeleporting {private get; set;}
    private Collider2D[] results;
    ContactFilter2D filter;

    private float teleportDistance;
    //shoot
    public bool isShooting {private get; set;}
    public GameObject shootProjectile;
    public float shootVelocity = 4;
    
    
    //environmental effects
    private float windForce;
    private Vector2 windDirection;
    public bool OnDropablePlatform {private get; set;}
    public JumpablePlatform jumpablePlatform {private get; set;}
    //Animation
    private PlayerAnimation playerAnimation;
    private bool inTransitionAnimation;
    [Header("Prefab")]
    [SerializeField] private GameObject teleportOutline;
    private GameObject teleportObject; 
    [SerializeField] private GameObject teleportX;
    private GameObject XObject; 

    //player death
    [Header("Death")]
    [SerializeField] private float deathDistanceMultiplier;
    [SerializeField] private AnimationCurve deathCurve;
    public float deathTimer {private get; set;}
    public bool Death {get; set;}
    private bool DeathPop;
    public Vector3 KnockDirection {private get; set;}

    #endregion




    #region Update
    private void Awake()
    {
        results = new Collider2D[1];
        filter = new ContactFilter2D();
        filter.SetLayerMask(teleportAllowLayer);


        playerAnimation = gameObject.GetComponentInChildren<PlayerAnimation>();

        NoteManager.Instance.player = this;
       
       
        isFacingRight = true;

        rb = GetComponent<Rigidbody2D>();
        triggerColl = GetComponent<BoxCollider2D>();

        MaxFallSpeed = moveStats.MaxFallSpeed;

        LevelManager.Instance.OnPlayerSpawn.AddListener(() => StopMovement());
        LevelManager.Instance.OnDeathEvent.AddListener(() => DisableCollider());
        LevelManager.Instance.OnPlayerSpawn.AddListener(() => EnableCollider());
        LevelManager.Instance.OnDeathEvent.AddListener(() => EnableDeathPop());
        LevelManager.Instance.OnPlayerSpawn.AddListener(() => DisableDeathPop());


        
    }

    private void Update()
    {
        CollisionChecks();
        AnimatonUpdate();
        CountTimers();
        JumpChecks();

        if (isTouchingLeftWall || isTouchingRightWall)
        {
            DashCancel();
        }
        
    }

    private void FixedUpdate()
    {
        if (Death)
        {
            
            InputManager.DisableInputs();
            rb.velocity = new Vector2(0,0);
            VerticalVelocity = 0;
            moveVelocity = new Vector2(0,0);
            if (DeathPop)
            {
                deathTimer += Time.fixedDeltaTime;
                rb.transform.position += new Vector3(deathCurve.Evaluate(deathTimer)*deathDistanceMultiplier*KnockDirection.x,deathCurve.Evaluate(deathTimer)*deathDistanceMultiplier*KnockDirection.y);
                 
            }

            return;
        }
        deathTimer = 0;

        if (!usingAbility)
        {
            
            //if not dashing
            Jump();
            WindForce();
            if(isGrounded)
            {
                Move(moveStats.GroundAcceleration, moveStats.GroundDeceleration, InputManager.Movement);
            }else
            {
                Move(moveStats.AirAcceleration, moveStats.AirDeceleration, InputManager.Movement);

            }
            if (usingMatrix)
            {
                matrixTimer += Time.deltaTime;
                if (matrixTimer < matrixTime)
                {
                    rb.velocity = new Vector2(rb.velocity.x*matrixSlowDownGraph.Evaluate(matrixTimer),rb.velocity.y*matrixSlowDownGraph.Evaluate(matrixTimer));
                }
                else
                {
                    usingMatrix = false;
                }
            
            }
            else
            {
                matrixTimer = 0;
            } 
        }
        else if (usingAbility && isDashing)
        {
            //if dashing
            Dash();
        }
        else if (usingAbility && isHopping)
        {
            //if hopping
            Hop();
        }
       
        else if (usingAbility && isShooting)
        {
            Shoot();
            usingAbility = false;
            isShooting = false;
        }
        else if (usingAbility && (isTeleporting || WasTeleporting))
        {
            if (!inTransitionAnimation)
            {
                if (teleportOutline != null && teleportX != null)
                {
                    teleportObject = Instantiate(teleportOutline, transform);
                    XObject = Instantiate(teleportX, transform);
                }
                playerAnimation.TeleportStart();
                inTransitionAnimation = true;
            }
            if (teleportObject != null && XObject != null)
            {
                SetDirection();
                //show the outline for tp dest
                teleportObject.transform.position = new Vector3(transform.position.x+teleportDistance*direction.x,transform.position.y+ teleportDistance*direction.y + 0.2f);
                XObject.transform.position = new Vector3(transform.position.x+teleportDistance*direction.x,transform.position.y+ teleportDistance*direction.y + 0.2f);
                results = new Collider2D[1];
                teleportObject.GetComponent<BoxCollider2D>().OverlapCollider(filter, results);
                if (results[0] == null)
                {
                    XObject.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    XObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            rb.velocity = new Vector2(0,0);
            teleportDistance += Time.deltaTime * moveStats.TeleportToTimeRatio;
            //finish charging teleport
            if (!isTeleporting)
            {
                inTransitionAnimation = false;
                InitiateTeleport();
                Destroy(teleportObject);
                Destroy(XObject);
            }
        
        
        }
    }
    #endregion

    #region General
    private void EnableCollider()
    {
        triggerColl.enabled = true;
        bodyColl.enabled = true;
        feetColl.enabled = true;
    }
    private void DisableCollider()
    {
        triggerColl.enabled = false;
        bodyColl.enabled = false;
        feetColl.enabled = false;

    }

    private void EnableDeathPop()
    {
        DeathPop = true;
    }
    private void DisableDeathPop()
    {
        DeathPop = false;
    }
    #endregion
    #region Animation
    private void AnimatonUpdate()
    {
        
        playerAnimation.Fall = (isFalling || isFastFalling) && !usingAbility && !inTransitionAnimation && !Death && !isGrounded;
        if (rb.velocity.y > 0.1f)
        {
            playerAnimation.Jump = true && !usingAbility && !inTransitionAnimation && !Death && !isGrounded;
            playerAnimation.Fall = false;
        }
        else
        {
            playerAnimation.Jump = false;
        }
        playerAnimation.Run = (WasRunning && isGrounded && !usingAbility && !Death);
        playerAnimation.Walk = (isWalking && isGrounded && !usingAbility && !Death);
        

        playerAnimation.Idle = (isIdle && isGrounded && !usingAbility) && !inTransitionAnimation && !Death;
        playerAnimation.Dash = isDashing && !Death;

        playerAnimation.teleport_hold = isTeleporting && !Death;

    }
    #endregion
    #region Movement

    private void StopMovement()
    {
        rb.velocity = new Vector2(0,0);
        VerticalVelocity = 0;
        moveVelocity = new Vector2(0,0);
    }
   
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        //moveVelocity = rb.velocity;
        if (moveInput.x != 0)
        {
            TurnCheck(moveInput);
            Vector2 targetVelocity = Vector2.zero;
            if((InputManager.RunIsHeld || (WasRunning && !reverseRun)) ^ reverseRun)
            {
                if (moveInput.x > 0.5f)
                {
                    targetVelocity = new Vector2(1,0f) * moveStats.MaxRunSpeed;
                }
                else
                {
                    targetVelocity = new Vector2(moveInput.x,0f) * moveStats.MaxRunSpeed;
                    
                }
                WasRunning = true;
                isWalking = false;
                isIdle = false;
              
            }else
            {
               
                if (moveInput.x > 0.5f)
                {   
                    targetVelocity = new Vector2(1, 0f) * moveStats.MaxWalkSpeed;
                }
                else
                {
                    targetVelocity = new Vector2(moveInput.x, 0f) * moveStats.MaxWalkSpeed;
                    
                }   
                WasRunning = false;
                isWalking = true;
                isIdle = false;

            }

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.deltaTime);

            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
            
        }else if (moveInput.x == 0)
        {
            WasRunning = false;
            isWalking = false;
            isIdle = true;
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.deltaTime);
            rb.velocity = new Vector2(moveVelocity.x,rb.velocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (moveInput.x > 0)
        {
            isFacingRight = true;
            Turn(true);
        }else
        {
            isFacingRight = false;
            Turn(false);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }
    #endregion
    
    #region Special Environment

    public void ApplyWind(float _windForce, Vector2 _windDirection)
    {
        windForce = _windForce;
        windDirection = _windDirection;
    }
    private void WindForce()
    {
        if (isGrounded)
        {
            moveVelocity.x += windForce*windDirection.normalized.x;
        }
        else
        {
            moveVelocity.x += windForce*windDirection.normalized.x;
            VerticalVelocity += windForce*windDirection.normalized.y;
        }
    }
    public void ApplyVelocity(Vector2 velocity, bool overrideCurrentVelocity)
    {
        if (overrideCurrentVelocity)
        {
            moveVelocity.x = velocity.x;
            VerticalVelocity = velocity.y;
        }
        else
        {
            moveVelocity.x += velocity.x;
            VerticalVelocity += velocity.y;
        }
        
    }

    #endregion
    #region Jump
    private void JumpChecks()
    {
        //press Jump
        if (InputManager.JumpWasPressed)
        {
            if (InputManager.Movement.y < 0 && OnDropablePlatform)
            {
                jumpablePlatform.LetPlayerDrop();
                return;
            }
            jumpBufferTime = moveStats.JumpBufferTime;
            jumpReleaseDuringBuffer = false;
        }

        //release Jump
        if (InputManager.JumpWasReleased)
        {
            if (jumpBufferTime > 0f)
            {
                jumpReleaseDuringBuffer = true;
            }

            if (isJumping && VerticalVelocity > 0)
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = moveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;

                }
                else 
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        //jump buffer with coyote time
        if (jumpBufferTime > 0f && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (jumpReleaseDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        //double jump
        else if (jumpBufferTime > 0f && isJumping && numberOfJumpsUsed < moveStats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);

        }

        //air jump after coyote time
        else if (jumpBufferTime > 0f && isFalling && numberOfJumpsUsed < moveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }
        //landing
        if ((isJumping || isFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int jumpsUsed)
    {
        if (!isJumping)
        {
            isJumping = true;
        }
        isFalling = false;
        jumpBufferTime = 0f;
        numberOfJumpsUsed += jumpsUsed;
        VerticalVelocity = moveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        //gravity
        if (isJumping)
        {
            //head bump check
            if (isBumpedHead)
            {
                isFastFalling = true;

            }

            if (VerticalVelocity >= 0f)
            {
                apexPoint = Mathf.InverseLerp(moveStats.InitialJumpVelocity, 0f, VerticalVelocity);
                if (apexPoint > moveStats.ApexThreshold)
                {
                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < moveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;

                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                //gravity ascending
                else
                {
                    VerticalVelocity += moveStats.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }
            //gravity descending
            else if (!isFastFalling)
            {
                VerticalVelocity += moveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            if (VerticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                }
            }

        }
        //cut jump early
        if (isFastFalling)
        {
           
            if (fastFallTime >= moveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += moveStats.Gravity * moveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;

            }
            else if (fastFallTime < moveStats.TimeForUpwardsCancel)
            {

                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / moveStats.TimeForUpwardsCancel));
            }

            fastFallTime += Time.fixedDeltaTime;

        }
        
        //falling with normal gravity
        if (!isGrounded && (!isJumping || isFalling) )
        {
            if(!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += moveStats.Gravity * Time.fixedDeltaTime;
        }

        //clamp max fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MaxFallSpeed, 50f);

        rb.velocity = new Vector2(rb.velocity.x,VerticalVelocity);
    

    }
    #endregion

    #region Ability
    private void SetDirection()
    {
        //right
        if (InputManager.Movement.x > 0.5 && InputManager.Movement.y < 0.5 && InputManager.Movement.y > -0.5)
        {
            direction = Vector2.right;
        }
        //left
        else if (InputManager.Movement.x < -0.5 && InputManager.Movement.y < 0.5 && InputManager.Movement.y > -0.5)
        {
            direction =  Vector2.left;
        }
        //up
        else if (InputManager.Movement.x < 0.5 && InputManager.Movement.y > 0.5 && InputManager.Movement.x > -0.5)
        {
            direction =  Vector2.up;
        }
        //down
        else if (InputManager.Movement.x < 0.5 && InputManager.Movement.y < -0.5 && InputManager.Movement.x > -0.5)
        {
            direction =  Vector2.down;

        }
        //upright
        else if (InputManager.Movement.x > 0.5 && InputManager.Movement.y > 0.5)
        {
            direction = (Vector2.up + Vector2.right).normalized;

        }
        //upleft
        else if (InputManager.Movement.x < -0.5 && InputManager.Movement.y > 0.5)
        {
            direction = (Vector2.up + Vector2.left).normalized;

        }
        //downright
        else if (InputManager.Movement.x > 0.5 && InputManager.Movement.y < -0.5)
        {
            direction = (Vector2.down + Vector2.right).normalized;

        }
        //downleft
        else if (InputManager.Movement.x < -0.5 && InputManager.Movement.y < -0.5)
        {
            direction = (Vector2.down + Vector2.left).normalized;

        } 
        else if (isFacingRight)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = Vector2.left;
        }


    }
    public void Dash()
    {
        if (dashTime >= moveStats.TimeTillCompleteDash)
        {
            usingMatrix = true;
            VerticalVelocity = -0.01f;
            isDashing = false;
            usingAbility = false;
        } else
        {
            if (dashTime == 0)
            {
                SetDirection();
            }
            dashTime += Time.deltaTime;
            rbVelocity = new Vector2(moveStats.dashCurve.Evaluate(dashTime/moveStats.TimeTillCompleteDash)*moveStats.DashDistance/dashTime,moveStats.dashCurve.Evaluate(dashTime/moveStats.TimeTillCompleteDash)*moveStats.DashDistance/dashTime); 
            rb.velocity = rbVelocity * direction;


        }
        
    }

    private void DashCancel()
    {
        if (isDashing)
        {
            isDashing = false;
            usingAbility = false;

        }
    }

    private void Hop()
    {
        if (!hasHopped)
        {
            VerticalVelocity = moveStats.InitialHopVelocity;
            hasHopped = true;
        }

        apexPoint = Mathf.InverseLerp(moveStats.InitialHopVelocity, 0f, VerticalVelocity);
        if (apexPoint > moveStats.ApexThreshold)
        {
            //head bump check
            if (isBumpedHead)
            {

                isFastFalling = true;
                isHopping = false;
                usingAbility = false;
            }
            //sets boolean when needed
            if (!isPastApexThreshold)
            {
                isPastApexThreshold = true;
                timePastApexThreshold = 0f;
            }

            if (isPastApexThreshold){
                timePastApexThreshold += Time.fixedDeltaTime;
                if (timePastApexThreshold < moveStats.ApexHangTime)
                {
                    VerticalVelocity = 0f;

                }
                else
                {
                    isHopping = false;
                    usingAbility = false;
                }
            }
        }
        //gravity ascending
        else
        {
            VerticalVelocity += moveStats.HopGravity * Time.fixedDeltaTime;
            //sets boolean when needed
            if (isPastApexThreshold)
            {
                isPastApexThreshold = false;
            }
        }
        
        
        rb.velocity = new Vector2(rb.velocity.x,VerticalVelocity);


    }


    private void InitiateTeleport()
    {
        if (results[0] == null)
        {
            SetDirection();
            rb.transform.position += new Vector3(teleportDistance * direction.x,teleportDistance* direction.y);
        }
        WasTeleporting = false;
        usingAbility = false;
        teleportDistance = 0;
        isFastFalling = true;
        usingMatrix = true;
    }

    private void Shoot()
    {
        usingMatrix = true;
        SetDirection();
        if (shootProjectile != null)
        {
            GameObject proj = Instantiate(shootProjectile, gameObject.transform);
            if (proj.GetComponent<Rigidbody2D>() != null)
            {
                proj.transform.parent = null;
                proj.GetComponent<Rigidbody2D>().velocity = direction * shootVelocity;
            }
            
        }

    }
    #endregion


    #region CollisionChecks
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x,feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, moveStats.GroundDetectionRayLength);

        Collider2D overlap = Physics2D.OverlapBox(feetColl.bounds.center, feetColl.bounds.size, 0f, moveStats.GroundLayer);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, moveStats.GroundDetectionRayLength, moveStats.GroundLayer);
        if (overlap != null || groundHit.collider != null)
        {
            isGrounded = true;
        }

        else
        {
            isGrounded = false;
        }

        #region debug visualization
        if (moveStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (isGrounded)
            {
                rayColor = Color.green;
            }

            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - moveStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
                
        #endregion
        }
    }
    private void CheckWallBump()
    {
        float rayLength = moveStats.WallDetectionRayLength;
        Vector2 originLeft = new Vector2(bodyColl.bounds.min.x, bodyColl.bounds.center.y);
        Vector2 originRight = new Vector2(bodyColl.bounds.max.x, bodyColl.bounds.center.y);
        Vector2 boxSize = new Vector2(rayLength, bodyColl.bounds.size.y * 0.8f); // Slim vertical box to avoid ground interference

        wallHitLeft = Physics2D.BoxCast(originLeft, boxSize, 0f, Vector2.left, 0.01f, moveStats.GroundLayer);
        wallHitRight = Physics2D.BoxCast(originRight, boxSize, 0f, Vector2.right, 0.01f, moveStats.GroundLayer);

        isTouchingLeftWall = wallHitLeft.collider != null;
        isTouchingRightWall = wallHitRight.collider != null;

        #region debug visualization
        if (moveStats.DebugShowWallCheck)
        {
            Color leftColor = isTouchingLeftWall ? Color.magenta : Color.cyan;
            Color rightColor = isTouchingRightWall ? Color.magenta : Color.cyan;

            Debug.DrawRay(originLeft + Vector2.up * (boxSize.y / 2), Vector2.left * rayLength, leftColor);
            Debug.DrawRay(originLeft - Vector2.up * (boxSize.y / 2), Vector2.left * rayLength, leftColor);
            Debug.DrawRay(originLeft - Vector2.up * (boxSize.y / 2) + Vector2.left * rayLength, Vector2.up * bodyColl.bounds.size.y * 0.8f, leftColor);

            Debug.DrawRay(originRight + Vector2.up * (boxSize.y / 2), Vector2.right * rayLength, rightColor);
            Debug.DrawRay(originRight - Vector2.up * (boxSize.y / 2), Vector2.right * rayLength, rightColor);
            Debug.DrawRay(originRight - Vector2.up * (boxSize.y / 2) + Vector2.right * rayLength, Vector2.up * bodyColl.bounds.size.y * 0.8f, rightColor);

        }
        #endregion
    }


    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x,bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x * moveStats.Width, moveStats.HeadDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, moveStats.HeadDetectionRayLength, moveStats.HeadLayer);
        if (headHit.collider != null)
        {
            isBumpedHead = true;
        }

        else
        {
            isBumpedHead = false;
        }

        #region debug visualization
        if (moveStats.DebugShowHeadBumpBox)
        {
            float headWidth = moveStats.Width;
            Color rayColor;
            if (isBumpedHead)
            {
                rayColor = Color.green;
            }

            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * moveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * moveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + moveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
                
        #endregion
        }
    }
    private void CollisionChecks()
    {
        CheckWallBump();
        IsGrounded();
        BumpedHead();
    }


    #endregion

    #region Timers
    private void CountTimers()
    {
        jumpBufferTime -= Time.deltaTime;

        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        } 
        else 
        { 
            coyoteTimer = moveStats.JumpCoyoteTime;
        }
    }

    #endregion

}
