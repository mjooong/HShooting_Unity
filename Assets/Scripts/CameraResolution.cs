using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    // ��ũ���� ���� ��ǥ
    public static Vector3 m_ScreenMin = new Vector3(-10.0f, -5.0f, 0.0f);
    public static Vector3 m_ScreenMax = new Vector3(10.0f, 5.0f, 0.0f);
    // ��ũ���� ���� ��ǥ

    // Start is called before the first frame update
    void Start()
    {
        Camera a_Cam = GetComponent<Camera>();
        Rect rect = a_Cam.rect;
        float scaleHeight = ((float)Screen.width / Screen.height) / ((float)16 / 9);
        float scaleWidth = 1.0f / scaleHeight;

        if(scaleHeight < 1.0f)
        {
            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1.0f - scaleWidth) / 2.0f;
        }

        a_Cam.rect = rect;

        //--- ��ũ���� ���� ��ǥ ���ϱ�
        Vector3 a_ScMin = new Vector3(0.0f, 0.0f, 0.0f);
        m_ScreenMin = a_Cam.ViewportToWorldPoint(a_ScMin);
        //ī�޶� ȭ�� �����ϴ�(ȭ�� �ּҰ�) �ڳ��� ���� ��ǥ

        Vector3 a_ScMax = new Vector3(1.0f, 1.0f, 1.0f);
        m_ScreenMax = a_Cam.ViewportToWorldPoint(a_ScMax);
        //ī�޶� ȭ�� �������(ȭ�� �ִ밪) �ڳ��� ���� ��ǥ
        //--- ��ũ���� ���� ��ǥ ���ϱ�

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        
    }
}
