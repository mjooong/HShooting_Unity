using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    MT_Zombi,
    MT_Missile,
    MT_Boss
}

public enum BossState
{
    BS_MOVE,        //���� �̵� ����
    BS_NORMAL_ATT,  //�⺻ ���� ����
    BS_FEVER_ATT    //�ǹ� Ÿ�� ���� ����
}

public class Monster_Ctrl : MonoBehaviour
{
    public MonType m_MonType = MonType.MT_Zombi;

    //--- ���� ü�� ����
    float m_MaxHP = 200.0f;
    float m_CurHP = 200.0f;
    public Image m_HpBar = null;
    //--- ���� ü�� ����

    float m_Speed = 4.0f;   //�̵��ӵ�
    Vector3 m_CurPos;       //��ġ ���� ����
    Vector3 m_SpawnPos;     //���� ��ġ

    float m_CacPosY = 0.0f; //���� �Լ��� �� ���� ���� ���� ����
    float m_RandY = 0.0f;   //������ ������ ����� ����
    float m_CycleY = 0.0f;  //������ ���� �ӵ� ����

    //--- �Ѿ� �߻� ���� ���� ����
    public GameObject m_ShootPos  = null;
    public GameObject m_BulletObj = null;
    float shoot_Time = 0.0f;        //�Ѿ� �߻� �ֱ� ���� ����
    float shoot_Delay = 1.5f;       //�Ѿ� �� Ÿ��
    float BulletMySpeed = 10.0f;    //�Ѿ� �̵� �ӵ�
    //--- �Ѿ� �߻� ���� ���� ����

    //--- �̻��� �ൿ���Ͽ� �ʿ��� ����
    HeroCtrl m_RefHero = null;      //���Ͱ� �����ϰ� �� ���ΰ� ��ü ����
    Vector3  m_DirVec;
    //--- �̻��� �ൿ���Ͽ� �ʿ��� ����

    //--- ������ �ൿ ���� ���� ����
    BossState m_BossState = BossState.BS_MOVE;  //���� �̵� ����
    int m_ShootCount = 0;
    //--- ������ �ൿ ���� ���� ����

    // Start is called before the first frame update
    void Start()
    {      
        m_RefHero = GameObject.FindObjectOfType<HeroCtrl>();

        m_SpawnPos = this.transform.position;
        m_RandY = Random.Range(0.2f, 2.6f);     //Sin �Լ��� ���� ����
        m_CycleY = Random.Range(1.0f, 2.2f);

        if(m_MonType == MonType.MT_Boss)
        {
            m_MaxHP = 3000.0f;  //�ִ� ü��ġ
            m_CurHP = m_MaxHP;  //���� ü��

            m_BossState = BossState.BS_MOVE; //���� �̵� ����
        }//if(m_MonType == MonType.MT_Boss)
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MonType == MonType.MT_Zombi)
            Zombi_AI_Update();
        else if (m_MonType == MonType.MT_Missile)
            Missile_AI_Update();
        else if (m_MonType == MonType.MT_Boss)
            Boss_AI_Update();

