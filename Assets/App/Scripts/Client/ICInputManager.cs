using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ICInputManager : MonoBehaviour
{
    // UI Function
    public GameObject mUIPanel;
    public GameObject mEmotionPanel;
    public Image MicImage;
    private Sprite mMicOff;
    private Sprite mMicOn;

    // Character Function
    public float mMoveSpeed = 3.0f;
    public float mRotationSpeed = 100.0f;

    private CharacterController mCharacterController;
    private bool bIsMuted = false;

    // Set Instance
    private static ICInputManager instance;

    public GameObject mOVRCam;
    public bool Ismine = false;
    //public int m_UID = 0;
    private void OnEnable()
    {
        mOVRCam.SetActive(true);
    }
    private void OnDisable()
    {
        mOVRCam.SetActive(false);   
    }
    //public static ICInputManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindObjectOfType<ICInputManager>();

    //            if (instance == null)
    //            {
    //                instance = new GameObject("ICInputManager").AddComponent<ICInputManager>();
    //            }

    //            DontDestroyOnLoad(instance.gameObject);
    //        }
    //        return instance;
    //    }
    //}

    void Start()
    {
        mCharacterController = GetComponent<CharacterController>();

        mMicOff = Resources.Load<Sprite>("Micoff");
        mMicOn = Resources.Load<Sprite>("MicOn");
    }

    void Update()
    {
        if (Ismine)
            return;
        // Controller Input Update
        MetaInputController();
    }

    #region Input - Controller 
    private void MetaInputController()
    {
        HandleLeftJoystickMovement();
        HandleRightJoystickRotation();

        // Right Controller
        if (OVRInput.GetActiveController() == OVRInput.Controller.RTouch)
        {
            // Trigger - Interaction Btn
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("오른손 트리거");
            }

            // Grip - Object Grap
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("왼손 잡기");
            }

            // A Button (Open Emotion panel)
            if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                bool bIsActive = mEmotionPanel.activeSelf;
                mEmotionPanel.SetActive(!bIsActive);
            }

            // B Button (Open 2D UI)
            if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            {
                bool bIsActive = mUIPanel.activeSelf;
                mUIPanel.SetActive(!bIsActive);
            }

        }

        // Left Controller
        if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
        {
            // Trigger - Interaction Btn
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("왼손 트리거");
            }

            // Grip - Object Grap
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("왼손 잡기");
            }

            // X Button - Mic Controll
            if (OVRInput.GetUp(OVRInput.Button.Three, OVRInput.Controller.LTouch))
            {
                // Default : MicOn Icon
                // Mute   : MicOff Icon
                bIsMuted = !bIsMuted;
                MicImage.sprite = bIsMuted ? mMicOff : mMicOn;
            }

            // Y Button (Open 2D UI)
            if (OVRInput.GetUp(OVRInput.Button.Four, OVRInput.Controller.LTouch))
            {
                bool bIsActive = mUIPanel.activeSelf;
                mUIPanel.SetActive(!bIsActive);  
            }

        }
    }

    // L Controller - Moving 
    private void HandleLeftJoystickMovement()
    {
        Vector2 mLeftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        Vector3 mMoveDirection = new Vector3(mLeftJoystick.x, 0, mLeftJoystick.y);
        mMoveDirection = transform.TransformDirection(mMoveDirection);
        //mCharacterController.Move(mMoveDirection * mMoveSpeed * Time.deltaTime);
    }

    // R Controller - Rotation
    private void HandleRightJoystickRotation()
    {
        Vector2 mRightJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        float mRotateDirection = mRightJoystick.x;
        transform.Rotate(0, mRotateDirection * mRotationSpeed * Time.deltaTime, 0);
    }

    #endregion
}
