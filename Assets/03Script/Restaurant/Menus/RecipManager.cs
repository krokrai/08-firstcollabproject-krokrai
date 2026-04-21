/*
    <레시피 관리자>
물고기에 대하 고유 번호 필요 ( 물고기를 소모한 경우 전달 등을 해야되기 때문)
레시피에서 사용하는 물고기 번호를 여기에 넘겨주면 DataTower에서 검색
레시피에 대한 참조 필요 (물고기가 0이 되거나 1이 됬을때 상태를 전달해야되기 때문에.)

Action으로 만들 수 있는 지 없는지만, 넘겨주기.

overload를 통해 최대 4개까지 받고 넘겨주기.

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipManager : MonoBehaviour
{
    
    [SerializeField] GameObject _scrollViewContent;
    [SerializeField] DataContainer _dataCon;
    [SerializeField] GameObject tempRecipeObj;
    [SerializeField] RecipeInfoUI _riu;

    RecipeContainer _rcps;
    GameObject obj;

    Dictionary<string,GameObject> _recipes; // 매니저가 SO에 대한 접근 및 지정 필요
    // 1개인 경우에는 딕셔너리 사용해도 되는데, 4개인 경우에는 어떻게 해금 할 것인지???

    // 기존 데이터는 약 30개의 레시피가 있으면 모두 호출 될 수 있기 때문에 삭제 처리

    private void Awake()
    {
        StartCoroutine(AddAction());
        StartCoroutine(DataRead());
    }

    IEnumerator AddAction()
    {
        yield return new WaitForSeconds(0.2f);
        DataTower.instance.OnFisingNewFish += FirstFishingFish;
        yield break;
    }

    private void OnDestroy()
    {
        DataTower.instance.OnFisingNewFish -= FirstFishingFish;
    }

    IEnumerator DataRead()
    {
        while ( !_dataCon.isDataLoaded )
        {
            yield return new WaitForSeconds(0.05f);
        }

        Debug.Log("Read to Write Recipe");

        if ( _dataCon.objs[0] is not RecipeContainer )
        {
            Debug.LogError($"{gameObject.name}에 저장된 DataContainer가 RecipeContainer가 들어있지 않는 컨테이너입니다.");
            yield break;
        }

        _recipes = new Dictionary<string, GameObject>(_dataCon.objs.Length);
        for (int i = 0; i < _dataCon.objs.Length; i++)
        {
            _rcps = _dataCon.objs[i] as RecipeContainer;
            obj = Instantiate(tempRecipeObj, transform.position, Quaternion.identity);
            obj.transform.SetParent(_scrollViewContent.transform, false);
            obj.transform.SetSiblingIndex(_dataCon.objs.Length * 3 + i);
            obj.GetComponent<RecipModel>().InitRecip(_rcps, _riu);
            obj.name = _rcps.name;

            _recipes.Add(_rcps.ingredient, obj);
        }
        yield break;
    }

    int num;

    public void SetSiblingIndex(RecipeContainer rcp, int multi)
    {
        num = 0;
        int.TryParse(rcp.recipe_ID.Split(' ')[1],out num);
        Debug.Log(num);
        num--;

        _recipes[rcp.ingredient].transform.SetSiblingIndex(_dataCon.objs.Length * multi + num);
    }

    public void ReCall(RecipeContainer rcp)
    {
        _recipes[rcp.ingredient].GetComponent<RecipModel>().PassToRecipeInfo();
    }

    /// <summary>
    /// 물고기 이름 넣으면 해당 레시피 잠금 해제
    /// </summary>
    /// <param name="s"></param>
    public void SetRecipeUnlock(string s)
    {
        _recipes[s].GetComponent<RecipModel>().UnlockThisRecipe();
    }

    /// <summary>
    /// 낚시에 성공한 물고기가 처음 낚인 물고기인 경우. n(임시)에 값을 기준으로 해금
    /// </summary>
    /// <param name="n"></param>
    public void FirstFishingFish(string fishID)
    {
        Debug.Log($"현재 물고기 레시피 : {fishID}");
        _recipes[fishID].GetComponent<RecipModel>().CanUnlockConditionsMet();
    }
}
