using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICClient : MonoBehaviour
{
    public static ICClient Instance;

    // TCP Connect �� ������ ���� ����
    public int m_UID = 0;

    #region ����� ���� �ϴ� ����
    public GameObject[] mActors;
    public GameObject[] spawnedActors; // ��ȯ�� ���͵��� �迭


    public void Actor_Spawn(int uid, int Result)
    {
        // Result�� 1�� �ƴ� ��� ����
        if (Result != 1)
            return;
        // UID�� ��ȿ�� �������� Ȯ��
        if (uid < 1 || uid > mActors.Length)
            return;
    

        // m_UID�� �־��� ù uid�� ���� ����
        if(m_UID == 0)
            m_UID = uid;

        // ���Ͱ� ��ȯ�� ���� ������ �ʱ�ȭ
        if (spawnedActors == null)
        {
            spawnedActors = new GameObject[mActors.Length];
        }

        // 0���� uid������ ��� ���� ��ȯ
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

        // ���� ICInputManager�� ����
        for (int i = 0; i < spawnedActors.Length; i++)
        {
            if (spawnedActors[i] != null)
            {
                ICInputManager inputManager = spawnedActors[i].GetComponent<ICInputManager>();
                if (inputManager != null)
                {
                    // m_UID�� ���� ��ȣ���� ��ȯ�� �����ո� Ȱ��ȭ
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
