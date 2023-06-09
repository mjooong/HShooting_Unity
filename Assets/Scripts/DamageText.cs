using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    float m_EffTime = 0.0f;         //연출 시간 계산용 변수
    public Text m_ThisText = null;  //Text UI 접근용 변수

    float MvVelocity = 1.1f / 1.05f;    //1.05초 동안에 1.1m 간다면... 속도
    float ApVelocity = 1.0f / (1.0f - 0.4f);
    //alpha 0.4(0.0f)초보터 연출 1.0초(1.0f)까지

    Vector3 m_CurPos;       //위치 계산용 변수
    Color   m_Color;        //컬러 계산용 변수

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        m_EffTime += Time.deltaTime;

        if(m_EffTime < 1.05f)
        {
            m_CurPos = m_ThisText.transform.position;
            m_CurPos.y += Time.deltaTime * MvVelocity;
            m_ThisText.transform.position = m_CurPos;
        }

        if(0.4f < m_EffTime)
        {
            m_Color = m_ThisText.color;
            m_Color.a -= (Time.deltaTime * ApVelocity);
            if (m_Color.a < 0.0f)
                m_Color.a = 0.0f;
            m_ThisText.color = m_Color;
        }

        if(1.05f < m_EffTime)
        {
            Destroy(this.gameObject);
        }

    } //void Update()

    public void InitDamage(float a_Damage, Color a_Color)
    {
        if (m_ThisText == null)
            m_ThisText = this.GetComponentInChildren<Text>();

        if(a_Damage <= 0.0f)
        {
            int a_Dmg = (int)Mathf.Abs(a_Damage);   //절대값 함수
            m_ThisText.text = "- " + a_Dmg;
        }
        else
        {
            m_ThisText.text = "+ " + (int)a_Damage;
        }

        a_Color.a = 1.0f;
        m_ThisText.color = a_Color;
    }
}
