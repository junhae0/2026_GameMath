using UnityEngine;

public class CameraSlerp : MonoBehaviour
{
    public Transform target;
    float speed = 2f;
    public Quaternion originRotation;

    private void Start()
    {
        originRotation = transform.rotation;
    }

    void Update()
    {
        if (target == null)
            return;

        Quaternion lookRot = Quaternion.LookRotation(target.position - transform.position);
        float t = 1f - Mathf.Exp(-speed * Time.deltaTime);

        // 직접 구현한 Slerp 함수를 호출하여 회전값을 적용합니다.
        transform.rotation = ManualSlerp(transform.rotation, lookRot, t);
    }

    Quaternion ManualSlerp(Quaternion from, Quaternion to, float t)
    {
        // 두 쿼터니언 사이의 내적(Dot Product)을 구합니다.
        float dot = Quaternion.Dot(from, to);

        // 내적이 음수라면, 두 쿼터니언이 반대 방향을 보고 있는 것입니다.
        // 이 경우 최단 경로로 회전하기 위해 방향을 반전시킵니다.
        if (dot < 0f)
        {
            to = new Quaternion(-to.x, -to.y, -to.z, -to.w);
            dot = -dot;
        }

        if (1f - dot < 0.01f)
        {
            Quaternion lerp = new Quaternion(
                Mathf.Lerp(from.x, to.x, t),
                Mathf.Lerp(from.y, to.y, t),
                Mathf.Lerp(from.z, to.z, t),
                Mathf.Lerp(from.w, to.w, t)
            );
            return lerp.normalized;
        }

        // 두 회전 사이의 각도(theta)를 계산합니다.
        float theta = Mathf.Acos(dot);
        float sinTheta = Mathf.Sin(theta);

        // 구면 선형 보간 계수를 계산합니다.
        // 분모가 0이 되는 경우를 대비한 처리가 실제 서비스 코드에선 필요할 수 있습니다.
        float ratioA = Mathf.Sin((1f - t) * theta) / sinTheta;
        float ratioB = Mathf.Sin(t * theta) / sinTheta;

        // 최종 쿼터니언 성분 계산
        Quaternion result = new Quaternion(
            ratioA * from.x + ratioB * to.x,
            ratioA * from.y + ratioB * to.y,
            ratioA * from.z + ratioB * to.z,
            ratioA * from.w + ratioB * to.w
        );

        return result.normalized;
    }
}