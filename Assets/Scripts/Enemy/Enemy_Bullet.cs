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
    /// �Ҹ� ���� ���
    /// </summary>
    /// <param name="type">���󰡴� Ÿ�� ( ������ / ��� )</param>
    /// <param name="damageType">����, ���� ������</param>
    /// <param name="isCritical">ũ��Ƽ�� ����</param>
    /// <param name="damage">������</param>
    /// <param name="speed">�Ҹ� �ӵ�</param>
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


    #region �׽�Ʈ �ʿ���! - � �߻�
    private IEnumerator SineShot()
    {
        // ��� �ð� ���.
        m_timerCurrent = 0;
        while (m_timerCurrent < 1)
        {
            m_timerCurrent += Time.deltaTime;

            // ������ ����� X,Y,Z ��ǥ ���.
            Vector3 nextPos = new Vector3(
                CubicBezierCurve(m_points[0].x, m_points[1].x, m_points[2].x, m_points[3].x),
                CubicBezierCurve(m_points[0].y, m_points[1].y, m_points[2].y, m_points[3].y),
                CubicBezierCurve(m_points[0].z, m_points[1].z, m_points[2].z, m_points[3].z)
            );

            transform.position = nextPos;

            // �̵� ������ �ٶ󺸰� ����
            transform.LookAt(nextPos + (nextPos - transform.position).normalized);

            yield return null;
        }

        // �ǰ� ����Ʈ
        HitVFX();
    }

    public void Init(Transform _startTr, Transform _endTr, float _speed, float _newPointDistanceFromStartTr, float _newPointDistanceFromEndTr)
    {
        m_speed = _speed;

        // ���� ������ �ð��� �������� ��.
        m_timerMax = Random.Range(0.8f, 1.0f);

        // ���� ����.
        m_points[0] = _startTr.position;

        // ���� ������ �������� ���� ����Ʈ ����.
        m_points[1] = _startTr.position +
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, 1.0f) * _startTr.right) + // X (��, �� ��ü)
            (_newPointDistanceFromStartTr * Random.Range(-0.15f, 1.0f) * _startTr.up) + // Y (�Ʒ��� ����, ���� ��ü)
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, -0.8f) * _startTr.forward); // Z (�� �ʸ�)

        // ���� ������ �������� ���� ����Ʈ ����.
        m_points[2] = _endTr.position +
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.right) + // X (��, �� ��ü)
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.up) + // Y (��, �Ʒ� ��ü)
            (_newPointDistanceFromEndTr * Random.Range(0.8f, 1.0f) * _endTr.forward); // Z (�� �ʸ�)

        // ���� ����.
        m_points[3] = _endTr.position;

        transform.position = _startTr.position;
    }

    private float CubicBezierCurve(float a, float b, float c, float d)
    {
        // (0~1)�� ���� ���� ������ � ���� ���ϱ� ������, ������ ���� �ð��� ���ߴ�.
        float t = m_timerCurrent / m_timerMax; // (���� ��� �ð� / �ִ� �ð�)

        // �����Ѵ�� ���ϰ� ����.
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
