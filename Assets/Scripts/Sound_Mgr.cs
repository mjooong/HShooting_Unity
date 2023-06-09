using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Mgr : G_Singleton<Sound_Mgr>
{
    [HideInInspector] public AudioSource m_AudioSrc = null;
    Dictionary<string, AudioClip> m_ADClipList = new Dictionary<string, AudioClip>();

    // --- ȿ���� ����ȭ�� ���� ���� ����
    int m_EffSdCount = 5;   // ������ 5���� ���̾�� �÷���..
    int m_SoundCount = 0;   // �ִ� 5������ ����ǰ� ���� (������ ����)..
    List<GameObject> m_sndObjList = new List<GameObject>();
    AudioSource[] m_sndSrcList = new AudioSource[10];
    float[] m_EffVolume = new float[10];
    // --- ȿ���� ����ȭ�� ���� ���� ����

    float m_BgmVolume = 0.2f;
    [HideInInspector] public bool m_SoundOnOff = true;
    [HideInInspector] public float m_SoundVolume = 1.0f;



    protected override void Init()  // Awake() �Լ� ��� ���
    {
        base.Init();    // �θ��ʿ� �ִ� Init() �Լ� ȣ��

        LoadChildGameObj();
    }

    // Start is called before the first frame update
    void Start()
    {
        // --- ���� �̸� �ε�   
        AudioClip a_GAudioClip = null;
        object[] temp = Resources.LoadAll("Sounds");
        for(int ii = 0; ii < temp.Length; ii++)
        {
            a_GAudioClip = temp[ii] as AudioClip;

            if (m_ADClipList.ContainsKey(a_GAudioClip.name) == true)
                continue;

            m_ADClipList.Add(a_GAudioClip.name, a_GAudioClip);
        }
        // --- ���� �̸� �ε�

    }//void Start()

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void LoadChildGameObj()
    {
        m_AudioSrc = this.gameObject.AddComponent<AudioSource>();   // AddComponent���� AudioSource ã��

        for(int ii = 0; ii < m_EffSdCount; ii++)
        {
            // �ִ� 5������ ����ǰ� ���� ������
            GameObject newSoundObj = new GameObject();
            newSoundObj.transform.SetParent(this.transform);
            newSoundObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundObj.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            newSoundObj.name = "SoundEffObj";

            m_sndSrcList[ii] = a_AudioSrc;
            m_sndObjList.Add(newSoundObj);

        }//for(int ii = 0; ii < m_EffSdCount; ii++)

        //--- ���� OnOff, ���� ���� ���� �ε� �� ����
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (a_SoundOnOff == 1)
            SoundOnOff(true);
        else
            SoundOnOff(false);

        float a_Value = PlayerPrefs.GetFloat("SoundOnOff", 1.0f);
        SoundVolume(a_Value);

        //--- ���� OnOff, ���� ���� ���� �ε� �� ����

    }// public void LoadChildGameObj()

    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;
        if(m_ADClipList.ContainsKey(a_FileName)==true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null)
            return;

        if (m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName)  // Ŭ���� ����Ǿ� �ִ� �̸��� ������ ����
            return;

        m_AudioSrc.clip = a_GAudioClip;
        m_AudioSrc.volume = fVolume * m_SoundVolume;    
        m_BgmVolume = fVolume;  
        m_AudioSrc.loop = true;
        m_AudioSrc.Play();

    }//public void PlayBGM(string a_FileName, float fVolume = 0.2f)

    public void PlayGUISound(string a_FileName, float fVolume = 0.2f)
    {// GUI ȿ���� �÷��� �ϱ����� ����
        if (m_SoundOnOff == false)
            return;

        AudioClip a_GAudioClip = null;
        if(m_ADClipList.ContainsKey(a_FileName)==true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null)
            return;

        m_AudioSrc.PlayOneShot(a_GAudioClip, fVolume * m_SoundVolume);  // PlayOneShot : �ߺ� ���� �÷���, ������ ����ÿ��� ����

    }// public void PlayGUISound(string a_FileName, float fVolume = 0.2f)

    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false)
            return;

        AudioClip a_GAudioClip = null;
        if(m_ADClipList.ContainsKey(a_FileName)==true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (a_GAudioClip == null)
            return;

        if(m_sndSrcList[m_SoundCount] != null)
        {
            m_sndSrcList[m_SoundCount].volume = fVolume * m_SoundVolume;
            m_sndSrcList[m_SoundCount].PlayOneShot(a_GAudioClip, fVolume * m_SoundVolume);
            m_EffVolume[m_SoundCount] = fVolume;

            m_SoundCount++;
            if (m_EffSdCount <= m_SoundCount)
                m_SoundCount = 0;

        }//if(m_sndSrcList[m_SoundCount] != null)

    }//public void PlayEffSound(string a_FileName, float fVolume = 0.2f)

    public void SoundOnOff(bool a_OnOff = true)
    {
        bool a_MuteOnOff = !a_OnOff;

        if(m_AudioSrc != null)
        {
            m_AudioSrc.mute = a_MuteOnOff;  // mute == true ����, mute == false �ѱ�
            if (a_MuteOnOff == false)   // ���带 �ٽ� ���� ��
                m_AudioSrc.time = 0;    // ó������ �ٽ� �÷���
        }

        for(int ii = 0; ii < m_sndSrcList.Length; ii++)
        {
            if(m_sndSrcList[ii] != null)
            {
                m_sndSrcList[ii].mute = a_MuteOnOff;

                if (a_MuteOnOff == false)
                    m_sndSrcList[ii].time = 0;  // ó������ �ٽ� �÷���
            }
        }

        m_SoundOnOff = a_OnOff;

    }//public void SoundOnOff(bool a_OnOff = true)

    // ������� ���� ������ ������ ������ �÷��� �� �ش�
    public void SoundVolume(float fVolume)
    {
        if (m_AudioSrc != null)
            m_AudioSrc.volume = m_BgmVolume * fVolume;

        for(int ii = 0; ii < m_sndSrcList.Length; ii++)
        {
            if (m_sndSrcList[ii] != null)
                m_sndSrcList[ii].volume = m_EffVolume[ii] * fVolume;
        }

        m_SoundVolume = fVolume;

    }
    // ������� ���� ������ ������ ������ �÷��� �� �ش�

}
