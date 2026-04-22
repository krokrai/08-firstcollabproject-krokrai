using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuCtrl : MonoBehaviour
{
    // 일단 메뉴 대기열이 추가로 더 늘어나지 않는다는 전제 조건으로 작업됌
    [SerializeField] DishUI[] dishUIs;
    List<DishUI> _inDishUList = new List<DishUI>(14);

    public event Action<bool> OnMaxMenu;
    public event Action<bool> OnDish;


    // 아래 2개의 값은 데이터 타워에서 받아오기
    public int InsertedDish { get; private set; } = 0;
    public int maxDsih { get; private set; } = 6;

    private void Start()
    {
        for (int i = 0; i < dishUIs.Length; i++)
        {
            dishUIs[i].Init(this);
        }

        for (byte i = 0; i < maxDsih; i++)
        {
            dishUIs[i].OpenMenuPanel();
        }

        OnMaxMenu?.Invoke(false);
    }

    /// <summary>
    /// 요리법 설정 및 요리하기 누른 경우 해당 함수에 넣어주기
    /// </summary>
    public void InsertDish(RecipeContainer rcp)
    {
        if (InsertedDish >= maxDsih)
        {
            Debug.Log("더 이상 메뉴를 추가할 수 없습니다."); // TODO : 임시용 코드 후에 재작업 필요 @@@@@
            OnMaxMenu?.Invoke(true);
            return;
        }

        OnDish?.Invoke(true);
            
        for (int i = 0; i < dishUIs.Length; i++)
        {
            if (!dishUIs[i].haveRcp())
            {
                dishUIs[i].DishOnDish(rcp);
                _inDishUList.Add(dishUIs[i]);
                dishUIs[i].SetSiblingList(InsertedDish++); // 의도적으로 넣었으며, 예상 동작은 0이 들어간 후 1이 증가하는 것.
                return;
            }
        }
    }

    public void UnlockedMenuPanel()
    {
        dishUIs[maxDsih].OpenMenuPanel();
        maxDsih++;
    }

    public int RandomEating()
    {
        if ( InsertedDish == 0)
        {
            OnDish?.Invoke(false);
            return 0;
        }

        return _inDishUList[UnityEngine.Random.Range(0, InsertedDish)].EatMenu();
    }

    /// <summary>
    /// 현재 메뉴 삭제 처리.
    /// </summary>
    /// <param name="dish"></param>
    public void ReturnDish(in DishUI dish)
    {
        InsertedDish--;
        if (InsertedDish == 0)
            OnDish?.Invoke(false);
        _inDishUList.Remove(dish);
        dish.SetSiblingList(maxDsih - 1);
    }
}
