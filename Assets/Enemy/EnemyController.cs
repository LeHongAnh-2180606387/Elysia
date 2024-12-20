using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;  // For UI (Image)

public class EnemyController : MonoBehaviour
{
    public enum AISTATE { PATROL, CHASE, ATTACK }

    public Transform player;
    public NavMeshAgent enemy;
    public Animator animator;

    public AISTATE enemyState = AISTATE.PATROL;

    float distanceOffset = 2f;
    float defensiveDistance = 1.5f;
    float attackCooldown = 2f;

    public bool isDefensive = false;
    public bool isAttacking = false;

    private float lastAttackTime;
    public List<Transform> waypoints = new List<Transform>();
    Transform currentWaypoint;

    public float enemyHealth = 500f;  // Enemy health
    public float attackDamage = 20f;
    public float enemyArmor = 10f;
    public float xpReward = 100f;


    public bool isTakeDamage = false;

    // Reference to UI health bar (Image)
    public Image healthBarFill;
            
    private bool isInitialized = false;
    private void Start()
    {
        currentWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        ChangeState(AISTATE.PATROL);

        // Initialize health bar if assigned
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;  // Full health bar
        }
        if (!isInitialized && gameObject.CompareTag("boss"))
        {
            attackDamage *= 2;
            enemyHealth *= 2;
            enemyArmor *= 2;
            xpReward *= 2;
            isInitialized = true;
        }
    }

private void Update()
{
float distanceToPlayer = Vector3.Distance(transform.position, player.position);

// Check if the enemy should go into defensive mode (close to player)
isDefensive = distanceToPlayer <= defensiveDistance;

// Vô hiệu hóa di chuyển khi ở trạng thái phòng thủ
if (isDefensive)
{
enemy.isStopped = true; // Stop the NavMeshAgent
enemy.velocity = Vector3.zero; // Ensure no movement occurs
}
else
{
// If not defensive, resume movement
enemy.isStopped = false;
}

// Attack logic with cooldown when defensive
if (isDefensive && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
{
StartAttack();
}

// Update animator states
animator.SetBool("isDefensive", isDefensive);
animator.SetBool("isAttacking", isAttacking);
}



public void ChangeState(AISTATE newState)
    {
        StopAllCoroutines();
        enemyState = newState;
        animator.SetBool("isPatrolling", enemyState == AISTATE.PATROL);
        animator.SetBool("isChasing", enemyState == AISTATE.CHASE);
        animator.SetBool("isAttacking", enemyState == AISTATE.ATTACK);

        switch (enemyState)
        {
            case AISTATE.PATROL:
                StartCoroutine(PatrolState());
                break;
            case AISTATE.CHASE:
                StartCoroutine(ChaseState());
                break;
            case AISTATE.ATTACK:
                StartCoroutine(AttackState());
                break;
            default:
                break;
        }
    }

public IEnumerator ChaseState()
{
while (enemyState == AISTATE.CHASE)
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    // If the enemy is close enough to attack the player, switch to attack state
    if (distanceToPlayer <= distanceOffset)
    {
        ChangeState(AISTATE.ATTACK);
        yield break; // Exit the chase state and enter attack state
    }

    // Move towards the player
    enemy.SetDestination(player.position);

    // Smoothly rotate towards the player
    Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

    // Yield control for the next frame to keep updating
    yield return null;
}
}



public IEnumerator AttackState()
{
while (enemyState == AISTATE.ATTACK)
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    // Nếu người chơi ra ngoài phạm vi tấn công, chuyển sang trạng thái Chase
    if (distanceToPlayer > distanceOffset)
    {
        ChangeState(AISTATE.CHASE);
        yield break;
    }

    // Kiểm tra nếu có thể tấn công
    if (CanAttackPlayer())
    {
        // Quay về hướng người chơi để tấn công
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Gây sát thương cho người chơi nếu có thể
        DealDamageToPlayer();
    }

    // Chờ thời gian cooldown trước khi tấn công lại
    yield return new WaitForSeconds(attackCooldown);
}
}


    public IEnumerator PatrolState()
    {
    while (enemyState == AISTATE.PATROL)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (isDefensive)
        {
            yield return null; // Không thực hiện gì nếu đang thủ
            continue;
        }

        if (distanceToPlayer <= distanceOffset)
        {
            ChangeState(AISTATE.CHASE);
            yield break;
        }

        enemy.SetDestination(currentWaypoint.position);

        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceOffset)
        {
            currentWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        }

        yield return null;
    }
   }


