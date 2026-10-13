using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;


    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float loseRange = 20f;
    [SerializeField] private float attackRange = 1f;


    public bool IsDead { get; private set; }


    private NavMeshAgent agent;
    private Animator animator;


    PatrolState patrol;


    public float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();


        patrol = new PatrolState(this);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }


    void ChangeState(State nextState)
    {


    }


    abstract class State
    {
        public EnemyController ai;


        public State(EnemyController enemyCon)
        {
            ai = enemyCon;
        }


        public abstract void Enter();
        public abstract void Execute();
    }


    class PatrolState : State
    {
        Transform target;


        public PatrolState(EnemyController enemyCon) : base(enemyCon) { }


        public override void Enter()
        {
            ai.animator.SetBool("Chasing", false);
            ai.agent.isStopped = false;
            ai.agent.speed = ai.patrolSpeed;


            if (target == null)
            {
                target = ai.pointA;
            }


            ai.agent.SetDestination(target.position);
        }


        public override void Execute()
        {
            if (ai.DistanceToPlayer() <= ai.detectionRange)
            {
                //Alteração de estado!
            }
        }
    }
}


