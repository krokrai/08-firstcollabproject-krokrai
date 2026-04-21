using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class FishingUpgradeManager : MonoBehaviour
{
    [SerializeField] private DataContainer _container;
    
    private FishingUpgradeDataReader _dataReader;
    private WaitForEndOfFrame _waitForEndOfFrame = new();
    
    private int _fishingGradeReqGold;
    private int _baitLevelReqGold;
    private int _rodLevelReqGold;
    private int _shipLevelReqGold;
    
    #region Events
    
    /// <summary>
    /// 낚시 등급 상승 시 효과 발동을 위한 이벤트
    /// </summary>
    public event Action<int> OnFishingUpgrade;
    
    /// <summary>
    /// 미끼 레벨 상승 시 효과 발동을 위한 이벤트
    /// </summary>
    public event Action<int> OnBaitUpgrade;
    
    /// <summary>
    /// 낚시대 레벨 상승 시 효과 발동을 위한 이벤트
    /// </summary>
    public event Action<int> OnRodUpgrade;
    
    /// <summary>
    /// 배 레벨 상승 시 효과 발동을 위한 이벤트
    /// </summary>
    public event Action<int> OnShipUpgrade;
    
    /// <summary>
    /// 업그레이드가 불가능하면 버튼을 사용 불가로 만드는 이벤트
    /// </summary>
    public event Action<bool> CanBaitLevelUpgrade;
    
    /// <summary>
    /// 업그레이드가 불가능하면 버튼을 사용 불가로 만드는 이벤트
    /// </summary>
    public event Action<bool> CanRodLevelUpgrade;
    
    /// <summary>
    /// 업그레이드가 불가능하면 버튼을 사용 불가로 만드는 이벤트
    /// </summary>
    public event Action<bool> CanShipLevelUpgrade;
    
    
    /// <summary>
    /// 골드가 부족하면 버튼의 색을 변화하는 이벤트
    /// </summary>
    public event Action<bool> EnoughGoldFishingGradeUpgrade;
    
    /// <summary>
    /// 골드가 부족하면 버튼의 색을 변화하는 이벤트
    /// </summary>
    public event Action<bool> EnoughGoldBaitLevelUpgrade;
    
    /// <summary>
    /// 골드가 부족하면 버튼의 색을 변화하는 이벤트
    /// </summary>
    public event Action<bool> EnoughGoldRodLevelUpgrade;
    
    /// <summary>
    /// 골드가 부족하면 버튼의 색을 변화하는 이벤트
    /// </summary>
    public event Action<bool> EnoughGoldShipLevelUpgrade;

    #endregion

    /// <summary>
    /// ToDo:DataTower로 변수 이관 되면 사용 안함
    /// </summary>
    private void Awake()
    {
        _dataReader = GetComponentInChildren<FishingUpgradeDataReader>();
    }

    private void OnEnable()
    {
        StartCoroutine(LoadingOnEnableRoutine());
    }

    private void OnDisable()
    {
        EventDisable();
    }

    #region 이벤트 구독/해제

    private void EventEnable()
    {
        DataTower.instance.OnChangedMoney += CheckEnoughGoldFishingGradeUpgrade;
        DataTower.instance.OnChangedMoney += CheckEnoughGoldBaitLevelUpgrade;
        DataTower.instance.OnChangedMoney += CheckEnoughGoldRodLevelUpgrade;
        DataTower.instance.OnChangedMoney += CheckEnoughGoldShipLevelUpgrade;
    }

    private void EventDisable()
    {
        DataTower.instance.OnChangedMoney -= CheckEnoughGoldFishingGradeUpgrade;
        DataTower.instance.OnChangedMoney -= CheckEnoughGoldBaitLevelUpgrade;
        DataTower.instance.OnChangedMoney -= CheckEnoughGoldRodLevelUpgrade;
        DataTower.instance.OnChangedMoney -= CheckEnoughGoldShipLevelUpgrade;
    }

    private IEnumerator LoadingOnEnableRoutine()
    {
        while (DataTower.instance == null)
        {
            yield return _waitForEndOfFrame;
        }

        while (!_container.isDataLoaded)
        {
            yield return _waitForEndOfFrame;
        }
        
        EventEnable();
        CheckCanUpgrades();
        CheckReqGolds();
    }

    #endregion

    #region 업그레이드 실행

    /// <summary>
    /// 낚시 레벨 증가
    /// </summary>
    public void FishingUpgrade()
    {
        if (CanFishingGradeUp(DataTower.instance.money))
        {
            DataTower.instance.fihserDatas.fishingGrade++;
            DataTower.instance.TryMoenyChanged((ulong)_fishingGradeReqGold);
            OnFishingUpgrade?.Invoke(DataTower.instance.fihserDatas.fishingGrade);
            CheckCanUpgrades();
            CheckReqGolds();
        }
        
    }

    /// <summary>
    /// 미끼 레벨 증가
    /// </summary>
    public void BaitUpgrade()
    {
        if (CanBaitLevelUp(DataTower.instance.money))
        {
            DataTower.instance.fihserDatas.baitLevel++;
            DataTower.instance.TryMoenyChanged((ulong)_baitLevelReqGold);
            OnBaitUpgrade?.Invoke(DataTower.instance.fihserDatas.baitLevel);
            CheckCanUpgrades();
            CheckReqGolds();
        }
        
    }

    /// <summary>
    /// 낚시대 레벨 증가
    /// </summary>
    public void RodUpgrade()
    {
        if (CanRodLevelUp(DataTower.instance.money))
        {
            DataTower.instance.fihserDatas.rodLevel++;
            DataTower.instance.TryMoenyChanged((ulong)_rodLevelReqGold);
            OnRodUpgrade?.Invoke(DataTower.instance.fihserDatas.rodLevel);
            CheckCanUpgrades();
            CheckReqGolds();
        }
        
    }

    /// <summary>
    /// 보트 레벨 증가
    /// </summary>
    public void ShipUpgrade()
    {
        if (CanShipLevelUp(DataTower.instance.money))
        {
            DataTower.instance.fihserDatas.shipLevel++;
            DataTower.instance.TryMoenyChanged((ulong)_shipLevelReqGold);
            OnShipUpgrade?.Invoke(DataTower.instance.fihserDatas.shipLevel);
            CheckCanUpgrades();
            CheckReqGolds();
        }
        
    }

    #endregion

    #region 업그레이드 관련 메서드

    private bool CanFishingGradeUp(ulong curGold)
    {
        bool chackLevel = _dataReader.Grades.Length > DataTower.instance.fihserDatas.fishingGrade;
        bool chackGold = curGold >= (ulong)_fishingGradeReqGold;
        
        return chackGold && chackLevel;
    }

    /// <summary>
    /// 골드 업그레이드 가능한지 체크해서 UI 필요 골드의 색상 변화
    /// 이벤트도 연결 되어있지만 UI Enable시 마다 실행 해줘야되서 public임
    /// </summary>
    /// <param name="curGold"></param>
    public void CheckEnoughGoldFishingGradeUpgrade(ulong curGold)
    {
        bool result = curGold >= (ulong)_fishingGradeReqGold;
        EnoughGoldFishingGradeUpgrade?.Invoke(result);
    }
    
    private bool CanBaitLevelUp(ulong curGold)
    {
        bool chackLevel = _dataReader.Grades.Length > DataTower.instance.fihserDatas.baitLevel;
        bool chackGold = curGold >= (ulong)_baitLevelReqGold;
        bool chackGrade = CheckCanBaitLevelUpgrade(DataTower.instance.fihserDatas.fishingGrade,DataTower.instance.fihserDatas.baitLevel);
        
        
        return chackGold && chackLevel && chackGrade;
    }
    
    /// <summary>
    /// 골드 업그레이드 가능한지 체크해서 UI 필요 골드의 색상 변화
    /// 이벤트도 연결 되어있지만 UI Enable시 마다 실행 해줘야되서 public임
    /// </summary>
    /// <param name="curGold"></param>
    public void CheckEnoughGoldBaitLevelUpgrade(ulong curGold)
    {
        bool result = curGold >= (ulong)_baitLevelReqGold;
        EnoughGoldBaitLevelUpgrade?.Invoke(result);
    }

    /// <summary>
    /// 버튼 활성화 가능한지 체크해서 버튼 활성화 세팅
    /// 이벤트도 연결 되어있지만 UI Enable시 마다 실행 해줘야되서 public임
    /// </summary>
    /// <param name="fishingGrade"></param>
    /// <param name="baitLevel"></param>
    /// <returns></returns>
    public bool CheckCanBaitLevelUpgrade(int fishingGrade, int baitLevel)
    {
        bool result = fishingGrade/2f > baitLevel;
        CanBaitLevelUpgrade?.Invoke(result);

        return result;
    }

    private bool CanRodLevelUp(ulong curGold)
    {
        bool chackLevel = _dataReader.Grades.Length > DataTower.instance.fihserDatas.rodLevel;
        bool chackGold = curGold >= (ulong)_rodLevelReqGold;
        bool chackGrade = CheckCanRodLevelUpgrade(DataTower.instance.fihserDatas.fishingGrade,DataTower.instance.fihserDatas.rodLevel);
        
        return chackGold && chackLevel && chackGrade;
    }
    
    /// <summary>
    /// 골드 업그레이드 가능한지 체크해서 UI 필요 골드의 색상 변화
    /// 이벤트도 연결 되어있지만 UI Enable시 마다 실행 해줘야되서 public임
    /// </summary>
    /// <param name="curGold"></param>
    public void CheckEnoughGoldRodLevelUpgrade(ulong curGold)
    {
        bool result = curGold >= (ulong)_rodLevelReqGold;
        EnoughGoldRodLevelUpgrade?.Invoke(result);
    }
    
    /// <summary>
    /// 버튼 활성화 가능한지 체크해서 버튼 활성화 세팅
    /// 이벤트도 연결 되어있지만 UI Enable시 마다 실행 해줘야되서 public임
    /// </summary>
    /// <param name="fishingGrade"></param>
    /// <param name="rodLevel"></param>
    /// <returns></returns>
    public bool CheckCanRodLevelUpgrade(int fishingGrade, int rodLevel)
    {
        bool result = fishingGrade/2f > rodLevel;
        CanRodLevelUpgrade?.Invoke(result);

        return result;
    }

    private bool CanShipLevelUp(ulong curGold)
    {
        bool chackLevel = _dataReader.Grades.Length > DataTower.instance.fihserDatas.shipLevel;
        bool chackGold = curGold >= (ulong)_shipLevelReqGold;
        bool chackGrade = CheckCanShipLevelUpgrade(DataTower.instance.fihserDatas.fishingGrade,DataTower.instance.fihserDatas.shipLevel);
        
        return chackGold && chackLevel && chackGrade;
    }
    
    /// <summary>
    /// 골드 업그레이드 가능한지 체크해서 UI 필요 골드의 색상 변화
    /// 이벤트도 연결 되어있지만 UI Enable시 마다 실행 해줘야되서 public임
    /// </summary>
    /// <param name="curGold"></param>
    public void CheckEnoughGoldShipLevelUpgrade(ulong curGold)
    {
        bool result = curGold >= (ulong)_shipLevelReqGold;
        EnoughGoldShipLevelUpgrade?.Invoke(result);
    }
    
    /// <summary>
    /// 버튼 활성화 가능한지 체크해서 버튼 활성화 세팅
    /// 이벤트도 연결 되어있지만 UI Enable시 마다 실행 해줘야되서 public임
    /// </summary>
    /// <param name="fishingGrade"></param>
    /// <param name="shipLevel"></param>
    /// <returns></returns>
    public bool CheckCanShipLevelUpgrade(int fishingGrade, int shipLevel)
    {
        bool result = fishingGrade/2f > shipLevel;
        CanShipLevelUpgrade?.Invoke(result);

        return result;
    }
    
    /// <summary>
    /// 업그레이드 가능한지 체크
    /// </summary>
    public void CheckCanUpgrades()
    {
        CheckCanBaitLevelUpgrade(DataTower.instance.fihserDatas.fishingGrade,DataTower.instance.fihserDatas.baitLevel);
        CheckCanRodLevelUpgrade(DataTower.instance.fihserDatas.fishingGrade, DataTower.instance.fihserDatas.rodLevel);
        CheckCanShipLevelUpgrade(DataTower.instance.fihserDatas.fishingGrade, DataTower.instance.fihserDatas.shipLevel);
    }
    
    public void CheckReqGolds()
    {
        _dataReader.GetFishingGradeReqGoldData(DataTower.instance.fihserDatas.fishingGrade ,out _fishingGradeReqGold);
        _dataReader.GetBaitLevelReqGoldData(DataTower.instance.fihserDatas.baitLevel ,out _baitLevelReqGold);
        _dataReader.GetRodLevelReqGoldData(DataTower.instance.fihserDatas.rodLevel ,out _rodLevelReqGold);
        _dataReader.GetShipLevelReqGoldData(DataTower.instance.fihserDatas.shipLevel ,out _shipLevelReqGold);
        CheckEnoughGoldFishingGradeUpgrade(DataTower.instance.money);
        CheckEnoughGoldBaitLevelUpgrade(DataTower.instance.money);
        CheckEnoughGoldRodLevelUpgrade(DataTower.instance.money);
        CheckEnoughGoldShipLevelUpgrade(DataTower.instance.money);
    }

    #endregion
}
