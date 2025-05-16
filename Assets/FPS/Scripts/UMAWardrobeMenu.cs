using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;

public class UMAWardrobeMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform Content;
    [SerializeField] private Text textWardrobe;

    [Header("Prefab Button")]
    [SerializeField] private GameObject prefabButton;

    [Header("Sprite")]
    [SerializeField] private Sprite noIcon;
    [SerializeField] private Sprite noneIcon;

    [Header("Another Panel (Optional)")]
    [SerializeField] private Toggle switchPanel;
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform otherPanel;

    [Header("Predefined Buttons")]
    [SerializeField] private List<Button> buttons;

    private DynamicCharacterAvatar avatar;

    private List<UMAWardrobeRecipe> clothsTaken = new List<UMAWardrobeRecipe>();
    private Func<string, string, List<UMAWardrobeRecipe>> getCloths;

    private string currSlot;
    private string currRace;
    private int currWardrobe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switchPanel?.onValueChanged.AddListener((x) => OnValueChangeSwitch(x));
        currWardrobe = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitWardrobe(DynamicCharacterAvatar _avatar, string _slot, Func<string, string, List<UMAWardrobeRecipe>> _getCloths)
    {
        avatar = _avatar;
        currSlot = _slot;
        getCloths = _getCloths;
        InitWardrobe();
    }

    private void InitWardrobe()
    {
        currRace = avatar.activeRace.racedata.raceName;

        clothsTaken?.Clear();
        clothsTaken = getCloths.Invoke(currSlot, currRace);


        textWardrobe.text = "None";

        if (clothsTaken.Count <= 0)
        {
            foreach (var item in buttons)
            {
                item.gameObject.SetActive(false);
            }
            return;
        }

        CreateNoneButton();

        if (buttons.Count > clothsTaken.Count)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                if (i>= clothsTaken.Count)
                {
                    DisableButton(index);
                }
                else CreateButton(index);
            }
        }
        else
        {
            for (int i = 0; i < clothsTaken.Count; i++)
            {
                int index = i;
                CreateButton(index);
            }
        }
    }

    private void DisableButton(int _index)
    {
        int temp = _index + 1;

        try
        {
            buttons[temp].onClick.RemoveAllListeners();
            buttons[temp].gameObject.SetActive(false);
        }
        catch { }
    }

    private void CreateButton(int _index)
    {
        int temp = _index + 1;
        Button newButton;
        try
        {
            newButton = buttons[temp];
            newButton.onClick.RemoveAllListeners();
            newButton.gameObject.SetActive(true);
            newButton.onClick.AddListener(() => {
                OnClickOneRecipe(_index);
            });

        }
        catch
        {
            newButton = Instantiate(prefabButton, Content).GetComponent<Button>();
            newButton.onClick.AddListener(() => {
                OnClickOneRecipe(_index);
            });

            buttons.Add(newButton);
        }

        Sprite sprite = clothsTaken[_index].GetWardrobeRecipeThumbFor(currRace);
        if (sprite == null) sprite = noIcon;
        newButton.image.sprite = sprite;
    }

    private void CreateNoneButton()
    {
        Button newButton;
        try
        {
            newButton = buttons[0];
            newButton.onClick.RemoveAllListeners();
            newButton.gameObject.SetActive(true);
            newButton.onClick.AddListener(() => {
                currWardrobe = 0;
                avatar.ClearSlot(currSlot);
                textWardrobe.text = "None";
                avatar.BuildCharacter();
            });

        }
        catch
        {
            newButton = Instantiate(prefabButton, Content).GetComponent<Button>();
            newButton.onClick.AddListener(() => {
                currWardrobe = 0;
                avatar.ClearSlot(currSlot);
                textWardrobe.text = "None";
                avatar.BuildCharacter();
            });

            buttons.Add(newButton);
        }

        newButton.image.sprite = noneIcon;
    }

    private void OnClickOneRecipe(int _index)
    {
        currWardrobe = _index + 1;

        if (clothsTaken[_index].DisplayValue.Equals(""))
        {
            textWardrobe.text = clothsTaken[_index].name;
        }
        else textWardrobe.text = clothsTaken[_index].DisplayValue;

        avatar.SetSlot(clothsTaken[_index]);
        avatar.BuildCharacter();
    }

    public void SetActive(bool _active)
    {
        if (_active)
        {
            if (currRace.Equals(avatar.activeRace.racedata.raceName))
            {
                gameObject.SetActive(_active);
                return;
            }

            InitWardrobe();
        }
        else
        {
            if(switchPanel != null) switchPanel.isOn = true;
        }

        gameObject.SetActive(_active);
    }

    #region AnotherPanel
    private void OnValueChangeSwitch(bool _isOn)
    {
        if (_isOn)
        {
            mainPanel.gameObject.SetActive(true);
            otherPanel.gameObject.SetActive(false);
        }
        else
        {
            mainPanel.gameObject.SetActive(false);
            otherPanel.gameObject.SetActive(true);
        }

    }
    #endregion

}
