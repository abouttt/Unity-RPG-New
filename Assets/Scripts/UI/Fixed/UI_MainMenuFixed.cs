using UnityEngine;

public class UI_MainMenuFixed : UI_Base
{
    enum Buttons
    {
        ContinueButton,
        NewGameButton,
        OptionButton,
        BackButton,
        ExitButton,
    }

    protected override void Init()
    {
        Managers.UI.Register<UI_MainMenuFixed>(this);

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.ContinueButton).onClick.AddListener(() =>
        {

        });

        GetButton((int)Buttons.NewGameButton).onClick.AddListener(() =>
        {
            Managers.Scene.LoadScene(SceneType.VillageScene);
        });

        GetButton((int)Buttons.OptionButton).onClick.AddListener(() => SetActiveOptionMenu(true));

        GetButton((int)Buttons.BackButton).onClick.AddListener(() => SetActiveOptionMenu(false));

        GetButton((int)Buttons.ExitButton).onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    private void Start()
    {
        SetActiveOptionMenu(false);
    }

    private void SetActiveOptionMenu(bool active)
    {
        if (active)
        {
            Managers.UI.Show<UI_GameOptionPopup>();
        }
        else
        {
            Managers.UI.Close<UI_GameOptionPopup>();
        }

        GetButton((int)Buttons.ContinueButton).gameObject.SetActive(false);
        GetButton((int)Buttons.NewGameButton).gameObject.SetActive(!active);
        GetButton((int)Buttons.OptionButton).gameObject.SetActive(!active);
        GetButton((int)Buttons.BackButton).gameObject.SetActive(active);
        GetButton((int)Buttons.ExitButton).gameObject.SetActive(!active);
    }
}