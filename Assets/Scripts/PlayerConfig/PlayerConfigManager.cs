using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using AnKuchen.Map;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Mode
{
    main,
    save,
    load
}
[Serializable]
public class PlayerInfo
{
    public int soundVolume;
    public int armPower;
    
    public PlayerInfo(int s, int a)
    {
        soundVolume = s;
        armPower = a;
    }
}
[Serializable]
public class PlayerConfig
{
    public Image img;
    public Toggle toggle;
    public TextMeshProUGUI text;
    public Color selectedColor;
    public Color normalColor;
    public PlayerInfo info = new PlayerInfo(0, 0);

    public PlayerConfig(Image i, Toggle t, TextMeshProUGUI te, Color s, Color n)
    {
        img = i;
        toggle = t;
        text = te;
        selectedColor = s;
        normalColor = n;
    }
    public void SetText(string t)
    {
        text?.SetText(t);
    }
    public void SetInfo(PlayerInfo i)
    {
        info = i;
    }
}
public class PlayerConfigManager : MonoBehaviour
{
    [SerializeField] private string machineName = "123456";
    [SerializeField] private UICache ui;
    [SerializeField] private PlayerConfigDataList dataList;
    [SerializeField] private GameObject uiPrefab;

    [Header("Page")]
    [SerializeField] private GameObject homePage;
    [SerializeField] private GameObject savePage;
    [SerializeField] private GameObject loadPage;

    private ReactiveProperty<Mode> mode = new(Mode.main);
    private Dictionary<Mode, GameObject> modeToPageMap;

    [SerializeField] private BoolReactiveProperty isUSBConnected = new (false);

    [SerializeField] private PlayerInfo info = new PlayerInfo(10, 10);
    [SerializeField] private List<PlayerConfig> configs = new List<PlayerConfig>();
    [SerializeField] private PlayerConfig currentConfig = null;

    void Awake()
    {
        modeToPageMap = new Dictionary<Mode, GameObject>
        {
            { Mode.main, homePage },
            { Mode.save, savePage },
            { Mode.load, loadPage }
        };
        Init();
    }

    private void Start()
    {
        isUSBConnected.Value = false;
    }

    private void Init()
    {
        mode.Pairwise().Subscribe(pair => SetPageActive(pair.Previous, pair.Current)).AddTo(this);
        InitHomePage();
        InitSavePage();     
    }
    private void InitHomePage()
    {
        var hp = ui.GetMapper("HomePage");
        hp.Get<Button>("SaveButton").OnClickAsObservable().Subscribe(_ => ChangeToSaveMode()).AddTo(this);
        hp.Get<Button>("LoadButton").OnClickAsObservable().Subscribe(_ => ChangeToLoadMode()).AddTo(this);
    }
    private void InitSavePage()
    {
        CheckUSBStatus();
        var sp = ui.GetMapper("SavePage");
        sp.Get<Button>("Back_btn").OnClickAsObservable().Subscribe(_ => BackToMain()).AddTo(this);
        //savePage Button
        sp.Get<Button>("Save_btn").OnClickAsObservable().Subscribe(_ => Save()).AddTo(this);
        //savePage prefab
        var parent = sp.GetMapper("BG/Config List/Scroll View/Viewport/Content").Get<RectTransform>();

        foreach (var playerConfigData in dataList.dataList)
        {
            GameObject uiInstance = Instantiate(uiPrefab, parent);
            TextMeshProUGUI uiText = uiInstance.GetComponentInChildren<TextMeshProUGUI>();

            var img = uiInstance.GetComponent<Image>();
            var toggle = uiInstance.GetComponent<Toggle>();
            toggle.group = parent.GetComponent<ToggleGroup>();

            var newConfig = new PlayerConfig(img, toggle, uiText, new Color(1, 0.27f, 0.55f), new Color(0.74f, 0.74f, 0.74f));
            newConfig.SetText(playerConfigData?.playerConfigName);

            newConfig.toggle.onValueChanged.AsObservable().Subscribe(isOn =>
            {
                Color32 c = isOn ? newConfig.selectedColor : newConfig.normalColor;
                newConfig.img.color = c;
                currentConfig = isOn ? newConfig : null;
            }).AddTo(this);

            configs.Add(newConfig);
        }
        var saveToUSB = parent.transform.GetChild(10);

        isUSBConnected.Subscribe(isConnected =>
        {
            if (isConnected)
            {
                saveToUSB.transform.SetAsFirstSibling();
            }
            else
            {
                saveToUSB.transform.SetAsLastSibling();
            }
        }).AddTo(this);
    }
    private void Save()
    {
        currentConfig.SetText(machineName);
        currentConfig.SetInfo(info);
    }
    private void CheckUSBStatus()
    {
        var connectedToUsb = DriveInfo.GetDrives().Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);
        connectedToUsb.ObserveEveryValueChanged(_ => connectedToUsb.Count()).Subscribe(c => isUSBConnected.Value = c > 0 ? true : false).AddTo(this);
    }
    void SetPageActive(Mode p, Mode c)
    {
        if (modeToPageMap.TryGetValue(p, out GameObject pPage))
        {
            pPage.SetActive(false);
        }
        if (modeToPageMap.TryGetValue(c, out GameObject cPage))
        {
            cPage.SetActive(true);
        }
    }
    void ChangeToSaveMode()
    {
        mode.Value = Mode.save;
    }
    void ChangeToLoadMode()
    {
        mode.Value = Mode.load;
    }
    void BackToMain()
    {
        mode.Value = Mode.main;
    }
}
