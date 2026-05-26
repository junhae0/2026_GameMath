using UnityEngine;

public class WBomb : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    public float explodeDelay = 2.0f;
    public float radius = 5f;
    public float force = 15f;
    public float upwardsModifier = 1f;

    private bool isGrounded = false;
    private Rigidbody rb;

    public GameObject effect;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // [수정 핵심 1] 폭탄이 굴러다니거나 부딪혀서 세로로 회전하는 것을 물리적으로 완전히 막습니다.
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void Setup(Vector3 launchDirection, float speed, Collider playerCollider)
    {
        Collider bombCollider = GetComponent<Collider>();

        // 플레이어와 폭탄 충돌 무시
        if (bombCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(bombCollider, playerCollider);
        }

        // [수정 핵심 2] 맵에 있는 모든 적("Enemy" 태그)과 이 폭탄의 물리 충돌을 통째로 무시합니다.
        // 이렇게 하면 적에게 막혀 세로로 서거나 뒤로 날아가는 일이 절대 일어나지 않습니다.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null && bombCollider != null)
            {
                Physics.IgnoreCollision(bombCollider, enemyCollider);
            }
        }

        // 앞+위 대각선 방향 속도 부여
        Vector3 launchDir = (launchDirection + Vector3.up * 0.6f).normalized;
        velocity = launchDir * speed;

        Invoke("Explode", explodeDelay);
    }

    void Update()
    {
        if (!isGrounded)
        {
            velocity += gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // 오직 바닥(Floor, Plane, Terrain)에 닿았을 때만 정확하게 멈춤
        if (col.gameObject.CompareTag("Floor") ||
            col.gameObject.name.Contains("Terrain") ||
            col.gameObject.name.Contains("Plane") ||
            col.gameObject.name.Contains("Floor"))
        {
            isGrounded = true;
            velocity = Vector3.zero;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; // 바닥에 안착하면 물리 휴면 상태로 전환
            }
        }
    }

    void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (var col in colliders)
        {
            Rigidbody targetRb = col.attachedRigidbody;
            if (targetRb == null) continue;

            Vector3 toTarget = targetRb.position - explosionPos;
            float distance = toTarget.magnitude;
            Vector3 dir = toTarget.normalized;

            float attenuation = 1f - Mathf.Clamp01(distance / radius);
            dir += Vector3.up * upwardsModifier;
            dir = dir.normalized;

            Vector3 impulse = dir * force * attenuation;

            targetRb.isKinematic = false;
            targetRb.AddForce(impulse, ForceMode.Impulse);
        }

        Debug.Log("폭발!");
        GameObject effectObject = Instantiate(effect) as GameObject;
        effectObject.transform.position = transform.position;
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}