using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.right; //날아가야 할 방향 벡터
    float m_MoveSpeed = 15.0f;        //이동속도

    //-- 유도탄 변수
    [HideInInspector] public bool IsHoming = false; //유도탄 OnOff 변수
    [HideInInspector] public bool IsTarget = false;
    //한번이라도 타겟이 잡힌 적이 있는지? 확인 하는 변수
    [HideInInspector] public GameObject Target_Obj = null; //타겟 참조 변수
    Vector3 m_DesiredDir;   //타겟을 향하는 방향 변수
    //-- 유도탄 변수

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHoming == true) //유도탄인 경우
        {
            if (Target_Obj == null && IsTarget == false)//추적해야 할 타겟이 없으면...
                FindEnemy();    //타겟을 잡아주는 함수

            if (Target_Obj != null)
                BulletHoming();   //타겟을 향해 추적 이동하는 행동 패턴 함수
            else //타겟이 사망했다면...
                transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
        }
        else  //일반 총알인 경우
        {
            transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
        }

        if(CameraResolution.m_ScreenMax.x + 1.0f < transform.position.x ||
           transform.position.x < CameraResolution.m_ScreenMin.x - 1.0f ||
           CameraResolution.m_ScreenMax.y + 1.0f < transform.position.y ||
           transform.position.y < CameraResolution.m_ScreenMin.y - 1.0f)
        { //총알이 화면을 벗어나면... 즉시 제거

            Destroy(gameObject);
        }
    }//void Update()

    public void BulletSpawn(Vector3 a_StPos, Vector3 a_DirVec, 
                                    float a_MySpeed = 15.0f, float att = 20.0f)
    {
        m_DirVec = a_DirVec;
        transform.position = new Vector3(a_StPos.x, a_StPos.y, 0.0f);
        m_MoveSpeed = a_MySpeed;
    }

    void FindEnemy() //타겟을 찾아주는 함수
    {
        GameObject[] a_EnemyList = GameObject.FindGameObjectsWithTag("Monster");

        if (a_EnemyList.Length <= 0) // 등장해 있는 몬스터가 하나도 없으면
            return; //추적할 대상을 찾지 못한다.

        GameObject a_Find_Mon = null;
        float a_CacDist = 0.0f;
        Vector3 a_CacVec = Vector3.zero;
        for(int ii = 0; ii < a_EnemyList.Length; ii++)
        {
            a_CacVec = a_EnemyList[ii].transform.position - transform.position;
            a_CacVec.z = 0.0f;
            a_CacDist = a_CacVec.magnitude;

            if (4.0f < a_CacDist) //총알로부터 4m 반경안에 있는 몬스터만...
                continue;

            a_Find_Mon = a_EnemyList[ii].gameObject;
            break;
        }//for(int ii = 0; ii < a_EnemyList.Length; ii++)

        Target_Obj = a_Find_Mon;
        if (Target_Obj != null)
            IsTarget = true;        //총알이 스폰된 후 타겟을 한번 잡으면
        //타겟이 죽어도 다른 타겟을 잡지 않고 총알이 가던 길 가게 하기 위해서...
    }

    void BulletHoming() //타겟을 향해 추적 이동하는 행동 패턴 함수
    {
        m_DesiredDir = Target_Obj.transform.position - transform.position;
        m_DesiredDir.z = 0.0f;
        m_DesiredDir.Normalize();

        //적을 향해 회전 이동하는 코드
        float angle = Mathf.Atan2(m_DesiredDir.y, m_DesiredDir.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = angleAxis;
        m_DirVec = transform.right;
        transform.Translate(Vector3.right * m_MoveSpeed * Time.deltaTime, Space.Self);
        //transform.Translate(transform.right * m_MoveSpeed * Time.deltaTime, Space.World);
        //적을 향해 회전 이동하는 코드
    }
}
