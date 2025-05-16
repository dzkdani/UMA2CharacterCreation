using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;

[DefaultExecutionOrder(1000)]
public class UMACharacterCreationMenu : MonoBehaviour
{
    [SerializeField] private DynamicCharacterAvatar avatar;

    [Header("UI Main Button")]
    public Button btnWardrobe;
    public Button btnDNA;

    [Header("UI Costume")]
    public Button btnChest;
    public Button btnHands;
    public Button btnLegs;
    public Button btnFeet;
    public Button btnUnderwear;
    public Button btnHelmet;

    [Header("UI Buttons")]
    public Button btnRace;
    public Button btnGender;
    public Button btnHair;
    public Button btnEye;
    public Button btnSkin;
    public Button btnHeight;
    public Button btnBelly;
    public Button btnGluteus;
    public Button btnLips;
    public Button btnMouth;
    public Button btnBreast;
    public Button btnWeight;

    [Header("Main Panel")]
    public GameObject DNAObj;
    public GameObject WardrobeObj;

    [Header("Target Panels / Objects")]
    public GameObject wardrobeObj;
    public GameObject raceObj;
    public GameObject GenderObj;
    public GameObject HairColorObj;
    public GameObject EyeObj;
    public GameObject SkinColorObj;
    public GameObject HeightObj;
    public GameObject BellyObj;
    public GameObject ButtObj;
    public GameObject LipSObj;
    public GameObject MouthSObj;
    public GameObject BreastSizeCleavageObj;
    public GameObject WeightObj;

    [Header("Color Picker")]
    public FlexibleColorPicker HairColor;
    public FlexibleColorPicker SkinColor;
    public FlexibleColorPicker EyeColor;

    [Header("Text")]
    [SerializeField] private Text textHair;
    [SerializeField] private Text textWardrobe;

    [Header("Slider")]
    [SerializeField] private Slider eyeSize;
    [SerializeField] private Slider height;
    [SerializeField] private Slider belly;
    [SerializeField] private Slider mouthSize;
    [SerializeField] private Slider lipsSize;
    [SerializeField] private Slider gluteusSize;
    [SerializeField] private Slider weightL;
    [SerializeField] private Slider weightU;
    [SerializeField] private Slider breastS;
    [SerializeField] private Slider hairType;
    [SerializeField] private Slider wardrobeSlider;

    [Header("Gender")]
    [SerializeField] private Button btnOther;
    [SerializeField] private Button btnMan;
    [SerializeField] private Button btnWoman;

    [Header("Prefab Button")]
    [SerializeField] private GameObject prefabrace;

    [Header("CameraTrans")]
    [SerializeField] private Transform currCam;
    [SerializeField] private Transform normalCam;
    [SerializeField] private Transform headCam;

    //Race Properties
    private List<RaceData> manRace;
    private List<RaceData> womanRace;
    private List<RaceData> otherRace;
    private RaceData currentRace;

    //Wardrobe Properties
    private List<UMAWardrobeRecipe> allWardrobes = new List<UMAWardrobeRecipe>();

    //Gender Properties
    private Enums.GenderType genderType;

    // DNA
    private Dictionary<string, DnaSetter> dna;
    private Dictionary<string, float> defaultDNA = new Dictionary<string, float>();

    //Hair
    private List<int> availableHair;
    private int currHair;

    //Wardrobe Properties
    private List<int> availableWardrobe;
    private int currWardrobe;

    private int currHelmet;
    private int currHands;
    private int currLegs;
    private int currFeet;
    private int currUnderwear;
    private int currChest;

    private void OnClickHeight() => OnClickSlider(CharacterFeatureID.Height, HeightObj, normalCam, height);
    private void OnClickBelly() => OnClickSlider(CharacterFeatureID.Belly, BellyObj, normalCam, belly);
    private void OnClickMouthS() => OnClickSlider(CharacterFeatureID.MouthS, MouthSObj, headCam, mouthSize);
    private void OnClickLipS() => OnClickSlider(CharacterFeatureID.LipS, LipSObj, headCam, lipsSize);
    private void OnClickButt() => OnClickSlider(CharacterFeatureID.Butt, ButtObj, normalCam, gluteusSize);

