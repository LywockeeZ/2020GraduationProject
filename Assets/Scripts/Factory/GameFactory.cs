using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFactory 
{
    private static IAssetFactory m_AssetFactory = null;
    private static IGameUnitFactory m_GameUnitFactory = null;
    private static IDataFactory m_DataFactory = null;

    //获取资源工厂
    public static IAssetFactory GetAssetFactory()
    {
        if (m_AssetFactory == null)
        {
            m_AssetFactory = new ResourceAssetFactory();
        }
        return m_AssetFactory;
    }

    //获取游戏单元制造工厂
    public static IGameUnitFactory GetGameUnitFactory()
    {
        if (m_GameUnitFactory == null)
        {
            m_GameUnitFactory = new GameUnitFactory();
        }
        return m_GameUnitFactory;

    }

    //获取读取文件数据的工厂
    public static IDataFactory GetDataFactory()
    {
        if (m_DataFactory == null)
        {
            m_DataFactory = new XMLDataFactory();
        }
        return m_DataFactory;
    }
}
