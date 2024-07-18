using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Rokoko.Core; // Json.NET ���̺귯�� ���

public class AnimationController : MonoBehaviour
{
    public CharacterAnim characterAnimator; // CharacterAnim �ν��Ͻ�
    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.dataPath, "Resources/Data.txt");
        StartCoroutine(ReadDataFromFile());
    }

    private IEnumerator ReadDataFromFile()
    {
        while (true)
        {
            string data = LoadFromFile();
            if (!string.IsNullOrEmpty(data))
            {
                var coreBoneDataWrapper = JsonConvert.DeserializeObject<CoreBoneDataWrapper>(data); // Json.NET ���
                if (coreBoneDataWrapper != null && coreBoneDataWrapper.actors.Length > 0)
                {
                    characterAnimator.ApplyCoreBoneData(coreBoneDataWrapper.actors[0]);
                }
            }
            yield return new WaitForSeconds(0.1f); // 0.1�ʸ��� ������ �о�� (�ʿ信 ���� ����)
        }
    }

    private string LoadFromFile()
    {
        try
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading from file: " + ex.Message);
            return null;
        }
    }
}
