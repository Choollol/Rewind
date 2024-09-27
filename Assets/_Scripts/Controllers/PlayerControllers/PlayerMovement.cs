using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : SpriteMovement
{
    [SerializeField] private int additionalForceInitial;

    public override void OnEnable()
    {
        base.OnEnable();

        EventMessenger.StartListening("PlayerDirectionChanged", ResetAdditionalForce);
        EventMessenger.StartListening("PlayerRewinded", ResetAdditionalForce);
    }
    public override void OnDisable()
    {
        base.OnDisable();

        EventMessenger.StopListening("PlayerDirectionChanged", ResetAdditionalForce);
        EventMessenger.StopListening("PlayerRewinded", ResetAdditionalForce);
    }
    private void ResetAdditionalForce()
    {
        additionalForce = Vector2.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Launch"))
        {
            if (rb.velocity.x > 0)
            {
                additionalForce = new Vector2(additionalForceInitial, 0);
            }
            else if (rb.velocity.x < 0)
            {
                additionalForce = new Vector2(-additionalForceInitial, 0);
            }
            rb.velocity = new Vector2(rb.velocity.x, 0);
            AudioPlayer.PlaySound("Launch Sound");
        }
    }
    public override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        if (collision.gameObject.CompareTag("Bounce") && 
            collider2d.bounds.min.y > collision.gameObject.GetComponent<Collider2D>().bounds.max.y && !inputController.isJumping)
        {
            Jump();
        }
    }
}
