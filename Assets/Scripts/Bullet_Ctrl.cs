using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.right; //���ư��� �� ���� ����
    float m_MoveSpeed = 15.0f;        //�̵��ӵ�

    //-- ����ź ����
    [HideInInspector] public bool IsHoming = false; //����ź OnOff ����
    [HideInInspector] public bool IsTarget = false;
    //�ѹ��̶� Ÿ���� ���� ���� �ִ���? Ȯ�� �ϴ� ����
    [HideInInspector] public GameObject Target_Obj = null; //Ÿ�� ���� ����
    Vector3 m_DesiredDir;   //Ÿ���� ���ϴ� ���� ����
    //-- ����ź ����

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHoming == true) //����ź�� ���
        {
            if (Target_Obj == null && IsTarget == false)//�����ؾ� �� Ÿ���� ������...
                FindEnemy();    //Ÿ���� ����ִ� �Լ�

            if (Target_Obj != null)
                BulletHoming();   //Ÿ���� ���� ���� �̵��ϴ� �ൿ ���� �Լ�
            else //Ÿ���� ����ߴٸ�...
                transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
        }
        else  //�Ϲ� �Ѿ��� ���
        {
            transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
        }

        if(CameraResolution.m_ScreenMax.x + 1.0f < transform.position.x ||
           transform.position.x < CameraResolution.m_ScreenMin.x - 1.0f ||
           CameraResolution.m_ScreenMax.y + 1.0f < transform.position.y ||
           transform.position.y < CameraResolution.m_ScreenMin.y - 1.0f)
        { //�Ѿ��� ȭ���� �����... ��� ����

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

    void FindEnemy() //Ÿ���� ã���ִ� �Լ�
    {
        GameObject[] a_EnemyList = GameObject.FindGameObjectsWithTag("Monster");

        if (a_EnemyList.Length <= 0) // ������ �ִ� ���Ͱ� �ϳ��� ������
            return; //������ ����� ã�� ���Ѵ�.

        GameObject a_Find_Mon = null;
        float a_CacDist = 0.0f;
        Vector3 a_CacVec = Vector3.zero;
        for(int ii = 0; ii < a_EnemyList.Length; ii++)
        {
            a_CacVec = a_EnemyList[ii].transform.position - transform.position;
            a_CacVec.z = 0.0f;
            a_CacDist = a_CacVec.magnitude;

            if (4.0f < a_CacDist) //�Ѿ˷κ��� 4m �ݰ�ȿ� �ִ� ���͸�...
                continue;

            a_Find_Mon = a_EnemyList[ii].gameObject;
            break;
        }//for(int ii = 0; ii < a_EnemyList.Length; ii++)

        Target_Obj = a_Find_Mon;
        if (Target_Obj != null)
            IsTarget = true;        //�Ѿ��� ������ �� Ÿ���� �ѹ� ������
        //Ÿ���� �׾ �ٸ� Ÿ���� ���� �ʰ� �Ѿ��� ���� �� ���� �ϱ� ���ؼ�...
    }

    void BulletHoming() //Ÿ���� ���� ���� �̵��ϴ� �ൿ ���� �Լ�
    {
        m_DesiredDir = Target_Obj.transform.position - transform.position;
        m_DesiredDir.z = 0.0f;
        m_DesiredDir.Normalize();

        //���� ���� ȸ�� �̵��ϴ� �ڵ�
        float angle = Mathf.Atan2(m_DesiredDir.y, m_DesiredDir.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = angleAxis;
        m_DirVec = transform.right;
        transform.Translate(Vector3.right * m_MoveSpeed * Time.deltaTime, Space.Self);
        //transform.Translate(transform.right * m_MoveSpeed * Time.deltaTime, Space.World);
        //���� ���� ȸ�� �̵��ϴ� �ڵ�
    }
}