    private void Start()
    {
        InitializeUIButtons();
        InitializeDNA();
        InitializeAvatarEvents();
        InitializeWardrobe();
        InitializeSliders();
        InitializeDefaultValues();
        InitializeFeatures();
        InitializeRaceAndGender();
    }

    private void InitializeUIButtons()
    {
        btnDNA.onClick.AddListener(() => {
            DNAObj?.SetActive(true);
            WardrobeObj?.SetActive(false);
            OnClickRace();
        });

        btnWardrobe.onClick.AddListener(() => {
            DNAObj?.SetActive(false);
            WardrobeObj?.SetActive(true);
            OnClickWardrobe(CharacterFeatureID.ChestSlot);
        });

        btnRace.onClick.AddListener(OnClickRace);
        btnGender.onClick.AddListener(OnClickGender);
        btnHair.onClick.AddListener(OnClickHair);
        btnEye.onClick.AddListener(OnClickEye);
        btnSkin.onClick.AddListener(OnClickSkin);
        btnHeight.onClick.AddListener(OnClickHeight);
        btnBelly.onClick.AddListener(OnClickBelly);
        btnGluteus.onClick.AddListener(OnClickButt);
        btnLips.onClick.AddListener(OnClickLipS);
        btnMouth.onClick.AddListener(OnClickMouthS);
        btnBreast.onClick.AddListener(OnClickBreastSizeCleavage);
        btnWeight.onClick.AddListener(OnClickWeight);

        btnHelmet.onClick.AddListener(() => OnClickWardrobe(CharacterFeatureID.HelmetSlot));
        btnHands.onClick.AddListener(() => OnClickWardrobe(CharacterFeatureID.HandsSlot));
        btnChest.onClick.AddListener(() => OnClickWardrobe(CharacterFeatureID.ChestSlot));
        btnLegs.onClick.AddListener(() => OnClickWardrobe(CharacterFeatureID.LegsSlot));
        btnFeet.onClick.AddListener(() => OnClickWardrobe(CharacterFeatureID.FeetSlot));
        btnUnderwear.onClick.AddListener(() => OnClickWardrobe(CharacterFeatureID.Underwear));
    }

    private void InitializeDNA()
    {
        dna = avatar.GetDNA();
        SaveDefaultDNA();
    }

    private void InitializeAvatarEvents()
    {
        avatar.CharacterUpdated.AddListener((x) => OnAvatarBuilt(x));
    }

    private void InitializeWardrobe()
    {
        InitWardrobe();
    }

    private void InitializeSliders()
    {
        InitSlider(CharacterFeatureID.Height, height);
        InitSlider(CharacterFeatureID.Belly, belly);
        InitSlider(CharacterFeatureID.MouthS, mouthSize);
        InitSlider(CharacterFeatureID.LipS, lipsSize);
        InitSlider(CharacterFeatureID.Butt, gluteusSize);
        InitBreast();
        InitWeight();
    }

    private void InitializeFeatures()
    {
        currHair = 0;
        InitEye();
        InitSkin();
        InitHair();
    }

    private void InitializeRaceAndGender()
    {
        OnClickRace();
        InitGenderRace();
    }

