using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType   //��ų ��ǰ ����
{
    Skill_0 = 0,
    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
    Skill_5,
    SkCount
}

public class Skill_Info  //�� Item ����
{
    public string m_Name = "";                  //ĳ���� �̸�
    public SkillType m_SkType = SkillType.Skill_0; //ĳ���� Ÿ��
    public Vector2 m_IconSize = Vector2.one;  //�������� ���� ������, ���� ������
    public int m_Price = 100;   //������ �⺻ ���� 
    public int m_UpPrice = 50; //���׷��̵� ����, Ÿ�Կ� ����
    public int m_Level = 0;  //���� 0 ������, 1 ���� ~ 5 ���� (�������� ������ ��ų ��밡�� Ƚ��)
    //������ Lock, ����0 �̸� ���� ���� �ȵ� (���Ű� �Ϸ�Ǹ� ���� 1����)
    public int m_CurSkillCount = 0;   //����� �� �ִ� ��ų ī��Ʈ
    //public int m_MaxUsable = 1;     //����� �� �ִ� �ִ� ��ų ī��Ʈ�� Level�� ����.
    public string m_SkillExp = "";    //��ų ȿ�� ����
    public Sprite m_IconImg = null;   //ĳ���� �����ۿ� ���� �̹���

    public void SetType(SkillType a_SkType)
    {
        m_SkType = a_SkType;

        if (a_SkType == SkillType.Skill_0)
        {
            m_Name = "������";
            m_IconSize.x = 0.766f; //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;   //���θ� �������� ���� ���̱� ������ �׳� 1.0f = 103 �ȼ�

            m_Price = 100; //�⺻����
            m_UpPrice = 50; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "Hp 50% ȸ��";
            m_IconImg = Resources.Load("IconImg/m0011", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_1)
        {
            m_Name = "�����";
            m_IconSize.x = 0.81f;    //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 200; //�⺻����
            m_UpPrice = 100; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "�ñر�";
            m_IconImg = Resources.Load("IconImg/m0367", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_2)
        {
            m_Name = "����ȣ";
            m_IconSize.x = 0.946f;     //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 400; //�⺻����
            m_UpPrice = 200; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "��ȣ��";
            m_IconImg = Resources.Load("IconImg/m0054", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_3)
        {
            m_Name = "�߿���";
            m_IconSize.x = 0.93f;     //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 800; //�⺻����
            m_UpPrice = 400; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "����ź";
            m_IconImg = Resources.Load("IconImg/m0423", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_4)
        {
            m_Name = "�巡��";
            m_IconSize.x = 0.93f;     //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 1600; //�⺻����
            m_UpPrice = 800; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "����";
            m_IconImg = Resources.Load("IconImg/m0244", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_5)
        {
            m_Name = "��Ŀ��";
            m_IconSize.x = 0.93f;    //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 3000;   //�⺻����
            m_UpPrice = 1600; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "��ȯ�� ����";
            m_IconImg = Resources.Load("IconImg/m0172", typeof(Sprite)) as Sprite;
        }

    }//public void SetType(SkillType a_SkType)
}

public class GlobalValue
{
    //��ȯ�� ��ų ������ ������ ����Ʈ
    public static List<Skill_Info> m_SkDataList = new List<Skill_Info>();

    public static string g_NickName = "";   //������ ����
    public static int    g_BestScore = 0;   //�ְ�����
    public static int    g_UserGold = 0;    //���� ���� �Ӵ�

    public static void LoadGameData()
    {
        //-- ���� ������ �ε�
        if(m_SkDataList.Count <= 0)
        {
            Skill_Info a_SkItemNd;
            for(int ii = 0; ii < (int)SkillType.SkCount; ii++)
            {
                a_SkItemNd = new Skill_Info();
                a_SkItemNd.SetType((SkillType)ii);
                m_SkDataList.Add(a_SkItemNd);
            }
        }
        //-- ���� ������ �ε�

        g_NickName = PlayerPrefs.GetString("NickName", "SBS����");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold  = PlayerPrefs.GetInt("UserGold", 0);

        //-- ������ ���ÿ� ����� ���� ���� �ε�
        string a_KeyBuff = "";
        for(int ii = 0; ii < (int)SkillType.SkCount; ii++)
        {
            if (m_SkDataList.Count <= ii)
                continue;

            a_KeyBuff = string.Format("Skill_Item_{0}", ii);
            m_SkDataList[ii].m_Level = PlayerPrefs.GetInt(a_KeyBuff, 0);

            //m_SkDataList[ii].m_Level = 3; //�׽�Ʈ�� ���� ������ 3���� ä��� ������
        }
        //-- ������ ���ÿ� ����� ���� ���� �ε�

    }//public static void LoadGameData()
}