public void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            StartCoroutine(EndAttack());
        }
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.5f); // Attack duration (0.5s)
        isAttacking = false;  // End attack state
    }

    // Method to deal damage to player during attack
    public void DealDamageToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= distanceOffset)
        {
            ThirdPersonController playerHealth = player.GetComponent<ThirdPersonController>();
            if (playerHealth != null)
            {
                if (gameObject.CompareTag("Enemy"))
                {
                    // Normal enemy damage
                    playerHealth.TakeDamage(attackDamage, this.gameObject);
                }
                else if (gameObject.CompareTag("boss"))
                {
                    // Boss deals double damage
                    playerHealth.TakeDamage(attackDamage, this.gameObject);
                }
            }
        }
    }

// Method to check if player is within attack angle
    private bool CanAttackPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // Kẻ địch chỉ tấn công nếu người chơi nằm trong phạm vi 45 độ và đủ gần
        return angle <= 45f && Vector3.Distance(transform.position, player.position) <= distanceOffset;
    }


    // Method to take damage for the enemy or boss
    public void TakeDamage(float damage)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            // Apply damage to regular enemy
            enemyHealth -= (damage - enemyArmor);
            enemyHealth = Mathf.Clamp(enemyHealth, 0, 500f);  // Clamp health from 0 to max health

            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = enemyHealth / 500f;  // Update health bar
            }

            // Set isTakeDamage parameter to true when the enemy takes damage
            animator.SetBool("isTakeDamage", true);

            // Reset the isTakeDamage parameter after a delay (time duration of the damage animation)
            StartCoroutine(ResetTakeDamage());

            if (enemyHealth <= 0)
            {
                Die();
            }
        }
        else if (gameObject.CompareTag("boss"))
        {
            // Apply damage to boss (double armor effect)
            enemyHealth -= (damage - enemyArmor);  // Boss armor is stronger
            enemyHealth = Mathf.Clamp(enemyHealth, 0, 1000f);  // Clamp health from 0 to max health

            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = enemyHealth / 1000f;  // Update health bar for boss
            }

            // Set isTakeDamage parameter to true when the boss takes damage
            animator.SetBool("isTakeDamage", true);

            // Reset the isTakeDamage parameter after a delay (time duration of the damage animation)
            StartCoroutine(ResetTakeDamage());

            if (enemyHealth <= 0)
            {
                Die();
            }
        }
    }

    // Coroutine to reset the 'isTakeDamage' parameter after a short delay
    private IEnumerator ResetTakeDamage()
    {
        // Wait for the animation duration (e.g., 0.5s or the length of the damage animation)
        yield return new WaitForSeconds(0.5f);  // Adjust the wait time as needed

        // Set isTakeDamage back to false after the delay
        animator.SetBool("isTakeDamage", false);
    }

    // Method to handle enemy death
    private void Die()
    {
        Debug.Log("Enemy died");

        // Give XP to the player based on tag
        if (player != null)
        {
            ThirdPersonController playerController = player.GetComponent<ThirdPersonController>();
            if (playerController != null)
            {
                if (gameObject.CompareTag("Enemy"))
                {
                    playerController.GainXP(xpReward);
                    Debug.Log("Player gained XP from Enemy: " + xpReward);
                }
                else if (gameObject.CompareTag("boss"))
                {
                    playerController.GainXP(xpReward); // Boss gives double XP
                    Debug.Log("Player gained XP from Boss: " + (xpReward));
                }
            }
        }

        // Disable enemy after death
        gameObject.SetActive(false);
    }

    // Trigger collision detection with player (could be used for triggering Chase state)
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChangeState(AISTATE.CHASE);
        }
    }
}
