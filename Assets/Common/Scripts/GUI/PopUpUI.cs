using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : BaseUIForm
{
    public Text popUpText;

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Translucence;

        popUpText = UITool.GetUIComponent<Text>(this.gameObject, "TxtPopUp");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close()
    {
        UIManager.Instance.CloseOrReturnUIForms("PopUpUI");
    }

    private void OnEnable()
    {
        Game.Instance.SetCanInput(false);
    }

    private void OnDisable()
    {
        void action() { Game.Instance.SetCanInput(true); }
        CoroutineManager.StartCoroutineTask(action, 0.5f);
        
    }
}
