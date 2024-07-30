using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICVoiceManager : MonoBehaviour
{
    private AudioClip micClip;
    private int sampleRate = 48000;
    private bool isRecording = false;
    private Queue<float[]> audioBuffer = new Queue<float[]>();
    private int bufferLength = 3; // Test ¿ë 3ÃÊ 
    private int samplesPerSecond;

    // Start is called before the first frame update
    void Start()
    {
        samplesPerSecond = sampleRate;
        StartRecording();

        // Test 

        isRecording = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRecording)
        {
            StartRecording();
        }
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            micClip = Microphone.Start(null, true, bufferLength * 2, sampleRate);
            isRecording = true;
            StartCoroutine(CaptureMicData());
            StartCoroutine(PlayBufferedData());
            Debug.Log("Start Recording");
        }
        else
        {
            Debug.Log("Recording is in progress already");
        }
    }

    private IEnumerator CaptureMicData()
    {
        while (isRecording)
        {
            int micPosition = Microphone.GetPosition(null);
            if (micPosition > 0)
            {
                float[] samples = new float[micClip.samples];
                micClip.GetData(samples, 0);

                float[] segment = new float[samplesPerSecond];
                if (micPosition < samplesPerSecond)
                {
                    Array.Copy(samples, micClip.samples - (samplesPerSecond - micPosition), segment, 0, samplesPerSecond - micPosition);
                    Array.Copy(samples, 0, segment, samplesPerSecond - micPosition, micPosition);
                }
                else
                {
                    Array.Copy(samples, micPosition - samplesPerSecond, segment, 0, samplesPerSecond);
                }

                if (audioBuffer.Count >= bufferLength)
                {
                    audioBuffer.Dequeue();
                }
                audioBuffer.Enqueue(segment);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator PlayBufferedData()
    {
        while (isRecording)
        {
            if (audioBuffer.Count > 0)
            {
                float[] dataToPlay = audioBuffer.Dequeue();
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }

                AudioClip clipToPlay = AudioClip.Create("BufferedClip", dataToPlay.Length, 1, sampleRate, false);
                clipToPlay.SetData(dataToPlay, 0);
                audioSource.clip = clipToPlay;
                audioSource.Play();
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
