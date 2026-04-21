using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUnlockUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _context;
    [SerializeField] Image _fishImage;
    [SerializeField] TextMeshProUGUI _fishText;
    [SerializeField] Image _canUnlockImage;
    [SerializeField] TextMeshProUGUI _canUnlockText;
    [SerializeField] Button _unlockButton;
    [SerializeField] TextMeshProUGUI _unlockButtonText;

    [SerializeField] Sprite[] _sprite;

    [SerializeField] TranslationDataReader _reader;
    TranslationData _contextData;
    TranslationData[] _haveRecipeData;
    TranslationData _unlockRecipeData;

    private void Awake()
    {
        
        _haveRecipeData = new TranslationData[2];
        /*
        _unlockRecipeData = new TranslationData();
        _contextData = new TranslationData();*/
    }

    public void RecipeUnllockInfo(FishData so, bool canUnlock)
    {
        if (so == null)
            return;

        _canUnlockImage.sprite = canUnlock ? _sprite[1] : _sprite[0];

        _contextData = _reader.GetTranslationData("Restaurant_Recipe_Condition");
        _haveRecipeData[0] = _reader.GetTranslationData("Restaurant_Recipe_No");
        _haveRecipeData[1] = _reader.GetTranslationData("Recipe_Acquisition_Complete");
        _unlockRecipeData = _reader.GetTranslationData("Restaurant_Recipe_Unlock");

        _unlockButton.interactable = canUnlock;

        switch (DataTower.instance.languageSetting)
        {
            case Language.KOR:
                _context.text = _contextData.Kor;
                _canUnlockText.text = canUnlock ? _haveRecipeData[1].Kor : _haveRecipeData[0].Kor;
                _fishText.text = so.korName;
                _unlockButtonText.text = _unlockRecipeData.Kor;
                break;
            case Language.ENG:
                _context.text = _contextData.En;
                _canUnlockText.text = canUnlock ? _haveRecipeData[1].En : _haveRecipeData[0].En;
                _fishText.text = so.engName;
                _unlockButtonText.text = _unlockRecipeData.En;
                break;
        }
    }
}
