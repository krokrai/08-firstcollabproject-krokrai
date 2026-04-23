using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeInfoUI : MonoBehaviour
{
    //[SerializeField] Image[] _img; // 1 : 요리될 메뉴 사진, 2번 금액, 3번 생산수, 4번 물고기
    [SerializeField] AddressableImageLoader[] _imageLoaders;
    [SerializeField] GameObject[] _panels; // 1 : 요리, 2. 해금
    [SerializeField] TextMeshProUGUI[] _tmpUGUI; // 1 : 메뉴 이름 2: 판매액, 3번 생산수, 4번 음식 설명, 5번 물고기 이름, 6번 물고기 소지 / 소모, 7번 버튼
    [SerializeField] Button _btn;
    [SerializeField] MenuCtrl _menuctrl;
    [SerializeField] InventorySystem _inventorySystem;
    [SerializeField] RecipeUnlockUI _unlockUI;
    [SerializeField] GameObject _unlockedPopup;
    [SerializeField] UnlockedPopup _unlockedPopupScript;
    [SerializeField] AudioManager _audioManager;
    [SerializeField] RecipManager _recipeManager;
    [SerializeField] GameObject _body;
    [SerializeField] DiningUpgradeDataReader _upgData;

    RecipeContainer _rcp;

    FishData fish;
    Language lng;

    GameObject _currentObj;

    bool _hasMenuSlot;
    bool _currentCanMake;
    byte _currentHasFish;

    public void ThisRecipeCanMake(bool b) => _hasMenuSlot = !b;

    private void OnEnable()
    {
        _menuctrl.OnMaxMenu += ThisRecipeCanMake;
    }

    private void OnDisable()
    {
        _menuctrl.OnMaxMenu -= ThisRecipeCanMake;
    }

    /// <summary>
    /// 선택된 레시피에 대한 정보 띄우기
    /// </summary>
    public void SelectedRecipe(in RecipeContainer rcp, in GameObject outline  ,in bool canMake, in bool isUnlock, in bool canUnlock)
    {
        if (!_body.activeSelf)
            _body.SetActive(true);
        if (rcp == null)
            return;
        _currentObj?.SetActive(false);
        _currentObj = outline;
        _currentObj.SetActive(true);

        lng = DataTower.instance.languageSetting;

        _rcp = rcp;
        _currentCanMake = canMake;

        
        _imageLoaders[0].SetImage(rcp.dish_Sprite,isUnlock);

        _tmpUGUI[0].text = lng == Language.KOR ? _rcp.recipe_KName : _rcp.recipe_EName;

        int a = (int)(rcp.prices
                * (1
                + _upgData.Bonus_Dish_Price_1[DataTower.instance.upgradeDatas.BonusDishPrice01Level - 1].Effect_Value_2
                + _upgData.Bonus_Dish_Price_2[DataTower.instance.upgradeDatas.BonusDishPrice02Level - 1].Effect_Value_2));

        _tmpUGUI[1].text = a.TextFormatCurrency();
        _tmpUGUI[2].text = rcp.yield.ToString();

        if (DataTower.instance.fishDatas.ContainsKey(rcp.ingredient))
            fish = DataTower.instance.fishDatas[rcp.ingredient];

        if (isUnlock)
        {
            RecipeInfo(isUnlock);
            

            // 렌더러 작업은 스프라이터 관련 매니저 완성 후
            /*
            _img[0].sprite =  여기에 나중에 만들 스프라이트 접근용 함수 입력
            */
        }
        else
        {
            _panels[1].SetActive(true);
            _panels[0].SetActive(false);

            _imageLoaders[2].SetImage(_rcp.ingredient, isUnlock);
            _unlockUI.RecipeUnllockInfo(fish, canUnlock);
        }

        // TODO : 물고기 소지 및 관련 인벤토리 작업 완료 후 진입. @@@@@@@@@
    }

    public void RecipeInfo(in bool isUnlock)
    {
        _currentHasFish = 0;
        if (_rcp == null)
            return;
        _panels[0].SetActive(true);
        _panels[1].SetActive(false);

        if (_currentCanMake && _hasMenuSlot)
            _btn.interactable = true;
        else
            _btn.interactable = false;
        
            _imageLoaders[1].SetImage(_rcp.ingredient, isUnlock);

        for (byte i = 0; i < DataTower.instance.Items.Count; i++)
        {
            if (fish.fishID == DataTower.instance.Items[i].fishID)
                _currentHasFish++;
        }

        if (_currentHasFish < 1)
        {
            _tmpUGUI[5].text = $"<color=red>{_currentHasFish} / 1</color>";
            _btn.interactable = false;
        }
        else
        {
            _tmpUGUI[5].text = $"{_currentHasFish} / 1";
            _btn.interactable = true;
        }

        switch (lng)
        {
            case Language.KOR:
                _tmpUGUI[3].text = _rcp.KDescription.ToString();
                _tmpUGUI[4].text = fish.korName.ToString();
                break;
            case Language.ENG:
                _tmpUGUI[3].text = _rcp.EDescription.ToString();
                _tmpUGUI[4].text = fish.engName.ToString();
                break;
        }
    }
    public void UnlockedPopup()
    {
        _audioManager.PlaySfxClick();
        _unlockedPopup.SetActive(true);
        _unlockedPopupScript.PopUp(_rcp);
    }

    public void MakeMenu()
    {
        _audioManager.PlaySfxClick();
        _currentHasFish--;
        _inventorySystem.Erase(fish.fishID);
        _menuctrl.InsertDish(_rcp);
        _recipeManager.ReCall(_rcp);
        if (_currentHasFish < 1)
        {
            _btn.interactable = false;
        }
    }
}
