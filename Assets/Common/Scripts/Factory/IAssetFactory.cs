using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAssetFactory
{
    public abstract GameObject LoadModel(string AssetName, Vector3 Position);

    public abstract GameObject LoadUI(string UIName);
}