    private void InitializeDefaultValues()
    {
        avatar.ChangeRace("humanMale");
        currentRace = avatar.activeRace.data;

        currHelmet = 0;
        currHands = 0;
        currLegs = 0;
        currFeet = 0;
        currUnderwear = 0;
        currChest = 0;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            avatar.transform.Rotate(new Vector3(0, 1, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            avatar.transform.Rotate(new Vector3(0, -1, 0));
        }

        UpdateCam();
    }

    private void DeactivateAll()
    {
        wardrobeObj?.SetActive(false);

        raceObj?.SetActive(false);
        GenderObj?.SetActive(false);
        HairColorObj?.SetActive(false);
        SkinColorObj?.SetActive(false);
        HeightObj?.SetActive(false);
        BellyObj?.SetActive(false);
        ButtObj?.SetActive(false);
        LipSObj?.SetActive(false);
        MouthSObj?.SetActive(false);
        BreastSizeCleavageObj?.SetActive(false);
        WeightObj?.SetActive(false);

        EyeObj?.SetActive(false);

        currCam.position = normalCam.position;
    }

    #region AvatarEvent
    private void OnAvatarBuilt(UMAData _umaData)
    {
        dna = avatar.GetDNA();

        avatar.SetColorValue(CharacterFeatureID.HairColor, HairColor.color);
        avatar.SetColorValue(CharacterFeatureID.SkinColor, SkinColor.color);
        avatar.SetColorValue(CharacterFeatureID.EyeColor, EyeColor.color);
        avatar.UpdateColors(true);
    }
    #endregion

    #region Camera
    private void UpdateCam()
    {
        UMAData umaData = avatar.umaData;
        int headHash = UMAUtils.StringToHash(CharacterFeatureID.HeadSkeleton); // Or use UMASkeleton.StringToHash("Head")
        Transform headTransform = umaData.skeleton.GetBoneTransform(headHash);
        headCam.position = new Vector3(headCam.position.x, headTransform.position.y, headCam.position.z);
    }
    #endregion

    #region DNA
    private void SaveDefaultDNA()
    {
        foreach (var kvp in dna)
        {
            defaultDNA[kvp.Key] = kvp.Value.Get();
        }
    }

    private void ResetDNA()
    {
        var dna = avatar.GetDNA();
        foreach (var kvp in defaultDNA)
        {
            if (dna.TryGetValue(kvp.Key, out var dnaValue))
                dnaValue.Set(kvp.Value);
        }

        avatar.BuildCharacter();
    }

    #endregion

    #region Wardrobe
    private void InitWardrobe()
    {
        var wardrobeAssets = UMAAssetIndexer.Instance.GetAllAssets<UMAWardrobeRecipe>();

        foreach (var wardrobe in wardrobeAssets)
        {
            allWardrobes.Add(wardrobe);
            //  Debug.Log("Found wardrobe: " + wardrobe.name);
        }
    }
    private void OnClickWardrobe(string _slot)
    {
        DeactivateAll();
        wardrobeObj?.SetActive(true);
        currCam.position = normalCam.position;

        InitWardrobe(_slot);
    }
    private void InitWardrobe(string _slot)
    {
        wardrobeSlider.minValue = 0;
        wardrobeSlider.onValueChanged.RemoveAllListeners();
        availableWardrobe?.Clear();

        availableWardrobe = new();

        for (int i = 0; i < allWardrobes.Count; i++)
        {
            if (allWardrobes[i].wardrobeSlot.Equals(_slot) && allWardrobes[i].compatibleRaces.Contains(currentRace.raceName))
            {
                availableWardrobe.Add(i);
            }
        }

        if (availableWardrobe.Count > 0)
        {
            wardrobeSlider.gameObject.SetActive(true);
            wardrobeSlider.maxValue = availableWardrobe.Count;
        }
        else wardrobeSlider.gameObject.SetActive(false);

        switch (_slot)
        {
            case CharacterFeatureID.HelmetSlot:
                currWardrobe = currHelmet;
                wardrobeSlider.value = currWardrobe;
                break;
            case CharacterFeatureID.HandsSlot:
                currWardrobe = currHands;
                wardrobeSlider.value = currWardrobe;
                break;
            case CharacterFeatureID.ChestSlot:
                currWardrobe = currChest;
                wardrobeSlider.value = currWardrobe;
                break;
            case CharacterFeatureID.LegsSlot:
                currWardrobe = currLegs;
                wardrobeSlider.value = currWardrobe;
                break;
            case CharacterFeatureID.FeetSlot:
                currWardrobe = currFeet;
                wardrobeSlider.value = currWardrobe;
                break;
            case CharacterFeatureID.Underwear:
                currWardrobe = currUnderwear;
                wardrobeSlider.value = currWardrobe;
                break;
        }


        int temp = currWardrobe - 1;
        if (currWardrobe == 0)
        {
            textWardrobe.text = "None";
        }
        else
        {
            if (allWardrobes[availableWardrobe[temp]].DisplayValue.Equals(""))
            {
                textWardrobe.text = allWardrobes[availableWardrobe[temp]].name;
            }
            else textWardrobe.text = allWardrobes[availableWardrobe[temp]].DisplayValue;
        }

        wardrobeSlider.onValueChanged.AddListener((x) => {
            currWardrobe = Mathf.RoundToInt(x);
            if (availableWardrobe.Count <= 0)
            {
                return;
            }

            switch (_slot)
            {
                case CharacterFeatureID.HelmetSlot:
                    currHelmet = currWardrobe;
                    break;
                case CharacterFeatureID.HandsSlot:
                    currHands = currWardrobe;
                    break;
                case CharacterFeatureID.ChestSlot:
                    currChest = currWardrobe;
                    break;
                case CharacterFeatureID.LegsSlot:
                    currLegs = currWardrobe;
                    break;
                case CharacterFeatureID.FeetSlot:
                    currFeet = currWardrobe;
                    break;
                case CharacterFeatureID.Underwear:
                    currUnderwear = currWardrobe;
                    break;
            }


            int temp = currWardrobe - 1;
            if (currWardrobe == 0)
            {
                avatar.ClearSlot(_slot);
                textWardrobe.text = "None";
            }
            else
            {
                if (allWardrobes[availableWardrobe[temp]].DisplayValue.Equals(""))
                {
                    textWardrobe.text = allWardrobes[availableWardrobe[temp]].name;
                }
                else textWardrobe.text = allWardrobes[availableWardrobe[temp]].DisplayValue;

                avatar.SetSlot(allWardrobes[availableWardrobe[temp]]);
            }
            avatar.BuildCharacter();
        });

    }
    #endregion

    #region Race & Gender
    private void InitGenderRace()
    {
        genderType = 0;

        //0male 1female 2other

        btnMan.onClick.AddListener(() => {
            //if (genderType > 1)
            //{
            //    genderType = 0;
            //    ChangeRace(manRace[0].name);
            //    return;
            //}
            genderType = Enums.GenderType.MALE;
            //string replacedGender = ReplaceGender(currentRace.name);
            ChangeRace(manRace[0].name);
        });
        btnWoman.onClick.AddListener(() => {
            //if (genderType > 1)
            //{
            //    genderType = 1;
            //    ChangeRace(womanRace[0].name);
            //    return;
            //}
            genderType = Enums.GenderType.FEMALE;
            //string replacedGender = ReplaceGender(currentRace.name);
            ChangeRace(womanRace[0].name);
        });
        //btnOther.onClick.AddListener(() => {
        //    genderType = 2;
        //    if (otherRace.Count > 0) ChangeRace(otherRace[0].name);
        //});
    }
    private void OnClickRace()
    {
        DeactivateAll();

        Enums.GenderType tempGender = Enums.GenderType.NONE;

        // Ensure lists are initialized
        //createdrace ??= new List<Button>();
        manRace ??= new List<RaceData>();
        womanRace ??= new List<RaceData>();
        otherRace ??= new List<RaceData>();

        foreach (Transform child in raceObj.transform.GetChild(0).GetChild(0))
        {
            Destroy(child.gameObject);
        }

        foreach (var item in avatar.context.GetAllRaces())
        {
            RaceData matchedRace = null;

            if (womanRace.Contains(item))
            {
                tempGender = Enums.GenderType.FEMALE;
                matchedRace = item;
            }
            else if (manRace.Contains(item))
            {
                tempGender = Enums.GenderType.MALE;
                matchedRace = item;
            }
            else if (otherRace.Contains(item))
            {
                tempGender = Enums.GenderType.OTHER;
                matchedRace = item;
            }

            if (matchedRace == null)
            {
                string lowerName = item.name.ToLower();
                matchedRace = item;

                if (lowerName.Contains("female") || lowerName.Contains("girl"))
                {
                    womanRace.Add(item);
                    tempGender = Enums.GenderType.FEMALE;
                }
                else if (lowerName.Contains("male") || lowerName.Contains("boy"))
                {
                    manRace.Add(item);
                    tempGender = Enums.GenderType.MALE;
                }
                else
                {
                    tempGender = Enums.GenderType.OTHER;
                    otherRace.Add(item);
                }
            }

            if (matchedRace == null) continue;

            if (!genderType.Equals(tempGender)) continue;

            GameObject newBtnObj = Instantiate(prefabrace, raceObj.transform.GetChild(0).GetChild(0));
            Button btn = newBtnObj.GetComponent<Button>();
            Text btnText = newBtnObj.GetComponentInChildren<Text>();

            string raceNameCopy = matchedRace.name;
            btn.name = raceNameCopy;
            btnText.text = raceNameCopy;


            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                //RaceData raceToUse = null;
                //string nameCheck = raceNameCopy.ToLower();

                //if (nameCheck.Contains("female") || nameCheck.Contains("girl"))
                //    raceToUse = womanRace.Find(r => r.name == raceNameCopy);
                //else if (nameCheck.Contains("male") || nameCheck.Contains("boy"))
                //    raceToUse = manRace.Find(r => r.name == raceNameCopy);
                //else
                //    raceToUse = otherRace.Find(r => r.name == raceNameCopy);

                //if (raceToUse != null)
                //{
                //    dCA.ChangeRace(raceToUse);
                //    currentRace = raceToUse;
                //}
                ChangeRace(raceNameCopy);
            });
        }

