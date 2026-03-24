using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("게임 클리어!");

            Time.timeScale = 0f; // ⛔ 게임 멈춤
        }
    }
}