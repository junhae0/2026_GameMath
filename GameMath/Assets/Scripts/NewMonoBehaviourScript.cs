using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private void Start()
    {

        float degrees = 45f;
        float radians = degrees * Mathf.Deg2Rad;
        Debug.Log("45도 -> 라디안 : " + radians);

        float radianValue = Mathf.PI / 3;
        float degreeValue = radianValue * Mathf.Deg2Rad;
        Debug.Log("파이/3 라디안 -> 도 변환: " + degreeValue);
    }

    void Update()
    {
        float speed = 5f;
        float angle = 30f; //이동할 방향 (도 단위)
        float radians = angle * Mathf.Deg2Rad;

        Vector3 direction = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        transform.position += direction * speed * Time.deltaTime;
    }

}
