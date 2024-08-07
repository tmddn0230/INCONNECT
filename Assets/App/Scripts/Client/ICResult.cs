using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICResult : MonoBehaviour
{
    public GameObject Continue;
    public GameObject No_Continue;
    public void Send_Result(int i) //0 성공 1 실패
    {
        ICNetworkManager.Instance.SendPacket_After(i);
    }

    public void Receive_Result(int i)
    {
        if(i == 0)
        {
            Continue.SetActive(true);
        }
        else
        {
            No_Continue.SetActive(true);
        }
    }
}
