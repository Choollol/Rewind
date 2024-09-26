using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : SpriteMovement
{
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
