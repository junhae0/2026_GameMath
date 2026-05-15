using UnityEngine;

public class BezierMover : MonoBehaviour
{
    public Vector3 p0; // 시작점
    public Vector3 p1; // 제어점 1
    public Vector3 p2; // 제어점 2
    public Vector3 p3; // 도착점

    public float speed = 1.5f;

    private float t = 0f;
    private bool isShooting = false;

    public void SetBezier(Vector3 start, Vector3 end)
    {
        p0 = start;
        p3 = end;

        Vector3 dir = (end - start).normalized;
        Vector3 side = Vector3.Cross(dir, Vector3.up).normalized;

        // 좌우 퍼짐
        float sidePower1 = Random.Range(-5f, 5f);
        float sidePower2 = Random.Range(-5f, 5f);

        // 공마다 높이가 다르게
        float height1 = Random.Range(-2f, 4f);
        float height2 = Random.Range(-2f, 4f);

        // 앞으로 가는 거리도 공마다 다르게
        float forward1 = Random.Range(1f, 4f);
        float forward2 = Random.Range(4f, 8f);

        p1 = start + dir * forward1 + side * sidePower1 + Vector3.up * height1;
        p2 = start + dir * forward2 + side * sidePower2 + Vector3.up * height2;

        t = 0f;
        isShooting = true;
    }

    void Update()
    {
        if (isShooting == false)
            return;

        t += Time.deltaTime * speed;

        transform.position = FourPointBezier(p0, p1, p2, p3, t);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    Vector3 FourPointBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        return Mathf.Pow(1 - t, 3) * a
             + 3 * Mathf.Pow(1 - t, 2) * t * b
             + 3 * (1 - t) * Mathf.Pow(t, 2) * c
             + Mathf.Pow(t, 3) * d;
    }
}