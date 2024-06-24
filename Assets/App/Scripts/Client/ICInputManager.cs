using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICInputManager : MonoBehaviour
{
    public GameObject mUIPanel;
    public GameObject mEmotionPanel;

    public float mMoveSpeed = 3.0f;
    public float mRotationSpeed = 100.0f;

    private CharacterController mCharacterController;
    private bool bIsMuted = false;
    void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 컨트롤러 이용한 인풋 업데이트
        MetaInputController();
    }

    #region 컨트롤러 인풋 부분
    private void MetaInputController()
    {
        HandleLeftJoystickMovement();
        HandleRightJoystickRotation();

        // 오른손 컨트롤러 입력 
        if (OVRInput.GetActiveController() == OVRInput.Controller.RTouch)
        {
            // 트리거 버튼 (오브젝트 선택 및 사용)
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("오른손 트리거");
            }

            // 그립 버튼 (오브젝트 잡기)
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("왼손 잡기");
            }

            // A 버튼 (이모티콘 패널 열기)
            if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                bool bIsActive = mEmotionPanel.activeSelf;
                mEmotionPanel.SetActive(!bIsActive);
            }

            // B 버튼 (2D UI 호출)
            if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            {
                bool bIsActive = mUIPanel.activeSelf;
                mUIPanel.SetActive(!bIsActive);
            }

        }

        // 왼손 컨트롤러 입력 
        if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
        {
            // 트리거 버튼 (오브젝트 선택 및 사용)
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("왼손 트리거");
            }

            // 그립 버튼 (오브젝트 잡기)
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("왼손 잡기");
            }

            // X 버튼 (음소거 On/Off)
            if (OVRInput.Get(OVRInput.Button.Three, OVRInput.Controller.LTouch))
            {
                bIsMuted = !bIsMuted;
                AudioListener.volume = bIsMuted ? 0 : 1;
            }

            // Y 버튼 (2D UI 호출)
            if (OVRInput.Get(OVRInput.Button.Four, OVRInput.Controller.LTouch))
            {
                bool bIsActive = mUIPanel.activeSelf;
                mUIPanel.SetActive(!bIsActive);  
            }

        }
    }
    
    // 왼손 컨트롤러로 움직이기
    private void HandleLeftJoystickMovement()
    {
        Vector2 mLeftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        Vector3 mMoveDirection = new Vector3(mLeftJoystick.x, 0, mLeftJoystick.y);
        mMoveDirection = transform.TransformDirection(mMoveDirection);
        mCharacterController.Move(mMoveDirection * mMoveSpeed * Time.deltaTime);
    }

    // 오른손 컨트롤러로 회전하기
    private void HandleRightJoystickRotation()
    {
        Vector2 mRightJoystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.RTouch);
        float mRotateDirection = mRightJoystick.x;
        transform.Rotate(0, mRotateDirection * mRotationSpeed * Time.deltaTime, 0);
    }

    #endregion
}
