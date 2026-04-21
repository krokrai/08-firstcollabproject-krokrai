using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("확률 데이터 리스트")]
    public List<FishRateData> fishRateList = new List<FishRateData>();

    [Header("스크립트 연결")]
    [SerializeField] private FishingTimer _timer;
    [SerializeField] private FishingUpgradeManager _upgradeManager; // 업그레이드 데이터 참조

    [SerializeField] private AudioManager _audioManager;

    public int fishingCount = 1; // 최대 낚시 가능한 횟수
    private int _currentCount; // 현재 낚시 가능한 횟수
    public FishingUI uiManager; // 낚시 횟수를 갱신할 Ui 참조

    public List<FishData> fishDatabase = new List<FishData>(); // 게임에 존재하는 모든 물고기 데이터 리스트
    public FishRateData fishCurrentRate; // 현재 레벨에 적용된 등급별 확률 데이터

    private Animator _animator;
    private Vector2 _pressPos;
    private FishData _lastCaughtFish;
    private GameObject _currentFish;
    private bool _isLoaded = false;

    [Header("물고기 연출")]
    public GameObject fishPrefab;
    public Transform popFishPoint;
    public List<Sprite> fishSprites;

    private void OnEnable()
    {
        if (_animator != null && _timer != null)
        {
            bool isFullNow = _timer.CheckingFull();
            _animator.SetBool("IsFull", isFullNow);

            if (isFullNow)
            {
                _animator.Play("Wait", 0, 0f);
            }
            else
            {
                _animator.Play("idle", 0, 0f);
            }
        }

        StopAllCoroutines();
        StartCoroutine(IdleVariationRoutine());
    }
    private void Awake()
    {
        if (_audioManager != null)
        {
            _audioManager.PlayBgmFishhook();
        }

        _animator = GetComponent<Animator>();

        // FishingUpgradeManager의 이벤트에 메서드들을 연결
        if (_upgradeManager != null)
        {
            _upgradeManager.OnRodUpgrade += RodgradeMaxCount;
            _upgradeManager.OnBaitUpgrade += BaitgradeMaxCount;
            _upgradeManager.OnFishingUpgrade += FishRateLevel;
            _upgradeManager.OnShipUpgrade += ShipUpgradeLevel;

            // 현재 저장된 레벨 데이터로 초기 설정
            RodgradeMaxCount(DataTower.instance.fihserDatas.rodLevel);
            BaitgradeMaxCount(DataTower.instance.fihserDatas.baitLevel);
            FishRateLevel(DataTower.instance.fihserDatas.fishingGrade);
            ShipUpgradeLevel(DataTower.instance.fihserDatas.fishingGrade);
        }

        _currentCount = fishingCount;

        if (uiManager != null)
        {
            uiManager.UpdateCountText(_currentCount, fishingCount);
        }

        _isLoaded = true;
}

    /*private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) RodgradeMaxCount(1);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) RodgradeMaxCount(2);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) RodgradeMaxCount(3);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) RodgradeMaxCount(4);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) RodgradeMaxCount(5);

        if (Keyboard.current.qKey.wasPressedThisFrame) FishRateLevel(1);
        if (Keyboard.current.wKey.wasPressedThisFrame) FishRateLevel(2);
        if (Keyboard.current.eKey.wasPressedThisFrame) FishRateLevel(3);
        if (Keyboard.current.rKey.wasPressedThisFrame) FishRateLevel(4);
        if (Keyboard.current.tKey.wasPressedThisFrame) FishRateLevel(5);
        if (Keyboard.current.yKey.wasPressedThisFrame) FishRateLevel(6);
        if (Keyboard.current.uKey.wasPressedThisFrame) FishRateLevel(7);
        if (Keyboard.current.iKey.wasPressedThisFrame) FishRateLevel(8);
        if (Keyboard.current.oKey.wasPressedThisFrame) FishRateLevel(9);
        if (Keyboard.current.pKey.wasPressedThisFrame) FishRateLevel(10);
    }*/

    /// <summary>
    ///  낚시대 업그레이드 시 호출되는 인터페이스 메서드
    /// </summary>
    public void UpgradeFishingRod()
    {
        _upgradeManager.RodUpgrade();
    }

    /// <summary>
    /// 낚시대 강화 레벨에 따라 낚시 가능한 최대 횟수 값을 UI에 갱신
    /// </summary>
    public void RodgradeMaxCount(int newLevel)
    {
        switch (newLevel)
        {
            case 1: fishingCount = 1; break;
            case 2: fishingCount = 2; break;
            case 3: fishingCount = 3; break;
            case 4: fishingCount = 4; break;
            case 5: fishingCount = 5; break;
        }

        Debug.Log($"낚싯대 레벨: {fishingCount}");

        if (uiManager != null)
        {
            uiManager.UpdateCountText(_currentCount, fishingCount);
        }
    }

    /// <summary>
    /// 미끼 업그레이드 시 호출되는 인터페이스 메서드
    /// </summary>
    public void UpgradeFishingBait()
    {
        _upgradeManager.BaitUpgrade();
    }

    /// <summary>
    /// 미끼 레벨에 따라 타이머의 최대 쿨타임을 설정
    /// </summary>
    public void BaitgradeMaxCount(int newLevel)
    {
        float newTimer = 3600f;

        switch (newLevel)
        {
            case 1: newTimer = 3600f; break;
            case 2: newTimer = 3300f; break;
            case 3: newTimer = 3000f; break;
            case 4: newTimer = 2400f; break;
            case 5: newTimer = 1800f; break;
            default:
                if (newLevel >= 5)
                    newTimer = 1800f; break;
        }

        Debug.Log($"현재 미끼 레벨: {newLevel}");

        if (_timer != null)
        {
            _timer.UpdateMaxTime(newTimer);
        }
    }

    /// <summary>
    /// 클릭 시작 위치 저장
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        _pressPos = eventData.position;
    }

    /// <summary>
    /// 낚시 화면 클릭 시 낚시 연출 실행
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        float distance = (eventData.position - _pressPos).sqrMagnitude;

        if (distance > 100f)
        {
            Debug.Log("드래그중! 낚시 X.");
            return;
        }

        else
        {
            GoFishing();
        }
    }

    /// <summary>
    /// 현재 애니메이션의 상태에 따라 낚시 연출 실행
    /// </summary>
    private void GoFishing()
    {
        if (!_isLoaded || fishCurrentRate == null)
        {
            Debug.Log("데이터 로딩 중");
            return;
        }

        string currentRarity = GetFishRarity();
        if (currentRarity == "None") return;

        AnimatorStateInfo aniState = _animator.GetCurrentAnimatorStateInfo(0);

        if (aniState.IsName("result"))
        {
            if (_currentCount > 0)
            {
                _currentCount--;
                UpdateUI();
                GetRandomFish();

                if (_audioManager != null)
                {
                    _audioManager.PlaySfxWater();
                }

                _animator.SetTrigger("resultCatch");
            }
            else
            {
                Debug.Log("낚시 횟수 0회");
            }
            return;
        }

        if (aniState.IsName("fishing"))
        {
            if (_currentCount > 0)
            {
                _currentCount--;
                UpdateUI();
                GetRandomFish();
            }

            if (_audioManager != null)
            {
                _audioManager.StopSfxFishing();
                _audioManager.PlaySfxWater();
            }

            _animator.SetTrigger("resultCatch");

            return;
        }

        if (_currentCount > 0)
        {
            {
                _animator.SetTrigger("Fishing");
                _currentCount--;
                UpdateUI();
                GetRandomFish();

                if (_audioManager != null)
                {
                    _audioManager.PlaySfxFishing();
                }
            }
        }
    }

    /// <summary>
    /// 현재 카운트 데이터를 UI 스크립트에 전달하여 화면 갱신
    /// </summary>
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateCountText(_currentCount, fishingCount);
        }
    }

    /// <summary>
    ///  쿨타임이 지나면 낚시 횟수를 1회 충전
    /// </summary>
    public void FishingChance()
    {
        if (_currentCount < fishingCount)
        {
            _currentCount++;

            if (uiManager != null)
            {
                uiManager.UpdateCountText(_currentCount, fishingCount);
            }
        }
    }

    /// <summary>
    /// 현재 남은 낚시 횟수를 반환
    /// </summary>
    public int GetCurrentCount()
    {
        return _currentCount;
    }

    /// <summary>
    /// 확률에 따라 무작위 물고기를 결정하고 데이터타워에 전달
    /// </summary>
    public void GetRandomFish()
    {
        if (fishDatabase.Count == 0)
        {
            Debug.Log("등록된 물고기가 없습니다.");
            return;
        }

        string currentRarity = GetFishRarity();
        FishData selectedFish = GetRandomRarityFish(currentRarity);

        if (selectedFish != null)
        {
            Debug.Log($"{currentRarity}, {selectedFish.korName}");

            _lastCaughtFish = selectedFish;

            if (DataTower.instance != null)
            {
                DataTower.instance.takeFish(selectedFish);
            }

            else
            {
                Debug.Log("DataTower 인스턴스를 찾을 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// 화면에 물고기 팝업 연출
    /// </summary>
    public void OnCatchAnimationEvent()
    {
        if (_lastCaughtFish != null)
        {
            PopFishResult(_lastCaughtFish);
            _lastCaughtFish = null;
        }
    }

    /// <summary>
    /// 시트에 설정된 확률에 따라 무작위 등급을 결정
    /// </summary>
    private string GetFishRarity()
    {
        float rarityRate = UnityEngine.Random.Range(0f, 100f);
        float baseValue = 0;

        if (fishCurrentRate == null)
        {
            Debug.Log("데이터가 연결되지 않습니다.");
            return "None";
        }

        if (rarityRate <= (baseValue += fishCurrentRate.trash))
        { return "Trash"; }
        if (rarityRate <= (baseValue += fishCurrentRate.normal))
        { return "Normal"; }
        if (rarityRate <= (baseValue += fishCurrentRate.fine))
        { return "Fine"; }
        if (rarityRate <= (baseValue += fishCurrentRate.superior))
        { return "Superior"; }
        if (rarityRate <= (baseValue += fishCurrentRate.rare))
        { return "Rare"; }
        if (rarityRate <= (baseValue += fishCurrentRate.elite))
        { return "Elite"; }
        if (rarityRate <= (baseValue += fishCurrentRate.fantastic))
        { return "Fantastic"; }
        if (rarityRate <= (baseValue += fishCurrentRate.legendary))
        { return "Legendary"; }
        return "None";
    }

    /// <summary>
    /// 결정된 등급에 따라 해당 등급에 속하는 물고기를 리스트 안에서 한 마리를 무작위로 선택
    /// </summary>
    public FishData GetRandomRarityFish(string rarity)
    {
        List<FishData> filteredFish = new List<FishData>();

        foreach (FishData randomFish in fishDatabase)
        {
            if (randomFish.fishRarity.ToString() == rarity)
            {
                filteredFish.Add(randomFish);
            }
        }

        if (filteredFish.Count == 0)
        {
            Debug.Log($"물고기가 없습니다");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, filteredFish.Count);
        return filteredFish[randomIndex];
    }

    /// <summary>
    /// 플레이어 레벨에 맞는 확률 데이터 시트를 적용
    /// </summary>
    public void FishRateLevel(int level)
    {
        int index = level - 1;

        if (index >= 0 && index < fishRateList.Count)
        {
            fishCurrentRate = fishRateList[index];
            Debug.Log($"현재 플레이어 레벨: {level}");
        }
    }

    /// <summary>
    /// 배 업그레이드 실행 요청
    /// </summary>
    public void UpgradeShipLevel()
    {
        _upgradeManager.ShipUpgrade();
    }

    /// <summary>
    /// 배 업그레이드 레벨 적용
    /// </summary>
    public void ShipUpgradeLevel(int newLevel)
    {
        Debug.Log($"현재 배 레벨: {newLevel}");
    }

    /// <summary>
    /// 낚시 대기 상태일 때 무작위 연출 애니메이션을 재생하는 코루틴
    /// </summary>
    IEnumerator IdleVariationRoutine()
    {
        yield return null;

        while (true)
        {
            bool isFullNow = _timer.CheckingFull();
            _animator.SetBool("IsFull", isFullNow);

            if (isFullNow)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            float waitTime = UnityEngine.Random.Range(10f, 15f);
            float currentWaitTime = 0f;

            while (currentWaitTime < waitTime)
            {
                currentWaitTime += Time.deltaTime;

                if (_timer.CheckingFull()) break;

                yield return null;
            }

            if (_timer.CheckingFull()) continue;

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                continue;

            float randomVar = UnityEngine.Random.Range(0f, 100f);
            int targetIdx = 0;

            if (randomVar <= 40f)
            {
                targetIdx = 1;
            }

            else if (randomVar <= 60f)
            {
                targetIdx = 2;
            }

            else targetIdx = 3;

            _animator.SetInteger("IdleIdx", targetIdx);

            yield return new WaitForSeconds(0.1f);
            _animator.SetInteger("IdleIdx", 0);

        }
    }

    /// <summary>
    /// 낚인 물고기를 프리팹으로 생성하고 스프라이트를 설정하여 화면에 연출
    /// </summary>
    private void PopFishResult(FishData data)
    {
        if (this == null || data == null || fishPrefab == null || popFishPoint == null)
        {
            return;
        }

        if (_currentFish != null)
        {
            Destroy(_currentFish);
        }

        GameObject go = Instantiate(fishPrefab, popFishPoint.position, Quaternion.identity, popFishPoint);
        _currentFish = go;

        SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {

            Sprite targetSprite = fishSprites.Find(x => x.name == data.fishSprite);
            if (targetSprite != null)
            {
                sr.sprite = targetSprite;
            }
            else
            {
                Debug.LogWarning($"{data.fishSprite} 없음");
            }

            sr.sortingOrder = 5;
        }
        Destroy(go, 3f);
    }
}
