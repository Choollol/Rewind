using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDeath : MonoBehaviour
{
    private bool isAlive = true;
    private void Update()
    {
        if (transform.position.y < -5)
        {
            Death();
        }
    }
    private void Death()
    {
        AudioPlayer.PlaySound("Death Sound");
        EventMessenger.TriggerEvent("Restart");
        isAlive = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death") && isAlive)
        {
            Death();
        }
    }
}
