using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InputController : MonoBehaviour
{
    public enum InputType
    {
        None, Player
    }
    public enum Direction
    {
        Left, Right
    }

    public bool canAct { get; protected set; }
    public float horizontalInput { get; protected set; }
    public bool doJump { get; protected set; }
    public bool isJumpHeld { get; protected set; }
    [HideInInspector] public bool isFalling;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isRising;

    public bool doAttack { get; protected set; }
    public int attackType { get; protected set; } // 0 for melee, 1 for ranged

    [SerializeField] protected InputType inputType;

    public virtual void OnEnable()
    {

    }
    public virtual void OnDisable()
    {

    }
    public virtual void Start()
    {
        canAct = true;
    }
    public virtual void Update()
    {
        horizontalInput = 0;
    }
}
