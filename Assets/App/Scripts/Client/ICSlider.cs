using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class ICSlider : MonoBehaviour
{
    public Slider silder;
    public Text text;
    public int score;

    public GameObject Success;
    public GameObject Fail;
    public void SetScore()
    {
        text.text = "Á¡¼ö" + (silder.value * 100).ToString("F0")+ "Á¡";
        string a = (silder.value * 100).ToString("F0");
        score = int.Parse(a);
    }

    public void SendScore()
    {
        ICNetworkManager.Instance.SendPacket_Attract(score);
    }

    public void Receive_Score(int Score)
    {
        if(Score < 70)
            Fail.SetActive(true);
        
        else
            Success.SetActive(true);
    }
}
