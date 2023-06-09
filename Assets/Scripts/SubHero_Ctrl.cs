using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubHero_Ctrl : MonoBehaviour
{
    HeroCtrl m_RefHero = null; //주인공 객체의 참조 변수
    float angle = 0.0f;   //회전 각도 계산용 변수(주인공을 중심으로 주변을 돌게 하기 위함)
    float radius = 1.0f;  //회전 반경
    float speed = 100.0f; //회전 속도

    Vector3 parent_Pos = Vector3.zero;  //주인공의 좌표를 받아올 변수

    float m_LifeTime = 0.0f;    //생명 타이머

    //--- 공격 관련 변수
    public GameObject m_BulletObj = null;
    float m_AttSpeed  = 0.5f;   //공격 속도(공속)
    float m_ShootCool = 0.0f;   //총알 발사 주기 계산용 변수

    GameObject a_CloneObj  = null;
    Bullet_Ctrl a_BulletSc = null;

    bool IsHoming = false;
    bool IsDuble  = false;
    //--- 공격 관련 변수

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_LifeTime -= Time.deltaTime;
        if(m_LifeTime <= 0.0f)
        {
            Destroy(this.gameObject);
            return;
        }

        angle += Time.deltaTime * speed;
        if (360.0f < angle)
            angle -= 360.0f;

        if (m_RefHero == null)
            return;

        parent_Pos = m_RefHero.transform.position;
        transform.position = parent_Pos +
                        new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                                    radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0.0f);
        FireUpdate();

    }//void Update()

    public void SubHeroSpawn(HeroCtrl a_Paren, float a_Angle, float a_LifeTime)
    {
        m_RefHero   = a_Paren;
        angle       = a_Angle;
        m_LifeTime  = a_LifeTime;
    }

    void FireUpdate()
    {
        if(m_RefHero != null)
        {
            IsHoming = m_RefHero.IsHoming;  //유도탄 상태 가져오기
            if (0.0f < m_RefHero.m_Double_OnTime) //더블샷 상태 가져오기
                IsDuble = true;
            else
                IsDuble = false;
        }//if(m_RefHero != null)

        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;

        if(m_ShootCool <= 0.0f)
        {
            if(IsDuble == true) //더블샷
            {
                Vector3 a_Pos;
                for(int ii = 0; ii < 2; ii++)
                {
                    a_CloneObj = (GameObject)Instantiate(m_BulletObj);
                    a_Pos = transform.position;
                    a_Pos.y += 0.2f - (ii * 0.4f);
                    a_CloneObj.transform.position = a_Pos;
                    a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                    if (a_BulletSc != null)
                        a_BulletSc.IsHoming = IsHoming;
                }
            }
            else  //일반총알
            {
                a_CloneObj = (GameObject)Instantiate(m_BulletObj);
                a_CloneObj.transform.position = transform.position;
                a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                if (a_BulletSc != null)
                    a_BulletSc.IsHoming = IsHoming;
            }

            m_ShootCool = m_AttSpeed; //공격속도 0.5초
        }//if(m_ShootCool <= 0.0f)
    }//void FireUpdate()
}