        raceObj?.SetActive(true);
    }

    private void ChangeRace(string _raceName)
    {
        RaceData raceToUse = null;
        string nameCheck = _raceName.ToLower();

        if (nameCheck.Contains("female") || nameCheck.Contains("girl"))
            raceToUse = womanRace.Find(r => r.name == _raceName);
        else if (nameCheck.Contains("male") || nameCheck.Contains("boy"))
            raceToUse = manRace.Find(r => r.name == _raceName);
        else
            raceToUse = otherRace.Find(r => r.name == _raceName);

        avatar.ClearSlot(CharacterFeatureID.HelmetSlot);
        avatar.ClearSlot(CharacterFeatureID.HandsSlot);
        avatar.ClearSlot(CharacterFeatureID.ChestSlot);
        avatar.ClearSlot(CharacterFeatureID.LegsSlot);
        avatar.ClearSlot(CharacterFeatureID.FeetSlot);
        avatar.ClearSlot(CharacterFeatureID.Underwear);

        if (raceToUse != null)
        {
            avatar.ChangeRace(raceToUse);
            currentRace = raceToUse;
        }

        avatar.ClearSlot(CharacterFeatureID.HairSlot);
        currHair = 0;

        ResetDNA();
    }

    private void OnClickGender()
    {
        DeactivateAll();
        GenderObj?.SetActive(true);
    }

    #endregion

    #region Eye
    private void OnClickEye()
    {
        DeactivateAll();
        EyeObj?.SetActive(true);
        currCam.position = headCam.position;

        eyeSize.value = dna[CharacterFeatureID.EyeSize].Get();

        avatar.characterColors.GetColor(CharacterFeatureID.EyeColor, out Color outputCol);
        EyeColor.SetColor(outputCol);

    }

    private void InitEye()
    {
        avatar.characterColors.GetColor(CharacterFeatureID.EyeColor, out Color outputCol);
        EyeColor.SetColor(outputCol);

        eyeSize.minValue = 0;
        eyeSize.maxValue = 1;

        eyeSize.value = dna[CharacterFeatureID.EyeSize].Get();
        eyeSize.onValueChanged.AddListener((x) => {
            dna[CharacterFeatureID.EyeSize].Set(x);
            avatar.BuildCharacter();
        });
    }
    #endregion

    #region Hair
    private void OnClickHair()
    {
        DeactivateAll();
        HairColorObj?.SetActive(true);
        currCam.position = headCam.position;

        InitHair();
    }
    private void InitHair()
    {
        avatar.characterColors.GetColor(CharacterFeatureID.HairColor, out Color outputCol);
        HairColor.SetColor(outputCol);

        hairType.minValue = 0;
        hairType.onValueChanged.RemoveAllListeners();
        availableHair?.Clear();

        availableHair = new();

        for (int i = 0; i < allWardrobes.Count; i++)
        {
            if (allWardrobes[i].wardrobeSlot.Equals(CharacterFeatureID.HairSlot) && allWardrobes[i].compatibleRaces.Contains(currentRace.raceName))
            {
                availableHair.Add(i);
            }
        }

        if (availableHair.Count > 0)
        {
            hairType.gameObject.SetActive(true);
            hairType.maxValue = availableHair.Count;
        }
        else hairType.gameObject.SetActive(false);

        hairType.value = currHair;

        int temp = currHair - 1;
        if (currHair == 0)
        {
            textHair.text = "None";
        }
        else
        {
            if (allWardrobes[availableHair[temp]].DisplayValue.Equals(""))
            {
                textHair.text = allWardrobes[availableHair[temp]].name;
            }
            else textHair.text = allWardrobes[availableHair[temp]].DisplayValue;
        }

        hairType.onValueChanged.AddListener((x) => {
            currHair = Mathf.RoundToInt(x);
            if (availableHair.Count <= 0)
            {
                return;
            }

            int temp = currHair - 1;
            if (currHair == 0)
            {
                avatar.ClearSlot(CharacterFeatureID.HairSlot);
                textHair.text = "None";
            }
            else
            {
                if (allWardrobes[availableHair[temp]].DisplayValue.Equals(""))
                {
                    textHair.text = allWardrobes[availableHair[temp]].name;
                }
                else textHair.text = allWardrobes[availableHair[temp]].DisplayValue;

                avatar.SetSlot(allWardrobes[availableHair[temp]]);
            }
            avatar.BuildCharacter();
        });

    }
    #endregion

    #region Skin
    private void OnClickSkin()
    {
        DeactivateAll();
        SkinColorObj?.SetActive(true);

        avatar.characterColors.GetColor(CharacterFeatureID.SkinColor, out Color outputCol);
        SkinColor.SetColor(outputCol);
    }

    private void InitSkin()
    {
        avatar.characterColors.GetColor(CharacterFeatureID.SkinColor, out Color outputCol);
        SkinColor.SetColor(outputCol);
    }
    #endregion

    #region Weight
    private void OnClickWeight()
    {
        DeactivateAll();
        WeightObj?.SetActive(true);
        weightL.value = dna[CharacterFeatureID.WeightL].Get();
        weightU.value = dna[CharacterFeatureID.WeightU].Get();
    }

    private void InitWeight()
    {
        InitSlider(CharacterFeatureID.WeightL, weightL);
        InitSlider(CharacterFeatureID.WeightU, weightU);
    }
    #endregion

    #region Breast
    private void OnClickBreastSizeCleavage()
    {
        DeactivateAll();
        BreastSizeCleavageObj?.SetActive(true);
        breastS.value = dna[CharacterFeatureID.BoobS].Get();
    }

    private void InitBreast()
    {
        InitSlider(CharacterFeatureID.BoobS, breastS);
    }
    #endregion

    #region Slider
    private void InitSlider(string _name, Slider _slider)
    {
        _slider.minValue = 0;
        _slider.maxValue = 1;

        _slider.value = dna[_name].Get();
        _slider.onValueChanged.AddListener((x) => {
            dna[_name].Set(x);
            avatar.BuildCharacter();
        });
    }

    private void OnClickSlider(string _name, GameObject _panel, Transform _target, Slider _slider)
    {
        DeactivateAll();
        _panel?.SetActive(true);
        currCam.position = _target.position;

        _slider.value = dna[_name].Get();
    }
    #endregion
}
