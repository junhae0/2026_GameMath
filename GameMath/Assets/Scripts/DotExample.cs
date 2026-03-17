using UnityEngine;

public class DotExample : MonoBehaviour
{
    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        // 적 -> 플레이어 방향 백터
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        Vector3 forward = transform.forward; // 적의 정면 방향
        forward.y = 0f;

        forward.Normalize();
        toPlayer.Normalize();

        float dot = Vector3.Dot(forward, toPlayer);

        if (dot > 0f)
        {
            Debug.Log("플레이어가 적의 앞쪽에 있음");
        }
        else if (dot < 0f)
        {
            Debug.Log("플레이어가 적의 뒤쪽에 있음");
        }
        else
        {
            Debug.Log("플레이어가 적의 옆쪽에 있음");
        }
    }
}
