using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CharacterController controller;
    [SerializeField] Camera cam;
    [SerializeField] float runSpeed;
    [SerializeField] float turnSpeed;

    bool freeze = false;
    bool hit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Input.GetKeyDown(KeyCode.W);

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical);

        if (moveDirection != Vector3.zero && CanMove())
        {
            moveDirection = Quaternion.AngleAxis(cam.transform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0.0f, moveDirection.z));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * 100.0f * Time.deltaTime);
            animator.SetBool("run", true);
        }
        else
        {
            animator.SetBool("run", false);
        }

        moveDirection.y = -9.8f;

        moveDirection.Normalize();
        Debug.Log(moveDirection);

        if (CanMove())
            controller.Move(moveDirection * runSpeed * Time.deltaTime);
    }

    bool CanMove()
    {
        return !freeze && !hit;
    }
}
