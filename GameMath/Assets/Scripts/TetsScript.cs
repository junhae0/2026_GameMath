using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f; // 스프린트 속도 배수
    private Vector2 mouseScreenPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isSprinting = false; // 스프린트 상태


    public void OnPoint(InputValue value)
    {
        mouseScreenPosition = value.Get<Vector2>();  // 마우스 위치 없데이트
    }
    public void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);  //레이저 경로에 있는 모든 물체를 탐색

            foreach (RaycastHit hit in hits) // 모든 물체에 한해 반복
            {
                if (hit.collider.gameObject != gameObject)  // 부딪힌 물체가 나 자신이 아닐 때만
                {
                    targetPosition = hit.point; // Plane에 부딪힌 지점을 타겟
                    targetPosition.y = transform.position.y;
                    isMoving = true;

                    break;  // 탐색 했으니 foreach 반복 중단
                }
            }
        }
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed; // Shift 누르면 true, 떼면 false
    }
    void Update()
    {
        if (isMoving)
        {

            Vector3 direction = targetPosition - transform.position;

            float sqrMag = (direction.x * direction.x) + (direction.y * direction.y) + (direction.z * direction.z);
            float magnitude = Mathf.Sqrt(sqrMag);

            if (magnitude < 0.05f)
            {
                isMoving = false;
                return;
            }
            Vector3 normalizedDir = direction / magnitude;
            float currentSpeed = moveSpeed;

            if (isSprinting)
            {
                currentSpeed = moveSpeed * sprintMultiplier; // 스프린트 시 속도 증가
            }

            transform.position += normalizedDir * currentSpeed * Time.deltaTime;
        }
    }
}
