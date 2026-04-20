/*
Data Tower : 중앙 데이터 관리자. / 대부분의 데이터에 대하여, 중앙에서 관리 및 저장에 용의성을 높이기 위해 제작되었습니다.

나중에 해야할 일 TODO : 해당 데이터들 전부다 SO로 빼서 관리 필요(예상보다 더 많은 변수들이 필요해져서 로드 과정 중에 병목이 발생한 거 같다는 예상.)

*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTower : MonoBehaviour
{
    #region 기본 변수

    [SerializeField] DataContainer _dataCon;

    /// <summary>
    /// 소지 금액 ulong으로 받아야함.
    /// </summary>
    public ulong money { get; private set; }

    /// <summary>
    /// 차감 후 금액 반환.
    /// </summary>
    public event Action<ulong> OnChangedMoney;

    /// <summary>
    /// 입력된 값으로 차감 시도
    /// 성공한 경우 : OnChangedMoney 호출 및 ture 반환
    /// 실패한 경우 : false 반환 (가급적이면 false 반환하지 않게 예외처리 할 것.)
    /// </summary>
    /// <param name="mny">차감할 금액</param>
    /// <param name="isWithdraw">돈 추가인지 차감인지</param>
    /// <returns></returns>
    public bool TryMoenyChanged(ulong mny, bool isWithdraw = true)
    {
        if (money - mny < 0)
            return false;
        if (isWithdraw)
            money -= mny;
        else
            money += mny;
        OnChangedMoney?.Invoke(money);
        return true;
    }

    public UpgradeDatas upgradeDatas;

    public int MasterLevel;

    public int MaxCustomerLimitLevel;

    public int MaxMenuLimitLevel;

    public int MaxSpawnLimit01Level;

    public int MaxSpawnLimit02Level;

    public int WeightLevel;

    public int BonusTipsMultiLevel;

    public int BonusDishPrice01Level;

    public int BonusDishPrice02Level;

    public int BonusFood01Level;

    public int BonusFood02Level;

    public int UnlockGramophoneLevel;

    public int UnlockCatObjectLevel;

    public int UnlockMenuLevel;
    #endregion

    public RestaurantDatas restaurantDatas;

    #region 낚시 변수
    #region 구형 변수
    /// <summary>
    /// 플레이어 등급 낚시 관련 레벨에 대해서 해당 부분에서 예외 처리가 없으므로, UI 작업자가 예외처리 열심히 해주셔야합니다.
    /// </summary>
    public int fishingGrade;
    /// <summary>
    /// 미끼 레벨
    /// </summary>
    public int baitLevel;
    /// <summary>
    /// 낚시대 레벨
    /// </summary>
    public int rodLevel;
    /// <summary>
    /// shipLevel;
    /// </summary>
    public int shipLevel;

    /// <summary>
    /// 최대로 저장할 수 있는 미끼 수
    /// </summary>
    public int fishingCount;

    /// <summary>
    /// 현재 갖고 있는 미끼 수.
    /// </summary>
    public int currentFishingCount;

    #endregion

    public FisherDatas fihserDatas;

    #region 구형변수
    /// <summary>
    /// 현재까지 잡은 총 물고기 수
    /// </summary>
    public ulong catchFishs;

    /// <summary>
    /// 희귀도가 쓰레기인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishTrash;

    /// <summary>
    /// 희귀도가 일반인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishNormal;

    /// <summary>
    /// 희귀도가 우수인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishFine;

    /// <summary>
    /// 희귀도가 고급인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishSuperior;

    /// <summary>
    /// 희귀도가 희귀인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishRare;

    /// <summary>
    /// 희귀도가 명품인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishElite;

    /// <summary>
    /// 희귀도가 환상인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishFantastic;

    /// <summary>
    /// 희귀도가 전설인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishLegendary;

    #endregion

    public CatchFishs catchedFishs;

    /// <summary>
    /// 현재 미끼 충전 타이머.
    /// </summary>
    public float fishingTime;

    /// <summary>
    /// 충전 타이머 시작점
    /// </summary>
    public float maxFishingTime;

    #endregion

    /// <summary>
    /// 데이터 초기화 되었는지 확인 함수
    /// </summary>
    public bool isDataInitiated = false;

    /// <summary>
    /// DataTower 싱글톤 패턴
    /// </summary>
    public static DataTower instance;

    /// <summary>
    /// 인벤토리용 리스트
    /// </summary>
    public List<FishData> Items = new List<FishData>();

    /// <summary>
    /// 인벤토리 슬롯 최댓값.
    /// </summary>
    public int InventorySlotMax;

    public event Action<string> OnFisingNewFish;
    public event Action<Language> OnLanguageSettingChanged;

    public event Action OnDataTowerLoaded;

    [SerializeField] private InventorySystem _inventorySystem; // 참조 걸어줄 방식 지정 필요.
    public Dictionary<string, FishData> fishDatas;  // 물고기 고유번호, 물고기 저장방식(SO) 기입 후 사용 예정. 목적 : 데이터 검사용 예시 : 해당 물고기가 도감에 등록 되어 있는지

    public Language languageSetting { get; private set; } /// <summary> 현재 설정된 언어, 기본 값 : 영어 </summary>

    public PersonalOptions pOptions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
        languageSetting = Language.ENG; // 기본 값 영어로 출력 되게 설정 되었습니다. 
        InitializedData();
        StartCoroutine(DataRead());
        OnDataTowerLoaded?.Invoke();
    }

    /// <summary>
    /// 데이터 타워 초기화 명령 함수
    /// true로 넣을 시 강제 초기화 진행됌
    /// </summary>
    /// <param name="ForcedInitialized">true인 경우 강제 초기화 실행</param>
    public void InitializedData(bool ForcedInitialized = false)
    {
        if (!isDataInitiated || !ForcedInitialized)
        {
            if (ForcedInitialized)
                Debug.Log("강제 초기화 실행");

            money = 1000;

            InventorySlotMax = 10;

            fihserDatas = new FisherDatas(1, 1, 1, 1, 1, 1);
            catchedFishs = new CatchFishs(0, 0, 0, 0, 0, 0, 0, 0, 0);
            pOptions = new PersonalOptions(0.5f, 0.5f, 0.5f, 100, Language.ENG);
            restaurantDatas = new RestaurantDatas(0,0,0,0);
            upgradeDatas = new UpgradeDatas(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            fishingTime = 3600;
            maxFishingTime = 3600;
        }
        else
        {
            Debug.LogWarning("강제 되지 않은 초기화 선언 감지됌");
        }
    }

    IEnumerator DataRead()
    {
        while (!_dataCon.isDataLoaded)
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (_dataCon.objs[0] is not FishData)
        {
            Debug.LogError($"{gameObject.name}에 저장된 DataContainer가 FishData가 들어있지 않는 컨테이너입니다.");
            yield break;
        }


        fishDatas = new Dictionary<string, FishData>(_dataCon.objs.Length);
        FishData _tmpFishData;
        for (int i = 0; i < _dataCon.objs.Length; i++)
        {
            _tmpFishData = _dataCon.objs[i] as FishData;
            fishDatas.Add(_tmpFishData.fishID, _tmpFishData);
        }
        yield break;
    }

    /// <summary>
    /// 저장을 위해서 데이터를 호출 할 수 있는 부분.
    /// </summary>
    public void PullData()
    {

    }

    /// <summary>
    /// 언어 설정을 위한 함수. 설정에서만 호출 할 것.
    /// </summary>
    /// <param name="lan">KOR : 한글 , ENG : 영어.</param>
    public void ChangeLanguage(Language lan)
    {
        languageSetting = lan;
        OnLanguageSettingChanged?.Invoke(languageSetting);
    }

    // 돈
    // 인벤토리, 생선
    // 도감????
    // 손님 방문 횟수. 잠수 중 지나간 손님 수
    // 업그레이드, 어떤 걸 얼만큼 업그레이드 되어 있는 지.
    // 초밥 수
    // 언어 등 개인 설정
    // 미끼 갯수
    // 타이머



    #region 식당 관련 함수들

    #endregion

    #region 낚시 관련 함수들
    // 아마 So로 넘어올거 같다.
    /// <summary>
    /// 물고기를 잡은 경우 호출.
    /// </summary>
    public void takeFish(FishData fish)
    {
        Debug.Log($"들어온 물고기 정보 : {fish.fishID}");
        _inventorySystem.Insert(fish.fishID); // Item SO 변경 후 작업 @@@@@@@@@@@@@@@@@@@@@@@@@@
        if (!fishDatas[fish.fishID].isCaught) // 딕셔너리에 있는 지 확인 및 있지 않다면 높은 확률로 새로운 물고기
        {
            fishDatas[fish.fishID].isCaught = true;

            OnFisingNewFish?.Invoke(fish.fishID);
        }
        CatchFishCounter(in fish);
    }
    #endregion


    void CatchFishCounter(in FishData fish)
    {
        switch (fish.fishRarity)
        {
            case EFish_Rarity.Trash:
                catchFishTrash++;
                break;
            case EFish_Rarity.Normal:
                catchFishNormal++;
                break;
            case EFish_Rarity.Fine:
                catchFishFine++;
                break;
            case EFish_Rarity.Superior:
                catchFishSuperior++;
                break;
            case EFish_Rarity.Rare:
                catchFishRare++;
                break;
            case EFish_Rarity.Elite:
                catchFishElite++;
                break;
            case EFish_Rarity.Fantastic:
                catchFishFantastic++;
                break;
            case EFish_Rarity.Legendary:
                catchFishLegendary++;
                break;
        }
    }
}
