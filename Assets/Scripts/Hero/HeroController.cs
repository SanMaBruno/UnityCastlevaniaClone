using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroController : MonoBehaviour, ITargetCombat

{
    [Header("Power Up Cooldown")]
    [SerializeField] private float powerUpCooldown = 5f;
    private bool canUsePowerUp = true;



    [Header("Power Up ")]
    [SerializeField] private PowerUpId currentPowerUp;
    [SerializeField] private int powerUpAmount;
    [SerializeField] SpellLauncherController bluePotionLauncher;
    [SerializeField] SpellLauncherController redPotionLauncher;


    [Header("Health Variables")]
    [SerializeField] int health = 10;
    [SerializeField] DamageFeedbackEffect damageFeedbackEffect;

    [Header("Attack Variables")]

    [SerializeField] SwordController swordController;


    [Header("Animation Variables")]
    [SerializeField] AnimatorController animatorController;

    [Header("Checker Variables")]
    [SerializeField] LayerChecker feet;

    [Header("Boolean Variables")]
    public bool playerIsAttacking;
    public bool playerIsUsingPowerUp;
    public bool playerIsRecovering;
    public bool canDoubleJump;
    public bool isLookingRight;

    [Header("Interruption Variables")]
    public bool canCheckGround;
    public bool canMove;

    public bool canFlip;



    [Header("Rigid Variables")]
    [SerializeField] private float damageForce;
    [SerializeField] private float damageForceUp;

    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleForce;

    [SerializeField] private float speed;

    [Header("Audio ")]
    [SerializeField] AudioClip attackSfx;


    //Control Variables
    [SerializeField] private Vector2 movementDirection;
    private bool jumPressed = false;
    private bool attackPressed = false;
    private bool usePowerUpPressed = false;


    private bool playerIsOnGround;

    private Rigidbody2D rigidbody2D;

    public static HeroController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

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
        HandleUsePowerUp();
    }

    public void ChangePowerUp(PowerUpId powerUpId, int amount)
    {
        currentPowerUp = powerUpId;
        powerUpAmount = amount;
        Debug.Log(currentPowerUp);
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
        usePowerUpPressed = Input.GetButtonDown("UsePowerUp");
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
        //if (!canFlip) return;
        if (rigidbody2D.velocity.magnitude > 0)
        {
            if (rigidbody2D.velocity.x >= 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                isLookingRight = true;
            }
            else
            {
                this.transform.rotation = Quaternion.Euler(0, 180, 0);
                isLookingRight = false; 
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
            AudioManager.Instance.PlaySfx(attackSfx);
            animatorController.Play(AnimationId.Attack);
            playerIsAttacking = true;
            swordController.Attack(0.1f,0.3f);
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
    void HandleUsePowerUp()
    {
        if (usePowerUpPressed && !playerIsUsingPowerUp && currentPowerUp != PowerUpId.Nothing && canUsePowerUp)
        {
            AudioManager.Instance.PlaySfx(attackSfx);
            animatorController.Play(AnimationId.UsePowerUp);
            playerIsUsingPowerUp = true;

            if (currentPowerUp == PowerUpId.BluePotion)
            {
                bluePotionLauncher.Launch((Vector2)transform.right + Vector2.up * 0.3f);
            }
            if (currentPowerUp == PowerUpId.RedPotion)
            {
                Vector2 direction = isLookingRight ? Vector2.right : Vector2.left;
                redPotionLauncher.Launch(direction);
            }

            StartCoroutine(RestoreUsePowerUp());

            powerUpAmount--;
            if (powerUpAmount <= 0)
            {
                currentPowerUp = PowerUpId.Nothing;
            }

            canUsePowerUp = false;
            StartCoroutine(PowerUpCooldown());
        }
    }


    IEnumerator PowerUpCooldown()
    {
        yield return new WaitForSeconds(powerUpCooldown);
        canUsePowerUp = true;
    }



    IEnumerator RestoreUsePowerUp()
    {
        if (playerIsOnGround)
            canMove = false;
        canMove = false;
        yield return new WaitForSeconds(0.30f);
        playerIsUsingPowerUp = false;
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

    public void TakeDamage(int damagePoints)
    {
        if (!playerIsRecovering)
        {
            health = Mathf.Clamp(health - damagePoints, 0, 10);
            StartCoroutine(StartPlayerRecover());
            if (isLookingRight)
            {
                rigidbody2D.AddForce(Vector2.left * damageForce + Vector2.up * damageForceUp, ForceMode2D.Impulse);
            }
            else
            {
                rigidbody2D.AddForce(Vector2.right * damageForce + Vector2.up * damageForceUp, ForceMode2D.Impulse);

            }
        }
    }
    IEnumerator StartPlayerRecover()
    {
        canMove = false;
        canFlip = false;
        animatorController.Play(AnimationId.Hurt);
        yield return new WaitForSeconds(0.2f);
        canMove = true;
        canFlip = true;
        rigidbody2D.velocity = Vector2.zero;
        playerIsRecovering = true;
        damageFeedbackEffect.PlayBlinkDamageEffect();
        yield return new WaitForSeconds(2);
        damageFeedbackEffect.StopPlayBlinkDamageEffect();
        playerIsRecovering = false;

    }
}
    