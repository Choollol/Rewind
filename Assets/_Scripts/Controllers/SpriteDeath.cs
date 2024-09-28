using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDeath : MonoBehaviour
{
    protected bool isAlive = true;
    protected virtual void Death()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death") && isAlive)
        {
            Death();
        }
    }
}
