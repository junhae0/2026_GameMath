using Unity.VisualScripting;
using UnityEngine;

public class sun : MonoBehaviour
{
    public Transform Sun;
    public float distance = 5f;
    public float speed = 1f;

    float angle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        angle += speed * Time.deltaTime;

        float x = Sun.position.x + Mathf.Cos(angle) * distance;
        float z = Sun.position.z + Mathf.Sin(angle) * distance;

        transform.position = new Vector3(x, 0, z);
    }
}
