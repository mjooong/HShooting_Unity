using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DlgBox_Ctrl : MonoBehaviour
{
    public delegate void DLT_Response();   //<-- ��������Ʈ ������(Ÿ��)�� �ϳ� ����
    DLT_Response DltMethod;     //<-- ��������Ʈ ���� ����(���� ����)

    public Button m_Ok_Btn = null;
    public Button m_Close_Btn  = null;
    public Button m_Cancel_Btn = null;
    public Text m_Contents_Txt = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Ok_Btn != null)  
            m_Ok_Btn.onClick.AddListener(() =>    //���� OK ��ư�� ������ ���
            {
                if (DltMethod != null)
                    DltMethod();

                Destroy(gameObject);
            });

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });

        if (m_Cancel_Btn != null)
            m_Cancel_Btn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMessage(string a_Mess, DLT_Response a_DltMtd = null)
    {
        m_Contents_Txt.text = a_Mess;
        DltMethod = a_DltMtd;
    }
}
