using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 플레이어
    private PlayerInput input;
    private Rigidbody playerRigid;
    private Collider collider;

    // 오디오 처리
    private AudioSource audio;
    [SerializeField]
    private AudioClip dieClip;
    [SerializeField]
    private AudioClip jumpClip;

    // 스킬 이펙트
    [SerializeField]
    private GameObject leftSkillParticle;
    [SerializeField]
    private GameObject rightSkillParticle;
    [SerializeField]
    private GameObject jumpParticle;
    [SerializeField]
    private Renderer playerRenderer;
    [SerializeField]
    private Material[] rightSkillMaterials;

    private Material[] originalMaterials;
    private bool rightSkillEffectOn;

    // 점프
    [SerializeField]
    private float jumpPower = 100f;

    // 사망 처리
    [SerializeField]
    private string obstacleTag = "Obstacle";
    [SerializeField]
    private Vector3 deathKnockback = new Vector3(-3f, 1f, 0f);

    private bool isDead;
    private bool jumpConsumed;


    // 왼쪽 클릭 스킬 게이지
    public float maxLeftGauge = 100f;
    public float currentLeftGauge;
    
    // 마우스 입력
    public Vector3 mouseScreenPosition;
    public Vector3 mouseWorldPosition;
    public Vector3 currentPosition;
    public float zDistance;

    // 오른쪽 클릭 스킬 게이지
    public float maxRightGauge = 100f;
    public float currentRightGauge;
   
   
    private void Awake()
    {
        TryGetComponent(out input);
        TryGetComponent(out playerRigid);
        TryGetComponent(out collider);
        TryGetComponent(out audio);

        if (leftSkillParticle != null)
        {
            leftSkillParticle.SetActive(false);
        }
        if (rightSkillParticle != null)
        {
            rightSkillParticle.SetActive(false);
        }
        if (jumpParticle != null)
        {
            jumpParticle.SetActive(false);
        }
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

        // 스페이스바를 눌렀다 뗄 때까지 점프는 한 번만. 계속 눌러도 다시 안 올라간다.
        if (input.isJump && !jumpConsumed)
        {
            jumpConsumed = true;

            playerRigid.linearVelocity = Vector3.zero;
            playerRigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            audio.PlayOneShot(jumpClip);

            // 스킬(좌/우) 쓰는 중엔 점프 파티클은 생략한다.
            bool usingSkill = input.rightSkill || (input.leftSkill && input.canLeftSkill);
            if (!usingSkill)
            {
                RestartParticle(jumpParticle);
            }
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

        UpdateEffects();
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

        if (leftSkillParticle != null)
        {
            leftSkillParticle.SetActive(false);
        }
        if (rightSkillParticle != null)
        {
            rightSkillParticle.SetActive(false);
        }

        ApplyRightSkillMaterial(false);
    }

    private void UpdateEffects()
    {
        if (leftSkillParticle != null)
        {
            // 왼쪽 스킬을 실제로 쓰는 동안만 켠다.
            leftSkillParticle.SetActive(input.leftSkill && input.canLeftSkill);
        }

        if (rightSkillParticle != null)
        {
            // 오른쪽 스킬을 실제로 쓰는 동안만 켠다.
            rightSkillParticle.SetActive(input.rightSkill);
        }

        // 오른쪽 스킬 쓰는 동안 머터리얼 교체, 끝나면 원래대로.
        ApplyRightSkillMaterial(input.rightSkill);
    }

    // 파티클 오브젝트를 껐다 켜서 처음부터 다시 재생시킨다.
    private void RestartParticle(GameObject particle)
    {
        if (particle == null)
        {
            return;
        }

        particle.SetActive(false);
        particle.SetActive(true);
    }

    private void ApplyRightSkillMaterial(bool useSkillMaterial)
    {
        if (playerRenderer == null || rightSkillMaterials == null || rightSkillMaterials.Length == 0)
        {
            return;
        }

        if (useSkillMaterial && !rightSkillEffectOn)
        {
            rightSkillEffectOn = true;
            originalMaterials = playerRenderer.sharedMaterials;
            playerRenderer.sharedMaterials = rightSkillMaterials;
        }
        else if (!useSkillMaterial && rightSkillEffectOn)
        {
            rightSkillEffectOn = false;
            playerRenderer.sharedMaterials = originalMaterials;
        }
    }


}
