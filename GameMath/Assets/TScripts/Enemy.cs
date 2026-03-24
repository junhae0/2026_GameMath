using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public float viewAngle = 90f;      // 시야각
    public float viewDistance = 10f;   // 시야 거리
    public float rotateSpeed = 30f;    // 기본 회전 속도
    public float chaseSpeed = 5f;      // 추적 속도

    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (!isChasing)
        {
            Patrol();
            DetectPlayer();
        }
        else
        {
            Chase();
        }
    }

    void Patrol()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    void DetectPlayer()
    {
        Vector3 dirToPlayer = player.position - transform.position;

        // --- 내적 직접 계산
        float dot = transform.forward.x * dirToPlayer.x +
                    transform.forward.y * dirToPlayer.y +
                    transform.forward.z * dirToPlayer.z;

        float lenA = Mathf.Sqrt(transform.forward.x * transform.forward.x +
                                transform.forward.y * transform.forward.y +
                                transform.forward.z * transform.forward.z);
        float lenB = Mathf.Sqrt(dirToPlayer.x * dirToPlayer.x +
                                dirToPlayer.y * dirToPlayer.y +
                                dirToPlayer.z * dirToPlayer.z);

        float angle = Mathf.Acos(dot / (lenA * lenB)) * Mathf.Rad2Deg;

        if (angle < viewAngle / 2 && dirToPlayer.magnitude < viewDistance)
        {
            isChasing = true;
        }
    }

    void Chase()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * chaseSpeed * Time.deltaTime;

        // 플레이어 바라보기
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0; // 수평 회전만
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);

        float dist = Vector3.Distance(transform.position, player.position);

        // PlayerController에 패링 가능 여부 전달
        PlayerController pc = player.GetComponent<PlayerController>();
        if (dist < 2f)
        {
            pc.SetNearestEnemy(transform, true);
        }
        else
        {
            pc.SetNearestEnemy(transform, false);
        }

        // 너무 가까우면 실패
        if (dist < 1f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}