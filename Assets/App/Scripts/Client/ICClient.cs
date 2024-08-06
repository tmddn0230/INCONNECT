using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICClient : MonoBehaviour
{
    // TCP Connect 시 서버로 부터 받음
    int UID;
    // UID : 1 이면 male , 2 이면 female
    bool IsMale;



    public void Actor_Spawn(int uid, bool Ismale)
    {
        // uid 1이면 male 소환하고,
        // 각 프리팹에서 getcomponent<ICInputManager>()을 하고 
        //   
        // 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
