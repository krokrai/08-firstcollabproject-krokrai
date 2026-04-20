using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Container", menuName = "DataReader/Container")]
public class DataContainer : ScriptableObject
{
    public URLReader reader;
    [Tooltip("데이터 테이블의 데이터 시작 위치 지정")][SerializeField] byte _mainLine = 4;

    [Tooltip("해당 시트 주소를 전체를 복사해서 넣어주세요.")]
    public string URL;

    [Tooltip("해당 시트의 실제 값 형태로 저장된 행의 갯수만큼 넣어주세요.")]
    public ScriptableObject[] objs;

    /// <summary>
    /// 현재 SO가 정보 저장 및 읽기 가능 상태인지 확인용
    /// false = 읽기 불가
    /// true = 읽기 가능
    /// </summary>
    public bool isDataLoaded { get; private set; } = false;

    [ContextMenu("SetOrigin")]
    void SetTest()
    {
        isDataLoaded = false;
    }

    private void Awake()
    {
        isDataLoaded = false;
    }

    public void SetDatas(char splitSymbol, string[] lines)
    {
        int mainLine = _mainLine - 1;

        if (lines == null)
        {
            Debug.LogError("Data 입력 없음.");
            return;
        }

        // 줄 단위로 들어온 데이터 가공
        for (int i = mainLine; i < lines.Length; i++)
        {
            if (i > objs.Length + mainLine)
            {
                Debug.LogWarning($"<color=yellow>경고, {this.name}에 저장된 SO가 입력된 값보다 적습니다. SO를 추가하시거나 데이터 테이블을 점검해주세요.</color>");
                return;
            }
            string[] cols = lines[i].Split(splitSymbol);

            if ( i - mainLine >= objs.Length )
            {
                Debug.LogError($"입력된 SO의 갯수가 읽어 드린 시트의 행수보다 적습니다. 위치 : {this.name} 혹은 {objs[0].name}");
                return;
            }

            if (objs[i - mainLine] is IDataSeter )
            {
                (objs[i - mainLine] as IDataSeter).SetData(cols);
            }
            else
            {
                Debug.LogWarning($"{objs[i - mainLine].name}에 <color=red>IDataSeter</color>가 포함되어 있지 않습니다.");
            }
        }

        isDataLoaded = true;
    }
}
