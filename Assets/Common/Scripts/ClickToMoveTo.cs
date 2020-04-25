using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

[RequireComponent(typeof(Highlighter))]
public class ClickToMoveTo : MonoBehaviour
{
    private Highlighter highlighter;

    void Start()
    {
        highlighter = GetComponent<Highlighter>();    
    }

    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if ((transform.position - Game.Instance.GetPlayerUnit().transform.position).magnitude < 8)
        {
            highlighter.ConstantOn();
        }
    }

    private void OnMouseExit()
    {
        highlighter.ConstantOff();
    }

    private void OnMouseDown()
    {
        Game.Instance.GetPlayerUnit().MoveByNavMesh((Game.Instance.GetPlayerUnit().transform.position - transform.position).normalized * 1f + transform.position);
    }
}
