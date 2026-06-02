using UnityEngine;

public class SimplePerlinTerrainProject : MonoBehaviour
{
    public int width = 30;
    public int depth = 30;
    public float scale = 0.1f;
    public float heightMultiplier = 8f;

    [Header("Block Prefabs")]
    public GameObject dirtPrefab;  // 흙 블록 프리팹
    public GameObject grassPrefab; // 잔디 블록 프리팹
    public GameObject waterPrefab; // 물 블록 프리팹

    [Header("Water Settings")]
    public int waterLevel = 3;     // 물이 차오를 기준 높이 (이 높이 이하에 블록이 없으면 물을 채움)

    SimplePerlinNoise simpleNoise;

    void Start()
    {
        simpleNoise = GetComponent<SimplePerlinNoise>();

        // 게임을 시작할 때마다 seed 변수에 0 ~ 99999 사이의 랜덤한 숫자를 부여합니다.
        simpleNoise.seed = Random.Range(0, 100000);

        Generate();
    }

    public void Generate()
    {
        // 1단계: 지형(흙과 잔디) 배치하기
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // Mathf.PerlinNoise 대신 직접 만든 simpleNoise 사용
                float xCoord = x * scale;
                float zCoord = z * scale;
                float noise = simpleNoise.Noise(xCoord, zCoord);

                int height = Mathf.RoundToInt(noise * heightMultiplier);

                CreateTerrainColumn(x, z, height);
            }
        }

        // 2단계: 배치가 끝난 후, 특정 높이(waterLevel) 이하의 빈 공간에 물 채우기
        FillWater();
    }

    // 블록 생성 로직 (과제 조건 1 반영)
    void CreateTerrainColumn(int x, int z, int height)
    {
        for (int y = 0; y <= height; y++)
        {
            Vector3 position = new Vector3(x, y, z);
            GameObject prefabToSpawn;

            // 과제 조건 1: 최상단일 경우 (y == height) Grass 배치, 나머지는 Dirt 배치
            if (y == height)
            {
                prefabToSpawn = grassPrefab;
            }
            else
            {
                prefabToSpawn = dirtPrefab;
            }

            Instantiate(prefabToSpawn, position, Quaternion.identity, transform);
        }
    }

    // 물 채우기 로직 (과제 조건 2 반영)
    void FillWater()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 물 기준 높이(waterLevel) 이하의 높이들을 검사
                for (int y = 0; y <= waterLevel; y++)
                {
                    Vector3 position = new Vector3(x, y, z);

                    // 과제 조건 2: 해당 위치에 지형 타일(Dirt나 Grass)이 있는지 체크
                    // Physics.CheckBox를 활용해 반경 0.1f 내에 이미 블록이 배치되어 있는지 확인합니다.
                    if (!Physics.CheckBox(position, new Vector3(0.1f, 0.1f, 0.1f)))
                    {
                        // 타일이 없다면 물(Water) 프리팹 배치
                        Instantiate(waterPrefab, position, Quaternion.identity, transform);
                    }
                }
            }
        }
    }
}