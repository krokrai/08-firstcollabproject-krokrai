using System.IO;
using UnityEngine;

public class SaveNLoadeManager : MonoBehaviour
{
    public FishData testFish;

    public void Save()
    {
        // SO데이터 -> JSON문자열로 변환
        string json = JsonUtility.ToJson(testFish, true);

        // 저장 경로 설정(폴더)
        string folderPath = Application.dataPath;

        // 폴더가 없다면 생성(안전장치)
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // json테스트 파일 최종 저장 경로
        string filePath = Path.Combine(folderPath, "test.json");

        // 파일 쓰기
        File.WriteAllText(filePath, json);
        Debug.Log($"JSON으로 저장 완료, 경로: {filePath}\n내용: {json}");
    }

    [ContextMenu("Load from JSON")]
    public void Load()
    {
        string folderPath = Path.Combine(Application.dataPath, "Resources", "HBTest");
        string filePath = Path.Combine(folderPath, "test.json");

        if (File.Exists(filePath))
        {
            // 파일 읽기
            string json = File.ReadAllText(filePath);

            // 읽은 내용 -> 기존 SO에 덮어쓰기
            JsonUtility.FromJsonOverwrite(json, testFish);
            Debug.Log("JSON 불러오기 완료");
        }
        else
        {
            Debug.LogError("저장된 JSON파일이 없음");
        }
    }
}
