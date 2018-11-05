using TMPro;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SliderText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text = null;

    [SerializeField]
    private Slider _slider = null;

    private void Start()
    {
        
#if UNITY_EDITOR
        if (Application.isPlaying == false) return;
#endif
        if (_slider == null || _text == null) return;
        _slider.onValueChanged.RemoveListener(SetupValue);
        _slider.onValueChanged.AddListener(SetupValue);

        SetupValue(_slider.value);
    }

    private void SetupValue(float f)
    {
        _text.text = ((int)(f)).ToString();
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update ()
    {
        if (Application.isPlaying) return;
        if (_slider == null || _text == null) return;
        SetupValue(_slider.value);
	}
#endif
}
