using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        SceneManager.SetActiveScene(gameObject.scene);

        inputType = InputType.Player;

        boxCollider = GetComponent<BoxCollider2D>();

        ResetDirection();
    }
    public override void Update()
    {
        base.Update();

        if (GameManager.isGameActive)
        {
            horizontalInput = direction == Direction.Right ? 1 : -1;
        }

        doJump = false;
        if (Input.GetButtonDown("Rewind") && GameManager.canRewind && GameManager.isGameActive && 
            GetComponent<PlayerRewinder>().rewinds > 0)
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
            //horizontalInput = 1;
        }
        else if (direction == Direction.Right)
        {
            direction = Direction.Left;
            //horizontalInput = -1;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D collisionCollider = collision.gameObject.GetComponent<Collider2D>();

        if (//collision.gameObject.CompareTag("Wall") && 
            ((direction == Direction.Left && boxCollider.bounds.min.x > collisionCollider.bounds.max.x) || 
            (direction == Direction.Right && boxCollider.bounds.max.x < collisionCollider.bounds.min.x)) && 
            (boxCollider.bounds.min.y < collisionCollider.bounds.max.y - 0.1f))
        {
            ToggleDirection();
        }
    }
}
