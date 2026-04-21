using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradeSelectPresenter : MonoBehaviour
{
    [SerializeField] private UpgradeUIView _upgradeUIView;
    
    [Header("Children Upgrade Select Views")]
    [SerializeField] UpgradeSelectView[] _views;
    
    [Header("Upgrade Type")]
    [field: SerializeField]
    public EMainUpgradeType MainUpgradeType { get; private set; }
    
    private FishingUpgradeManager _fUpgrade;
    private FishingUpgradeDataReader _fDataReader;
    
    private DiningUpgradeManager _dUpgrade;
    private DiningUpgradeDataReader _dDataReader;
    
    private TranslationDataReader _tDataReader;
    private WaitForEndOfFrame _waitForEndOfFrame = new ();
    
    private bool _isTransDataLoaded;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        StartCoroutine(LoadingOnEnableRoutine());
    }

    private void OnDisable()
    {
        DisableEvents();
    }

    private void Init()
    {
        _fUpgrade = FindFirstObjectByType<FishingUpgradeManager>();
        _fDataReader = FindFirstObjectByType<FishingUpgradeDataReader>();
        _dUpgrade = FindFirstObjectByType<DiningUpgradeManager>();
        _dDataReader = FindFirstObjectByType<DiningUpgradeDataReader>();
        _tDataReader = FindFirstObjectByType<TranslationDataReader>();
        _isTransDataLoaded = false;
        _tDataReader.OnDataLoaded += TransDataReaderReady;
    }

    private void RunSettings()
    {
        switch (MainUpgradeType)
        {
            case EMainUpgradeType.Fishing:
                SetFishingTranslationTextSetting();
                SetViewPlayerGrade();
                SetViewBaitLevel();
                SetViewRodLevel();
                SetViewShipLevel();
                _fUpgrade.CheckCanUpgrades();
                _fUpgrade.CheckReqGolds();
                break;
            
            case EMainUpgradeType.Dining:
                SetDiningTranslationTextSetting();
                SetViewMasterLevel();
                SetViewMaxCustomerLimitLevel();
                SetViewMaxSpawnLimit01Level();
                SetViewMaxSpawnLimit02Level();
                SetViewWeightLevel();
                SetViewBonusTipsMultiLevel();
                SetViewBonusDishPrice01Level();
                SetViewBonusDishPrice02Level();
                SetViewBonusFood01Level();
                SetViewBonusFood02Level();
                SetViewUnlockCatObjectLevel();
                _dUpgrade.CheckCanUpgrades();
                _dUpgrade.CheckCosts();
                break;
        }
    }

    private void TransDataReaderReady()
    {
        _isTransDataLoaded = true;
    }

    #region 이벤트 구독/해제

    private void EnableEvents()
    {
        for (int i = 0; i < _views.Length; i++)
        {
            DataTower.instance.OnLanguageSettingChanged += _views[i].TranslationText;
        }
        switch (MainUpgradeType)
        {
            case EMainUpgradeType.Fishing:
                
                _fUpgrade.OnFishingUpgrade += RenewalPlayerGrade;
                _fUpgrade.OnBaitUpgrade += RenewalBaitLevel;
                _fUpgrade.OnRodUpgrade += RenewalRodLevel;
                _fUpgrade.OnShipUpgrade += RenewalShipLevel;
                
                // EFishingUpgradeType 인덱스 번호로 정렬
                for (int i = 0; i < _views.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _views[i].OnTryUpgrade += _fUpgrade.FishingUpgrade;
                            _fUpgrade.EnoughGoldFishingGradeUpgrade += _views[i].ToggleReqGoldTextColor;
                            break;
                        case 1:
                            _views[i].OnTryUpgrade += _fUpgrade.BaitUpgrade;
                            _fUpgrade.EnoughGoldBaitLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _fUpgrade.CanBaitLevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 2:
                            _views[i].OnTryUpgrade += _fUpgrade.RodUpgrade;
                            _fUpgrade.EnoughGoldRodLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _fUpgrade.CanRodLevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 3:
                            _views[i].OnTryUpgrade += _fUpgrade.ShipUpgrade;
                            _fUpgrade.EnoughGoldShipLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _fUpgrade.CanShipLevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        default:
                            break;
                    }
                }
                _upgradeUIView.isFReady = true;
                break;
            
            // EDiningUpgradeType 인덱스 번호로 정렬
            case EMainUpgradeType.Dining:

                _dUpgrade.OnMasterLevelUpgrade += RenewalMasterLevel;
                _dUpgrade.OnMaxCustomerLimitLevelUpgrade += RenewalMaxCustomerLimitLevel;
                _dUpgrade.OnMaxSpawnLimit01LevelUpgrade += RenewalMaxSpawnLimit01Level;
                _dUpgrade.OnMaxSpawnLimit02LevelUpgrade += RenewalMaxSpawnLimit02Level;
                _dUpgrade.OnWeightLevelUpgrade += RenewalWeightLevel;
                _dUpgrade.OnBonusTipsMultiLevelUpgrade += RenewalBonusTipsMultiLevel;
                _dUpgrade.OnBonusDishPrice01LevelUpgrade += RenewalBonusDishPrice01Level;
                _dUpgrade.OnBonusDishPrice02LevelUpgrade += RenewalBonusDishPrice02Level;
                _dUpgrade.OnBonusFood01LevelUpgrade += RenewalBonusFood01Level;
                _dUpgrade.OnBonusFood02LevelUpgrade += RenewalBonusFood02Level;
                _dUpgrade.OnUnlockCatObjectLevelUpgrade += RenewalUnlockCatObjectLevel;
                
                for (int i = 0; i < _views.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _views[i].OnTryUpgrade += _dUpgrade.MasterLevelUpgrade;
                            _dUpgrade.EnoughGoldMasterLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            break;
                        case 1:
                            _views[i].OnTryUpgrade += _dUpgrade.MaxCustomerLimitLevelUpgrade;
                            _dUpgrade.EnoughGoldMaxCustomerLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanMaxCustomerLimitLevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 2:
                            _views[i].OnTryUpgrade += _dUpgrade.MaxSpawnLimit01LevelUpgrade;
                            _dUpgrade.EnoughGoldMaxSpawnLimit01LevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanMaxSpawnLimit01LevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 3:
                            _views[i].OnTryUpgrade += _dUpgrade.MaxSpawnLimit02LevelUpgrade;
                            _dUpgrade.EnoughGoldMaxSpawnLimit02LevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanMaxSpawnLimit02LevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 4:
                            _views[i].OnTryUpgrade += _dUpgrade.WeightLevelUpgrade;
                            _dUpgrade.EnoughGoldWeightLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanWeightLevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 5:
                            _views[i].OnTryUpgrade += _dUpgrade.BonusTipsMultiLevelUpgrade;
                            _dUpgrade.EnoughGoldBonusTipsMultiLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusTipsMultiLevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 6:
                            _views[i].OnTryUpgrade += _dUpgrade.BonusDishPrice01LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusDishPrice01LevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusDishPrice01LevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 7:
                            _views[i].OnTryUpgrade += _dUpgrade.BonusDishPrice02LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusDishPrice02LevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusDishPrice02LevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 8:
                            _views[i].OnTryUpgrade += _dUpgrade.BonusFood01LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusFood01LevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusFood01LevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 9:
                            _views[i].OnTryUpgrade += _dUpgrade.BonusFood02LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusFood02LevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusFood02LevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        case 10:
                            _views[i].OnTryUpgrade += _dUpgrade.UnlockCatObjectLevelUpgrade;
                            _dUpgrade.EnoughGoldUnlockCatObjectLevelUpgrade += _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanUnlockCatObjectLevelUpgrade += _views[i].ToggleButtenState;
                            break;
                        default:
                            break;
                    }
                }
                _upgradeUIView.isDReady = true;
                break;
            default:
                break;
        }
    }

    private void DisableEvents()
    {
        for (int i = 0; i < _views.Length; i++)
        {
            DataTower.instance.OnLanguageSettingChanged -= _views[i].TranslationText;
        }
        switch (MainUpgradeType)
        {
            case EMainUpgradeType.Fishing:
                
                _fUpgrade.OnFishingUpgrade -= RenewalPlayerGrade;
                _fUpgrade.OnBaitUpgrade -= RenewalBaitLevel;
                _fUpgrade.OnRodUpgrade -= RenewalRodLevel;
                _fUpgrade.OnShipUpgrade -= RenewalShipLevel;
                
                for (int i = 0; i < _views.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _views[i].OnTryUpgrade -= _fUpgrade.FishingUpgrade;
                            _fUpgrade.EnoughGoldFishingGradeUpgrade -= _views[i].ToggleReqGoldTextColor;
                            break;
                        case 1:
                            _views[i].OnTryUpgrade -= _fUpgrade.BaitUpgrade;
                            _fUpgrade.EnoughGoldBaitLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _fUpgrade.CanBaitLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 2:
                            _views[i].OnTryUpgrade -= _fUpgrade.RodUpgrade;
                            _fUpgrade.EnoughGoldRodLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _fUpgrade.CanRodLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 3:
                            _views[i].OnTryUpgrade -= _fUpgrade.ShipUpgrade;
                            _fUpgrade.EnoughGoldShipLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _fUpgrade.CanShipLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case EMainUpgradeType.Dining:

                _dUpgrade.OnMasterLevelUpgrade -= RenewalMasterLevel;
                _dUpgrade.OnMaxCustomerLimitLevelUpgrade -= RenewalMaxCustomerLimitLevel;
                _dUpgrade.OnMaxSpawnLimit01LevelUpgrade -= RenewalMaxSpawnLimit01Level;
                _dUpgrade.OnMaxSpawnLimit02LevelUpgrade -= RenewalMaxSpawnLimit02Level;
                _dUpgrade.OnWeightLevelUpgrade -= RenewalWeightLevel;
                _dUpgrade.OnBonusTipsMultiLevelUpgrade -= RenewalBonusTipsMultiLevel;
                _dUpgrade.OnBonusDishPrice01LevelUpgrade -= RenewalBonusDishPrice01Level;
                _dUpgrade.OnBonusDishPrice02LevelUpgrade -= RenewalBonusDishPrice02Level;
                _dUpgrade.OnBonusFood01LevelUpgrade -= RenewalBonusFood01Level;
                _dUpgrade.OnBonusFood02LevelUpgrade -= RenewalBonusFood02Level;
                _dUpgrade.OnUnlockCatObjectLevelUpgrade -= RenewalUnlockCatObjectLevel;
                
                for (int i = 0; i < _views.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _views[i].OnTryUpgrade -= _dUpgrade.MasterLevelUpgrade;
                            _dUpgrade.EnoughGoldMasterLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            break;
                        case 1:
                            _views[i].OnTryUpgrade -= _dUpgrade.MaxCustomerLimitLevelUpgrade;
                            _dUpgrade.EnoughGoldMaxCustomerLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanMaxCustomerLimitLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 2:
                            _views[i].OnTryUpgrade -= _dUpgrade.MaxCustomerLimitLevelUpgrade;
                            _dUpgrade.EnoughGoldMaxCustomerLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanMaxCustomerLimitLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 3:
                            _views[i].OnTryUpgrade -= _dUpgrade.MaxSpawnLimit01LevelUpgrade;
                            _dUpgrade.EnoughGoldMaxSpawnLimit01LevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanMaxSpawnLimit01LevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 4:
                            _views[i].OnTryUpgrade -= _dUpgrade.MaxSpawnLimit02LevelUpgrade;
                            _dUpgrade.EnoughGoldMaxSpawnLimit02LevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanMaxSpawnLimit02LevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 5:
                            _views[i].OnTryUpgrade -= _dUpgrade.WeightLevelUpgrade;
                            _dUpgrade.EnoughGoldWeightLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanWeightLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 6:
                            _views[i].OnTryUpgrade -= _dUpgrade.BonusTipsMultiLevelUpgrade;
                            _dUpgrade.EnoughGoldBonusTipsMultiLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusTipsMultiLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 7:
                            _views[i].OnTryUpgrade -= _dUpgrade.BonusDishPrice01LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusDishPrice01LevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusDishPrice01LevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 8:
                            _views[i].OnTryUpgrade -= _dUpgrade.BonusDishPrice02LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusDishPrice02LevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusDishPrice02LevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 9:
                            _views[i].OnTryUpgrade -= _dUpgrade.BonusFood01LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusFood01LevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusFood01LevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 10:
                            _views[i].OnTryUpgrade -= _dUpgrade.BonusFood02LevelUpgrade;
                            _dUpgrade.EnoughGoldBonusFood02LevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanBonusFood02LevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        case 11:
                            _views[i].OnTryUpgrade -= _dUpgrade.UnlockCatObjectLevelUpgrade;
                            _dUpgrade.EnoughGoldUnlockCatObjectLevelUpgrade -= _views[i].ToggleReqGoldTextColor;
                            _dUpgrade.CanUnlockCatObjectLevelUpgrade -= _views[i].ToggleButtenState;
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator LoadingOnEnableRoutine()
    {
        while (DataTower.instance == null)
        {
            yield return _waitForEndOfFrame;
        }
        
        while (!_isTransDataLoaded)
        {
            yield return _waitForEndOfFrame;
        }
        
        EnableEvents();
        RunSettings();
    }

    #endregion

    #region 낚시 부분

    // 낚시 등급
    private void SetViewPlayerGrade()
    {
        int level = DataTower.instance.fishingGrade;
        int maxLevel = _fDataReader.Grades.Length;
        _fUpgrade.CheckEnoughGoldFishingGradeUpgrade(DataTower.instance.money);
        _fDataReader.GetFishingGradeReqGoldData(level, out int reqGold);
        _views[0].RenewalLevelText(level,maxLevel);
        _views[0].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[0].RenewalToolTipMaxTextForFishingGrade(level);
        }
        else
        {
            _views[0].RenewalTooltipTextForFishingGrade(level);
        }
    }
    
    private void RenewalPlayerGrade(int level)
    {
        int maxLevel = _fDataReader.Grades.Length;
        _fDataReader.GetFishingGradeReqGoldData(level, out int reqGold);
        _views[0].RenewalLevelText(level,maxLevel);
        _views[0].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[0].RenewalToolTipMaxTextForFishingGrade(level);
        }
        else
        {
            _views[0].RenewalTooltipTextForFishingGrade(level);
        }
    }

    // 미끼 레벨
    private void SetViewBaitLevel()
    {
        int level = DataTower.instance.baitLevel;
        int maxLevel = _fDataReader.Baits.Length;
        _fUpgrade.CheckEnoughGoldBaitLevelUpgrade(DataTower.instance.money);
        _fUpgrade.CheckCanBaitLevelUpgrade(DataTower.instance.fishingGrade, DataTower.instance.baitLevel);
        _fDataReader.GetBaitLevelReqGoldData(level, out int reqGold);
        _views[1].RenewalLevelText(level,maxLevel);
        _views[1].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[1].RenewalToolTipMaxText(level,_fDataReader.Baits[level-1].Apply_Value.ToString());
        }
        else
        {
            _views[1].RenewalTooltipText(level,_fDataReader.Baits[level-1].Apply_Value.ToString(),_fDataReader.Baits[level].Apply_Value.ToString());
        }
    }
    
    private void RenewalBaitLevel(int level)
    {
        int maxLevel = _fDataReader.Baits.Length;
        _fDataReader.GetBaitLevelReqGoldData(level, out int reqGold);
        _views[1].RenewalLevelText(level,maxLevel);
        _views[1].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[1].RenewalToolTipMaxText(level,_fDataReader.Baits[level-1].Apply_Value.ToString());
        }
        else
        {
            _views[1].RenewalTooltipText(level,_fDataReader.Baits[level-1].Apply_Value.ToString(),_fDataReader.Baits[level].Apply_Value.ToString());
        }
    }
    
    // 낚시대 레벨
    private void SetViewRodLevel()
    {
        int level = DataTower.instance.rodLevel;
        int maxLevel = _fDataReader.Rods.Length;
        _fUpgrade.CheckEnoughGoldRodLevelUpgrade(DataTower.instance.money);
        _fUpgrade.CheckCanRodLevelUpgrade(DataTower.instance.fishingGrade, DataTower.instance.rodLevel);
        _fDataReader.GetRodLevelReqGoldData(level, out int reqGold);
        _views[2].RenewalLevelText(level,maxLevel);
        _views[2].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[2].RenewalToolTipMaxText(level,_fDataReader.Rods[level-1].Apply_Value.ToString());
        }
        else
        {
            _views[2].RenewalTooltipText(level,_fDataReader.Rods[level-1].Apply_Value.ToString(),_fDataReader.Rods[level].Apply_Value.ToString());
        }
    }
    
    private void RenewalRodLevel(int level)
    {
        int maxLevel = _fDataReader.Rods.Length;
        _fDataReader.GetRodLevelReqGoldData(level, out int reqGold);
        _views[2].RenewalLevelText(level,maxLevel);
        _views[2].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[2].RenewalToolTipMaxText(level,_fDataReader.Rods[level-1].Apply_Value.ToString());
        }
        else
        {
            _views[2].RenewalTooltipText(level,_fDataReader.Rods[level-1].Apply_Value.ToString(),_fDataReader.Rods[level].Apply_Value.ToString());
        }
    }

    // 배 레벨
    private void SetViewShipLevel()
    {
        int level = DataTower.instance.shipLevel;
        int maxLevel = _fDataReader.Ships.Length;
        _fUpgrade.CheckEnoughGoldShipLevelUpgrade(DataTower.instance.money);
        _fUpgrade.CheckCanShipLevelUpgrade(DataTower.instance.fishingGrade, DataTower.instance.shipLevel);
        _fDataReader.GetShipLevelReqGoldData(level, out int reqGold);
        _views[3].RenewalLevelText(level,maxLevel);
        _views[3].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[3].RenewalToolTipMaxText(level,_fDataReader.Ships[level-1].Apply_Value.ToString());
        }
        else
        {
            _views[3].RenewalTooltipText(level,_fDataReader.Ships[level-1].Apply_Value.ToString(),_fDataReader.Ships[level].Apply_Value.ToString());
        }
    }
    
    private void RenewalShipLevel(int level)
    {
        int maxLevel = _fDataReader.Ships.Length;
        _fDataReader.GetShipLevelReqGoldData(level, out int reqGold);
        _views[3].RenewalLevelText(level,maxLevel);
        _views[3].RenewalReqGoldText(reqGold);
        if (level == maxLevel)
        {
            _views[3].RenewalToolTipMaxText(level,_fDataReader.Ships[level-1].Apply_Value.ToString());
        }
        else
        {
            _views[3].RenewalTooltipText(level,_fDataReader.Ships[level-1].Apply_Value.ToString(),_fDataReader.Ships[level].Apply_Value.ToString());
        }
    }

    // 텍스트 세팅
    private void SetFishingTranslationTextSetting()
    {
        for (int i = 0; i < _views.Length; i++)
        {
            _views[i].UpgradeTargetData = _tDataReader.GetTranslationData(_tDataReader.GetFishingTranslationID((EFishingUpgradeType)i, 1));
            _views[i].UpgradeDescriptionData = _tDataReader.GetTranslationData(_tDataReader.GetFishingTranslationID((EFishingUpgradeType)i, 2));
            _views[i].UpgradeToolTipData = _tDataReader.GetTranslationData(_tDataReader.GetFishingTranslationID((EFishingUpgradeType)i, 3));
            _views[i].UpgradeToolTipMaxData = _tDataReader.GetTranslationData(_tDataReader.GetFishingTranslationID((EFishingUpgradeType)i, 4));
            _views[i].TranslationText(DataTower.instance.languageSetting);
        }
    }

    #endregion

    #region 식당 부분

    // 마스터 레벨
    private void SetViewMasterLevel()
    {
        int level = DataTower.instance.upgradeDatas.MasterLevel;
        int maxLevel = _dDataReader.Master_Lv.Length;
        _dUpgrade.CheckEnoughGoldMasterLevelUpgrade(DataTower.instance.money);
        _dDataReader.GetMasterLevelCostData(level, out int cost);
        _views[0].RenewalLevelText(level,maxLevel);
        _views[0].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[0].RenewalToolTipMaxText(level,_dDataReader.Master_Lv[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[0].RenewalTooltipText(level,_dDataReader.Master_Lv[level-1].Effect_Value_1.ToString(),_dDataReader.Master_Lv[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalMasterLevel(int level)
    {
        int maxLevel = _dDataReader.Master_Lv.Length;
        _dDataReader.GetMasterLevelCostData(level, out int cost);
        _views[0].RenewalLevelText(level,maxLevel);
        _views[0].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[0].RenewalToolTipMaxText(level,_dDataReader.Master_Lv[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[0].RenewalTooltipText(level,_dDataReader.Master_Lv[level-1].Effect_Value_1.ToString(),_dDataReader.Master_Lv[level].Effect_Value_1.ToString());
        }
    }
    
    // 좌석 업그레이드 레벨
    private void SetViewMaxCustomerLimitLevel()
    {
        int level = DataTower.instance.upgradeDatas.MaxCustomerLimitLevel;
        int maxLevel = _dDataReader.Max_Customer_Limit.Length;
        _dUpgrade.CheckEnoughGoldMaxCustomerLimitLevelUpgrade(DataTower.instance.money);
        _dDataReader.GetMaxCustomerLimitCostData(level, out int cost);
        _views[1].RenewalLevelText(level,maxLevel);
        _views[1].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[1].RenewalToolTipMaxText(level,_dDataReader.Max_Customer_Limit[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[1].RenewalTooltipText(level,_dDataReader.Max_Customer_Limit[level-1].Effect_Value_1.ToString(),_dDataReader.Max_Customer_Limit[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalMaxCustomerLimitLevel(int level)
    {
        int maxLevel = _dDataReader.Max_Customer_Limit.Length;
        _dDataReader.GetMaxCustomerLimitCostData(level, out int cost);
        _views[1].RenewalLevelText(level,maxLevel);
        _views[1].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[1].RenewalToolTipMaxText(level,_dDataReader.Max_Customer_Limit[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[1].RenewalTooltipText(level,_dDataReader.Max_Customer_Limit[level-1].Effect_Value_1.ToString(),_dDataReader.Max_Customer_Limit[level].Effect_Value_1.ToString());
        }
    }
    
    // 특별 손님 업그레이드 레벨
    private void SetViewMaxSpawnLimit01Level()
    {
        int level = DataTower.instance.upgradeDatas.MaxSpawnLimit01Level;
        int maxLevel = _dDataReader.Max_Spawn_Limit_1.Length;
        _dUpgrade.CheckEnoughGoldMaxSpawnLimit01LevelUpgrade(DataTower.instance.money);
        _dDataReader.GetMaxSpawnLimit01CostData(level, out int cost);
        _views[2].RenewalLevelText(level,maxLevel);
        _views[2].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[2].RenewalToolTipMaxText(level,_dDataReader.Max_Spawn_Limit_1[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[2].RenewalTooltipText(level,_dDataReader.Max_Spawn_Limit_1[level-1].Effect_Value_1.ToString(),_dDataReader.Max_Spawn_Limit_1[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalMaxSpawnLimit01Level(int level)
    {
        int maxLevel = _dDataReader.Max_Spawn_Limit_1.Length;
        _dDataReader.GetMaxSpawnLimit01CostData(level, out int cost);
        _views[2].RenewalLevelText(level,maxLevel);
        _views[2].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[2].RenewalToolTipMaxText(level,_dDataReader.Max_Spawn_Limit_1[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[2].RenewalTooltipText(level,_dDataReader.Max_Spawn_Limit_1[level-1].Effect_Value_1.ToString(),_dDataReader.Max_Spawn_Limit_1[level].Effect_Value_1.ToString());
        }
    }
    
    // VIP 업그레이드 레벨
    private void SetViewMaxSpawnLimit02Level()
    {
        int level = DataTower.instance.upgradeDatas.MaxSpawnLimit02Level;
        int maxLevel = _dDataReader.Max_Spawn_Limit_2.Length;
        _dUpgrade.CheckEnoughGoldMaxSpawnLimit02LevelUpgrade(DataTower.instance.money);
        _dDataReader.GetMaxSpawnLimit02CostData(level, out int cost);
        _views[3].RenewalLevelText(level,maxLevel);
        _views[3].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[3].RenewalToolTipMaxText(level,_dDataReader.Max_Spawn_Limit_2[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[3].RenewalTooltipText(level,_dDataReader.Max_Spawn_Limit_2[level-1].Effect_Value_1.ToString(),_dDataReader.Max_Spawn_Limit_2[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalMaxSpawnLimit02Level(int level)
    {
        int maxLevel = _dDataReader.Max_Spawn_Limit_2.Length;
        _dDataReader.GetMaxSpawnLimit02CostData(level, out int cost);
        _views[3].RenewalLevelText(level,maxLevel);
        _views[3].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[3].RenewalToolTipMaxText(level,_dDataReader.Max_Spawn_Limit_2[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[3].RenewalTooltipText(level,_dDataReader.Max_Spawn_Limit_2[level-1].Effect_Value_1.ToString(),_dDataReader.Max_Spawn_Limit_2[level].Effect_Value_1.ToString());
        }
    }
    
    // 팁주는 손님 가중치 업그레이드 레벨
    private void SetViewWeightLevel()
    {
        int level = DataTower.instance.upgradeDatas.WeightLevel;
        int maxLevel = _dDataReader.Weight.Length;
        _dUpgrade.CheckEnoughGoldWeightLevelUpgrade(DataTower.instance.money);
        _dDataReader.GetWeightCostData(level, out int cost);
        _views[4].RenewalLevelText(level,maxLevel);
        _views[4].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[4].RenewalToolTipMaxText(level,_dDataReader.Weight[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[4].RenewalTooltipText(level,_dDataReader.Weight[level-1].Effect_Value_1.ToString(),_dDataReader.Weight[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalWeightLevel(int level)
    {
        int maxLevel = _dDataReader.Weight.Length;
        _dDataReader.GetWeightCostData(level, out int cost);
        _views[4].RenewalLevelText(level,maxLevel);
        _views[4].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[4].RenewalToolTipMaxText(level,_dDataReader.Weight[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[4].RenewalTooltipText(level,_dDataReader.Weight[level-1].Effect_Value_1.ToString(),_dDataReader.Weight[level].Effect_Value_1.ToString());
        }
    }
    
    // 모금함(팁 액수 증가) 업그레이드 레벨
    private void SetViewBonusTipsMultiLevel()
    {
        int level = DataTower.instance.upgradeDatas.BonusTipsMultiLevel;
        int maxLevel = _dDataReader.Bonus_Tips_Multi.Length;
        _dUpgrade.CheckEnoughGoldBonusTipsMultiLevelUpgrade(DataTower.instance.money);
        _dDataReader.GetBonusTipsMultiCostData(level, out int cost);
        _views[5].RenewalLevelText(level,maxLevel);
        _views[5].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[5].RenewalToolTipMaxText(level,_dDataReader.Bonus_Tips_Multi[level-1].Effect_Value_2.ToString());
        }
        else
        {
            _views[5].RenewalTooltipText(level,_dDataReader.Bonus_Tips_Multi[level-1].Effect_Value_2.ToString(),_dDataReader.Bonus_Tips_Multi[level].Effect_Value_2.ToString());
        }
    }
    
    private void RenewalBonusTipsMultiLevel(int level)
    {
        int maxLevel = _dDataReader.Bonus_Tips_Multi.Length;
        _dDataReader.GetBonusTipsMultiCostData(level, out int cost);
        _views[5].RenewalLevelText(level,maxLevel);
        _views[5].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[5].RenewalToolTipMaxText(level,_dDataReader.Bonus_Tips_Multi[level-1].Effect_Value_2.ToString());
        }
        else
        {
            _views[5].RenewalTooltipText(level,_dDataReader.Bonus_Tips_Multi[level-1].Effect_Value_2.ToString(),_dDataReader.Bonus_Tips_Multi[level].Effect_Value_2.ToString());
        }
    }

    // 계산대(요리 가격 증가) 업그레이드 레벨
    private void SetViewBonusDishPrice01Level()
    {
        int level = DataTower.instance.upgradeDatas.BonusDishPrice01Level;
        int maxLevel = _dDataReader.Bonus_Dish_Price_1.Length;
        _dUpgrade.CheckEnoughGoldBonusDishPrice01LevelUpgrade(DataTower.instance.money);
        _dDataReader.GetBonusDishPrice01CostData(level, out int cost);
        _views[6].RenewalLevelText(level,maxLevel);
        _views[6].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[6].RenewalToolTipMaxText(level,_dDataReader.Bonus_Dish_Price_1[level-1].Effect_Value_2.ToString());
        }
        else
        {
            _views[6].RenewalTooltipText(level,_dDataReader.Bonus_Dish_Price_1[level-1].Effect_Value_2.ToString(),_dDataReader.Bonus_Dish_Price_1[level].Effect_Value_2.ToString());
        }
    }
    
    private void RenewalBonusDishPrice01Level(int level)
    {
        int maxLevel = _dDataReader.Bonus_Dish_Price_1.Length;
        _dDataReader.GetBonusDishPrice01CostData(level, out int cost);
        _views[6].RenewalLevelText(level,maxLevel);
        _views[6].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[6].RenewalToolTipMaxText(level,_dDataReader.Bonus_Dish_Price_1[level-1].Effect_Value_2.ToString());
        }
        else
        {
            _views[6].RenewalTooltipText(level,_dDataReader.Bonus_Dish_Price_1[level-1].Effect_Value_2.ToString(),_dDataReader.Bonus_Dish_Price_1[level].Effect_Value_2.ToString());
        }
    }
    
    // 밥솥(요리 가격 증가) 업그레이드 레벨
    private void SetViewBonusDishPrice02Level()
    {
        int level = DataTower.instance.upgradeDatas.BonusDishPrice02Level;
        int maxLevel = _dDataReader.Bonus_Dish_Price_2.Length;
        _dUpgrade.CheckEnoughGoldBonusDishPrice02LevelUpgrade(DataTower.instance.money);
        _dDataReader.GetBonusDishPrice02CostData(level, out int cost);
        _views[7].RenewalLevelText(level,maxLevel);
        _views[7].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[7].RenewalToolTipMaxText(level,_dDataReader.Bonus_Dish_Price_2[level-1].Effect_Value_2.ToString());
        }
        else
        {
            _views[7].RenewalTooltipText(level,_dDataReader.Bonus_Dish_Price_2[level-1].Effect_Value_2.ToString(),_dDataReader.Bonus_Dish_Price_2[level].Effect_Value_2.ToString());
        }
    }
    
    private void RenewalBonusDishPrice02Level(int level)
    {
        int maxLevel = _dDataReader.Bonus_Dish_Price_2.Length;
        _dDataReader.GetBonusDishPrice02CostData(level, out int cost);
        _views[7].RenewalLevelText(level,maxLevel);
        _views[7].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[7].RenewalToolTipMaxText(level,_dDataReader.Bonus_Dish_Price_2[level-1].Effect_Value_2.ToString());
        }
        else
        {
            _views[7].RenewalTooltipText(level,_dDataReader.Bonus_Dish_Price_2[level-1].Effect_Value_2.ToString(),_dDataReader.Bonus_Dish_Price_2[level].Effect_Value_2.ToString());
        }
    }
    
    // 식칼(요리 개수 증가) 업그레이드 레벨
    private void SetViewBonusFood01Level()
    {
        int level = DataTower.instance.upgradeDatas.BonusFood01Level;
        int maxLevel = _dDataReader.Bonus_Food_1.Length;
        _dUpgrade.CheckEnoughGoldBonusFood01LevelUpgrade(DataTower.instance.money);
        _dDataReader.GetBonusFood01CostData(level, out int cost);
        _views[8].RenewalLevelText(level,maxLevel);
        _views[8].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[8].RenewalToolTipMaxText(level,_dDataReader.Bonus_Food_1[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[8].RenewalTooltipText(level,_dDataReader.Bonus_Food_1[level-1].Effect_Value_1.ToString(),_dDataReader.Bonus_Food_1[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalBonusFood01Level(int level)
    {
        int maxLevel = _dDataReader.Bonus_Food_1.Length;
        _dDataReader.GetBonusFood01CostData(level, out int cost);
        _views[8].RenewalLevelText(level,maxLevel);
        _views[8].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[8].RenewalToolTipMaxText(level,_dDataReader.Bonus_Food_1[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[8].RenewalTooltipText(level,_dDataReader.Bonus_Food_1[level-1].Effect_Value_1.ToString(),_dDataReader.Bonus_Food_1[level].Effect_Value_1.ToString());
        }
    }
    
    // 도마(요리 개수 증가) 업그레이드 레벨
    private void SetViewBonusFood02Level()
    {
        int level = DataTower.instance.upgradeDatas.BonusFood02Level;
        int maxLevel = _dDataReader.Bonus_Food_2.Length;
        _dUpgrade.CheckEnoughGoldBonusFood02LevelUpgrade(DataTower.instance.money);
        _dDataReader.GetBonusFood02CostData(level, out int cost);
        _views[9].RenewalLevelText(level,maxLevel);
        _views[9].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[9].RenewalToolTipMaxText(level,_dDataReader.Bonus_Food_2[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[9].RenewalTooltipText(level,_dDataReader.Bonus_Food_2[level-1].Effect_Value_1.ToString(),_dDataReader.Bonus_Food_2[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalBonusFood02Level(int level)
    {
        int maxLevel = _dDataReader.Bonus_Food_2.Length;
        _dDataReader.GetBonusFood02CostData(level, out int cost);
        _views[9].RenewalLevelText(level,maxLevel);
        _views[9].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[9].RenewalToolTipMaxText(level,_dDataReader.Bonus_Food_2[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[9].RenewalTooltipText(level,_dDataReader.Bonus_Food_2[level-1].Effect_Value_1.ToString(),_dDataReader.Bonus_Food_2[level].Effect_Value_1.ToString());
        }
    }
    
    // 고양이 업그레이드 레벨
    private void SetViewUnlockCatObjectLevel()
    {
        int level = DataTower.instance.upgradeDatas.UnlockCatObjectLevel;
        int maxLevel = _dDataReader.Unlock_Cat_Object.Length;
        _dUpgrade.CheckEnoughGoldUnlockCatObjectLevelUpgrade(DataTower.instance.money);
        _dDataReader.GetUnlockCatObjectCostData(level, out int cost);
        _views[10].RenewalLevelText(level,maxLevel);
        _views[10].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[10].RenewalToolTipMaxText(level,_dDataReader.Unlock_Cat_Object[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[10].RenewalTooltipText(level,_dDataReader.Unlock_Cat_Object[level-1].Effect_Value_1.ToString(),_dDataReader.Unlock_Cat_Object[level].Effect_Value_1.ToString());
        }
    }
    
    private void RenewalUnlockCatObjectLevel(int level)
    {
        int maxLevel = _dDataReader.Unlock_Cat_Object.Length;
        _dDataReader.GetUnlockCatObjectCostData(level, out int cost);
        _views[10].RenewalLevelText(level,maxLevel);
        _views[10].RenewalReqGoldText(cost);
        if (level == maxLevel)
        {
            _views[10].RenewalToolTipMaxText(level,_dDataReader.Unlock_Cat_Object[level-1].Effect_Value_1.ToString());
        }
        else
        {
            _views[10].RenewalTooltipText(level,_dDataReader.Unlock_Cat_Object[level-1].Effect_Value_1.ToString(),_dDataReader.Unlock_Cat_Object[level].Effect_Value_1.ToString());
        }
    }
    
    // 텍스트 세팅
    private void SetDiningTranslationTextSetting()
    {
        for (int i = 0; i < _views.Length; i++)
        {
            _views[i].UpgradeTargetData = _tDataReader.GetTranslationData(_tDataReader.GetDiningTranslationID((EDiningUpgradeType)i, 1));
            _views[i].UpgradeDescriptionData = _tDataReader.GetTranslationData(_tDataReader.GetDiningTranslationID((EDiningUpgradeType)i, 2));
            _views[i].UpgradeToolTipData = _tDataReader.GetTranslationData(_tDataReader.GetDiningTranslationID((EDiningUpgradeType)i, 3));
            // _views[i].UpgradeToolTipMaxData = _tDataReader.GetTranslationData(_tDataReader.GetDiningTranslationID((EDiningUpgradeType)i, 4));
            _views[i].TranslationText(DataTower.instance.languageSetting);
        }
    }

    #endregion
    
}

