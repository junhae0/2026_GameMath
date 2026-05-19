using UnityEngine;

public class BilliardsBall : MonoBehaviour
{
    public enum BallType { Player1, Player2, Target }
    public BallType type;

    private Rigidbody rb;
    private const float StopThreshold = 0.05f; // 이 속도 이하이면 멈춘 것으로 판정

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 과제 조건: 공이 위아래(Y축)로 튀지 않도록 강제 억제
        if (Mathf.Abs(rb.linearVelocity.y) > 0.01f)
        {
            Vector3 currentVel = rb.linearVelocity;
            currentVel.y = 0f;
            rb.linearVelocity = currentVel;
        }

        // 미세하게 굴러다니는 현상을 차단하고 완벽히 세우기
        if (rb.linearVelocity.magnitude < StopThreshold && rb.linearVelocity.magnitude > 0f)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 공들끼리 부딪혔을 때 GameManager에게 보고하는 로직
    private void OnCollisionEnter(Collision collision)
    {
        BilliardsBall otherBall = collision.gameObject.GetComponent<BilliardsBall>(); //
        if (otherBall != null) //
        {
            // 💡 마지막 자리에 , otherBall 을 추가하여 3개의 인자를 모두 맞춰줍니다!
            GameManager.Instance.RecordCollision(this.type, otherBall.type, otherBall);
        }
    }

    // 현재 공이 정지 상태인지 여부 반환
    public bool IsStopped()
    {
        return rb.linearVelocity.sqrMagnitude < 0.001f;
    }
}