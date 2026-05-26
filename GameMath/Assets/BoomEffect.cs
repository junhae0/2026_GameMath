using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestorySelf", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestorySelf()
    {
        Destroy(gameObject);
    }


}
