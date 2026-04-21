using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenu : MonoBehaviour
{
    [SerializeField] GameObject _menu;
    [SerializeField] GameObject _restaurantScreen;
    [SerializeField] GameObject _fish;
    [SerializeField] Button _masterBtn;
    [SerializeField] AudioManager _audioManager;
    [SerializeField] GameObject _cat;
    [SerializeField] CanvasGroup _loadCanvas;
    [SerializeField] CanvasGroup _gaugeCanvas;

    public event Action<bool> OnChangeSceneToRestaurant;

    bool _active = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _restaurantScreen.SetActive(false);
        _loadCanvas.alpha = 0;
        _loadCanvas.interactable = false;
        _loadCanvas.blocksRaycasts = false;
        _gaugeCanvas.alpha = 0;
        _gaugeCanvas.interactable = false;
        _gaugeCanvas.blocksRaycasts = false;
    }

    public void OnFishScene()
    {
        _fish.SetActive(true);
        _loadCanvas.alpha = 0;
        _loadCanvas.interactable = false;
        _loadCanvas.blocksRaycasts = false;
        //_menu.SetActive(false);
        _audioManager.PlayBgmFishhook();
        _restaurantScreen.SetActive(false);
        OnChangeSceneToRestaurant?.Invoke(false);
    }

    public void OnRestaurantScene()
    {
        if (DataTower.instance.upgradeDatas.UnlockCatObjectLevel > 1)
            _cat.SetActive(true); // 구조를 바꿀 수 없었어...
        _audioManager.PlayBgmRestaurant();
        _fish.SetActive(false);
        _restaurantScreen.SetActive(true);
        OnChangeSceneToRestaurant?.Invoke(true);
    }

    public void OpenMenuPanel()
    {
        _audioManager.PlaySfxClick();
        _active = !_active;
        
        if (_active)
        {
            _loadCanvas.alpha = 1;
            _loadCanvas.interactable = true;
            _loadCanvas.blocksRaycasts = true;
            _gaugeCanvas.alpha = 1;
            _gaugeCanvas.interactable = true;
            _gaugeCanvas.blocksRaycasts = true;
        }
        else
        {
            _loadCanvas.alpha = 0;
            _loadCanvas.interactable = false;
            _loadCanvas.blocksRaycasts = false;
            _gaugeCanvas.alpha = 0;
            _gaugeCanvas.interactable = false;
            _gaugeCanvas.blocksRaycasts = false;
        }
        
        _masterBtn.interactable = !_active;
        //_menu.SetActive(_active);
    }
}
