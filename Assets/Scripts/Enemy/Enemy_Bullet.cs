using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class Enemy_Bullet : MonoBehaviour
{
    [Header("===bullt setting===")]
    [SerializeField] private ShotType shotType;
    [SerializeField] private DamageType damageType;
    [SerializeField] private int damage;
    [SerializeField] private bool isCritical;
    [SerializeField] private float moveSpeed;
    public enum ShotType { Normal, Sine }
    public enum DamageType { Physical, Magical }


    [Header("=== Component ===")]
    private Collider bulletCollider;
    private GameObject target;

    private Coroutine moveCoroutine;


    [Header("=== Prefabs ===")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private GameObject fireVFX;


    [Header("=== Test ===")]
    Vector3[] m_points = new Vector3[4];

    private float m_timerMax = 0;
    private float m_speed;
    private float m_timerCurrent;


    private void Awake()
    {
        bulletCollider = GetComponent<Collider>();
    }


    /// <summary>
    /// 불릿 셋팅 기능
    /// </summary>
    /// <param name="type">날라가는 타입 ( 직선형 / 곡선형 )</param>
    /// <param name="damageType">물리, 마법 데미지</param>
    /// <param name="isCritical">크리티컬 여부</param>
    /// <param name="damage">데미지</param>
    /// <param name="speed">불릿 속도</param>
    public void Bullet_Setting(ShotType type, DamageType damageType, bool isCritical, int damage, float speed, GameObject target)
    {
        shotType = type;
        this.damageType = damageType;
        this.damage = damage;
        this.isCritical = isCritical;
        moveSpeed = speed;
        this.target = target;


        Init(transform, target.transform, speed, 6, 3);

        switch (type)
        {
            case ShotType.Normal:
                moveCoroutine = StartCoroutine(NormalShot());
                break;

            case ShotType.Sine:
                moveCoroutine = StartCoroutine(SineShot());
                break;
        }
    }

    private IEnumerator NormalShot()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = target.transform.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
    }


    #region 테스트 필요함! - 곡선 발사
    private IEnumerator SineShot()
    {
        // 경과 시간 계산.
        m_timerCurrent = 0;
        while (m_timerCurrent < 1)
        {
            m_timerCurrent += Time.deltaTime;

            // 베지어 곡선으로 X,Y,Z 좌표 얻기.
            Vector3 nextPos = new Vector3(
                CubicBezierCurve(m_points[0].x, m_points[1].x, m_points[2].x, m_points[3].x),
                CubicBezierCurve(m_points[0].y, m_points[1].y, m_points[2].y, m_points[3].y),
                CubicBezierCurve(m_points[0].z, m_points[1].z, m_points[2].z, m_points[3].z)
            );

            transform.position = nextPos;

            // 이동 방향을 바라보게 설정
            transform.LookAt(nextPos + (nextPos - transform.position).normalized);

            yield return null;
        }

        // 피격 이펙트
        HitVFX();
    }

    public void Init(Transform _startTr, Transform _endTr, float _speed, float _newPointDistanceFromStartTr, float _newPointDistanceFromEndTr)
    {
        m_speed = _speed;

        // 끝에 도착할 시간을 랜덤으로 줌.
        m_timerMax = Random.Range(0.8f, 1.0f);

        // 시작 지점.
        m_points[0] = _startTr.position;

        // 시작 지점을 기준으로 랜덤 포인트 지정.
        m_points[1] = _startTr.position +
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, 1.0f) * _startTr.right) + // X (좌, 우 전체)
            (_newPointDistanceFromStartTr * Random.Range(-0.15f, 1.0f) * _startTr.up) + // Y (아래쪽 조금, 위쪽 전체)
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, -0.8f) * _startTr.forward); // Z (뒤 쪽만)

        // 도착 지점을 기준으로 랜덤 포인트 지정.
        m_points[2] = _endTr.position +
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.right) + // X (좌, 우 전체)
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.up) + // Y (위, 아래 전체)
            (_newPointDistanceFromEndTr * Random.Range(0.8f, 1.0f) * _endTr.forward); // Z (앞 쪽만)

        // 도착 지점.
        m_points[3] = _endTr.position;

        transform.position = _startTr.position;
    }

    private float CubicBezierCurve(float a, float b, float c, float d)
    {
        // (0~1)의 값에 따라 베지어 곡선 값을 구하기 때문에, 비율에 따른 시간을 구했다.
        float t = m_timerCurrent / m_timerMax; // (현재 경과 시간 / 최대 시간)

        // 이해한대로 편하게 쓰면.
        float ab = Mathf.Lerp(a, b, t);
        float bc = Mathf.Lerp(b, c, t);
        float cd = Mathf.Lerp(c, d, t);

        float abbc = Mathf.Lerp(ab, bc, t);
        float bccd = Mathf.Lerp(bc, cd, t);

        return Mathf.Lerp(abbc, bccd, t);
    }
#endregion


    private void HitVFX()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        bulletCollider.enabled = false;

        Player_Manager.instnace.Take_Damage(damage, damageType == DamageType.Physical ? Player_Manager.DamageType.Physical : Player_Manager.DamageType.Magical, isCritical);

        Instantiate(hitVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            HitVFX();
        }
    }
}
