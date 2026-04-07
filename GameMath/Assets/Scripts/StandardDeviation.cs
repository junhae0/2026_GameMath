using System.Linq;
using UnityEngine;

public class StandardDeviation : MonoBehaviour
{
    public int sampleCount = 1000;
    public float randomMin = 0;
    public float randomMax = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StandardDev();
    }

    // Update is called once per frame
    void StandardDev()
    {
        int n = 1000;       //»ùÇÃ ¼ö
        float[] samples = new float[n];
        for (int i = 0; i < n; i++)
        {
            samples[i] = Random.Range(randomMin, randomMax);
        }

        float mean = samples.Average();
        float sumOfSquares = samples.Sum(x => Mathf.Pow(x - mean, 2));
        float stdDev = Mathf.Sqrt(sumOfSquares / n);

        Debug.Log($"Æò±Õ : {mean}, Ç¥ÁØÆíÂ÷ : {stdDev}");
    }
}
