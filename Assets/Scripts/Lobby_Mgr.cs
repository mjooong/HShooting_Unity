using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby_Mgr : MonoBehaviour
{
    public Button m_ClearSvDataBtn;

    public Button m_StoreBtn;
    public Button m_GameStartBtn;
    public Button m_ExitBtn;

    public Text m_GoldText;
    public Text m_MyInfoText;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //���� �ӵ���...

        GlobalValue.LoadGameData();

        // --- ��ư Ŭ��
        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(() =>
            {
                if (Fade_Mgr.Inst != null && Fade_Mgr.Inst.IsFadeOut == true)
                    Fade_Mgr.Inst.SceneOut("StoreScene");
                else
                    SceneManager.LoadScene("StoreScene");

                Sound_Mgr.Instance.PlayGUISound("Pop", 1.0f);

            });

        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(() =>
            {
                if (Fade_Mgr.Inst != null && Fade_Mgr.Inst.IsFadeOut == true)
                    Fade_Mgr.Inst.SceneOut("InGameScene");
                else
                    SceneManager.LoadScene("InGameScene");

                Sound_Mgr.Instance.PlayGUISound("Pop", 1.0f);

            });

        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
                if (Fade_Mgr.Inst != null && Fade_Mgr.Inst.IsFadeOut == true)
                    Fade_Mgr.Inst.SceneOut("TitleScene");
                else
                    SceneManager.LoadScene("TitleScene");

                Sound_Mgr.Instance.PlayGUISound("Pop", 1.0f);

            });
        // --- ��ư Ŭ��

        if (m_GoldText != null)
        {
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");
            // "N0" : ������ <-- �Ҽ��� �����δ� ���ܽ�Ű�� õ���� ���� ��ǥ �ٿ��ֱ�...
        }

        if (m_MyInfoText != null)
            m_MyInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") : ����("
                + "1��" + ") : ����(" + GlobalValue.g_BestScore + ")";

        if (m_ClearSvDataBtn != null)
            m_ClearSvDataBtn.onClick.AddListener(ClearSvData);

        Sound_Mgr.Instance.PlayBGM("sound_bgm_title_001", 1.0f);

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearSvData()
    {
        PlayerPrefs.DeleteAll();
        GlobalValue.LoadGameData();

        if (m_GoldText != null)
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");

        if (m_MyInfoText != null)
            m_MyInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") : ����("
                + "1��" + ") : ����(" + GlobalValue.g_BestScore + ")";

        Sound_Mgr.Instance.PlayGUISound("Pop", 1.0f);

    }//void ClearSvData()
}
