using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;

using Random = UnityEngine.Random;

public class RandomizePreset : MonoBehaviour
{
    [Header("Button Executor")]
    [SerializeField] private Button button;

    [Header("Shared Color Table")]
    [SerializeField] private SharedColorTable eyeColor;
    [SerializeField] private SharedColorTable hairColor;
    [SerializeField] private SharedColorTable skinColor;

    [Header("Limiter (Optional)")]
    [SerializeField] private List<RandomizeLimiter> limiters;

    #region Unity

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    public void InitRandomizeColor(string _name, Action<Color> _changeColor)
    {
        switch (_name)
        {
            case CharacterFeatureID.EyeColor:
                button.onClick.AddListener(() => {
                    int index = Random.Range(0, eyeColor.colors.Length);
                    _changeColor.Invoke(eyeColor.colors[index].color);
                });
                break;
            case CharacterFeatureID.HairColor:
                button.onClick.AddListener(() => {
                    int index = Random.Range(0, hairColor.colors.Length);
                    _changeColor.Invoke(hairColor.colors[index].color);
                });
                break;
            case CharacterFeatureID.SkinColor:
                button.onClick.AddListener(() => {
                    int index = Random.Range(0, skinColor.colors.Length);
                    _changeColor.Invoke(skinColor.colors[index].color);
                });
                break;
        }
    }

    public void InitRandomizeSlider(string _name, Slider _slider) {
        int index = -1;
        if (limiters.Count > 0)
        {
            index = limiters.FindIndex(x => x.Name == _name);
        }
        button.onClick.AddListener(() => RandomizeSliderInvoker(index, _slider));
    }

    private void RandomizeSliderInvoker(int _index, Slider _slider)
    {
        float max = _slider.minValue;
        float min = _slider.maxValue;
        float randomResult;

        try
        {
            min = Mathf.Max(min,limiters[_index].MinValue);
            max = Mathf.Min(max,limiters[_index].MaxValue);
        }
        catch { }

        randomResult = Random.Range(min, max);
        _slider.value = randomResult;
    }

    public void InitRandomWardrobe(Action<Func<int, int>> _action)
    {
        button.onClick.AddListener(() => _action((x) => {
            int index = Random.Range(0, x);
            return index;
        }));
    }

    [Serializable]
    private class RandomizeLimiter
    {
        public string Name;
        public float MinValue;
        public float MaxValue;
    }
}
