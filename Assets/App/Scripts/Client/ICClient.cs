using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICClient : MonoBehaviour
{
    public static ICClient Instance;

    // TCP Connect 시 서버로 부터 받음
    public int m_UID = 0;

    #region 사람들 스폰 하는 로직
    public GameObject[] mActors;
    public GameObject[] spawnedActors; // 소환된 액터들의 배열


    public void Actor_Spawn(int uid, int Result)
    {
        // Result가 1이 아닌 경우 리턴
        if (Result != 1)
            return;
        // UID가 유효한 범위인지 확인
        if (uid < 1 || uid > mActors.Length)
            return;
    

        // m_UID를 주어진 첫 uid로 고정 설정
        if(m_UID == 0)
            m_UID = uid;

        // 액터가 소환된 적이 없으면 초기화
        if (spawnedActors == null)
        {
            spawnedActors = new GameObject[mActors.Length];
        }

        // 0부터 uid까지의 모든 액터 소환
        for (int i = 0; i < uid; i++)
        {
            if (spawnedActors[i] == null)
            {
                spawnedActors[i] = Instantiate(mActors[i], Vector3.zero, Quaternion.identity);
               
                
                if (spawnedActors[i].gameObject.name == "Man")
                {
                    ICUIEvents.Instance.Man = spawnedActors[i].gameObject;
                }
                else
                    ICUIEvents.Instance.Women = spawnedActors[i].gameObject;
            }
        }

        // 본인 ICInputManager만 켜줌
        for (int i = 0; i < spawnedActors.Length; i++)
        {
            if (spawnedActors[i] != null)
            {
                ICInputManager inputManager = spawnedActors[i].GetComponent<ICInputManager>();
                if (inputManager != null)
                {
                    // m_UID와 같은 번호에서 소환된 프리팹만 활성화
                    inputManager.Ismine = (i == m_UID - 1);
                   
                    //if(inputManager.m_UID == 0)
                    //    inputManager.m_UID = uid;
                }
            }
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    


}
