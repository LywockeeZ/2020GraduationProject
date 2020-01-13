using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
    NavMeshAgent m_Agent;
    RaycastHit m_HitInfo = new RaycastHit();
    Animator m_Animator;
    Vector3 destination;
    bool isFirst = true;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                m_Agent.destination = m_HitInfo.point;
                destination = m_HitInfo.point;
            }
            m_Animator.SetBool("isWalking", true);
            isFirst = true;
        }

        if ((transform.position - destination).magnitude < 0.65f)
        {
            m_Animator.SetBool("isWalking", false);
            if (isFirst)
            {
                m_Animator.SetFloat("Blend", Random.Range(0f, 1f));
                isFirst = false;
            }
        }
    }
}
