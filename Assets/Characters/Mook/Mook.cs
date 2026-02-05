using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mook : MonoBehaviour
{
    [SerializeField] GameObject attackInstance;
    [SerializeField] Transform attackSpawnPoint;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] AudioClip[] footstepSoundClips;
    [SerializeField] AudioClip[] attackSounds;
    [SerializeField] AudioClip meleeSound;

    GameObject player;
    Animator animator;
    List<Transform> patrolPoints;
    GameManager gm;

    NavMeshAgent agent;
    bool aggro = false;
    float aggroDistance = 10.0f;
    int currentPatrolPoint = 0;
    float waitTimer;
    bool dead = false;

    float attackTimer = 4.0f;
    float attackDistance = 6.0f;
    bool damaged = false;
    float health = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patrolPoints = new List<Transform>();
        attackTimer = 10.0f;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject patrolPointParent = GameObject.Find("PatrolPoints");

        //find patrol points in level
        foreach (Transform child in patrolPointParent.transform)
        {
            patrolPoints.Add(child);
        }

        agent.destination = patrolPoints[currentPatrolPoint % patrolPoints.Count].position;
        player = GameObject.Find("Player");
    }

    bool attacking = false;
    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = new Vector3();
        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //patrol points
        if(!aggro && !dead && !damaged)
        {
            agent.speed = walkSpeed;
            //walk to next patrol point
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                lookDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
                lookDirection.y = 0.0f;
                transform.forward = lookDirection;
                agent.isStopped = false;
                animator.SetBool("walk", true);

                //check if player is close enough for agro
                if (distanceFromPlayer < aggroDistance && !aggro)
                {
                    aggro = true;
                }
            }
            //wait then set next control point
            else
            {
                animator.SetBool("walk", false);
                agent.isStopped = true;
                waitTimer += Time.deltaTime;
                if (waitTimer > 4.0f)
                {
                    currentPatrolPoint = ++currentPatrolPoint % patrolPoints.Count;
                    Debug.Log(currentPatrolPoint);
                    agent.destination = patrolPoints[currentPatrolPoint].position;
                    waitTimer = 0.0f;
                }
            }
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
            if(distanceFromPlayer < attackDistance)
            {
                //throw attack
                if(playerHealth.Shielded())
                {
                    attackTimer += Time.deltaTime;

                    if (attackTimer >= 3.0f)
                    {
                        agent.isStopped = true;
                        animator.SetTrigger("attack");
                        attacking = true;
                        attackTimer = 0.0f;
                    }
                }//melee attack
                else if (distanceFromPlayer < agent.stoppingDistance && !attacking)
                {
                    animator.SetTrigger("melee");
                    attacking = true;
                    agent.isStopped = true;
                    animator.SetBool("run", false);
                }
                else if(!attacking)
                {
                    agent.isStopped = false;
                    animator.SetBool("run", true);

                }
            }
            else if(!attacking && !dead) // get into attack distance
            {
                agent.isStopped = false;
                animator.SetBool("run", true);
            }
            
        }
    }

    public void UnstopAgent()
    {
        Debug.Log("un stop agent");
        agent.isStopped = false;
        attacking = false;
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

    public void Damage(float damageAmount)
    {
        if(!dead)
        {
            agent.isStopped = true;
            damaged = true;
            health -= damageAmount;
            aggro = true;
            attacking = false;

            if(health <= 0.0f)
            {
                dead = true;
                animator.SetBool("dead", true);
                gm.AddKill();
                Destroy(gameObject, 10.0f);
            }
            else
            {
                animator.SetTrigger("damage");
            }
        }
    }

    public void EndDamage()
    {
        if(!dead)
        {
            damaged = false;
            agent.isStopped = false;
        }
    }

    int prevIndexAttack = 0;
    public void Attack()
    {
        int index = Random.Range(0, attackSounds.Length - 1);

        while (index == prevIndexAttack)
        {
            index = Random.Range(0, attackSounds.Length - 1);
        }

        SoundManager.instance.PlaySound(attackSounds[index], transform);
        prevIndexAttack = index;


        GameObject attackObj = Instantiate(attackInstance, attackSpawnPoint.position, Quaternion.identity);
        Destroy(attackObj, 5.0f);
        Rigidbody rb = attackObj.GetComponent<Rigidbody>();
        Vector3 shootDirection = player.transform.position - transform.position;
        rb.AddForce(shootDirection * Time.deltaTime * 5000.0f);
        rb.angularVelocity = new Vector3(20.0f, 3.0f, 11.0f);
    }

    int prevIndex = 0;
    public void Footstep()
    {
        int index = Random.Range(0, footstepSoundClips.Length - 1);

        while (index == prevIndex)
        {
            index = Random.Range(0, footstepSoundClips.Length - 1);
        }

        SoundManager.instance.PlaySound(footstepSoundClips[index], transform);
        prevIndex = index;
    }
}
