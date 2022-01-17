using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool Enable_Input;

    [Space(10)]
    public InputType E_inputType;

    private Vector3 Ini_Pos;
    private Touch m_Touch;


    private Vector2 Dir;

    public float Min_Input_Treshhold = 0.1f;
    public float Scenctivity;
    public float DeScenctivity;

    public float H_Res = 0;
    public float RatioFactor = 4;

    public void OnEnable()
    {
        Application.targetFrameRate = 1000;
        Dir = Vector2.zero;
        H_Res = Screen.width;
    }
    public float Delta;

    public bool IsInside = false;

    void Update()
    {


        if (Enable_Input)
        {
            //ReferenceManager.instance._UI_Handler.SwipeToMoveHelp(false);
        }
        else
        {
            return;
        }

        if (E_inputType == InputType.Editor)
            Handle_EditorInput();
        else
            Handle_MobileInput();

        Dir.x = Mathf.Clamp(Dir.x, -1, 1);
        Dir.y = Mathf.Clamp(Dir.y, -1, 1);

        if (Dir.x < 0.01f && Dir.x > -0.01f)
        {
            Dir.x = 0;
        }

        if (Dir.y < 0.01f && Dir.y > -0.01f)
        {
            Dir.y = 0;
        }

        CrossPlatformInputManager.SetAxis("Vertical", Dir.y);
        CrossPlatformInputManager.SetAxis("Horizontal", Dir.x);

    }

    public void Handle_MobileInput()
    {
        if (Input.touchCount > 0)
        {
            m_Touch = Input.touches[0];
            if (m_Touch.phase == TouchPhase.Began)
            {
                On_PointerDown(m_Touch.position);
            }
            else if (m_Touch.phase == TouchPhase.Moved)
            {
                On_PointerDragging(m_Touch.position);
            }
            else if (m_Touch.phase == TouchPhase.Ended || m_Touch.phase == TouchPhase.Canceled || m_Touch.phase == TouchPhase.Stationary)
            {
                On_PointerUp();
            }
        }
        else
        {
            On_PointerUp();
        }


    }

    public void Handle_EditorInput()
    {
        if (Input.GetMouseButtonDown(0) && IsInside)
        {
            On_PointerDown(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && IsInside)
        {
            On_PointerDragging(Input.mousePosition);
        }
        else
        {
            On_PointerUp();
        }




    }

    public Vector2 DeltaPos;
    public void On_PointerDown(Vector3 start_pos)
    {
        Ini_Pos = Input.mousePosition;
        CrossPlatformInputManager.SetButtonDown("TapnHold");

    }

    public void On_PointerDragging(Vector3 C_pos)
    {
        //C_pos = Input.mousePosition;

        DeltaPos = C_pos - Ini_Pos;
        float m_Sign = Mathf.Sign(DeltaPos.magnitude);

        Delta = 1 - (H_Res - DeltaPos.magnitude) / H_Res;

        Ini_Pos = Vector2.Lerp(Ini_Pos, C_pos, Time.deltaTime * Scenctivity);

        if (Mathf.Abs(DeltaPos.magnitude) > Min_Input_Treshhold)
        {
            Dir = new Vector2(Delta * Mathf.Sign(DeltaPos.x), Delta);
        }
        else
        {
            Dir = Vector2.Lerp(Dir, Vector2.zero, Time.deltaTime * DeScenctivity);
        }

        CrossPlatformInputManager.SetButtonDown("TapnHold");
    }

    public float normalizeratio = 0;
    public void On_PointerDragging_A(Vector3 C_pos)
    {

        normalizeratio = (C_pos.x - Ini_Pos.x) / (H_Res / RatioFactor);

        //C_pos = Input.mousePosition;

        //Vector2 DeltaPos = C_pos - Ini_Pos;
        //float m_Sign = Mathf.Sign(DeltaPos.magnitude);

        //Delta = DeltaPos.magnitude;

        if (normalizeratio > 0)
        {
            Debug.Log("A");
            Dir = Vector2.Lerp(Dir, Vector2.one, Mathf.Abs(normalizeratio) * Scenctivity);
        }
        else if (normalizeratio < 0)
        {
            Debug.Log("B");
            Dir = Vector2.Lerp(Dir, Vector2.one * -1, Mathf.Abs(normalizeratio) * Scenctivity);
        }


        //if (normalizeratio > Min_Input_Treshhold)
        //{
        //    Dir = new Vector2(normalizeratio, 0);
        //}
        //else
        //{
        //    Dir = Vector2.Lerp(Dir, Vector2.zero, Time.deltaTime * DeScenctivity);
        //}

        Ini_Pos = Vector2.Lerp(Ini_Pos, C_pos, Time.deltaTime * Scenctivity);

    }

    public void On_PointerUp()
    {
        Dir = Vector2.Lerp(Dir, Vector2.zero, Time.deltaTime * DeScenctivity);
        CrossPlatformInputManager.SetButtonUp("TapnHold");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsInside = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsInside = false;
    }
}


public enum InputType
{
    Editor,
    Touch,
}