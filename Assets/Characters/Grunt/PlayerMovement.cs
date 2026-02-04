using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CharacterController controller;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] Camera cam;
    [SerializeField] float runSpeed;
    [SerializeField] float aimRunSpeed;
    [SerializeField] float turnSpeed;

    PlayerHealth playerHealth;

    Vector2 knowckbackVelocity;

    bool freeze = false;
    bool knockBack = false;
    float knockBackTimer;
    bool dead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    float fallTimer;
    bool falling = false;

    public bool Falling()
    {
        return falling;
    }

    // Update is called once per frame
    void Update()
    {
        Input.GetKeyDown(KeyCode.W);

        
        //falling downwards
        if(transform.position.y < 0.0f)
        {
            animator.SetBool("fall", true);
            falling = true;
            fallTimer += Time.deltaTime;

            if (fallTimer > 5.0f)
                SceneManager.LoadScene(0);

            Vector3 fallDirection = new Vector3();
            fallDirection.y = -9.8f;
            fallDirection.Normalize();
            controller.Move(fallDirection * runSpeed * Time.deltaTime);

            return;
        }

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical);


        if(playerAttack.Aiming())
        {
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(cam.transform.forward.x, 0.0f, cam.transform.forward.z));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * 1000.0f * Time.deltaTime);
            animator.SetFloat("moveX", horizontal, 0.15f, Time.deltaTime);
            animator.SetFloat("moveZ", vertical, 0.1f, Time.deltaTime);
        }

        //rotation is handled by camera forward if aiming
        if (moveDirection != Vector3.zero && CanMove())
        {
            moveDirection = Quaternion.AngleAxis(cam.transform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
            
            if(!playerAttack.Aiming())
            {
                Quaternion toRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0.0f, moveDirection.z));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * 100.0f * Time.deltaTime);
            }

            animator.SetBool("run", true);
        }
        else if(knockBack)
        {
            knockBackTimer += Time.deltaTime;
            moveDirection = new Vector3(0, 0, 0);
            moveDirection.x += knowckbackVelocity.x;
            moveDirection.z += knowckbackVelocity.y;
            moveDirection.y = -9.8f;
            moveDirection.Normalize();
            controller.Move(moveDirection * runSpeed * Time.deltaTime);

            if(knowckbackVelocity.x > 0.0f)
                knowckbackVelocity.x -= Time.deltaTime/2.0f;
            else
                knowckbackVelocity.x += Time.deltaTime / 2.0f;

            if (knowckbackVelocity.y > 0.0f)
                knowckbackVelocity.y -= Time.deltaTime / 2.0f;
            else
                knowckbackVelocity.y += Time.deltaTime / 2.0f;

            animator.SetBool("run", false);

            if (knowckbackVelocity.magnitude <= 0.1f)
            {
                knockBackTimer = 0.0f;
                knockBack = freeze = false;
            }
        }
        else
        {
            animator.SetBool("run", false);
        }

        moveDirection.y = -9.8f;
        moveDirection.Normalize();

        if (CanMove())
        {
            if(playerAttack.Aiming())
                controller.Move(moveDirection * aimRunSpeed * Time.deltaTime);
            else
                controller.Move(moveDirection * runSpeed * Time.deltaTime);

        }

    }

    bool CanMove()
    {
        return !freeze && !knockBack && !dead;
    }

    public void Freeze()
    {
        freeze = true;
    }

    public void Unfreeze()
    {
        freeze = false;
    }

    public void Die()
    {
        dead = true;
        animator.SetBool("dead", true);
    }

    public bool isDead()
    {
        return dead;
    }

    public void Knockback(Vector2 collision)
    {
        if (!knockBack)
        {
            knockBack = true;
            Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
            knowckbackVelocity = (position2D - collision).normalized;
            playerHealth.TakeDamage(0.25f);

            if (dead)
                knowckbackVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        playerHealth.TakeDamage(1.25f);
    }
}
