using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField]
    private Button _button = null;

    [SerializeField]
    private TextMeshProUGUI _text = null;

    [SerializeField]
    private Slider _width = null;

    [SerializeField]
    private Slider _height = null;

    public event Action<int, int> OnNewGame = null;

    private void Start ()
    {
        if (_width == null || _height == null) return;
        _width.onValueChanged.RemoveListener(OnSliderChanged);
        _height.onValueChanged.RemoveListener(OnSliderChanged);
        _width.onValueChanged.AddListener(OnSliderChanged);
        _height.onValueChanged.AddListener(OnSliderChanged);

        OnSliderChanged(0);

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnNewGameClick);
        }
	}
    private void OnNewGameClick()
    {
        if (_width == null || _height == null) return;
        if (OnNewGame != null) OnNewGame((int)_width.value, (int)_height.value);
    }
    private void OnSliderChanged(float f)
    {
        if (_text == null || _height == null || _width == null) return;

        int h = (int)_height.value;
        int w = (int)_width.value;

        _text.text = (h * w - 1).ToString() + "-puzzle";
    }
}
