using UnityEngine;
using UnityEngine.EventSystems;

public class MainGame_SettingsController : MonoBehaviour
{
    protected EventSystem _eventSystem;
    protected Character_Base mainPlayer, secondaryPlayer;
    public GameObject _pauseMenu;

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
            _pauseMenu.SetActive(false);
            return;
        }
        _pauseMenu.SetActive(true);
        return;
    }
}
