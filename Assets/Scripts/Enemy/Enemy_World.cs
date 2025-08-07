using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_World : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private MovementType movementType;
    [SerializeField] private State curState;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private float chaseRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isChase;
    [SerializeField] private bool isOver;
    private float animValue;
    private enum MovementType { None, Chase }
    private enum State { Idle, Chase, Return, Find }


    [Header("=== Target ===")]
    [SerializeField] private GameObject chaseTarget;
    [SerializeField] private GameObject chaseBody;
    [SerializeField] private Vector3 targetDir;
    [SerializeField] private float curMoveRange;


    [Header("=== Comopnent ===")]
    [SerializeField] private SphereCollider searchCollider;
    [SerializeField] private NavMeshAgent nav;
    [SerializeField] private Animator anim;
    private Coroutine chaseCoroutine;
    private Coroutine animCoroutine;


    private void Awake()
    {
        nav.speed = moveSpeed;
        searchCollider.radius = chaseRange;
    }


    #region 애니메이션
    private void MoveAnim(bool isOn)
    {
        if(animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }
        animCoroutine = StartCoroutine(MoveAnimCall(isOn));
    }

    private IEnumerator MoveAnimCall(bool isOn)
    {
        if(isOn)
        {
            while (animValue < 1)
            {
                animValue += Time.deltaTime * 2f;
                anim.SetFloat("Movement", animValue);
                yield return null;
            }
            animValue = 1;
            anim.SetFloat("Movement", animValue);
        }
        else
        {
            while (animValue > 0)
            {
                animValue -= Time.deltaTime * 2f;
                anim.SetFloat("Movement", animValue);
                yield return null;
            }
            animValue = 0;
            anim.SetFloat("Movement", animValue);
        }
    }

    #endregion


    #region 추적 & 복귀
    public void Find()
    {
        if(isOver)
        {
            return;
        }

        if(chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
        }

        curState = State.Find;
        isOver = true;
        Destroy(gameObject);
    }


    public void Chase(GameObject target)
    {
        if (isChase && isOver)
        {
            return;
        }

        chaseTarget = target;
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
        }
        chaseCoroutine = StartCoroutine(ChaseCall());
    }

    private IEnumerator ChaseCall()
    {
        curState = State.Chase;
        isChase = true;

        // 애니메이션
        MoveAnim(true);

        nav.enabled = true;
        while (isChase)
        {
            nav.SetDestination(chaseTarget.transform.position);
            yield return null;
        }

        curState = State.Idle;
        isChase = false;
        nav.enabled = false;
    }


    public void Return()
    {
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
        }
        chaseCoroutine = StartCoroutine(ReturnCall());
    }

    private IEnumerator ReturnCall()
    {
        curState = State.Return;
        nav.enabled = true;
        isChase = false;

        Vector3 dir = chaseBody.transform.position - transform.position;
        float range = dir.magnitude;
        while (range >= 0.1f)
        {
            range = (nav.gameObject.transform.position - transform.position).magnitude;
            nav.SetDestination(transform.position);
            yield return null;
        }

        curState = State.Idle;
        nav.enabled = false;

        // 애니메이션
        MoveAnim(false);
    }
    #endregion
}
