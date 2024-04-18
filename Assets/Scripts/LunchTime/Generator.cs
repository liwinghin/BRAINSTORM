using System.Collections.Generic;
using UnityEngine;
using AnKuchen.Map;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class Generator : MonoBehaviour
{
    [SerializeField] private List<string> randomList = new List<string>();
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject itemsParent;
    [SerializeField] private GameObject itemsPrefab;
    [Header("Input")]
    [SerializeField] private TMP_InputField inputField;
    public Subject<string> GeneratedResult = new Subject<string>();

    private void Start()
    {
        var ui = root.GetComponent<UICache>();
        ui.Get<Button>("Btn_Random")
             .OnClickAsObservable()
             .Subscribe(_ =>
             {
                 StartRandom();
             }).AddTo(this);

        ui.Get<Button>("Btn_Add")
             .OnClickAsObservable()
             .Subscribe(_ =>
             {
                 AddItems();
             }).AddTo(this);
    }

    public void StartRandom()
    {
        int res = Random.Range(0, randomList.Count - 1);
        GeneratedResult.OnNext(randomList[res]);
    }

    private void AddItems()
    {
        string text = inputField.text;

        if (!string.IsNullOrEmpty(text)) 
        {
            randomList.Add(text);
            Instantiate(itemsPrefab, itemsParent.transform);
        }
    }
}
