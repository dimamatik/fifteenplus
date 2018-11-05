
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField]
    private Button _button = null;

    private void Start()
    {
        if (_button == null) return;

#if UNITY_ANDROID || UNITY_STANDALONE || UNITY_EDITOR
        _button.gameObject.SetActive(true);
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
#else
        _button.gameObject.SetActive(false);
#endif
    }

    private void OnClick()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#elif UNITY_ANDROID || UNITY_STANDALONE
        Application.Quit();
#endif
    }
}
