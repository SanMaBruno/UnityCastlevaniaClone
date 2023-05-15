using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroController : MonoBehaviour

{
    [Header("Attack Variables")]

    [SerializeField] SwordController swordController;


    [Header("Animation Variables")]
    [SerializeField] AnimatorController animatorController;

    [Header("Checker Variables")]
    [SerializeField] LayerChecker feet;

    [Header("Boolean Variables")]
    public bool playerIsAttacking;

    public bool canDoubleJump;

    [Header("Interruption Variables")]
    public bool canCheckGround;
    public bool canMove;



    [Header("Rigid Variables")]

    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleForce;

    [SerializeField] private float speed;

    //Control Variables
    [SerializeField] private Vector2 movementDirection;
    private bool jumPressed = false;
    private bool attackPressed = false;


    private bool playerIsOnGround;

    private Rigidbody2D rigidbody2D;

    void Start()
    {
        canCheckGround = true;
        canMove = true;
        rigidbody2D = GetComponent<Rigidbody2D>();
        animatorController.Play(AnimationId.Idle);
    }

    void Update()
    {
        HandleIsGrounding();
        HandleControls();
        HandleMovement();
        HandleFlip();
        HandleJump();
        HandleAttack();
    }

    void HandleIsGrounding()
    {
        if (!canCheckGround) return;
        playerIsOnGround = feet.isTouching;
    }


    void HandleControls()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        jumPressed = Input.GetButtonDown("Jump");
        attackPressed = Input.GetButtonDown("Attack");
    }

    void HandleMovement()
    {
        if (!canMove) return;

        rigidbody2D.velocity = new Vector2(movementDirection.x * speed, rigidbody2D.velocity.y);

        if (playerIsOnGround)
        {
            if (Mathf.Abs(rigidbody2D.velocity.x) > 0)
            {
                animatorController.Play(AnimationId.Run);
            }
            else
            {
                animatorController.Play(AnimationId.Idle);

            }
        }
    }

    void HandleFlip()
    {
        if (rigidbody2D.velocity.magnitude > 0)
        {
            if (rigidbody2D.velocity.x >= 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                this.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void HandleJump()
    {
        if (canDoubleJump && jumPressed && !playerIsOnGround)
        {
            this.rigidbody2D.velocity = Vector2.zero;
            this.rigidbody2D.AddForce(Vector2.up * doubleForce, ForceMode2D.Impulse);
            canDoubleJump = false;
        }
        if (jumPressed && playerIsOnGround)
        {
            this.rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(HandleJumpAnimation());
            canDoubleJump = true;
        }
    }

    void HandleAttack()
    {
        if (attackPressed && !playerIsAttacking)
        {
            animatorController.Play(AnimationId.Attack);
            playerIsAttacking = true;
            swordController.Attack(0.4f);
            StartCoroutine(RestoreAttack());
        }
    }

    IEnumerator RestoreAttack()
    {
        if (playerIsOnGround)
            canMove = false;
        canMove = false;
        yield return new WaitForSeconds(0.30f);
        playerIsAttacking = false;
        if (!playerIsOnGround)
            animatorController.Play(AnimationId.Jump);
        canMove = true;
    }



    IEnumerator HandleJumpAnimation()
    {
        canCheckGround = false;
        playerIsOnGround = false;
        if(!playerIsAttacking)
            animatorController.Play(AnimationId.PrepareJump);
        yield return new WaitForSeconds(0.10f);
        if (!playerIsAttacking)
            animatorController.Play(AnimationId.Jump);
        canCheckGround = true;
    }
}
    