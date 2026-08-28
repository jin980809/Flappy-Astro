using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //컴포넌트
    private PlayerInput input;
    private Rigidbody playerRigid;

    //점프 변수
    [SerializeField]
    private float jumpPower = 100f;

    //public 다 프로퍼티로 할거임

    //왼쪽 스킬 변수
    public float maxLeftGauge = 100f;
    public float currentLeftGauge;


    //마우스 좌표값 계산 변수
    public Vector3 mouseScreenPosition;
    public Vector3 mouseWorldPosition;
    public Vector3 currentPosition;
    public float zDistance;

    //오른쪽 스킬 변수
    public float maxRightGauge = 100f;
    public float currentRightGauge;



    private void Awake()
    {
        TryGetComponent(out input);
        TryGetComponent(out playerRigid);
    }

    private void Start()
    {
        currentLeftGauge = maxLeftGauge;
        currentRightGauge = maxRightGauge;
        zDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    private void Update()
    {
        //점프 코드
        if (input.isJump)
        {
            Vector3 jumpDirection = transform.up * jumpPower * Time.deltaTime;

            playerRigid.AddForce(Vector3.up * jumpPower);
            playerRigid.linearVelocity = Vector3.zero;

        }

        // 왼쪽 스킬
        if (input.leftSkill && currentLeftGauge >= 0)
        {
            mouseScreenPosition = input.mousePosition;
            mouseScreenPosition.z = zDistance;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            currentPosition = transform.position;
            currentPosition.y = Mathf.Lerp(currentPosition.y, mouseWorldPosition.y, 0.3f);

            transform.position = currentPosition;
            playerRigid.linearVelocity = Vector3.zero;

            currentLeftGauge -= 25f * Time.deltaTime;
        }
        else if (currentLeftGauge <= 100)
        {
            currentLeftGauge += 25f * Time.deltaTime;
        }
    }
}
