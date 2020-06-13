using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    #region 单例模式
    private static Game _instance;
    public static Game Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Game();
            return _instance;
        }
    }
    private Game() { }
    #endregion

    public bool isTest = false;

    private StageSystem m_StageSystem = null;
    private GameEventSystem m_GameEventSystem = null;
    private APSystem m_APSystem = null;
    private UISystem m_UISystem = null;
    private SkillSystem m_SkillSystem = null;



    private bool m_canInput = false;
    private bool m_canFreeMove = false;
    private bool m_isGameOver = false;
    private Player playerUnit = null;





    public void Initinal()
    {
        m_GameEventSystem = new GameEventSystem();
        m_StageSystem = new StageSystem();
        m_APSystem = new APSystem();
        m_UISystem = new UISystem();
        m_SkillSystem = new SkillSystem();
    }

    private void ResigerGameEvent()
    {

    }

    public void Release()
    {
        m_GameEventSystem.Release();
        m_StageSystem.Release();
        m_APSystem.Release();
    }

    public void Updata()
    {
        m_GameEventSystem.Update();
        m_StageSystem.Update();
        m_APSystem.Update();
        m_UISystem.Update();
        m_SkillSystem.Update();
    }




    #region 事件系统接口
    public void RegisterEvent(ENUM_GameEvent type, EventListenerDelegate listener)
    {
        m_GameEventSystem.RegisterEvent(type, listener);
    }

    public void DetachEvent(ENUM_GameEvent type, EventListenerDelegate listener)
    {
        m_GameEventSystem.DetachEvent(type, listener);
    }

    public void NotifyEvent(ENUM_GameEvent type, params System.Object[] param)
    {
        Debug.Log(type);
        m_GameEventSystem.NotifyEvent(type, param);
    }

    public void NotifyEvent(Message evt)
    {
        m_GameEventSystem.NotifyEvent(evt);
    }

    public void ClearAllEvent()
    {
        m_GameEventSystem.ClearAllEvent();
    }
    #endregion



    #region 输入系统接口
    public void SetCanInput(bool value)
    {
        Debug.Log("SetCanInput:" + value);
        //自由模式时，每当停止输入时让角色立刻停下
        if (value == false && GetCanFreeMove())
        {
            if (GetPlayerUnit() != null)
            {
                GetPlayerUnit().MoveByNavMesh(GetPlayerUnit().transform.position);
            }
        }
        m_canInput = value;
    }

    public bool GetCanInput()
    {
        return m_canInput;
    }

    public void SetCanFreeMove(bool value)
    {
        m_canFreeMove = value;
    }

    public bool GetCanFreeMove()
    {
        return m_canFreeMove;
    }
    #endregion



    #region AP系统接口
    public void SetMaxAP(int value)
    {
        m_APSystem.ResetAPSystem();
        m_APSystem.SetRoundActionPts(value);
    }

    public int GetCurrentAP()
    {
        return m_APSystem.GetCurrentAP();
    }

    public void ResetRoundAP()
    {
        m_APSystem.ResetActionPoints();
    }

    public bool CostAP(int value, int additionValue)
    {
        return m_APSystem.CostAP(value, additionValue);
    }

    public int GetTotalCostPts()
    {
        return m_APSystem.GetTotalCostPts();
    }
    #endregion



    #region 关卡系统接口
    public bool GetIsInStage()
    {
        return m_StageSystem.GetIsInStage();
    }

    public void SetIsInStage(bool value)
    {
        m_StageSystem.SetIsInStage(value);
    }

    public bool IsCurrentStageEnd()
    {
        return m_StageSystem.IsStageEnd();
    }

    /// <summary>
    /// 只加载关卡内容，不切换场景
    /// </summary>
    public void LoadNextStage()
    {
        m_StageSystem.LoadNextStage();
    }

    public void LoadStage(string stageName)
    {
        m_StageSystem.LoadStage(stageName);
    }

    /// <summary>
    /// 获取全部关卡的字典
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, IStageHandler> GetStages()
    {
        return m_StageSystem.GetStages();
    }



    /// <summary>
    /// 加载关卡，加载关卡包括场景转换和加载关卡内容
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="loadingSceneName"></param>
    public void LoadLevel(string levelName)
    {
        m_StageSystem.LoadLevel(levelName);
    }

    public void LoadNextLevel()
    {
        m_StageSystem.LoadNextLevel();
    }

    public void LoadLevelOnMain(string sceneName, string levelName)
    {
        if (isTest)
        {
            Game.Instance.CloseUI("SkillSelectUI");
            Game.Instance.NotifyEvent(ENUM_GameEvent.RoundBegain);
        }
        else
            m_StageSystem.LoadLevelOnMain(sceneName,levelName);
    }

    public void SetPlayerUnit(Player m_playerUnit)
    {
        playerUnit = m_playerUnit;
    }

    public Player GetPlayerUnit()
    {
        return playerUnit;
    }

    public IStageHandler GetCurrentStage()
    {
        return m_StageSystem?.GetCurrentStage();
    }

    public string GetLevelWillToOnMain()
    {
        return m_StageSystem.GetLevelWillToOnMain();
    }

    public string GetSceneWillToOnMain()
    {
        return m_StageSystem.GetSceneWillToOnMain();
    }

    public void SetLevelWillToOnMain(string sceneName, string levelName)
    {
        m_StageSystem.SetLevelWillToOnMain(sceneName, levelName);
    }

    /// <summary>
    /// 将已进行的关卡清空
    /// </summary>
    public void ResetStage()
    {
        m_StageSystem.ResetStage();
    }

    #endregion



    #region UI系统的接口
    /// <summary>
    /// 设置BattleUI中的ActionBar
    /// </summary>
    /// <param name="ap"></param>
    /// <param name="round"></param>
    public void SetActionBar(int ap)
    {
        m_UISystem.SetActionBar(ap);
    }


    public void SetRoundTag(int round)
    {
        m_UISystem.SetRoundTag(round);
    }


    public void ShowUI(string UIFormName)
    {
        m_UISystem.ShowUI(UIFormName);
    }

    public void CloseUI(string UIFormName)
    {
        m_UISystem.CloseUI(UIFormName);
    }

    public void CloseAll()
    {
        m_UISystem.CloseAll();
    }

    /// <summary>
    /// 发动弹窗
    /// </summary>
    /// <param name="content"></param>
    public void TriggerPopUp(string content)
    {
        m_UISystem.TriggerPopUp(content);
    }

    /// <summary>
    /// UI必须重写此方法才能调用
    /// </summary>
    /// <param name="UIFormName"></param>
    /// <param name="content"></param>
    public void UIShowMessag(string UIFormName , string content)
    {
        m_UISystem.ShowMessage(UIFormName, content);
    }
    #endregion
     


    #region 技能系统接口
    public SkillInstanceBase GetSkill(string skillName)
    {
        return m_SkillSystem.GetSkill(skillName);
    }

    public bool UnlockSkill(string skillName)
    {
        return m_SkillSystem.UnlockSkill(skillName);
    }

    public void UnlockAllSkill()
    {
        m_SkillSystem.UnlockAllSkill();
    }

    public void SetMainSkill(string skillName)
    {
        m_SkillSystem.SetMainSkill(skillName);
    }

    public SkillInstanceBase GetMainSkill()
    {
        return m_SkillSystem.GetMainSkill();
    }

    public void SetMainItems(List<string> mainItems)
    {
        m_SkillSystem.SetMainItems(mainItems);
    }

    public List<SkillInstanceBase> GetMainItems()
    {
        return m_SkillSystem.GetMainItems();
    }

    public List<SkillButtonStates> GetMainItemButtonState()
    {
        return m_SkillSystem.GetMainItemButtonState();
    }

    public void SetMainItemButtonState(List<SkillButtonStates> buttonStates)
    {
        m_SkillSystem.SetMainItemButtonState(buttonStates);
    }

    public Dictionary<string, SkillInstanceBase> GetUnlockSkills()
    {
        return m_SkillSystem.GetUnlockedSkills();
    }

    public Queue<SkillInstanceBase> GetSkillsToUnlock()
    {
        return m_SkillSystem.GetSkillsToUnlock();
    }

    public SkillInstanceBase GetExecutingSkill()
    {
        return m_SkillSystem.GetExecutingSkill();
    }

    public void SetExecutingSkill(SkillInstanceBase skill)
    {
        m_SkillSystem.SetExecutingSkill(skill);
    }

    public SkillInstanceBase GetSelectedSkill()
    {
        return m_SkillSystem.GetSelectedSkill();
    }

    public void SetSelectedSkill(SkillInstanceBase skill)
    {
        m_SkillSystem.SetSelectedSkill(skill);
    }

    public void ClearUnlockedSkills()
    {
        m_SkillSystem.ClearUnlockedSkill();
    }
    #endregion
}
