using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class PredictionLineRender : MonoBehaviour
{
    public Transform startPos;  // A
    public Transform endPos;    // B

    [Range(1f, 5f)] public float extend = 1.5f;

    private LineRenderer lr;

    public CameraSlerp cam;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;              // 단순 직선이므로 점 2개
        lr.widthMultiplier = 0.05f;        // 두께
        lr.material = new Material(Shader.Find("Unlit/Color"))
        {
            color = Color.red
        };
    }

    void Update()
    {
        if (!startPos || !endPos) return;

        Vector3 a = startPos.position;
        Vector3 b = endPos.position;

        // LerpUnclamped를 사용하여 b 지점을 넘어서는 연장선 좌표를 계산합니다.
        Vector3 pred = Vector3.LerpUnclamped(a, b, extend);

        lr.SetPosition(0, a);
        lr.SetPosition(1, pred);
    }
    public void OnRightClick(InputValue value)
    {
        if (!value.isPressed) return;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // 타게팅
                startPos = transform;
                endPos = hit.transform;
                cam.target = endPos;
                lr.positionCount = 2;
            }
        }
        else
        {
            // 초기화
            startPos = null;
            endPos = null;
            cam.target = null;
            cam.transform.rotation = cam.originRotation;
            lr.positionCount = 0;
        }
    }
}