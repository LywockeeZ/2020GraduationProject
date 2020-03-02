using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TriggersLevel1 : MonoBehaviour
{
    private bool isFirstTime = true;

    public void Trigger1()
    {
        if (isFirstTime)
        {
            Game.Instance.TriggerPopUp("鼠标点击角色邻近单元来移动角色\n每一点行动点数可使角色移动一个单元");
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, "Level1");
            Game.Instance.UIShowMessag("TestStartUI", "选择一个技能来开始关卡\n技能每个回合只能使用一次");
            isFirstTime = false;
        }
        else
        {
            Game.Instance.UIShowMessag("TestStartUI", "选择一个技能来开始关卡\n技能每个回合只能使用一次");
        }


    }

    public void Trigger2()
    {
        Game.Instance.TriggerPopUp("火焰每回合结束会向四周扩散\n角色走过的地方会形成阻燃带防止火焰扩散");
        TeachStageHandler stageHandler = Game.Instance.GetCurrentStage() as TeachStageHandler;
        stageHandler.GetBaseUnit(12, 1).SetState(new Fire(stageHandler.GetBaseUnit(12, 1)));
    }

    public void Trigger3()
    {
        Game.Instance.TriggerPopUp("水桶等物体会被火焰烧毁而对周围产生影响\n也可由玩家花费行动点数手动破坏");
        TeachStageHandler stageHandler = Game.Instance.GetCurrentStage() as TeachStageHandler;
        stageHandler.GetBaseUnit(4, 1).SetState(new Fire(stageHandler.GetBaseUnit(4, 1)));
        stageHandler.GetBaseUnit(4, 3).SetState(new Fire(stageHandler.GetBaseUnit(4, 3)));
        GameFactory.GetGameUnitFactory().BuildUpperUnit(ENUM_Build_UpperUnit.WaterTank, stageHandler.GetBaseUnit(3, 2));

    }

}
