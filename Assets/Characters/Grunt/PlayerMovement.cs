using UnityEngine;
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

    bool freeze = false;
    bool hit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
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
        else
        {
            animator.SetBool("run", false);
        }

        if(playerAttack.Aiming())
        {
            animator.SetFloat("strafe", horizontal);
            animator.SetFloat("forward", vertical);
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
        return !freeze && !hit;
    }
}
