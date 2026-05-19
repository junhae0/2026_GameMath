using UnityEngine;
using UnityEngine.InputSystem; // 스페이스바 입력을 받기 위해 추가

public class CameraOrbit : MonoBehaviour
{
    public Transform target;          // 플레이어의 턴에 맞는 공 (GameManager가 실시간 대입)
    private float yaw = 0f;           //
    [HideInInspector] public float moveInput = 0f; //

    public float rotateSpeed = 100f;  //
    public Vector3 offset = new Vector3(0f, 4f, -7f); //

    [Header("전체 보기 (스페이스바) 설정")]
    public Transform tableCenterTarget; // 💡 당구대 바닥(Table_Floor) 오브젝트를 여기에 연결할 겁니다.
    private bool isViewingTotal = false;

    void Update()
    {
        // 💡 1. 스페이스바 입력 체크 (누르고 있는 동안 전체 화면 보기)
        if (Keyboard.current != null)
        {
            // 스페이스바를 누르고 있으면 true, 떼면 false
            isViewingTotal = Keyboard.current.spaceKey.isPressed;
        }

        // 💡 2. 현재 카메라가 쳐다볼 최종 타겟 결정
        Transform finalTarget = target;

        // 만약 스페이스바를 누르고 있고, 당구대 중심 오브젝트가 등록되어 있다면 당구대 중심으로 시선 변경!
        if (isViewingTotal && tableCenterTarget != null)
        {
            finalTarget = tableCenterTarget;
        }

        // 만약 둘 다 없다면 에러 방지를 위해 리턴
        if (finalTarget == null) return;

        // 좌우 방향키로 회전 연산
        yaw += moveInput * rotateSpeed * Time.deltaTime; //
        Quaternion rotation = Quaternion.Euler(0f, yaw, 0f); //
        Vector3 rotatedOffset = rotation * offset; //

        // 결정된 finalTarget을 중심으로 카메라 위치와 시선 고정
        transform.position = finalTarget.position + rotatedOffset;
        transform.LookAt(finalTarget);
    }
}