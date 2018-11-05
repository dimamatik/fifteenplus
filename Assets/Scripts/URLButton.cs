using UnityEngine;
using UnityEngine.UI;

public class URLButton : MonoBehaviour
{
    [SerializeField]
    private Button _button = null;

    [SerializeField]
    private string _url = null;

    private void Start()
    {
        if (_button == null) return;
        _button.onClick.RemoveListener(OpenURLClick);
        _button.onClick.AddListener(OpenURLClick);
    }

    private void OpenURLClick()
    {
        if (_url != null) Application.OpenURL(_url);
    }
}
