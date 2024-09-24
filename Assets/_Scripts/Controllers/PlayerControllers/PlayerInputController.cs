using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : InputController
{
    [SerializeField] private Direction startingDirection;
    private Direction direction;

    private BoxCollider2D boxCollider;
    public override void OnEnable()
    {
        base.OnEnable();

        EventMessenger.StartListening("PlayerRewinded", ResetDirection);
    }
    public override void OnDisable()
    {
        base.OnDisable();

        EventMessenger.StopListening("PlayerRewinded", ResetDirection);
    }
    public override void Start()
    {
        base.Start();

        inputType = InputType.Player;

        boxCollider = GetComponent<BoxCollider2D>();

        ResetDirection();
    }
    public override void Update()
    {
        if (Input.GetButtonDown("Rewind") && GameManager.canRewind && GameManager.isGameActive)
        {
            StartCoroutine(StartRewind());
        }
    }
    private IEnumerator StartRewind()
    {
        GameManager.canRewind = false;
        AudioPlayer.PlaySound("Rewind Sound");
        EventMessenger.TriggerEvent("FreezeTime");
        while (AudioPlayer.IsSoundPlaying("Rewind Sound"))
        {
            yield return null;
        }
        EventMessenger.TriggerEvent("Rewind");
        EventMessenger.TriggerEvent("UnfreezeTime");
        ResetDirection();
        yield return new WaitForSeconds(0.5f);
        GameManager.canRewind = true;
        yield break;
    }
    private void ResetDirection()
    {
        direction = startingDirection;
        if (direction == Direction.Left)
        {
            horizontalInput = -1;
        }
        else if (direction == Direction.Right)
        {
            horizontalInput = 1;
        }
    }
    private void ToggleDirection()
    {
        if (direction == Direction.Left)
        {
            direction = Direction.Right;
            horizontalInput = 1;
        }
        else if (direction == Direction.Right)
        {
            direction = Direction.Left;
            horizontalInput = -1;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D collisionCollider = collision.gameObject.GetComponent<Collider2D>();

        if (collision.gameObject.CompareTag("Wall") && 
            (direction == Direction.Left && transform.position.x > collision.transform.position.x || 
            direction == Direction.Right && transform.position.x < collision.transform.position.x) && 
            (boxCollider.bounds.min.y < collisionCollider.bounds.max.y - 0.1f))
        {
            ToggleDirection();
        }
        else if (collision.gameObject.CompareTag("Death"))
        {
            AudioPlayer.PlaySound("Death Sound");
            EventMessenger.TriggerEvent("Restart");
        }
    }
}
