using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TriggersLevel1 : MonoBehaviour
{

    public void Trigger1()
    {
        Flowchart.BroadcastFungusMessage("PopUp1");
        //Game.Instance.TriggerPopUp("鼠标点击角色邻近单元来移动角色\n每一点行动点数可使角色移动一个单元");
    }

    public void Trigger2()
    {
        Flowchart.BroadcastFungusMessage("PopUp2");
        //Game.Instance.TriggerPopUp("火焰每回合结束会向四周扩散\n角色走过的地方会形成阻燃带防止火焰扩散");
        TeachStageHandler stageHandler = Game.Instance.GetCurrentStage() as TeachStageHandler;
        stageHandler.GetBaseUnit(12, 1).SetState(new Fire(stageHandler.GetBaseUnit(12, 1)));
    }

    public void Trigger3()
    {
        Flowchart.BroadcastFungusMessage("PopUp3");
        //Game.Instance.TriggerPopUp("水桶等物体会被火焰烧毁而对周围产生影响\n也可由玩家花费行动点数手动破坏");
        TeachStageHandler stageHandler = Game.Instance.GetCurrentStage() as TeachStageHandler;
        stageHandler.GetBaseUnit(4, 1).SetState(new Fire(stageHandler.GetBaseUnit(4, 1)));
        stageHandler.GetBaseUnit(4, 3).SetState(new Fire(stageHandler.GetBaseUnit(4, 3)));
        GameFactory.GetGameUnitFactory().BuildUpperUnit(ENUM_Build_UpperUnit.WaterTank, stageHandler.GetBaseUnit(3, 2));
    }

    public void Trigger4()
    {
        Game.Instance.LoadLevelOnMain("Part2", "Part2");
    }

    public void Trigger5()
    {
        Flowchart.BroadcastFungusMessage("主角1");
    }

    public void TriggerPart2_1()
    {
        Game.Instance.LoadLevelOnMain("Part2-1", "Level2");
    }

    public void TriggerPart4()
    {
        Game.Instance.LoadLevelOnMain("Part4", "Part4");
    }

    public void TriggerPart5()
    {
        Game.Instance.LoadLevelOnMain("Part5", "Part5");
    }

    public void TriggerPart5_1()
    {
        Game.Instance.LoadLevelOnMain("Part5-1", "Part5-1");
    }

    public void Test()
    {
        //Game.Instance.LoadLevelOnMain("Level1");
    }

    public void Test2()
    {
        //Game.Instance.LoadLevelOnMain("Part1");
    }

}
