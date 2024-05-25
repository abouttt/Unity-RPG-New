using UnityEngine;
using DG.Tweening;

public class UI_TopCanvas : UI_Base
{
    enum GameObjects
    {
        InitBG,
    }

    protected override void Init()
    {
        Managers.UI.Register<UI_TopCanvas>(this);

        BindObject(typeof(GameObjects));
    }

    public void FadeInitBG()
    {
        GetObject((int)GameObjects.InitBG).GetComponent<DOTweenAnimation>().DOPlay();
    }
}
