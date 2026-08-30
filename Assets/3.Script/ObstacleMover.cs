using UnityEngine;

/// <summary>
/// 장애물을 오른쪽에서 왼쪽으로 이동시키고,
/// 스폰 지점부터의 이동 거리가 despawnDistance 를 넘으면 스스로 풀로 반환한다.
/// 활성화될 때마다 무작위 축을 하나 정하고, 그 축을 중심으로 무작위 속도로 회전한다.
/// </summary>
public class ObstacleMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float despawnDistance = 25f;

    [Header("Rotation")]
    [SerializeField] private float minRotationSpeed = 30f;
    [SerializeField] private float maxRotationSpeed = 180f;

    private ObjectPool pool;
    private float traveledDistance;
    private Vector3 rotationAxis;
    private float rotationSpeed;

    /// <summary>
    /// 스포너가 풀에서 꺼낸 직후 호출한다.
    /// 반환할 풀을 기억하고, 이동 거리와 회전(축·속도)을 새로 뽑는다.
    /// </summary>
    public void Activate(ObjectPool owningPool)
    {
        pool = owningPool;
        traveledDistance = 0f;
        rotationAxis = Random.onUnitSphere;
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    private void Update()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.left * step, Space.World);

        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);

        traveledDistance += step;
        if (traveledDistance >= despawnDistance)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Return(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
