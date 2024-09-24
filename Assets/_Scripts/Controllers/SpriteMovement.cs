using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(InputController))]
public class SpriteMovement : MonoBehaviour
{
    /*
     * Requires child "Ground Check" object at feet/bottom of object
     * Create layer named "Ground" and add to groundLayers and objects sprite can jump off of
     * Default paramters: 1 mass, 3 gravity, 12 jump force, 10 speed
     */

    protected Rigidbody2D rb;
    protected InputController inputController;

    protected static float inputControllerUpdateVelocityMin = 0.1f;

    protected Transform groundCheck;
    [SerializeField] protected List<LayerMask> groundLayers;

    [SerializeField] protected float jumpForce;

    protected bool isGrounded;

    protected float jumpTimeCounter = 0;

    protected float minJumpTime = 0.08f;
    protected float minJumpTimeCounter = 0;

    protected int extraJumps = 0;
    protected int extraJumpsCounter = 0;

    protected float coyoteTime = 0.06f;
    protected float coyoteTimeCounter = 0;
    protected float jumpBuffer = 0.1f;
    protected float jumpBufferCounter = 0;

    protected Vector2 velocity;

    [SerializeField] protected float speed;

    protected Vector2 additionalForce;
    protected Vector2 additionalForceDecrement;
    public virtual void OnEnable()
    {
        EventMessenger.StartListening("DisableInputControllerAction", InputControllerDisabled);
    }
    public virtual void OnDisable()
    {
        EventMessenger.StopListening("DisableInputControllerAction", InputControllerDisabled);
    }
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputController = GetComponent<InputController>();

        groundCheck = transform.GetChild(0);
    }

    private void FixedUpdate()
    {
        if (GameManager.isGameActive)
        {
            MovementUpdate();
        }
    }
    void Update()
    {
        if (GameManager.isGameActive)
        {
            JumpUpdate();

            if (!isGrounded && Mathf.Abs(rb.velocity.y) > inputControllerUpdateVelocityMin)
            {
                if (rb.velocity.y < 0)
                {
                    inputController.isFalling = true;
                }
                else if (rb.velocity.y > 0)
                {
                    inputController.isRising = true;
                    inputController.isFalling = false;
                }
            }

            additionalForce -= additionalForceDecrement;
        }
    }
    protected virtual void InputControllerDisabled()
    {
        rb.velocity = Vector2.zero;
    }
    private void MovementUpdate()
    {
        velocity = new Vector3(inputController.horizontalInput, 0) * speed;

        rb.velocity = new Vector2(velocity.x, rb.velocity.y) + additionalForce;
    }
    private void JumpUpdate()
    {
        float dt = Time.deltaTime;

        // Coyote time
        if (isGrounded || extraJumpsCounter < extraJumps)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= dt;
        }

        // Jump buffer
        if (inputController.doJump)
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= dt;
        }

        // Jump
        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0 && (!inputController.isJumping || jumpTimeCounter > 0.1f))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (!isGrounded)
            {
                extraJumpsCounter++;
            }
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            inputController.isJumping = true;
            inputController.isFalling = false;
            jumpTimeCounter = 0;
            minJumpTimeCounter = 0;
            //AudioManager.PlaySound("Jump Sound");
        }
        // Variable jump height
        if (!inputController.isJumpHeld && rb.velocity.y > 0 && minJumpTimeCounter > minJumpTime && inputController.isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.96f);

            coyoteTimeCounter = 0;
        }
        if (inputController.isJumping)
        {
            minJumpTimeCounter += dt;
            jumpTimeCounter += dt;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D collisionCollider = collision.gameObject.GetComponent<Collider2D>();

        float comparisonPoint = collisionCollider.bounds.max.y - 0.01f;
        if (collisionCollider is CircleCollider2D)
        {
            comparisonPoint = collisionCollider.bounds.center.y;
        }

        // Grounded
        foreach (LayerMask groundLayer in groundLayers)
        {
            if (1 << collision.gameObject.layer == groundLayer && comparisonPoint < groundCheck.position.y &&
                rb.velocity.y <= 0)
            {
                isGrounded = true;
                extraJumpsCounter = 0;
                inputController.isJumping = false;
                inputController.isFalling = false;
                inputController.isRising = false;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        float collisionTopY = collision.gameObject.GetComponent<Collider2D>().bounds.max.y;

        // Ungrounded
        foreach (LayerMask groundLayer in groundLayers)
        {
            if (1 << collision.gameObject.layer == groundLayer && collisionTopY < groundCheck.position.y + 0.01f)
            {
                isGrounded = false;
            }
        }
    }
}
