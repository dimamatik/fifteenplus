using Fifteen;

using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    [SerializeField]
    private GameField _field = null;

    [SerializeField]
    private Button _victory = null;


    [SerializeField]
    private NewGameMenu _menu = null;

    private Game _game = null;

    private void Start()
    {
        if (_field == null || _victory == null || _menu == null) return;

        ShowMenu();

        _menu.OnNewGame -= StartNewGame;
        _menu.OnNewGame += StartNewGame;

        _victory.onClick.RemoveAllListeners();
        _victory.onClick.AddListener(ShowMenu);
    }
    private void ShowMenu()
    {
        _victory.gameObject.SetActive(false);
        _menu.gameObject.SetActive(true);
        _field.gameObject.SetActive(false);
    }

    private void StartNewGame(int w, int h)
    {
        _game = new Game(w, h);

        while (_game.CheckVictory()) _game.Shuffle();

        _menu.gameObject.SetActive(false);
        _field.gameObject.SetActive(true);

        _field.Initialize(w, h);

        int[] indexes = new int[w * h];

        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = _game[i % w, i / w] - 1;
        }

        _field.Shuffle(indexes);


        _field.OnButtonClick -= OnButtonClick;
        _field.OnButtonClick += OnButtonClick;
    }

    private void OnButtonClick(int ind)
    {
        if (_game == null || _field == null) return;
        int n = _game.EmptyIndex();
        if (_game.Play(ind % _game.Width, ind / _game.Width))
        {
            if (_game.CheckVictory()) ShowVictory();
            else _field.Swap(ind, n);
        }
    }

    private void ShowVictory()
    {
        if (_field == null || _victory == null) return;

        _field.gameObject.SetActive(false);
        _victory.gameObject.SetActive(true);
    }
}
