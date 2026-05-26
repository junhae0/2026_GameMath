using UnityEngine;

public class QBomb : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    private int bounceCount = 0;
    private const int maxBounces = 3;
    private float lastBounceTime = 0f; // 중복 충돌 방지용 시간 체크


    public GameObject effect;
    public void Setup(Vector3 launchDirection, float speed, Collider playerCollider)
    {
        // 플레이어와 폭탄의 충돌을 무시 (내가 밀리는 현상 방지)
        Collider bombCollider = GetComponent<Collider>();
        if (bombCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(bombCollider, playerCollider);
        }

        // 대각선 위 방향으로 포물선을 그리며 날아가도록 약간 위쪽 힘을 더함
        Vector3 launchDir = (launchDirection + Vector3.up * 0.5f).normalized;
        velocity = launchDir * speed;
    }

    void Update()
    {
        velocity += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

    void OnCollisionEnter(Collision col)
    {
        // 1. 적에게 닿았을 경우 즉시 폭발
        if (col.gameObject.CompareTag("Enemy"))
        {
            Explode();
            return;
        }

        // 연속 충돌 버그 방지 (0.05초 이내의 연속 충돌은 무시)
        if (Time.time - lastBounceTime < 0.05f) return;
        lastBounceTime = Time.time;

        // 2. 바닥 등에 닿았을 경우 반사 벡터 계산 (내적 공식)
        Vector3 normal = col.contacts[0].normal.normalized;
        float dot = Vector3.Dot(velocity, normal);
        Vector3 reflect = velocity - 2f * dot * normal;

        velocity = reflect * 0.75f; // 튕길 때마다 속도 감쇄

        bounceCount++;

        // 3. 정확히 땅에 3번 튕긴 후 폭발
        if (bounceCount >= maxBounces)
        {
            Explode();
        }
    }

    void Explode()
    {
        GameObject effectObject = Instantiate(effect) as GameObject;
        effectObject.transform.position = transform.position; 
        Debug.Log("폭발!");
        Destroy(gameObject);
    }
}