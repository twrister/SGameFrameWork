using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MediaDemoController : MonoBehaviour
{
    public RawImage m_RawImage;
    
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            string deviceName = devices[0].name;
            
            Logger.Log($"cam deviceName = {deviceName}");
            
            WebCamTexture webCamTexture = new WebCamTexture(deviceName, 1920, 1080, 30);
            
            if (m_RawImage != null)
            {
                m_RawImage.texture = webCamTexture;
            }
            webCamTexture.Play();
        }

        InitAudioDevice();
        
    }

    private void OnDisable()
    {
        // DisableMicrophone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region 音频
    
    private AudioClip m_RecordClip;

    private bool m_IsRecording;

    public Button m_RecordBtn;
    public TextMeshProUGUI m_BtnTxt;
    
    public AudioSource m_AudioSource;
    private string m_MicrophoneDeviceName;

    private void InitAudioDevice()
    {
        m_RecordBtn.onClick.AddListener(OnClickRecord);
        m_IsRecording = false;
        UpdateRecordBtnView();
    }

    private void OnClickRecord()
    {
        if (m_IsRecording)
        {
            StopRecord();
        }
        else
        {
            StartRecord();
        }

        UpdateRecordBtnView();
    }

    private void StartRecord()
    {
        // 获取设备麦克风名称
        string micDevice = Microphone.devices[0];
        m_RecordClip = Microphone.Start(micDevice, false, 10, AudioSettings.outputSampleRate);
        // m_AudioSource.clip = m_RecordClip;
        // m_AudioSource.loop = true;
        // m_AudioSource.Play();
        
        m_IsRecording = true;
    }

    private void StopRecord()
    {
        // m_AudioSource.Stop();
        Microphone.End(null);
        m_IsRecording = false;

        string fileName = $"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}";
        string savePath = $"Demo/MediaDemo/Records/{fileName}.wav";
        SaveWav.Save(savePath, m_RecordClip);
        // SaveAudioClipAsWav(m_RecordClip);
    }

    private void UpdateRecordBtnView()
    {
        // m_RecordBtn
        m_BtnTxt.text = m_IsRecording ? "stop" : "record";
    }
    #endregion
}
