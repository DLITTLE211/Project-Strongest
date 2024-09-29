using UnityEngine;
using UnityEngine.EventSystems;

public class MainGame_SettingsController : MonoBehaviour
{
    protected EventSystem _eventSystem;
    protected Character_Base mainPlayer, secondaryPlayer;
    public GameObject _pauseMenu;
    public bool _isPause = false;
    public virtual void SetTeleportPositions(EventSystem eventSystem)
    {
        //Nothing TODO
    }
    public void SetPlayerData(Character_Base curBase)
    {
        if (curBase._subState == Character_SubStates.Controlled)
        {
            if (mainPlayer == null)
            {
                mainPlayer = curBase;
            }
            else
            {
                secondaryPlayer = curBase;
            }
        }
        else
        {
            if (mainPlayer == null)
            {
                mainPlayer = curBase;
            }
            else
            {
                secondaryPlayer = curBase;
            }
        }
    }

    public virtual void SetPlayersPosition() 
    {

    }
    public virtual void TogglePauseMenu() 
    {
        if (_pauseMenu.activeInHierarchy)
        {
            if (GameManager.instance._gameModeSet.gameMode == GameMode.Training)
            {
                _pauseMenu.GetComponent<TrainingMenu_Controller>().DeactivateMenuOnStart();
            }
            if (GameManager.instance._gameModeSet.gameMode == GameMode.Versus) 
            {
                _pauseMenu.GetComponent<VersusMenu_Controller>().DeactivateMenuOnStart();
            }
            _pauseMenu.SetActive(false);
            mainPlayer.UnlockPlayerInPause();
            secondaryPlayer.UnlockPlayerInPause();
            _isPause = false;
            return;
        }
        mainPlayer.LockPlayerInPause();
        secondaryPlayer.LockPlayerInPause();
        _pauseMenu.SetActive(true);
        _isPause = true;
        return;
    }
}
