using UnityEngine;

public class MoneyAdder : MonoBehaviour
{
    public void Add1kMoney()
    {
        DataTower.instance.TryMoenyChanged(1000,false);
    }

    public void Add5kMoney()
    {
        DataTower.instance.TryMoenyChanged(5000, false);
    }

    public void Add1mMoney()
    {
        DataTower.instance.TryMoenyChanged(1000000, false);
    }
}
