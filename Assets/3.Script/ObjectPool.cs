using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 운석 프리팹들을 재사용하는 Queue 기반 오브젝트 풀.
/// 프리팹들은 서로 교체 가능(전부 운석)하다고 보고 큐 하나만 쓴다.
/// 새 인스턴스가 필요할 때는 셔플백에서 프리팹을 뽑아 같은 프리팹이 연달아 나오는 것을 줄인다.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] private int prewarmCount = 20;

    private readonly Queue<GameObject> available = new Queue<GameObject>();
    private readonly List<GameObject> shuffleBag = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            GameObject instance = CreateInstance();
            available.Enqueue(instance);
        }
    }

    /// <summary>
    /// 풀에서 인스턴스를 하나 꺼내 지정한 위치/회전으로 활성화한다.
    /// </summary>
    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject instance;
        if (available.Count > 0)
        {
            instance = available.Dequeue();
        }
        else
        {
            instance = CreateInstance();
        }

        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    /// <summary>
    /// 다 쓴 인스턴스를 비활성화하고 큐로 되돌린다.
    /// </summary>
    public void Return(GameObject instance)
    {
        instance.SetActive(false);
        available.Enqueue(instance);
    }

    private GameObject CreateInstance()
    {
        GameObject prefab = NextPrefab();
        GameObject instance = Instantiate(prefab, transform);
        instance.SetActive(false);
        return instance;
    }

    private GameObject NextPrefab()
    {
        if (shuffleBag.Count == 0)
        {
            RefillShuffleBag();
        }

        int lastIndex = shuffleBag.Count - 1;
        GameObject prefab = shuffleBag[lastIndex];
        shuffleBag.RemoveAt(lastIndex);
        return prefab;
    }

    private void RefillShuffleBag()
    {
        shuffleBag.AddRange(prefabs);

        // Fisher-Yates 셔플
        for (int i = shuffleBag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = shuffleBag[i];
            shuffleBag[i] = shuffleBag[j];
            shuffleBag[j] = temp;
        }
    }
}
