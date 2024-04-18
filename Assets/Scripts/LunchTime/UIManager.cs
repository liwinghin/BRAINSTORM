using System.Collections.Generic;
using UnityEngine;
using AnKuchen.Map;
using UnityEngine.UI;
using UniRx;
using TMPro;

namespace LunchScene
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Generator generator;
        [SerializeField] private TextMeshProUGUI resText;

        // Start is called before the first frame update
        void Start()
        {
            generator = GetComponent<Generator>(); 
            generator.GeneratedResult.Subscribe(res => 
            {
                resText.text = $"Result: {res}";
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}