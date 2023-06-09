using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public delegate void GFG_Response();    // ��������Ʈ ������(Ÿ��)�� �ϳ� ����
    public GFG_Response DltMethod = null;   // ��������Ʈ ���� ����(���� ����)

    public Button m_Ok_Btn = null;
    public Button m_Close_Btn = null;

    public InputField IDInputField = null;

    public Toggle m_Sound_Toggle = null;
    public Slider m_Sound_Slider = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Ok_Btn != null)
            m_Ok_Btn.onClick.AddListener(OkbtnClick);

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(CloseBtnClick);

        // üũ ���°� ����Ǿ��� �� ȣ��Ǵ� �Լ��� ����ϴ� �ڵ�
        if (m_Sound_Toggle != null)
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);

        // �����̵� ���°� ���� �Ǿ��� �� ȣ��Ǵ� �Լ� ����ϴ� �ڵ�
        if (m_Sound_Slider != null)
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);

        // --- �г��� �ε� �� UI ��Ʈ�ѿ� ����
        if(IDInputField != null)
            IDInputField.text = PlayerPrefs.GetString("NickName", "SBS����");
        // --- �г��� �ε� �� UI ��Ʈ�ѿ� ����

        // --- üũ ����, �����̵� ���� �ε� �� UI ��Ʈ�ѿ� ����
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if(m_Sound_Toggle != null)
        {
            if (a_SoundOnOff == 1)
                m_Sound_Toggle.isOn = true;
            else
                m_Sound_Toggle.isOn = false;
        }

        if (m_Sound_Slider != null)
            m_Sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

        // --- üũ ����, �����̵� ���� �ε� �� UI ��Ʈ�ѿ� ����

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        
    }

    void OkbtnClick()
    {
        string a_NickStr = IDInputField.text;
        a_NickStr = a_NickStr.Trim();   // �յ� ������ ������ �ִ� �Լ�
        if(string.IsNullOrEmpty(a_NickStr)==true)
        {
            Debug.Log("������ ��ĭ ���� �Է��� �ּ���.");
            return;
        }

        if((2 <= a_NickStr.Length && a_NickStr.Length < 20) == false)
        {
            Debug.Log("������ 2���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        GlobalValue.g_NickName = a_NickStr;
        PlayerPrefs.SetString("NickName", a_NickStr);

        Game_Mgr a_GmMgr = GameObject.FindObjectOfType<Game_Mgr>();
        if(a_GmMgr != null)
        {
            if (a_GmMgr.m_UserInfoText != null)
                a_GmMgr.m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ")";
        }

        if (DltMethod != null)
            DltMethod();

        Time.timeScale = 1.0f;  // �Ͻ����� Ǯ��
        Destroy(gameObject);

    }//void OkbtnClick()

    void CloseBtnClick()
    {
        Time.timeScale = 1.0f;  // �Ͻ����� Ǯ��
        Destroy(gameObject);

    }//void CloseBtnClick()

    void SoundOnOff(bool value) // üũ ���°� ����Ǿ��� �� ȣ��Ǵ� �Լ�
    {
        if(m_Sound_Toggle != null)
        {
            if (value == true)
                PlayerPrefs.SetInt("SoundOnOff", 1);
            else
                PlayerPrefs.SetInt("SoundOnOff", 0);

            Sound_Mgr.Instance.SoundOnOff(value);   // ���� �� / ��
        }

    }//void SoundOnOff(bool Value)

    void SliderChanged(float value) // value 0.0f ~ 1.0f �����̵� ���°� ���� �Ǿ��� �� ȣ��Ǵ� �Լ�
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        Sound_Mgr.Instance.SoundVolume(value);
    }//void SliderChanged(float value)

}