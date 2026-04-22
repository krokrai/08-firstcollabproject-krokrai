using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DishUI : MonoBehaviour
{
    [SerializeField] Image[] _img; // 1 : 메뉴 사진, 2 : 남은 수량
    [SerializeField] TextMeshProUGUI[] _tmpUGUI; // 1 : 메뉴 이름, 2 : 남은 수량
    [SerializeField] GameObject[] _lockUnits;
    [SerializeField] DisposeFood _SecondPopup;

    private MenuCtrl _menuCtrl;

    RecipeContainer _rcp;

    int _sushi;

    /// <summary>
    /// 현재 레시피가 등록되어 있는 상태인지 체크
    /// </summary>
    public bool haveRcp() => _rcp != null;

    public void OpenMenuPanel()
    {
        _lockUnits[0].SetActive(false);
        _lockUnits[1].SetActive(true);
        if (_rcp == null)
            _lockUnits[1].SetActive(false);
    }

    public void Init(MenuCtrl ctrl)
    {
        _menuCtrl = ctrl;
    }

    // SO 그대로 받아서 사용할 시 금액, 생산량이 달라진 상태로 넘어 올 수 있는지 확인 필요.
    public void DishOnDish(RecipeContainer rcp)
    {
        _rcp = rcp;
        _lockUnits[1].SetActive(true);
        _sushi = _rcp.yield + (DataTower.instance.upgradeDatas.BonusFood01Level + DataTower.instance.upgradeDatas.BonusFood02Level - 2);

        _tmpUGUI[0].text = _rcp.recipe_KName; // 로컬라이제이션 진행 때 수정 해야됌 @@@@@@@@@@@@@@@@
        _tmpUGUI[1].text = _sushi.ToString();
    }

    /*
    transform.SetSiblingIndex(index); // index 값을 기준으로 이동 (0이 최상단)
    transform.SetAsFirstSibling(); // 현재 리스트 중 최상단으로 이동
    transform.SetAsLastSibling();  // 현재 리스트 중 최하단으로 이동

    */

    /// <summary>
    /// 해당 스크롤 뷰에 특정 위치로 바꾸는 함수로 0 입력시 최 상단으로 이동.
    /// </summary>
    /// <param name="i">이동할 위치</param>
    public void SetSiblingList(int i)
    {
        transform.SetSiblingIndex(i);
    }

    // 실제 메뉴가 정렬 방식 선택 필요,.

    /// <summary>
    /// 해당 매뉴 섭취
    /// </summary>
    public int EatMenu()
    {
        _sushi--;
        _tmpUGUI[1].text = _sushi.ToString();

        if (_sushi <= 0)
        {
            DeletMenu();
            return 0;
        }
        return _rcp.prices;
    }

    /// <summary>
    /// 등록된 요리 제거용 함수.
    /// </summary>
    public void DeletMenu()
    {
        _rcp = null;
        _menuCtrl.ReturnDish(this);

        _lockUnits[1].SetActive(false);

        // image 초기화

        // tmp 초기화
        for (int i = 0; i < _tmpUGUI.Length; i++)
        {
            _tmpUGUI[i].text = "";
        }
    }

    public void DeleteMenuinPopup()
    {
        _SecondPopup.SetData(_rcp, this);
    }
}
