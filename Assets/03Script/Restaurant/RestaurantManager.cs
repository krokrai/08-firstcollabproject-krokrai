using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RestaurantManager : MonoBehaviour
{
    // 후에 아래 리소스는 정리 및 DataTower로 인계
    [Header("Resources")]
    // 후에 메뉴판이랑 연동해서 관리할 것이기 때문에 메뉴판 완성 시 함수 안에 포함된 모든 _sushiCount 수정 해야함 @@@@@@@@@@@@@@@@@@@@@@@
    [SerializeField] private MenuCtrl _menuCtrl;
    [SerializeField] private DataContainer _container;
    [SerializeField] private CustomerDataSO[] _customerData;
    [SerializeField] private Customer_Tips[] _customerTips;
    [SerializeField] private Restaurant_Fixed_value _fixedValue;
    [SerializeField] private DiningUpgradeDataReader _diningUpgradeDataReader;
    [SerializeField] private GaugeSetter[] _gaugeSetter;
    [SerializeField] SpriteLibraryAsset[] _customerAsset;
    [SerializeField] AudioManager _audioManager;

    [Header("Spawn")]
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private GameObject _customers;
    [SerializeField] private Transform _spawnPointRight;
    [SerializeField] private Transform _exitPointLeft;

    [Header("Seats")]
    [SerializeField] private List<RestaurantSeat> _seats = new List<RestaurantSeat>();

    [Header("SceneChange")]
    [SerializeField] private OpenMenu _openMenu;
    private Queue<GameObject> _customerPool; // 오브젝트 풀 패턴 기반 작동 예정
    private GameObject _randomPrefab;

    private GaugeSetter _currentGauge;
    private SpriteLibraryAsset _currentSpriteLibrary;

    private RestaurantSeat _emptySeat;
    private bool _haveDish;
    private bool _canVisual;

    private byte _normalCustomers;
    private byte _specialCustomers;

    private int _maxNormalCustomersWeight;
    private int _maxSpecialCustomersWeight;
    private int _maxVIPCustomersWeight;

    private int _currentmaxSpecialCustomerWeight;

    private int _halfNormalCustomersWeight;
    private int _halfSpecialCustomersWeight;

    private byte _currentSpawnedSpecialCustomer;
    private byte _currentSpawnedVIPCustomer;

    int _currentWeightLevel;
    int _temp_Numbers;

    private void Awake()
    {
        for (byte i = 0; i < _gaugeSetter.Length; i++)
        {
            _gaugeSetter[i].GaugeOff();
        }
    }

    private void OnDestroy()
    {
        _menuCtrl.OnDish += HaveDish;
        _openMenu.OnChangeSceneToRestaurant -= OnVisual;
    }
    public void OnVisual(bool b)
    {
        _canVisual = b;
        Debug.Log($"시각 효과 상태 : {_canVisual}");
    }

    public void HaveDish(bool b)
    {
        _haveDish = b;
    }

    public bool hasDish() => _haveDish;

    private void Start()
    {
        _menuCtrl.OnDish += HaveDish;
        _openMenu.OnChangeSceneToRestaurant += OnVisual;

        _haveDish = false;
        byte i = 0;
        _halfNormalCustomersWeight = 0;
        _halfSpecialCustomersWeight = 0;

        _normalCustomers = 0;
        _specialCustomers = 0;
        

        _currentmaxSpecialCustomerWeight = 0;

        _maxNormalCustomersWeight = 0;
        _maxSpecialCustomersWeight = 0;
        _maxVIPCustomersWeight = 0;

        _currentSpawnedSpecialCustomer = 0;
        _currentSpawnedVIPCustomer = 0;

        _customerPool = new Queue<GameObject>(10);

        for (i = 0; i < 10 ; i++)
        {
            GameObject obj = Instantiate(_customerPrefab);
            obj.GetComponent<CustomerController>().ConnectRestaurant(this, _openMenu, _audioManager);
            obj.transform.SetParent(_customers.transform);
            obj.name = $"customer {i}";
            obj.SetActive(false);
            _customerPool.Enqueue( obj );
        }

        StartCoroutine(CoTrySpawnCustomer());
    }

    IEnumerator Waiter()
    {
        while (!_container.isDataLoaded)
        {
            yield return null;
        }

        Debug.Log($"{_customerData[0].name} / {_customerData[0].weight}");

        for (byte i = 0; i < _customerData.Length; i++)
        {
            switch (_customerData[i].grade)
            {
                case CustomerGrade.NA:
                    Debug.LogWarning("값 오류");
                    break;
                case CustomerGrade.NORMAL:
                    _normalCustomers++;
                    _maxNormalCustomersWeight += _customerData[i].weight;
                    break;
                case CustomerGrade.SPECIAL:
                    _specialCustomers++;
                    _maxSpecialCustomersWeight += _customerData[i].weight;
                    break;
                case CustomerGrade.VIP:
                    _maxVIPCustomersWeight += _customerData[i].weight;
                    break;
            }
        }

        Debug.Log($"손님 수 : {_normalCustomers} / {_specialCustomers}");

        for (byte i = 0; i < _normalCustomers / 2; i++)
            _halfNormalCustomersWeight += _customerData[i].weight;
        for (byte i = 0; i < _specialCustomers / 2; i++)
            _halfSpecialCustomersWeight += _customerData[i + _normalCustomers].weight;
    }

    private IEnumerator CoTrySpawnCustomer()
    {
        yield return StartCoroutine(Waiter());

        // 설거지? 시간 => 대기시간 => 스폰
        while (true)
        {
            if (!_haveDish)
            {
                yield return null;
                continue;
            }

            // 빈자리 탐색 및 반환 받음.
            _emptySeat = GetEmptySeat();
            // 자리가 없는 경우 예외처리
            if (_emptySeat != null)
            {
                // 스폰 대기 시간.
                //yield return new WaitForSeconds(UnityEngine.Random.Range(_fixedValue.minSpawnDelay, _fixedValue.maxSpawnDelay + 1));
                yield return new WaitForSeconds(5);
                // 손님 성향 및 이미지 랜덤 생성
                GetRandomCustomerPrefab();

                // 예외처리.
                if (_randomPrefab == null)
                    yield return null;
                // 후에 추가 딜레이 필요

                // 최종적으로 빈자리와 손님을 소환
                SpawnCustomer(_emptySeat);
            }
            else
            {
                yield return null;
            }
        }
    }

    void GetRandomCustomerSprite()
    {
        _currentSpriteLibrary = _customerAsset[UnityEngine.Random.Range(0, _customerAsset.Length)];
    }

    private void SpawnCustomer(RestaurantSeat seat)
    {
        // 자리에 앉음 상태로 전환
        //_currentWeightLevel = DataTower.instance.WeightLevel;
        //_temp_Numbers = Random.Range(0, 20);//CustomerWeightSelecter(UnityEngine.Random.Range(0, 600)); //UnityEngine.Random.Range(0,
            /*(_maxNormalCustomersWeight
            + CanSpawnVIPCustomer()
            + CanSpawnSpecialCustomer() ) ));*/
        _temp_Numbers = CustomerWeightSelecter(UnityEngine.Random.Range(0, 
        (_maxNormalCustomersWeight
        + CanSpawnVIPCustomer()
        + CanSpawnSpecialCustomer() ) ));

        Debug.Log(_temp_Numbers);

        seat.SetOccupied();

        GetRandomCustomerSprite();

        _randomPrefab.SetActive(true);
        _randomPrefab.transform.position = _spawnPointRight.position;
        _randomPrefab.GetComponent<CustomerController>().SetInfo(seat,
            _exitPointLeft,
            _customerTips[(int)_customerData[_temp_Numbers].grade],
            _customerData[_temp_Numbers],
            _currentGauge,
            _currentSpriteLibrary,
            _canVisual);
    }

    private int CanSpawnSpecialCustomer()
    {
        if (_currentSpawnedSpecialCustomer < DataTower.instance.upgradeDatas.MaxSpawnLimit01Level - 1)
        {
            return _maxSpecialCustomersWeight + _specialCustomers * _diningUpgradeDataReader.Weight[_currentWeightLevel].Effect_Value_1;
        }
        else
            return 0;
    }

    private int CanSpawnVIPCustomer()
    {
        if (_currentSpawnedVIPCustomer < DataTower.instance.upgradeDatas.MaxSpawnLimit02Level - 1)
        {
            return _maxVIPCustomersWeight;
        }
        else
            return 0;
    }

    private int CustomerWeightSelecter(int weight)
    {
        // weight 업글시 스페셜 손님 가중치
        _currentmaxSpecialCustomerWeight = _maxSpecialCustomersWeight 
            + (_diningUpgradeDataReader.Weight[_currentWeightLevel].Effect_Value_1
            * _specialCustomers);

        Debug.Log($"손님 가중치 : {_maxNormalCustomersWeight} / {_currentmaxSpecialCustomerWeight} / {_maxVIPCustomersWeight}\n현재 스페셜 손님 가중치 : {CanSpawnSpecialCustomer()} / 현재 VIP 손님 가중치 : {CanSpawnVIPCustomer()}");

        if (weight - _maxNormalCustomersWeight < 0)
        {
            Debug.Log($"Normal Spawn, {weight}");
            if (weight - _halfNormalCustomersWeight < 0)
            {
                return CustomerWeightFinder(weight, 0);
            }
            else
            {
                weight -= _halfNormalCustomersWeight;
                return CustomerWeightFinder(weight, (byte)(_normalCustomers / 2));
            }
        }
        else
        {
            // 그 외 스페셜 및 vip
            weight -= _maxNormalCustomersWeight;
            if (weight - _maxSpecialCustomersWeight < 0 && DataTower.instance.upgradeDatas.MaxSpawnLimit01Level > 1)
            {
                // 스페셜만
                Debug.Log($"Special Spawn, {weight}");
                _currentSpawnedSpecialCustomer++;
                if ( weight - _halfSpecialCustomersWeight < 0)
                {
                    return CustomerWeightFinder(weight, _normalCustomers);
                }
                else
                {
                    weight -= _halfSpecialCustomersWeight;
                    return CustomerWeightFinder(weight, (byte)((_normalCustomers + _specialCustomers) / 2));
                }
            }
            else 
            {
                weight -= _maxSpecialCustomersWeight; // VIP 제대로 작동 안됌 나중에 고치기.
                _currentSpawnedVIPCustomer++;
                Debug.Log($"VIP Spawn, {weight}");
                return CustomerWeightFinder(weight, (byte)(_normalCustomers + _specialCustomers));
            }
        }
    }

    private int CustomerWeightFinder(int weight, byte startNumber)
    {
        for (byte i = startNumber; i < _customerData.Length; i++)
        {
            //Debug.Log($"CustomerWeightFinder 반복 횟수 : {i} / {_customerData.Length}\n시작 숫자 : {startNumber}");
            if (weight - _customerData[i].weight <= 0)
            {
                return i;
            }
            else
            {
                weight -= _customerData[i].weight;
            }
        }
        return -1;
    }

    /// <summary>
    /// CustomerController 기반 오브젝트를 object pool로 반환.
    /// </summary>
    /// <param name="obj"></param>
    public void DeSpawnCustomer(GameObject obj, CustomerGrade grade)
    {
        // 비용은 비싸지만 프레임마다 호출이 아니니 괜찮을 듯한 생각.
        if ( !(obj != null && obj.GetComponent<CustomerController>() is CustomerController))
        {
            Debug.Log("잘 못된 반환 형식");
            return;
        }

        switch(grade)
        {
            case CustomerGrade.SPECIAL:
                _currentSpawnedSpecialCustomer--;
                Debug.Log($"현재 스페셜 손님 수 : {_currentSpawnedSpecialCustomer}");
                break;
            case CustomerGrade.VIP:
                _currentSpawnedVIPCustomer--;
                Debug.Log($"현재 스페셜 손님 수 : {_currentSpawnedVIPCustomer}");
                break;
        }
            
        obj.SetActive(false);
        _customerPool.Enqueue(obj);
    }

    private RestaurantSeat GetEmptySeat()
    {
        for (int i = 0; i < DataTower.instance.upgradeDatas.MaxCustomerLimitLevel; i++)
        {
            // 반복 문으로 모든 자리 탐색
            if (_seats[i].IsOccupied == false)
            {
                _currentGauge = _gaugeSetter[i];
                return _seats[i];
            }
        }
        return null;
    }

    private void GetRandomCustomerPrefab()
    {
        if (_customerPrefab == null)
        {
            Debug.LogWarning("프리팹이 등록되지 않았거나, 없습니다.");
            return;
        }

        if (_customerPool.Count == 0)
        {
            _customerPool.Enqueue(_customerPrefab);
            _randomPrefab = _customerPool.Dequeue();
        }
        else
            _randomPrefab = _customerPool.Dequeue();
    }

    /// <summary>
    /// 초밥 갯수를 체크하고 초밥이 있으면 갯수를 하나 줄이고 초밥 가격을 자산에 추가한다.
    /// </summary>
    /// <param name="price"></param>
    /// <returns></returns>
    public void TryCounsumeSushiAndEarnMoney(float multi)
    {// 기본 가격 = RandomEating // 현재 비용 높으니 후에 Action으로 전환 조치 필요.
        int eating = _menuCtrl.RandomEating();
        if (eating == 0)
            return;
        DataTower.instance.TryMoenyChanged(
            (ulong)(
            (eating
                * (1 
                    + _diningUpgradeDataReader.Bonus_Dish_Price_1[DataTower.instance.upgradeDatas.BonusDishPrice01Level].Effect_Value_1 // 호출 횟수가 많지 않아서 임시 작업, 성능 문제 발생 시 수정 필요.
                    + _diningUpgradeDataReader.Bonus_Dish_Price_2[DataTower.instance.upgradeDatas.BonusDishPrice02Level].Effect_Value_2))
                * multi
                * DataTower.instance.upgradeDatas.BonusTipsMultiLevel
            ),
            false);
    }
}
