using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameObject goalParticle;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioPlayer.PlaySound("Level Complete Sound");
            EventMessenger.TriggerEvent("LevelComplete");
            Instantiate(goalParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
