using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeavyMook : MonoBehaviour
{
    [SerializeField] GameObject attackInstance;
    [SerializeField] Transform attackSpawnPoint;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] AudioClip[] damageSounds;

    GameObject player;
    Animator animator;
    GameManager gm;

    NavMeshAgent agent;
    bool aggro = false;
    float aggroDistance = 12.0f;
    float waitTimer;
    bool dead = false;

    float attackTimer;
    float attackDistance = 9.0f;
    bool damaged = false;
    float health = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTimer = 10.0f;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        player = GameObject.Find("Player");
    }

    bool attacking = false;
    bool punching = false;
    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = new Vector3();
        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //stay idle
        if (!aggro && !dead && !damaged)
        {
            //check if player is close enough for agro
            if (distanceFromPlayer < aggroDistance && !aggro)
            {
                aggro = true;
            }

            animator.SetBool("walk", false);
        }
        //chase player
        else if (!damaged && aggro && !dead)
        {
            agent.speed = runSpeed;
            Vector3 playerPosition = player.transform.position;
            agent.destination = playerPosition;
            lookDirection = (player.transform.position - transform.position).normalized;
            transform.forward = lookDirection;

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            //attack distance
            attackTimer += Time.deltaTime;
            if (distanceFromPlayer < attackDistance)
            {
                agent.isStopped = true;

                //melee attack
                if (distanceFromPlayer < agent.stoppingDistance && !punching)
                {
                    animator.SetTrigger("melee");
                    punching = true;
                }
                else if (attackTimer >= 7.0f && !attacking)
                {
                    animator.SetTrigger("attack");
                    attacking = true;
                    attackTimer = 0.0f;
                }
                else
                {
                    animator.SetBool("walk", false);
                }

            }
            else if (!attacking && !dead) // get into attack distance
            {
                agent.isStopped = false;
                animator.SetBool("walk", true);
            }

        }
    }

    public void UnstopAgent()
    {
        Debug.Log("un stop agent");
        agent.isStopped = false;
        attacking = false;
        punching = false;
    }

    public void DealDamage()
    {
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
            playerMovement.Knockback(position2D);
        }
    }

    int prevDamageIndex = 0;
    public void Damage(float damageAmount)
    {
        if (!dead && aggro && !attacking && !punching)
        {
            health -= damageAmount;

            int damageIndex = Random.Range(0, damageSounds.Length - 1);

            while (damageIndex == prevDamageIndex)
            {
                damageIndex = Random.Range(0, damageSounds.Length - 1);
            }

            SoundManager.instance.PlaySound(damageSounds[damageIndex], transform, 1.0f, 0.0f);

            if (health <= 0.0f)
            {
                dead = true;
                animator.SetBool("dead", true);
                gm.AddKill();
                Destroy(gameObject, 10.0f);
            }
        }
    }

    public void EndDamage()
    {
        if (!dead)
        {
            damaged = false;
            agent.isStopped = false;
        }
    }

    public void Attack()
    {
        Debug.Log("Attack()");
        GameObject attackObj = Instantiate(attackInstance, attackSpawnPoint.position, Quaternion.identity);
        Destroy(attackObj, 5.0f);
        Rigidbody rb = attackObj.GetComponent<Rigidbody>();
        Vector3 shootDirection = player.transform.position - transform.position;
        rb.AddForce(shootDirection * Time.deltaTime * 5000.0f);
        rb.angularVelocity = new Vector3(20.0f, 3.0f, 11.0f);
    }
}
