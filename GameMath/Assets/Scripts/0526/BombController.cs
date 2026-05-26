using UnityEngine;

public class BombController : MonoBehaviour
{
    public GameObject qBombPrefab;
    public GameObject wBombPrefab;

    [Header("스킬 발사 속도")]
    public float qLaunchSpeed = 12f;
    public float wLaunchSpeed = 8f;

    [Header("플레이어 이동 설정")]
    public float moveSpeed = 5f; // 이동 속도

    private Collider playerCollider;
    private Rigidbody rb;

    void Start()
    {
        playerCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        // 이동 중 물리 마찰로 인해 캐릭터가 회전되거나 넘어지는 현상 방지
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Update()
    {
        // 1. WASD 이동 처리
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D / 왼쪽, 오른쪽 방향키
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S / 위쪽, 아래쪽 방향키

        // 카메라 기준이 아닌 월드 좌표 기준으로 이동 벡터 생성
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // 이동 키를 누르고 있을 때만 해당 방향으로 위치 이동 및 회전
        if (moveDirection.magnitude >= 0.1f)
        {
            // 위치 이동 (Time.deltaTime을 곱해 부드럽게)
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // 캐릭터가 이동하는 방향을 자연스럽게 바라보도록 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }

        // 현재 플레이어가 정면으로 바라보고 있는 방향 계산
        Vector3 lookDir = transform.forward;


        // 2. Q 스킬 (반동 폭탄 발사)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 spawnPos = transform.position + lookDir * 0.5f + Vector3.up * 0.5f;
            GameObject bomb = Instantiate(qBombPrefab, spawnPos, Quaternion.identity);

            QBomb qScript = bomb.GetComponent<QBomb>();
            if (qScript != null)
            {
                qScript.Setup(lookDir, qLaunchSpeed, playerCollider);
            }
        }


        // 3. E 스킬 (휴대용 폭탄 투척)
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 spawnPos = transform.position + lookDir * 0.5f + Vector3.up * 0.5f;
            GameObject bomb = Instantiate(wBombPrefab, spawnPos, Quaternion.identity);

            WBomb wScript = bomb.GetComponent<WBomb>();
            if (wScript != null)
            {
                wScript.Setup(lookDir, wLaunchSpeed, playerCollider);
            }
        }
    }
}