using UnityEngine;

public class tlftmq5 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform player;
    public float viewAngle = 60f; //시야각


    void Update()
    {
        Vector3 toPlayer = (player.position - transform.position), normalized;
        Vector3 forward = transform.forward;

        float dot = Vector3.Dot(forward.normalized, toPlayer);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // 내적을 각도로 변환

        if (angle < viewAngle / 2)
        {
            Debug.Log("플레이어가 시야 안에 있음!");
        }
    }
}