using UnityEngine;

/// <summary>
/// 일정 시간 간격마다 장애물을 1~2개 생성한다.
/// - 1개: 중앙 기준 위/아래 범위 안에서 랜덤 Y.
/// - 2개: 같은 X 에 놓되 두 장애물 사이 간격은 항상 pairGap 으로 고정하고,
///   그 쌍의 중심을 범위 안에서 랜덤하게 배치한다.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private ObjectPool pool;

    [Header("Score")]
    [SerializeField] private Transform player;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnX = 15f;
    [SerializeField] private float spawnZ = 0f;
    [SerializeField] private float centerY = 0f;
    [SerializeField] private float verticalRange = 3f;

    [Header("Two-Obstacle Gap")]
    [SerializeField] private float pairGap = 3f;

    private float timer;

    private void Awake()
    {
        // 인스펙터에 안 넣었으면 "Player" 태그가 붙은 오브젝트를 찾아 쓴다.
        if (player == null)
        {
            GameObject tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged != null)
            {
                player = tagged.transform;
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        int count = Random.Range(1, 3); // 1 또는 2
        if (count == 1)
        {
            SpawnAt(RandomY(), true);
        }
        else
        {
            SpawnPair();
        }
    }

    private void SpawnPair()
    {
        // 두 장애물 사이 간격은 항상 pairGap. 쌍의 중심만 범위 안에서 랜덤하게 고른다.
        float halfGap = pairGap * 0.5f;
        float minCenter = centerY - verticalRange + halfGap;
        float maxCenter = centerY + verticalRange - halfGap;

        float pairCenterY;
        if (minCenter <= maxCenter)
        {
            pairCenterY = Random.Range(minCenter, maxCenter);
        }
        else
        {
            // pairGap 이 허용 범위보다 넓으면 쌍을 중앙에 고정한다.
            pairCenterY = centerY;
        }

        // 쌍 중 아래쪽 하나만 득점 담당으로 삼아 웨이브당 1점만 들어오게 한다.
        SpawnAt(pairCenterY - halfGap, true);
        SpawnAt(pairCenterY + halfGap, false);
    }

    private float RandomY()
    {
        return Random.Range(centerY - verticalRange, centerY + verticalRange);
    }

    private void SpawnAt(float y, bool countsForScore)
    {
        Vector3 position = new Vector3(spawnX, y, spawnZ);
        GameObject obstacle = pool.Get(position, Quaternion.identity);

        ObstacleMover mover = obstacle.GetComponent<ObstacleMover>();
        if (mover != null)
        {
            mover.Activate(pool, player, countsForScore);
        }
    }
}
