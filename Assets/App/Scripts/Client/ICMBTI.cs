using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ICMBTI : MonoBehaviour
{
    public Text[] mbti;

    public Text otherMBTI;
    public void Send_MBTI(int i)
    {
        ICNetworkManager.Instance.SendPacket_MBTI(i);
    }

    public void Receive_MBTI(int i)
    {
        otherMBTI.text = mbti[i].text;
    }
}
