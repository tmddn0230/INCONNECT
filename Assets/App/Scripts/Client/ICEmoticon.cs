using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class ICEmoticon : MonoBehaviour
{
    public float Movespeed = 0.1f; // ������Ʈ �̵� �ӵ�
    public float movementDistance = 1f; // �翷���� ������ �Ÿ�


    private Material mat;
    
    
    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
    private void OnEnable()
    {
        Invoke("DisableObject", 2f);
    }

    // path = ���ҽ� ���� �̸� ���� �ֱ� ~~~.png / ~~.jpg��..
    public void DefalutEmoAnim(Sprite emotion) 
    {
        StartCoroutine(MoveForward(emotion));
    }
    IEnumerator MoveForward(Sprite emotion)
    {

        transform.position = new Vector3(0f,1.9f,0f);

        Texture2D newAlbedoTexture = emotion.texture;
       
        mat.SetTexture("_MainTex", newAlbedoTexture);

        float startTime = Time.time;
        
        while (Time.time < startTime + 1f)
        {
            transform.Translate(Vector3.forward * Movespeed * Time.deltaTime);
            yield return null;
        }

    }

    // itween���� ���� ������ ������ ���� �� �� �ʿ�
    //private void StartSideToSideMovement()
    //{
    //    Vector3[] path = new Vector3[3];
    //    path[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z); // ������
    //    path[1] = new Vector3(transform.position.x + 2 / 2, transform.position.y + 1, transform.position.z); // �߰��� (���� �ö�)
    //    path[2] = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z); // ���� (�ٽ� ������)

    //    iTween.MoveTo(gameObject, iTween.Hash(
    //        "path", path,
    //        "time", 2,
    //        "easetype", iTween.EaseType.easeInOutSine
    //    ));
    //}

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
