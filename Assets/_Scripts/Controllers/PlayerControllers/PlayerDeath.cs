using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : SpriteDeath
{
    public void Die()
    {
        Death();
    }
    protected override void Death()
    {
        AudioPlayer.PlaySound("Death Sound");
        EventMessenger.TriggerEvent("Restart");
        isAlive = false;
    }
}
