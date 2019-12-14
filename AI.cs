using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public enum State
    {

        PATROL,
        CHASE

    }

    private Animator animator;
    private NavMeshAgent agent;
    public Transform[] waypoints;
    public Transform target;
    public float FOV = 120 / 2f;
    public State state = State.PATROL;
    public int health = 100;
    private float distance;
    private PlayerMotor motor;
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);

        if (state == State.CHASE)
        {
            Chase();
        }
        else
        {
            Patrol();
            Search();
        }

        if (distance >= 6f)
        {
            state = State.PATROL;
            animator.SetBool("Wander",true);
            agent.speed = 1f;
        }
    }

    public void Attack()
    {
        agent.speed = 2f;

        if (distance <= 1f)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
            animator.SetBool("Attack", true);
            health -= 1;
        }
        else
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Wander", false);
        }

    }

    public void Chase(){
                                                
        agent.speed = 2f;
        agent.SetDestination(target.position);
        animator.SetBool("Wander",false);
        Attack();

    }

    public void Search(){

        if (Vector3.Distance(transform.position,target.position)<=20f)
        {
            if (Vector3.Angle(Vector3.forward,transform.InverseTransformPoint(target.position)) <= 50f)
            {
                RaycastHit hit;

                if (Physics.Linecast(transform.position,target.transform.position,out hit))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        state = State.CHASE;
                        animator.SetBool("Wander",false);
                    }

                }

            }
        }

    }

    public void Patrol(){

        animator.SetBool("Wander", true);
        agent.speed = 1f;

        if (index < waypoints.Length){

            if (Vector3.Distance(transform.position, waypoints[index].position) < 2f){

                index++;

            }
            else{

                agent.SetDestination(waypoints[index].position);

            }
        }
        else{

            index = 0;

        }
    }

}