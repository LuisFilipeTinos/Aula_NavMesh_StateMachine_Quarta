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
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDuration = 0.8f;


    public bool IsDead { get; private set; }


    private NavMeshAgent agent;
    private Animator animator;

    State current;
    PatrolState patrol;
    ChaseState chase;
    AttackState attack;


    public float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();


        patrol = new PatrolState(this);
        chase = new ChaseState(this);
        attack = new AttackState(this);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeState(patrol);
    }


    // Update is called once per frame
    void Update()
    {
        current.Execute();
    }


    void ChangeState(State nextState)
    {
        current = nextState;
        current.Enter();
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
                ai.ChangeState(ai.chase);
                return;
            }

            if (!ai.agent.pathPending && ai.agent.remainingDistance < 0.5f)
            {
                target = target == ai.pointA ? ai.pointB : ai.pointA;
                ai.agent.SetDestination(target.position);
            }
        }
    }

    class ChaseState : State
    {
        public ChaseState(EnemyController enemyCon) : base(enemyCon) { }

        public override void Enter()
        {
            ai.animator.SetBool("Chasing", true);
            ai.agent.isStopped = false;
            ai.agent.speed = ai.chaseSpeed;
        }

        public override void Execute()
        {
            ai.agent.SetDestination(ai.player.position);

            float distance = ai.DistanceToPlayer();

            if (distance <= ai.attackRange)
            {
                //Trocar para o estado de ataque!
                ai.ChangeState(ai.attack);
            }
            else if (distance > ai.loseRange)
            {
                ai.ChangeState(ai.patrol);
            }
        }
    }

    class AttackState : State
    {
        float timer;

        public AttackState(EnemyController enemyCon) : base(enemyCon) { }

        public override void Enter()
        {
            ai.animator.SetBool("Chasing", false);
            ai.animator.SetTrigger("Attack");
            ai.agent.isStopped = true;
            ai.agent.velocity = Vector3.zero;
            timer = ai.attackCooldown;
        }

        public override void Execute()
        {
            timer += Time.deltaTime;

            if (timer < ai.attackDuration)
            {
                return;
            }

            if (ai.DistanceToPlayer() > ai.attackRange)
            {
                ai.ChangeState(ai.chase);
                return;
            }

            Vector3 lookPosition = ai.player.position;
            lookPosition.y = ai.transform.position.y;
            ai.transform.LookAt(lookPosition);

            if (timer >= ai.attackCooldown)
            {
                ai.animator.ResetTrigger("Attack");
                ai.animator.SetTrigger("Attack");
                timer = 0f;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}


