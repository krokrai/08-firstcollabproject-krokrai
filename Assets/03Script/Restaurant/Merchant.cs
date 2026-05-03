using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] GameObject MerchantUI;

    bool isActive;

    private void Awake()
    {
        isActive = false;
    }

    public void OnMerchan()
    {
        isActive = !isActive;

        MerchantUI.SetActive(isActive);
    }
}
