using System;
using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class GameField : MonoBehaviour
{
    [SerializeField]
    private RectTransform _root = null;

    [SerializeField]
    private RectTransform _prefab = null;

    [SerializeField][Range(2, 10)]
    private int _width = 4;

    [SerializeField][Range(2, 10)]
    private int _height = 4;

    [SerializeField]
    private int _size = 100;
    [SerializeField]
    private int _space = 10;

    [SerializeField][Range(0.01f,5f)]
    private float _swapTime = 0.5f;


    [SerializeField][HideInInspector]
    private RectTransform[] _buttons = new RectTransform[0];

    private bool _inAnimation = false;
    private int _rw = -1;
    private int _rh = -1;

    public bool InAnimation
    {
        get { return _inAnimation; }
        private set { _inAnimation = value; }
    }

    public event Action<int> OnButtonClick = null;

    public void Initialize(int width, int height)
    {
        ResizeField(width, height);
        AlignField();
        AssignField();
    }
    public void Shuffle(int[] indexes)
    {
        if (indexes == null || indexes.Length != _buttons.Length)
        {
            Debug.LogError("Не хватает индексов для перемешивания");
            return;
        }
        RectTransform[] buttons = new RectTransform[_buttons.Length];

        for (int i = 0; i < _buttons.Length; i++)
        {
            int ind = indexes[i];
            buttons[i] = _buttons[ind];
            _buttons[ind] = null;

            if (buttons[i] == null) continue;

            var bu = buttons[i].GetComponentInChildren<Button>(true);
            if (bu != null)
            {
                bu.onClick.RemoveAllListeners();
                int index = i;
                bu.onClick.AddListener(() => OnPointerClick(index));
            }
        }
        _buttons = buttons;

        AlignField();
    }

    public void Swap(int a, int b)
    {
        StartCoroutine(SwapCoroutine(a, b).GetEnumerator());
    }

    private void ResizeField(int width, int height)
    {
        if (_root == null || _prefab == null)
        {
            Debug.LogError("Корень или префаб не установленны");
            return;
        }

        if (width < 2 || height < 2)
        {
            Debug.LogError("Слишком маленькие ширина и высота");
            return;
        }

        _width = width;
        _height = height;
        int n = height * width;

        if (n < _buttons.Length)
        {
            for (int i = _buttons.Length - 1; i >= n; i--)
            {
                var go = _buttons[i] == null || _buttons[i].gameObject == null ? null : _buttons[i].gameObject;
                _buttons[i] = null;
                if (go == null) continue;
                if (Application.isPlaying) Destroy(go);
                else DestroyImmediate(go);
            }
            Array.Resize(ref _buttons, n);
        }
        else if (n > _buttons.Length)
        {
            Array.Resize(ref _buttons, n);
        }
        for (int i = 0; i < n; i++)
        {
            if (_buttons[i] != null) continue;

            var rt = Instantiate(_prefab, Vector3.zero, Quaternion.identity, _root);
            if (rt == null || rt.gameObject == null) continue;

            rt.gameObject.SetActive(true);

            _buttons[i] = rt;
        }
    }

    private void AlignField()
    {
        if (_root == null || _prefab == null)
        {
            Debug.LogError("Корень или префаб не установленны");
            return;
        }
        if (_size < 0 || _space < 0 || _root.rect.height < 1 || _root.rect.width < 1)
        {
            Debug.LogError("Некорректные размеры поля");
            return;
        }

        int w = _size * _width + _space * (_width - 1);
        int h = _size * _height + _space * (_height - 1);

        int s = _size;
        int p = _space;

        if (w > _root.rect.width || h > _root.rect.height)
        {
            float f = Math.Max(w / _root.rect.width, h / _root.rect.height);
            s = (int)(s / f);
            p = (int)(p / f);
        }

        float xmin = _width % 2 == 0 ? -s * (_width / 2) - p * (_width / 2) + (p + s) / 2.0f :
                                       -s * (_width / 2) - p * (_width / 2);
        float ymin = _height % 2 == 0 ? s * (_height / 2) + p * (_height / 2) - (p + s) / 2.0f :
                                        s * (_height / 2) + p * (_height / 2);

        for (int i = 0; i < _buttons.Length; i ++)
        {
            RectTransform rt = _buttons[i];
            if (rt == null || rt.parent != _root) continue;

            int x = i % _width;
            int y = i / _width;

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, s);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, s);

            rt.localPosition = new Vector3(xmin + (s + p) * x, ymin - (s + p) * y, 0);

        }
    }
    private void AssignField()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            RectTransform rt = _buttons[i];

            if (rt == null) continue;

            var tx = rt.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tx != null) tx.text = i == _buttons.Length - 1 ? "" : (i + 1).ToString();

            rt.gameObject.name = "_button_" + String.Format("{0:d3}", i + 1);

            var bu = rt.GetComponentInChildren<Button>(true);
            if (bu != null)
            {
                bu.onClick.RemoveAllListeners();
                int index = i;
                bu.onClick.AddListener(() => OnPointerClick(index));
            }

            rt.gameObject.SetActive(i < _buttons.Length - 1);
        }
    }

    private void OnPointerClick (int index)
    {
        if (InAnimation) return;
        if (OnButtonClick != null) OnButtonClick(index);
    }

    public IEnumerable SwapCoroutine(int a, int b)
    {
        if (a < 0 || b < 0 || a >= _buttons.Length || b >= _buttons.Length ||
            _buttons[a] == null || _buttons[b] == null) yield break;
        while (InAnimation) yield return null;
        InAnimation = true;

        RectTransform rta = _buttons[a];
        RectTransform rtb = _buttons[b];

        Vector3 start = rta.localPosition;
        Vector3 end = rtb.localPosition;

        float time = _swapTime;
        float t = 0;

        while (t <= time)
        {
            t += Time.deltaTime;
            float p = t >= time ? 1f : t / time;
            rta.localPosition = start + p * (end - start);
            rtb.localPosition = end + p * (start - end);
            yield return null;
        }

        rta.localPosition = end;
        rtb.localPosition = start;

        Button ba = rta.GetComponentInChildren<Button>(true);
        Button bb = rtb.GetComponentInChildren<Button>(true);

        var eva = ba == null ? new Button.ButtonClickedEvent() : ba.onClick;
        var evb = bb == null ? new Button.ButtonClickedEvent() : bb.onClick;

        if (ba != null) ba.onClick = evb;
        if (bb != null) bb.onClick = eva;

        _buttons[a] = rtb;
        _buttons[b] = rta;

        InAnimation = false;
    }
    private void LateUpdate()
    {
        if (Application.isPlaying == false || InAnimation) return;
        if (_root == null || _prefab == null) return;
        if ((int)_root.rect.width != _rw || (int)_root.rect.height != _rh)
        {
            _rw = (int)_root.rect.width;
            _rh = (int)_root.rect.height;
            AlignField();
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying) return;
        if (_root == null || _prefab == null) return;
        if (_root.childCount != _width * _height)
        {
            ResizeField(_width, _height);
            AlignField();
            AssignField();
        }
    }
#endif
}
