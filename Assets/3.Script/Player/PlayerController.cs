using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //������Ʈ
    private PlayerInput input;
    private Rigidbody playerRigid;
    private Collider collider;

    // 오디오 처리
    private AudioSource audio;
    [SerializeField]
    private AudioClip dieClip;
    [SerializeField]
    private AudioClip jumpClip;

    //���� ����
    [SerializeField]
    private float jumpPower = 100f;

    // 사망 처리
    [SerializeField]
    private string obstacleTag = "Obstacle";
    [SerializeField]
    private Vector3 deathKnockback = new Vector3(-3f, 1f, 0f);

    private bool isDead;
    private bool jumpConsumed;

    //public �� ������Ƽ�� �Ұ���

    //���� ��ų ����
    public float maxLeftGauge = 100f;
    public float currentLeftGauge;
    
    //���콺 ��ǥ�� ��� ����
    public Vector3 mouseScreenPosition;
    public Vector3 mouseWorldPosition;
    public Vector3 currentPosition;
    public float zDistance;

    //������ ��ų ����
    public float maxRightGauge = 100f;
    public float currentRightGauge;
   
   
    private void Awake()
    {
        TryGetComponent(out input);
        TryGetComponent(out playerRigid);
        TryGetComponent(out collider);
        TryGetComponent(out audio);
        
    }

    private void Start()
    {
        currentLeftGauge = maxLeftGauge;
        currentRightGauge = maxRightGauge;
        zDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        //���� �ڵ�
        // 스페이스바를 눌렀다 뗄 때까지 점프는 한 번만. 계속 눌러도 다시 안 올라간다.
        if (input.isJump && !jumpConsumed)
        {
            jumpConsumed = true;

            playerRigid.linearVelocity = Vector3.zero;
            playerRigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            audio.PlayOneShot(jumpClip);
        }
        else if (!input.isJump)
        {
            jumpConsumed = false;
        }

        // ���� ��ų
        if (input.leftSkill && currentLeftGauge >= 0 && input.canLeftSkill)
        {
            if (currentLeftGauge <= 0)
            {
                input.leftSkill = false;
            }
            mouseScreenPosition = input.mousePosition;
            mouseScreenPosition.z = zDistance;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            currentPosition = transform.position;
            currentPosition.y = Mathf.Lerp(currentPosition.y, mouseWorldPosition.y, 0.3f);

            transform.position = currentPosition;
            playerRigid.linearVelocity = Vector3.zero;

            currentLeftGauge -= 25f * Time.deltaTime;
        }
        else if (!input.leftSkill && currentLeftGauge <= 100)
        {
            currentLeftGauge += 10f * Time.deltaTime;
        }
        
        //������ ��ų
        if (input.rightSkill && currentRightGauge >=0)
        {
            if (currentRightGauge <= 0)
            {
                input.rightSkill = false;
            }
            collider.enabled = false;
            currentRightGauge -= 25f * Time.deltaTime;
        }
        else if (!input.rightSkill && currentRightGauge <= 100)
        {
            collider.enabled = true;
            currentRightGauge += 10f * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag("Obstacle"))
        {
            Die();
        }
    }


    // 중력을 끄고 뒤로 살짝 밀어낸 뒤, 게임오버 처리는 GameManager 에 맡긴다.
    private void Die()
    {
        isDead = true;

        playerRigid.useGravity = false;
        playerRigid.linearVelocity = Vector3.zero;
        playerRigid.angularVelocity = Vector3.zero;
        playerRigid.AddForce(deathKnockback, ForceMode.Impulse);
        audio.PlayOneShot(dieClip);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandlePlayerDeath();
        }
    }


}
