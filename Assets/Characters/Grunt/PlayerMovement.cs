using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.XR;

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

    Vector3 knowckbackVelocity;

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

    // Update is called once per frame
    void Update()
    {
        Input.GetKeyDown(KeyCode.W);

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
            moveDirection += knowckbackVelocity;
            controller.Move(moveDirection * runSpeed * Time.deltaTime);

            if(knowckbackVelocity.x > 0.0f)
                knowckbackVelocity.x -= Time.deltaTime/2.0f;
            else
                knowckbackVelocity.x += Time.deltaTime / 2.0f;

            if (knowckbackVelocity.z > 0.0f)
                knowckbackVelocity.z -= Time.deltaTime / 2.0f;
            else
                knowckbackVelocity.z += Time.deltaTime / 2.0f;

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

    private void OnCollisionEnter(Collision collision)
    {
        if(!knockBack)
        {
            knockBack = true;
            knowckbackVelocity = (transform.position - collision.gameObject.transform.position).normalized;
            knowckbackVelocity.y = 0.0f;
            playerHealth.TakeDamage(1.25f);

            if (dead)
                knowckbackVelocity = Vector3.zero;
        }
    }
}
