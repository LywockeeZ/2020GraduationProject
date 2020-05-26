using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;

public class InputManager : MonoBehaviour
{
    public delegate void mydelegate(ENUM_InputEvent inputEvent);
    public static event mydelegate InputEvent;
    [HideInInspector]
    public GameObject clickTag;
    private Material clickTagMaterial;
    private bool isTagFade = false;

    void Start()
    {
        if (clickTag == null)
        {
            clickTag = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Prefabs/Others/ClickTag", Vector3.zero);
            clickTag.transform.rotation = Quaternion.Euler(90f, 0, 0);
            clickTagMaterial = clickTag.GetComponent<Projector>().material;
        }
    }


    void Update()
    {
        if (Game.Instance.GetCanInput())
        {
            InputProcess();
        }

        if ((Game.Instance.GetPlayerUnit().transform.position - clickTag.transform.position).magnitude < 0.1f || !Game.Instance.GetCanInput())
        {
            if (!isTagFade)
            {
                clickTagMaterial.DOFade(0, 0.2f);
                isTagFade = true;
            }
        }

    }

    private void InputProcess()
    {
        MouseInput();
        if (!Game.Instance.GetCanFreeMove())
        {
            KeyboardInput();
        }
    }


    private void MouseInput()
    {
        HighlightTarget();

        //鼠标操作,按键弹起时进入
        if (Input.GetMouseButtonUp(0) == false)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("BaseUnit")))
        {
            Vector3 clickedPos;

            //关卡模式中
            if (!Game.Instance.GetCanFreeMove())
            {
                clickedPos = hitInfo.transform.position;
                if (Game.Instance.GetPlayerUnit().CurrentOn.Up != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Up.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Up);
                        return;
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Down != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Down.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Down);
                        return;
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Left != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Left.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Left);
                        return;
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Right != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Right.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Right);
                        return;
                    }
                }

            }

            if (Physics.Raycast(ray2, out RaycastHit hitInfo1, 200f, 1 << LayerMask.NameToLayer("CanMove")))
            {
                //自由移动模式中的移动
                if (Game.Instance.GetCanFreeMove() && (hitInfo.point - Game.Instance.GetPlayerUnit().transform.position).magnitude <=10f)
                {
                    clickedPos = hitInfo.point;
                    Game.Instance.GetPlayerUnit().MoveByNavMesh(clickedPos);
                    clickTagMaterial.DOFade(1, 0.2f);
                    isTagFade = false;
                    clickTag.transform.position = clickedPos;
                    Debug.Log(clickedPos);
                }
            }

        }
    }

    private void KeyboardInput()
    {
        //键盘操作
        if (Input.GetKeyDown(KeyCode.RightArrow)) InputEvent(ENUM_InputEvent.Right);
        else
        if (Input.GetKeyDown(KeyCode.LeftArrow)) InputEvent(ENUM_InputEvent.Left);
        else
        if (Input.GetKeyDown(KeyCode.UpArrow)) InputEvent(ENUM_InputEvent.Up);
        else
        if (Input.GetKeyDown(KeyCode.DownArrow)) InputEvent(ENUM_InputEvent.Down);

    }

    private void HighlightTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Game.Instance.GetCanFreeMove())
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("BaseUnit")))
            {
                Vector3 hoverPos = hitInfo.transform.position;
                if (CanUnitHighlight(hoverPos))
                {
                    Highlighter highlighter = hitInfo.transform.gameObject.GetComponent<Highlighter>();
                    highlighter.Hover(Color.white);
                }
            }
        }
    }

    private bool CanUnitHighlight(Vector3 hoverPos)
    {
        return CheckCanUnitHighlight(hoverPos, Game.Instance.GetPlayerUnit().CurrentOn.Right) ||
               CheckCanUnitHighlight(hoverPos, Game.Instance.GetPlayerUnit().CurrentOn.Left) ||
               CheckCanUnitHighlight(hoverPos, Game.Instance.GetPlayerUnit().CurrentOn.Up) ||
               CheckCanUnitHighlight(hoverPos, Game.Instance.GetPlayerUnit().CurrentOn.Down) ;
    }

    private bool CheckCanUnitHighlight(Vector3 hoverPos, BaseUnit unit)
    {
        if (hoverPos == unit?.Model.transform.position)
        {
            if (unit.CanWalk)
            {
                return true;
            }
        }
        return false;
    }
}
