using UnityEngine;
using UnityEngine.UI;

public class CatEatMotion : MonoBehaviour
{
    [SerializeField] CatEatFish _cat;

    private void Awake()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void MoreFish()
    {
        _cat.CanEatFishState();
    }
}
