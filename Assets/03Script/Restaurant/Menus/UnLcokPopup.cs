using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnLcokPopup : MonoBehaviour
{
    [SerializeField] GameObject _unlockPanel;
    [SerializeField] MenuCtrl _menuCtrl;
    [SerializeField] MenuUpgData[] _datas;
    [SerializeField] Button _confirmButton;
    [SerializeField] TextMeshProUGUI[] _texts; // 1. 잠금, 2번 비용, 3번 취소, 4번 확인
    [SerializeField] TranslationDataReader _reader;
    [SerializeField] AudioManager _audioManager;

    private TranslationData[] _tranData = new TranslationData[3];

    int _currentLvl;

    public void PopUpPanel()
    {
        _tranData[0] = _reader.GetTranslationData("Unlock");
        _tranData[1] = _reader.GetTranslationData("Cancel");
        _tranData[2] = _reader.GetTranslationData("OK");

        _unlockPanel.SetActive(true);

        switch(DataTower.instance.languageSetting)
        {
            case Language.KOR:
                _texts[0].text = _tranData[0].Kor;
                _texts[2].text = _tranData[1].Kor;
                _texts[3].text = _tranData[2].Kor;
                break;
            case Language.ENG:
                _texts[0].text = _tranData[0].En;
                _texts[2].text = _tranData[1].En;
                _texts[3].text = _tranData[2].En;
                break;
        }
        
        _currentLvl = DataTower.instance.upgradeDatas.UnlockMenuLevel - 1;
        if (DataTower.instance.money < (ulong)_datas[_currentLvl].MenuPanelUpgCost)
        {
            _texts[1].color = Color.red;
            _texts[1].text = _datas[_currentLvl].MenuPanelUpgCost.TextFormatCurrency();
            _confirmButton.interactable = false;
        }
        else
        {
            _confirmButton.interactable = true;
            _texts[1].color = Color.black;
            _texts[1].text = _datas[_currentLvl].MenuPanelUpgCost.TextFormatCurrency();
        }
    }

    public void CancleUnlock()
    {
        _audioManager.PlaySfxClick();
        _unlockPanel.SetActive(false);
    }

    public void confirmUnlock()
    {
        _audioManager.PlaySfxClick();
        DataTower.instance.upgradeDatas.UnlockMenuLevel++;
        _menuCtrl.UnlockedMenuPanel();
        _unlockPanel.SetActive(false);
    }
}
