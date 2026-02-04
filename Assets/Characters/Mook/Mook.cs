using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mook : MonoBehaviour
{
    [SerializeField] GameObject attackInstance;
    [SerializeField] Transform attackSpawnPoint;

    GameObject player;
    Animator animator;
    List<Transform> patrolPoints;

    NavMeshAgent agent;
    bool aggro = false;
    float aggroDistance = 6.0f;
    int currentPatrolPoint = 0;
    float waitTimer;
    bool dead = false;

    float attackTimer;
    float attackDistance = 10.0f;
    bool damaged = false;
    float health = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patrolPoints = new List<Transform>();
        attackTimer = 10.0f;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GameObject patrolPointParent = GameObject.Find("PatrolPoints");

        //find patrol points in level
        foreach (Transform child in patrolPointParent.transform)
        {
            patrolPoints.Add(child);
        }

        agent.destination = patrolPoints[currentPatrolPoint % patrolPoints.Count].position;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = new Vector3();
        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        agent.isStopped = false;

        //patrol points
        if(!aggro && !dead && !damaged)
        {
            //walk to next patrol point
            if(agent.remainingDistance > 0.5f)
            {
                lookDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
                lookDirection.y = 0.0f;
                transform.forward = lookDirection;
                animator.SetBool("run", true);

                //check if player is close enough for agro
                if (distanceFromPlayer < aggroDistance && !aggro)
                {
                    aggro = true;
                }
            }
            //wait then set next control point
            else
            {
                animator.SetBool("run", false);
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
        else if (!damaged)
        {
            Vector3 playerPosition = player.transform.position;
            agent.destination = playerPosition;
            lookDirection = (player.transform.position - transform.position).normalized;
            transform.forward = lookDirection;

            if(distanceFromPlayer < attackDistance)
            {
                attackTimer += Time.deltaTime;

                if(attackTimer >= 3.0f)
                {
                    animator.SetTrigger("attack");
                    attackTimer = 0.0f;
                }

                agent.isStopped = true;
                animator.SetBool("run", false);
            }
            else
                animator.SetBool("run", true);
        }
    }

    public void Damage(float damageAmount)
    {
        damaged = true;
        health -= damageAmount;
        aggro = true;

        if(health <= 0.0f)
        {
            agent.isStopped = true;
            dead = true;
            animator.SetBool("dead", true);
            Destroy(gameObject, 10.0f);
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
        rb.AddForce(shootDirection * Time.deltaTime * 5000.0f);
        rb.angularVelocity = new Vector3(20.0f, 3.0f, 11.0f);
    }
}
