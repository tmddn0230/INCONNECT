using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class ICEmoticon : MonoBehaviour
{
    public float Movespeed = 0.1f; // 오브젝트 이동 속도
    public float movementDistance = 1f; // 양옆으로 움직일 거리


    private Material mat;
    
    
    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
    private void OnEnable()
    {
        Invoke("DisableObject", 2f);
    }

    // path = 리소스 파일 이름 전부 넣기 ~~~.png / ~~.jpg등..
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

    // itween으로 예시 였으나 움직임 연구 좀 더 필요
    //private void StartSideToSideMovement()
    //{
    //    Vector3[] path = new Vector3[3];
    //    path[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z); // 시작점
    //    path[1] = new Vector3(transform.position.x + 2 / 2, transform.position.y + 1, transform.position.z); // 중간점 (위로 올라감)
    //    path[2] = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z); // 끝점 (다시 내려옴)

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
