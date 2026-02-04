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
    int currentPatrolPoint = 0;
    float waitTimer;
    bool dead = false;

    float attackTimer;
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
        agent.isStopped = false;

        if(!aggro && !dead && !damaged)
        {
            if(agent.remainingDistance > 0.5f)
            {
                lookDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
                lookDirection.y = 0.0f;
                transform.forward = lookDirection;
                animator.SetBool("run", true);
            }
            else
            {
                animator.SetBool("run", false);
                waitTimer += Time.deltaTime;
                if(waitTimer > 4.0f)
                {
                    currentPatrolPoint = ++currentPatrolPoint % patrolPoints.Count;
                    Debug.Log(currentPatrolPoint);
                    agent.destination = patrolPoints[currentPatrolPoint].position;
                    waitTimer = 0.0f;
                }
            }
        }
        else if (!damaged)
        {
            Vector3 playerPosition = player.transform.position;
            agent.destination = playerPosition;
            lookDirection = (player.transform.position - transform.position).normalized;
            transform.forward = lookDirection;
            animator.SetBool("run", true);
        }
        else
        {
            attackTimer += Time.deltaTime;
            Vector3 playerPosition = player.transform.position;
            agent.destination = playerPosition;
            lookDirection = (player.transform.position - transform.position).normalized;
            transform.forward = lookDirection;

            if(attackTimer >= 10.0f)
            {
                animator.SetTrigger("attack");
                attackTimer = 0.0f;
            }

            agent.isStopped = true;
            animator.SetBool("run", false);
        }

    }

    public void Damage(float damageAmount)
    {
        damaged = true;
        health -= damageAmount;
        aggro = true;

        if(health <= 0.0f)
        {
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
        rb.AddForce(shootDirection * Time.deltaTime * 11000.0f);
        rb.angularVelocity = new Vector3(20.0f, 3.0f, 11.0f);
    }
}
