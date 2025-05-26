using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject previousSelection;

    [Header("Menu Objects")]
    public GameObject pause_menu;
    public GameObject setting_menu;

    [Header("First Selected Options")]
    public GameObject _pauseMenuFirst;
    public GameObject _settingMenuFirst;

    private bool wasPaused;
    // Start is called before the first frame update
    private void Start()
    {
        pause_menu.SetActive(false);
        setting_menu.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.isPaused != wasPaused)
        {
            if (GameManager.Instance.isPaused)
            {
                MenuPause();
            }
            else
            {
                MenuUnpause();
            }
            wasPaused = GameManager.Instance.isPaused;
        }
        
    }

    #region Pause Functions

    private void MenuPause()
    {
        OpenPauseMenu();
    }

    private void MenuUnpause()
    {
        CloseSettingMenu();
        ClosePauseMenu();
    }

    #endregion

    #region Open Menu

    public void OpenPauseMenu()
    {
        pause_menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_pauseMenuFirst);

        if (EventSystem.current.IsPointerOverGameObject())
        {
        // Deselect the current selected object to prevent keyboard/controller highlight
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    public void ClosePauseMenu()
    {
        pause_menu.SetActive(false);
    }

    public void OpenSettingMenu()
    {
        setting_menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_settingMenuFirst);
    }

    public void CloseSettingMenu()
    {
        setting_menu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_pauseMenuFirst);
    }
    public void ReturnTitle()
    {
        MenuUnpause();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        previousSelection = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (previousSelection != null)
        {
            EventSystem.current.SetSelectedGameObject(previousSelection);
            previousSelection = null;
        }
    }

    #endregion
}
