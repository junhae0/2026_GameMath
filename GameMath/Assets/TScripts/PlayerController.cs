using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 120f;

    [HideInInspector] public bool canParry = false; // 패링 가능 상태
    private Transform nearestEnemy;

    void Update()
    {
        Move();

        if (canParry && nearestEnemy != null)
        {
            CheckParryInput();
        }
    }

    void Move()
    {
        float move = Input.GetAxis("Vertical");   // W/S
        float rotate = Input.GetAxis("Horizontal"); // A/D
        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * rotate * rotateSpeed * Time.deltaTime);
    }

    public void SetNearestEnemy(Transform enemy, bool status)
    {
        nearestEnemy = status ? enemy : null;
        canParry = status;
    }

    void CheckParryInput()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            Vector3 toEnemy = nearestEnemy.position - transform.position;

            // --- 외적 y 성분 직접 계산 (플레이어 앞 기준)
            float crossY = transform.forward.x * toEnemy.z - transform.forward.z * toEnemy.x;

            if (crossY > 0 && Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("패링 성공!");
                Destroy(nearestEnemy.gameObject);
            }
            else if (crossY < 0 && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("패링 성공!");
                Destroy(nearestEnemy.gameObject);
            }
            else
            {
                Debug.Log("패링 실패!");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            canParry = false;
            nearestEnemy = null;
        }
    }
}