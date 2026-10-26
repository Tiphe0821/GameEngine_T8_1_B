using UnityEngine;

// Unity 6용. 기존 Health, Bullet, MeleeAttackTrigger와 함께 사용합니다.
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Health))]
public class JumpEnemy : MonoBehaviour
{
    [Header("플레이어 감지")]
    [Tooltip("비워 두면 Player 태그가 있는 오브젝트를 자동으로 찾습니다.")]
    public Transform player;
    [Min(0.1f)] public float detectRange = 15f;

    [Header("점프 이동")]
    [Tooltip("한 번 점프할 때 올라가는 높이입니다.")]
    [Min(0.1f)] public float jumpHeight = 1.5f;
    [Tooltip("점프 중 앞으로 이동하는 최대 속도입니다.")]
    [Min(0.1f)] public float jumpMoveSpeed = 5f;
    [Tooltip("착지한 뒤 다음 점프까지 기다리는 시간입니다.")]
    [Min(0.05f)] public float jumpCooldown = 0.8f;
    [Tooltip("양수로 입력합니다. 낮추면 더 천천히 떨어집니다.")]
    [Min(0.1f)] public float gravity = 20f;

    [Header("착지 후 근접 공격")]
    [Tooltip("적 몸의 중심에서 플레이어 몸의 가장 가까운 부분까지의 거리입니다.")]
    [Min(0.1f)] public float attackRange = 1.5f;
    [Min(1)] public int damage = 1;
    [Min(0.05f)] public float attackCooldown = 1f;

    [Header("바닥과 장애물")]
    [Tooltip("처음에는 Everything으로 두세요. 필요하면 바닥 레이어만 선택합니다.")]
    public LayerMask groundMask = ~0;
    [Tooltip("공격을 가로막는 벽과 지형의 레이어입니다. 기본값은 Everything입니다.")]
    public LayerMask obstacleMask = ~0;
    [Min(0.01f)] public float groundCheckDistance = 0.06f;

    private Rigidbody body;
    private CapsuleCollider bodyCollider;
    private Health playerHealth;
    private Collider playerCollider;
    private Transform cachedPlayer;
    private bool jumping;
    private float nextJumpTime;
    private float nextAttackTime;
    private float nextPlayerSearchTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();

        // 중력은 이 스크립트의 gravity 값으로 직접 적용합니다.
        body.useGravity = false;
        body.isKinematic = false;
        body.linearDamping = 0f;
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        bodyCollider.isTrigger = false;
        bodyCollider.direction = 1; // 세로(Y축) 캡슐
    }

    private void FixedUpdate()
    {
        body.AddForce(Vector3.down * Mathf.Max(0.1f, gravity), ForceMode.Acceleration);
        UpdatePlayerReference();

        // 위로 올라갈 때는 발밑에 바닥이 있어도 착지로 판단하지 않습니다.
        bool grounded = body.linearVelocity.y <= 0.1f && IsGrounded();
        if (!grounded) return;

        if (jumping)
        {
            jumping = false;
            nextJumpTime = Time.time + Mathf.Max(0.05f, jumpCooldown);
        }

        // 지상에서는 걷거나 미끄러지지 않고 다음 점프를 기다립니다.
        Vector3 velocity = body.linearVelocity;
        body.linearVelocity = new Vector3(0f, velocity.y, 0f);

        if (player == null || playerHealth == null || playerHealth.currentHp <= 0) return;

        Vector3 targetCenter = playerCollider != null
            ? playerCollider.bounds.center : player.position;
        Vector3 toPlayer = targetCenter - bodyCollider.bounds.center;
        if (toPlayer.sqrMagnitude > detectRange * detectRange) return;

        Vector3 direction = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (direction.sqrMagnitude > 0.001f)
            body.MoveRotation(Quaternion.LookRotation(direction));

        Vector3 nearestPoint = playerCollider != null
            ? playerCollider.ClosestPoint(bodyCollider.bounds.center) : player.position;
        bool inAttackRange = (nearestPoint - bodyCollider.bounds.center).sqrMagnitude
            <= attackRange * attackRange;

        if (inAttackRange)
        {
            if (Time.time >= nextAttackTime && HasClearAttackPath(targetCenter))
            {
                nextAttackTime = Time.time + Mathf.Max(0.05f, attackCooldown);
                playerHealth.TakeDamage(Mathf.Max(1, damage));
            }
            return;
        }

        if (Time.time < nextJumpTime || direction.sqrMagnitude < 0.001f) return;

        float jumpSpeed = Mathf.Sqrt(2f * Mathf.Max(0.1f, gravity) * Mathf.Max(0.1f, jumpHeight));
        float flightTime = 2f * jumpSpeed / Mathf.Max(0.1f, gravity);
        // 평지에서 플레이어를 지나치지 않도록 가까울수록 이동 속도를 줄입니다.
        float travelDistance = Mathf.Max(0f, direction.magnitude - attackRange * 0.65f);
        float horizontalSpeed = Mathf.Min(Mathf.Max(0.1f, jumpMoveSpeed), travelDistance / flightTime);

        body.linearVelocity = direction.normalized * horizontalSpeed + Vector3.up * jumpSpeed;
        jumping = true;
        // 공중에서는 방향을 바꾸지 않으므로 플레이어가 옆으로 피할 수 있습니다.
    }

    private void UpdatePlayerReference()
    {
        if (player == null && Time.time >= nextPlayerSearchTime)
        {
            nextPlayerSearchTime = Time.time + 1f;
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        if (player == cachedPlayer) return;
        cachedPlayer = player;
        playerHealth = player != null ? player.GetComponentInParent<Health>() : null;
        playerCollider = player != null ? player.GetComponent<Collider>() : null;

        if (playerHealth != null && playerCollider == null)
            playerCollider = playerHealth.GetComponentInChildren<Collider>();

        if (player != null && playerHealth == null)
            Debug.LogWarning("JumpEnemy: 플레이어 본체에 Health가 필요합니다.", this);
    }

    private bool IsGrounded()
    {
        Bounds bounds = bodyCollider.bounds;
        float radius = Mathf.Max(0.01f, Mathf.Min(bounds.extents.x, bounds.extents.z) * 0.85f);
        float distance = Mathf.Max(0.01f, bounds.extents.y - radius) + groundCheckDistance;
        RaycastHit[] hits = Physics.SphereCastAll(bounds.center, radius, Vector3.down,
            distance, groundMask, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            // 플레이어나 다른 적을 바닥으로 취급하지 않습니다.
            if (hit.collider.GetComponentInParent<Health>() != null) continue;
            // 처음부터 겹친 물체와 가파른 벽을 바닥으로 잘못 판단하지 않습니다.
            if (hit.distance > 0f && hit.normal.y >= 0.6f) return true;
        }
        return false;
    }

    private bool HasClearAttackPath(Vector3 targetCenter)
    {
        Vector3 origin = bodyCollider.bounds.center;
        Vector3 offset = targetCenter - origin;
        if (offset.sqrMagnitude < 0.0001f) return true;

        RaycastHit[] hits = Physics.RaycastAll(origin, offset.normalized, offset.magnitude,
            obstacleMask, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            if (hit.collider.GetComponentInParent<Health>() == playerHealth) continue;
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Vector3 center = capsule != null ? capsule.bounds.center : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
    }
}