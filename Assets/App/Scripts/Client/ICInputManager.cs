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
        // ��Ʈ�ѷ� �̿��� ��ǲ ������Ʈ
        MetaInputController();
    }

    #region ��Ʈ�ѷ� ��ǲ �κ�
    private void MetaInputController()
    {
        HandleLeftJoystickMovement();
        HandleRightJoystickRotation();

        // ������ ��Ʈ�ѷ� �Է� 
        if (OVRInput.GetActiveController() == OVRInput.Controller.RTouch)
        {
            // Ʈ���� ��ư (������Ʈ ���� �� ���)
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("������ Ʈ����");
            }

            // �׸� ��ư (������Ʈ ���)
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("�޼� ���");
            }

            // A ��ư (�̸�Ƽ�� �г� ����)
            if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                bool bIsActive = mEmotionPanel.activeSelf;
                mEmotionPanel.SetActive(!bIsActive);
            }

            // B ��ư (2D UI ȣ��)
            if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            {
                bool bIsActive = mUIPanel.activeSelf;
                mUIPanel.SetActive(!bIsActive);
            }

        }

        // �޼� ��Ʈ�ѷ� �Է� 
        if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
        {
            // Ʈ���� ��ư (������Ʈ ���� �� ���)
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("�޼� Ʈ����");
            }

            // �׸� ��ư (������Ʈ ���)
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("�޼� ���");
            }

            // X ��ư (���Ұ� On/Off)
            if (OVRInput.Get(OVRInput.Button.Three, OVRInput.Controller.LTouch))
            {
                bIsMuted = !bIsMuted;
                AudioListener.volume = bIsMuted ? 0 : 1;
            }

            // Y ��ư (2D UI ȣ��)
            if (OVRInput.Get(OVRInput.Button.Four, OVRInput.Controller.LTouch))
            {
                bool bIsActive = mUIPanel.activeSelf;
                mUIPanel.SetActive(!bIsActive);  
            }

        }
    }
    
    // �޼� ��Ʈ�ѷ��� �����̱�
    private void HandleLeftJoystickMovement()
    {
        Vector2 mLeftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        Vector3 mMoveDirection = new Vector3(mLeftJoystick.x, 0, mLeftJoystick.y);
        mMoveDirection = transform.TransformDirection(mMoveDirection);
        mCharacterController.Move(mMoveDirection * mMoveSpeed * Time.deltaTime);
    }

    // ������ ��Ʈ�ѷ��� ȸ���ϱ�
    private void HandleRightJoystickRotation()
    {
        Vector2 mRightJoystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.RTouch);
        float mRotateDirection = mRightJoystick.x;
        transform.Rotate(0, mRotateDirection * mRotationSpeed * Time.deltaTime, 0);
    }

    #endregion
}
