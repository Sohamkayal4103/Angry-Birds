using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private IconHandler _iconHandler;
    private List<Baddie> _baddies = new List<Baddie>();

    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject _restartScreen;
    [SerializeField] private SlingShotHandler _slingShotHandler;
    private int _usedNumberOfShots = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _iconHandler = FindObjectOfType<IconHandler>();

        //getting all Baddie Game Objects from a scene into a list
        Baddie[] baddies = FindObjectsOfType<Baddie>();
        for (int i = 0; i < baddies.Length; i++)
        {
            _baddies.Add(baddies[i]);
        }
    }

    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);
        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        return _usedNumberOfShots < MaxNumberOfShots;
    }

    public void CheckForLastShot()
    {
        if (_usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);
        if (_baddies.Count == 0)
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        if (_usedNumberOfShots != 3)
        {
            CheckForAllDeadBaddies();
        }
    }

    private void CheckForAllDeadBaddies()
    {
        if (_baddies.Count == 0)
        {
            WinGame();
        }
    }

    #region Win/Lose
    private void WinGame()
    {
        //Debug.Log("Win Game");
        _restartScreen.SetActive(true);
        _slingShotHandler.enabled = false;
    }

    private void LoseGame()
    {
        //Debug.Log("Lose Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

}
