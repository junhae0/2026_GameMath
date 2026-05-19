using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycastTest : MonoBehaviour
{
    public float rayDistance = 100f;
    float moveInput;
    public CameraOrbit cam;

    // 8번 보너스 요구사항: 힘 조절을 위한 내장 변수들
    public float basePower = 20f;      // 최소 파워
    public float maxPower = 70f;       // 최대 누적 파워
    private float currentChargeTime = 0f;
    private bool isCharging = false;
    private Rigidbody targetRb;
    private Vector3 savedHitPoint;

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
        cam.moveInput = moveInput;
    }

    public void OnClick(InputValue value)
    {
        // 규칙 3: 공이 움직이는 중에는 입력 원천 차단
        if (!GameManager.Instance.CanPlay) return;

        // 마우스를 클릭하기 시작했을 때 (힘 모으기 충전 시작)
        if (value.isPressed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;

                if (rb != null)
                {
                    BilliardsBall ball = rb.GetComponent<BilliardsBall>();
                    // 규칙 2: 현재 내 턴에 맞는 올바른 공을 클릭했는지 판단
                    if (ball != null && GameManager.Instance.IsMyBall(ball.type))
                    {
                        targetRb = rb;
                        savedHitPoint = hit.point;
                        isCharging = true;
                        currentChargeTime = 0f;
                        Debug.Log(" 게이지 충전 중... 마우스를 떼면 발사됩니다!");
                    }
                }
            }
        }
        // 마우스를 뗄 때 (충전된 힘으로 발사)
        else
        {
            if (isCharging && targetRb != null)
            {
                // 충전 시간에 비례해 힘 계산 (최대 1.5초 충전 기준)
                float chargeRatio = Mathf.Clamp(currentChargeTime / 1.5f, 0f, 1f);
                float finalPower = Mathf.Lerp(basePower, maxPower, chargeRatio);

                // 사진속 기존 연산 공식 완벽 유지
                Vector3 ballCenter = targetRb.transform.position;
                Vector3 forceDirection = ballCenter - savedHitPoint;
                forceDirection.y = 0f;
                forceDirection = forceDirection.normalized;

                // 공 타격
                targetRb.AddForce(forceDirection * finalPower, ForceMode.Impulse);
                Debug.Log($" 발사 파워: {Mathf.RoundToInt(chargeRatio * 100)}% 적용됨!");

                // 매니저에게 공이 굴러가기 시작했다고 보고
                GameManager.Instance.OnBallShot();

                // 차징 상태 초기화
                isCharging = false;
                targetRb = null;
            }
        }
    }

    void Update()
    {
        // 마우스를 꾹 누르고 있으면 게이지가 쌓임
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
        }
    }
}