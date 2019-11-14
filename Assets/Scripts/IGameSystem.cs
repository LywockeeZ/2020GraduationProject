using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IGameSystem 
{
    protected GameManager m_GameManager = null;
    public IGameSystem (GameManager gameManager)
    {
        m_GameManager = gameManager;
    }


    public virtual void Initialize() { }
    public virtual void Release() { }
    public virtual void Update() { }
}
