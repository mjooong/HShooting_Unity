using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public delegate void GFG_Response();    // 델리게이트 데이터(타입)형 하나 선언
    public GFG_Response DltMethod = null;   // 델리게이트 변수 선언(소켓 역할)

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

        // 체크 상태가 변경되었을 때 호출되는 함수를 대기하는 코드
        if (m_Sound_Toggle != null)
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);

        // 슬라이드 상태가 변경 되었을 때 호출되는 함수 대기하는 코드
        if (m_Sound_Slider != null)
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);

        // --- 닉네임 로딩 후 UI 컨트롤에 적용
        if(IDInputField != null)
            IDInputField.text = PlayerPrefs.GetString("NickName", "SBS전사");
        // --- 닉네임 로딩 후 UI 컨트롤에 적용

        // --- 체크 상태, 슬라이드 상태 로딩 후 UI 컨트롤에 적용
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

        // --- 체크 상태, 슬라이드 상태 로딩 후 UI 컨트롤에 적용

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        
    }

    void OkbtnClick()
    {
        string a_NickStr = IDInputField.text;
        a_NickStr = a_NickStr.Trim();   // 앞뒤 공백을 제거해 주는 함수
        if(string.IsNullOrEmpty(a_NickStr)==true)
        {
            Debug.Log("별명은 빈칸 없이 입력해 주세요.");
            return;
        }

        if((2 <= a_NickStr.Length && a_NickStr.Length < 20) == false)
        {
            Debug.Log("별명은 2글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        GlobalValue.g_NickName = a_NickStr;
        PlayerPrefs.SetString("NickName", a_NickStr);

        Game_Mgr a_GmMgr = GameObject.FindObjectOfType<Game_Mgr>();
        if(a_GmMgr != null)
        {
            if (a_GmMgr.m_UserInfoText != null)
                a_GmMgr.m_UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName + ")";
        }

        if (DltMethod != null)
            DltMethod();

        Time.timeScale = 1.0f;  // 일시정지 풀기
        Destroy(gameObject);

    }//void OkbtnClick()

    void CloseBtnClick()
    {
        Time.timeScale = 1.0f;  // 일시정지 풀기
        Destroy(gameObject);

    }//void CloseBtnClick()

    void SoundOnOff(bool value) // 체크 상태가 변경되었을 때 호출되는 함수
    {
        if(m_Sound_Toggle != null)
        {
            if (value == true)
                PlayerPrefs.SetInt("SoundOnOff", 1);
            else
                PlayerPrefs.SetInt("SoundOnOff", 0);

            Sound_Mgr.Instance.SoundOnOff(value);   // 사운드 켜 / 꺼
        }

    }//void SoundOnOff(bool Value)

    void SliderChanged(float value) // value 0.0f ~ 1.0f 슬라이드 상태가 변경 되었을 때 호출되는 함수
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        Sound_Mgr.Instance.SoundVolume(value);
    }//void SliderChanged(float value)

}