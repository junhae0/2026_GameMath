using UnityEngine;
using TMPro;

public class sixProject : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI statusDisplay;   // 좌측 상단: 정보
    public TextMeshProUGUI logDisplay;      // 좌측 중단: 로그 및 요약 (통합 사용)
    public TextMeshProUGUI resultDisplay;   // 우측 상단: 누적 데이터
    public TextMeshProUGUI rangeDisplay;    // 우측 하단: 예상 범위

    [Header("데이터")]
    private int level = 1;
    private float baseDamage = 20f;
    private string weaponName;
    private float stdDevMult, critRate, critMult;

    // 통계용 변수
    private int attackCount = 0;
    private float totalDamage = 0;
    private int weakPointHits = 0, missHits = 0, critHits = 0;
    private float maxDamageRecorded = 0f;

    void Start() => SetWeapon(0);

    private void ResetData()
    {
        totalDamage = 0; attackCount = 0;
        weakPointHits = 0; missHits = 0; critHits = 0;
        maxDamageRecorded = 0f;
    }

    public void SetWeapon(int id)
    {
        ResetData();
        if (id == 0) SetStats("단검", 0.1f, 0.4f, 1.5f);
        else if (id == 1) SetStats("장검", 0.2f, 0.3f, 2.0f);
        else SetStats("도끼", 0.3f, 0.2f, 3.0f);

        logDisplay.text = $"{weaponName} 장착!";
        UpdateUI();
    }

    private void SetStats(string _name, float _stdDev, float _critRate, float _critMult)
    {
        weaponName = _name; stdDevMult = _stdDev; critRate = _critRate; critMult = _critMult;
    }

    public void LevelUp()
    {
        ResetData();
        level++;
        baseDamage = level * 20f;
        logDisplay.text = $"레벨업! 현재 레벨: {level}";
        UpdateUI();
    }

    // [과제] 공격 x 1000 버튼 연결
    public void OnSimulate1000()
    {
        ResetData();
        for (int i = 0; i < 1000; i++)
        {
            ProcessAttack(true); // 시뮬레이션 모드 실행
        }

        // 1000회 종료 후 logDisplay에 요약 결과 표시 (이미지 형식 적용)
        logDisplay.text = $"약점 공격 {weakPointHits}회, 명중 실패 {missHits}회, 전체 크리티컬 {critHits}회,\n최대 데미지 {maxDamageRecorded:F4}";
        UpdateUI();
    }

    public void OnAttack() => ProcessAttack(false);

    private void ProcessAttack(bool isSimulating)
    {
        float sd = baseDamage * stdDevMult;
        float normalDamage = GetNormalStdDevDamage(baseDamage, sd);

        bool isWeak = false; bool isMiss = false;

        // 2σ 판정 로직
        if (normalDamage > baseDamage + (2 * sd)) { isWeak = true; weakPointHits++; normalDamage *= 2f; }
        else if (normalDamage < baseDamage - (2 * sd)) { isMiss = true; missHits++; normalDamage = 0f; }

        // 치명타 판정
        bool isCrit = Random.value < critRate;
        if (isCrit) { critHits++; normalDamage *= critMult; }

        float finalDamage = normalDamage;
        attackCount++;
        totalDamage += finalDamage;
        if (finalDamage > maxDamageRecorded) maxDamageRecorded = finalDamage;

        // 일반 공격(1회)일 때만 실시간 로그 표시
        if (!isSimulating)
        {
            string tags = (isMiss ? "<color=gray>[실패]</color> " : "") +
                          (isWeak ? "<color=yellow>[약점]</color> " : "") +
                          (isCrit ? "<color=red>[치명타!]</color> " : "");
            logDisplay.text = $"{tags}데미지: {finalDamage:F1}";
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        statusDisplay.text = $"Level: {level} / 무기: {weaponName}\n기본 데미지: {baseDamage} / 치명타: {critRate * 100}% (x{critMult})";

        float dpa = attackCount > 0 ? totalDamage / attackCount : 0;
        resultDisplay.text = $"누적 데미지: {totalDamage:F1}\n공격 횟수: {attackCount}\n평균 DPA: {dpa:F2}";

        float sd = baseDamage * stdDevMult;
        rangeDisplay.text = $"예상 일반 데미지 범위 : [{baseDamage - 3 * sd:F1} ~ {baseDamage + 3 * sd:F1}]";
    }

    private float GetNormalStdDevDamage(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.value; float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }
}