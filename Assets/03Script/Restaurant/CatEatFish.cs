using UnityEngine;
using UnityEngine.UI;

public class CatEatFish : MonoBehaviour
{
    [SerializeField] InventorySystem _inven;
    [SerializeField] Button _btn;
    [SerializeField] Animation _ani;
    [SerializeField] AudioManager _audioManager;

    public void EatFish()
    {
        _btn.interactable = false;
        
        _audioManager.PlaySfxCat();
        _inven.SetCatFood();

        _ani.Play();
    }

    public void CanEatFishState()
    {
        _btn.interactable = true;
    }
}
