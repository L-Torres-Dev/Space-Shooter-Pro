using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance => _instance;

    [SerializeField] private Transform _player;
    [SerializeField] UIManager _uiManager;
    public Transform PlayerTransform => _player;

    private bool _gameOver = false;
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            print($"Game Manager already exists! Destroying {name} now...");
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (_gameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void GameOver()
    {
        _gameOver = true;
        _uiManager.GameOver();
    }

    public void YouWin()
    {
        _gameOver = true;
        _uiManager.YouWin();
    }
}
