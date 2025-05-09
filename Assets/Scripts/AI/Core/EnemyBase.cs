using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    [SerializeField] private bool isStrong; //if this is true, enemy will NOT respawn with the player(for bosses or strong enemies)
    [SerializeField] private int startingHearts = 1; //how many hearts they will have (healthbars)
    [SerializeField] private float maxPoise = 100; // how much poise they have(health)
    [SerializeField] private float staggeredDuration = 5; // how long they will remain motionless when their poise reaches 100%

    public float poiseRegenerationInterval = 0.1f;
    public float poiseRegenerationAmount = 1f;
    private float minPoise = 0;
    private float timer = 0f;

    [Header("Perception")]
    [SerializeField] private VisionCone vision; //reference to a visionCone object for player detection

    [Header("Attack List")]
    [SerializeField] private AttackData[] attacks; // list of their attacks

    public Transform Target { get; protected set; } // target getters and setters
    public int HeartsRemaining => hearts;
    public event System.Action<IEnemy> HeartLost; // broadcast when they lose a heart

    protected NavMeshAgent agent;
    protected Animator anim;

    protected EnemyState state = EnemyState.Patrolling; //start them in patrolling
    protected float currentPoise;
    private int hearts;

    #region Unity‑lifecycle
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        hearts = startingHearts;
        currentPoise = minPoise; //set to min poise since 100% is dead and 0 is starting
    }
    protected virtual void Start()
    {
        vision.OnPlayerDetected += HandlePlayerSeen; //what does this mean?
        ResetPatrol();
    }

    protected virtual void Update() // calls the EnemyState method every frame
    {
        switch (state)
        {
            case EnemyState.Patrolling: TickPatrol(); break;
            case EnemyState.Chasing: TickChase(); break;
            case EnemyState.Attacking: TickAttack(); break;
        }
    }
    #endregion

    public void TakeDamage(float amount, bool parried) //take damage logic, will have to reverse soon
    {
        if (state == EnemyState.Staggered)
        {
            hearts--; //play special animation
            if (hearts <= 0) { Die(); return; }
            StartCoroutine(ConsumeHeart());
            return;
        }

        currentPoise += amount;
        if (currentPoise >= maxPoise)
        {
            StartCoroutine(EnterStaggeredState());
            return;
        }
    }

    void DoPoiseRegeneration()
    {
        timer += Time.deltaTime;
        if (timer >= poiseRegenerationInterval)
        {
            timer = 0f;
            currentPoise -= poiseRegenerationAmount;

            if (currentPoise <= minPoise)
            {
                currentPoise = minPoise;
            }
        }
    }

    private IEnumerator EnterStaggeredState()
    {
        ChangeState(EnemyState.Staggered);
        anim.SetTrigger("Staggered");
        currentPoise = minPoise; // reset early for next phase
        float t = 0;

        while (t < staggeredDuration && hearts > 0)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (hearts > 0) ChangeState(EnemyState.Chasing);
    }

    public AttackData GetNextAttack() // get random attack from list
        => attacks.Length == 0 ? null : attacks[Random.Range(0, attacks.Length)];

    private IEnumerator ConsumeHeart()
    {
        CombatEvents.OnEnemyHeartLost?.Invoke(this);
        HeartLost?.Invoke(this);

        // visual feedback
        anim.SetTrigger("HeartLost");
        yield return new WaitForSeconds(0.2f);

        currentPoise = maxPoise;

        OnHeartLost();
    }

    protected virtual void OnHeartLost() { } // what to do when a heart is lost(subclasses determine this)

    void HandlePlayerSeen(Transform player) // go into chase if the enemy sees a player
    {
        Target = player;
        if (state == EnemyState.Patrolling) ChangeState(EnemyState.Chasing);
    }

    protected void ChangeState(EnemyState next)
    {
        state = next;
        anim.SetInteger("State", (int)next);
    }

    protected abstract void TickPatrol();
    protected abstract void TickChase();
    protected abstract void TickAttack();

    protected virtual void ResetPatrol()
    {
        agent.SetDestination(transform.position); // idle at pos1 by default
    }

    // kill the enemy, wait for respawn trigger
    protected virtual void Die()
    {
        if (isStrong)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(EnemySpawner.Respawn(this));
        }
    }

    void OnValidate() // gives feedback to testers about patrolling
    {
        if (transform.childCount == 0)
            Debug.LogWarning($"{name} has no patrol points – will idle.", this);
    }


    // expose flag
    public bool IsStrong => isStrong; //if its a boss or strong enemy
}
