using UnityEngine;

public class Mook : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject player;
    [SerializeField] Animator animator;
    [SerializeField] float stoppingDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject attackInstance;
    [SerializeField] Transform attackSpawnPoint;

    float distanceFromPlayer;
    float attackTimer;
    bool damaged = false;
    float health = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTimer = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log(distanceFromPlayer);
        Vector3 moveDirection = new Vector3();

        if(distanceFromPlayer > stoppingDistance && !damaged)
        {
            moveDirection = player.transform.position - transform.position;
            moveDirection.y = 0.0f;
            moveDirection.Normalize();

            transform.forward = moveDirection;

            animator.SetBool("run", true);
            controller.Move(moveDirection * Time.deltaTime * moveSpeed);
        }
        else
        {
            attackTimer += Time.deltaTime;

            if(attackTimer >= 10.0f)
            {
                animator.SetTrigger("attack");
                attackTimer = 0.0f;
            }

            animator.SetBool("run", false);
        }
    }

    public void Damage(float damageAmount)
    {
        damaged = true;
        health -= damageAmount;

        if(health <= 0.0f)
        {
            animator.SetBool("dead", true);
        }
        else
        {
            animator.SetTrigger("damage");

        }
    }

    public void EndDamage()
    {
        damaged = false;
    }

    public void Attack()
    {
        GameObject attackObj = Instantiate(attackInstance, attackSpawnPoint.position, Quaternion.identity);
        Destroy(attackObj, 5.0f);
        Rigidbody rb = attackObj.GetComponent<Rigidbody>();
        Vector3 shootDirection = player.transform.position - transform.position;
        rb.AddForce(shootDirection * Time.deltaTime * 4000.0f);
        rb.angularVelocity = new Vector3(20.0f, 3.0f, 11.0f);
    }
}
