using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject spherePrefab;
    public Transform target;
    public int sphereCount = 10;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }
    }

    public void OnAttack()
    {
        for (int i = 0; i < sphereCount; i++)
        {
            Vector3 startPos = transform.position;

            // 시작 위치 랜덤 퍼짐
            startPos += new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.3f, 0.8f),
                Random.Range(-0.3f, 0.3f)
            );

            GameObject sphere = Instantiate(
                spherePrefab,
                startPos,
                Quaternion.identity
            );

            BezierMover bezier = sphere.GetComponent<BezierMover>();

            Vector3 targetPos = target.position;

            bezier.SetBezier(startPos, targetPos);
        }
    }
}