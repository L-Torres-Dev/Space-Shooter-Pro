using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] TMP_Text _gameOverText;
    [SerializeField] TMP_Text _restartText;
    [SerializeField] Image _livesImage;
    [SerializeField] Sprite[] _liveSprites;
    [SerializeField] Player _player;

    [SerializeField] float _gameOverInterval = .5f;

    private bool _gameOver = false;

    private void Update()
    {
        if(_gameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
    public void UpdateScore(int score)
    {
        _player.AddScore(score);
        _scoreText.SetText($"Score {_player.Score.ToString("D5")}");
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _liveSprites[currentLives];
    }
    public void GameOver()
    {
        _gameOver = true;
        _restartText.gameObject.SetActive(true);
        StartCoroutine(CO_GameOver());
    }

    public IEnumerator CO_GameOver()
    {
        _gameOverText.gameObject.SetActive(true);

        var wait = new WaitForSeconds(_gameOverInterval);
        while (true)
        {
            yield return wait;
            _gameOverText.gameObject.SetActive(false);
            yield return wait;
            _gameOverText.gameObject.SetActive(true);
        }
    }
}
