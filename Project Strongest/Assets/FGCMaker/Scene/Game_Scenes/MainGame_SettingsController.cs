using UnityEngine;

public class MainGame_SettingsController : MonoBehaviour
{
    protected Character_Base mainPlayer, secondaryPlayer;
    protected GameObject _pauseMenu;

    public virtual void SetTeleportPositions() 
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
    public void TogglePauseMenu() 
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
