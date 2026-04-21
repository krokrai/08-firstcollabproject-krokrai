using NUnit.Framework.Constraints;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockedPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _alert;
    [SerializeField] TextMeshProUGUI _fishName;
    [SerializeField] TextMeshProUGUI _discription;
    [SerializeField] TextMeshProUGUI _confirmButton;
    [SerializeField] TranslationDataReader _reader;
    [SerializeField] RecipeInfoUI _recipeUI;
    [SerializeField] RecipManager _recipeManager;
    [SerializeField] AudioManager _audioManager;
    [SerializeField] AddressableImageLoader _fishImageLoader;

    TranslationData[] _datas;
    RecipeContainer _rcp;

    private void Awake()
    {
        _datas = new TranslationData[3];
    }

    public void PopUp(RecipeContainer recipe)
    {
        _datas[0] = _reader.GetTranslationData("Restaurant_Recipe_Unlock");
        _datas[1] = _reader.GetTranslationData("Restaurant_Recipe_Unlock_2");
        _datas[2] = _reader.GetTranslationData("OK");
        _rcp = recipe;

        _fishImageLoader.SetImage(_rcp.dish_Sprite);

        _audioManager.PlaySfxRecipe();

        switch(DataTower.instance.languageSetting)
        {
            case Language.KOR:
                _alert.text = _datas[0].Kor;
                _discription.text = _datas[1].Kor;
                _confirmButton.text = _datas[2].Kor;
                _fishName.text = recipe.recipe_KName;
                break;
            case Language.ENG:
                _alert.text = _datas[0].En;
                _discription.text = _datas[1].En;
                _confirmButton.text = _datas[2].En;
                _fishName.text = recipe.recipe_EName;
                break;
        }
    }

    public void Confirm()
    {
        _audioManager.PlaySfxClick();
        _recipeUI.RecipeInfo(true);
        _recipeManager.SetRecipeUnlock(_rcp.ingredient);
        gameObject.SetActive(false);
    }
}