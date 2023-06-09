using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkSmallNode : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;

    [HideInInspector] public int m_CurSkCount = 0;
    public Image m_RootBtnImg;   //Root�� ��ư���� ��������Ƿ� ��ư ���̹��� ���ٿ� ����
    public Text  m_ShortcutText; //����Ű �ؽ�Ʈ
    public Text  m_LvText;       //���� ��ų�� ������ ǥ���� �� �ؽ�Ʈ
    public Text  m_SkCountText;  //��ų ī��Ʈ �ؽ�Ʈ
    public Image m_SkIconImg;    //ĳ���� ������ �̹���

    // Start is called before the first frame update
    void Start()
    {
        Button a_BtnCom = this.GetComponent<Button>();
        if (a_BtnCom != null)
            a_BtnCom.onClick.AddListener(() =>
            { //�� ��ư�� ������ ��

                if (GlobalValue.m_SkDataList[(int)m_SkType].m_CurSkillCount <= 0)
                    return; //��ų �������� ����� �� ����

                HeroCtrl a_Hero = GameObject.FindObjectOfType<HeroCtrl>();
                if (a_Hero != null)
                    a_Hero.UseSkill(m_SkType);
                Refresh_UI(m_SkType);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitState(Skill_Info a_SkInfo)
    {
        m_SkType = a_SkInfo.m_SkType;
        m_ShortcutText.text = ((int)m_SkType + 1).ToString();
        m_SkIconImg.sprite = a_SkInfo.m_IconImg;
        m_SkIconImg.GetComponent<RectTransform>().sizeDelta =
                new Vector2(a_SkInfo.m_IconSize.x * 103.0f, 103.0f);
        //��������Ʈ ������ �̹����� ������ �°� ����
        m_CurSkCount = a_SkInfo.m_Level;
        m_LvText.text = "Lv " + a_SkInfo.m_Level.ToString();
        m_SkCountText.text = m_CurSkCount.ToString(); 
                                //+ " / " + a_SkInfo.m_Level.ToString();

    } //public void InitState(Skill_Info a_SkInfo)

    public void Refresh_UI(SkillType a_SkType)
    {
        if (m_SkType != a_SkType)
            return;

        m_CurSkCount = GlobalValue.m_SkDataList[(int)m_SkType].m_CurSkillCount;
        if (m_SkCountText != null)
            m_SkCountText.text = m_CurSkCount.ToString(); 
                //+ " / " + GlobalValue.m_SkDataList[(int)m_SkType].m_Level.ToString();

        if(m_CurSkCount <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