        if (this.transform.position.x < CameraResolution.m_ScreenMin.x - 2.0f)
            Destroy(gameObject);    //���� ȭ���� ����� ��� ����
    }

    void Zombi_AI_Update()
    {
        m_CurPos = transform.position;
        m_CurPos.x += (-1.0f * Time.deltaTime * m_Speed);
        m_CacPosY += Time.deltaTime * (m_Speed / m_CycleY);
        m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_RandY;
        transform.position = m_CurPos;

        if (Time.time - MonsterGenerator.m_StartTime < 30.0f) //������ ���۵ǰ� 30�� ���� �ں��� �Ѿ� �߻� �����ϰ�...
            return;

        //--- �Ѿ� �߻�
        if (m_BulletObj == null)
            return;

        shoot_Time += Time.deltaTime;
        if(shoot_Delay <= shoot_Time)
        {
            GameObject a_NewObj = Instantiate(m_BulletObj) as GameObject;
            Bullet_Ctrl a_BulletSc = a_NewObj.GetComponent<Bullet_Ctrl>();
            a_BulletSc.BulletSpawn(m_ShootPos.transform.position,
                                            Vector3.left, BulletMySpeed);
            shoot_Time = 0.0f;
        }
        //--- �Ѿ� �߻�
    }

    void Missile_AI_Update()
    {
        m_CurPos = transform.position;

        m_DirVec.x = -1.0f;
        m_DirVec.y = 0.0f;
        m_DirVec.z = 0.0f;

        if(m_RefHero != null)
        {
            Vector3 a_CacVec = m_RefHero.transform.position - transform.position;
            m_DirVec = a_CacVec;

            //�̻����� ���ΰ����� �Ÿ��� ������������ 3.5m �̻��̸� ������ ��ȭ����
            //�������θ� �̵���Ű���� �ǵ�
            if (a_CacVec.x < -3.5f)
                m_DirVec.y = 0.0f;

        } //if(m_RefHero != null)

        m_DirVec.Normalize();
        m_DirVec.x = -1.0f;     //������ ���� �������� �̵��ϰ� �ϱ� ���ؼ�...
        m_DirVec.z = 0.0f;

        m_CurPos += (m_DirVec * Time.deltaTime * m_Speed);
        transform.position = m_CurPos;
    }

    void Boss_AI_Update()
    {
        if(m_BossState == BossState.BS_MOVE) //���� �̵� ����
        {
            m_CurPos = this.transform.position;

            float a_ArrivePos = CameraResolution.m_ScreenMax.x - 1.9f; //������ġ
            if(a_ArrivePos < m_CurPos.x)
            {
                m_CurPos.x += (-1.0f * Time.deltaTime * m_Speed); //�����̵�
                if(m_CurPos.x <= a_ArrivePos)
                {
                    shoot_Time = 1.28f;
                    m_BossState = BossState.BS_FEVER_ATT;
                }
            }//if(a_ArrivePos < m_CurPos.x)

            this.transform.position = m_CurPos;
        }//if(m_BossState == BossState.BS_MOVE) //���� �̵� ����
        else if(m_BossState == BossState.BS_NORMAL_ATT) //�⺻ ����
        {
            shoot_Time -= Time.deltaTime;
            if(shoot_Time <= 0.0f)
            {
                Vector3 a_TargetV =
                    m_RefHero.transform.position - this.transform.position;
                a_TargetV.Normalize();
                GameObject a_NewObj = (GameObject)Instantiate(m_BulletObj);
                Bullet_Ctrl a_BulletSc = a_NewObj.GetComponent<Bullet_Ctrl>();
                a_BulletSc.BulletSpawn(m_ShootPos.transform.position,
                                    a_TargetV, BulletMySpeed);
                float a_CacAngle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                a_CacAngle += 180.0f;
                a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CacAngle);

                m_ShootCount++;
                if (m_ShootCount < 7)   //�⺻���� 7�������� ���� �ֱ�
                    shoot_Time = 0.7f;
                else
                {
                    m_ShootCount = 0;
                    shoot_Time = 2.0f;
                    m_BossState = BossState.BS_FEVER_ATT;
                }
            }//if(shoot_Time <= 0.0f)
        }//else if(m_BossState == BossState.BS_NORMAL_ATT) //�⺻ ����
        else if(m_BossState == BossState.BS_FEVER_ATT) //�ǹ� ����
        {
            shoot_Time -= Time.deltaTime;
            if(shoot_Time <= 0.0f)
            {
                float Radius = 100.0f;
                Vector3 a_TargetV = Vector3.zero;
                GameObject a_NewObj = null;
                Bullet_Ctrl a_BulletSc = null;
                float a_CacAngle = 0.0f;
                for(float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)
                {
                    a_TargetV.x = Radius * Mathf.Cos(Angle * Mathf.Deg2Rad);
                    a_TargetV.y = Radius * Mathf.Sin(Angle * Mathf.Deg2Rad);
                    a_TargetV.Normalize();
                    a_NewObj = (GameObject)Instantiate(m_BulletObj);
                    a_BulletSc = a_NewObj.GetComponent<Bullet_Ctrl>();
                    a_BulletSc.BulletSpawn(this.transform.position,
                                                a_TargetV, BulletMySpeed);
                    a_CacAngle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                    a_CacAngle += 180.0f;
                    a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CacAngle);
                }//for(float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)

                Sound_Mgr.Instance.PlayEffSound("explosion_large_01", 0.8f);

                m_ShootCount++;
                if (m_ShootCount < 3) //�ñر� 3�������� ���� �ֱ�
                    shoot_Time = 1.0f;
                else //�ñر⿡�� �⺻ �������� �Ѿ �� 1.5 �� ������ ��
                {
                    m_ShootCount = 0;
                    shoot_Time = 1.5f;
                    m_BossState = BossState.BS_NORMAL_ATT;
                }
            
            }//if(shoot_Time <= 0.0f)
        }//else if(m_BossState == BossState.BS_FEVER_ATT) //�ǹ� ����

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        //���� ���忡���� ���ΰ��� �� �Ѿ˸� �������� �߻��ϵ��� ó��
        if(coll.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(coll.gameObject); //���Ϳ� �浹�� �Ѿ� ����
        }
    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f)
            return;

        float a_CacDmg = a_Value;
        if (m_CurHP < a_Value)
            a_CacDmg = m_CurHP;

        Vector3 a_CacPos = transform.position;

        //Game_Mgr a_GameMgr = null;
        //GameObject a_GObj = GameObject.Find("Game_Mgr");
        //if (a_GObj != null)
        //    a_GameMgr = a_GObj.GetComponent<Game_Mgr>();
        //if (a_GameMgr != null)
        //    a_GameMgr.DamageText(-a_CacDmg, a_CacPos, Color.red);

        Game_Mgr.Inst.DamageText(-a_CacDmg, a_CacPos, Color.red);

        m_CurHP -= a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f)
        {  //���� ��� ó��

            //����
            //--- ���� ����
            Game_Mgr.Inst.AddScore();
            //--- ���� ����

            //--- ��Ʈ ����
            if (m_MonType == MonType.MT_Boss)
                Game_Mgr.Inst.SpawnHeart(transform.position);
            //--- ��Ʈ ����

            //--- ��� ����
            Game_Mgr.Inst.SpawnCoin(transform.position);
            //--- ��� ����

            //--- ����� ���Ͱ� ������ //������ ���� �ֱ� ����
            if(m_MonType == MonType.MT_Boss)
            {
                MonsterGenerator.m_SpBossTimer = Random.Range(25.0f, 30.0f);
            }
            //--- ����� ���Ͱ� ������ //������ ���� �ֱ� ����

            Destroy(gameObject);    //���� GameObject ����
        }
    }//public void TakeDamage(float a_Value)
}
