using UnityEngine;
using TMPro;

public class sixProject : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI statusDisplay;
    public TextMeshProUGUI logDisplay;
    public TextMeshProUGUI resultDisplay;
    public TextMeshProUGUI rangeDisplay;

    [Header("데이터")]
    private int level = 1;
    private float totalDamage = 0;
    private float baseDamage = 20f;
    private int attackCount = 0;

    private string weaponName;
    private float stdDevMult, critRate, critMult;

    // 통계용 변수
    private int weakPointHits = 0, missHits = 0, critHits = 0;
    private float maxDamageRecorded = 0f;

    void Start() => SetWeapon(0);

    // 데이터를 완전히 초기화하는 메서드 (레벨 포함)
    private void ResetDataFull()
    {
        level = 1;
        baseDamage = 20f;
        totalDamage = 0;
        attackCount = 0;
        weakPointHits = 0;
        missHits = 0;
        critHits = 0;
        maxDamageRecorded = 0f;
    }

    // 통계 데이터만 초기화하는 메서드 (레벨업 시 사용)
    private void ResetStatsOnly()
    {
        totalDamage = 0;
        attackCount = 0;
        weakPointHits = 0;
        missHits = 0;
        critHits = 0;
        maxDamageRecorded = 0f;
    }

    #region 무기 및 레벨 설정
    public void SetWeapon(int id)
    {
        // 무기를 바꿀 때 레벨과 모든 데이터를 초기화합니다.
        ResetDataFull();

        if (id == 0) SetStats("단검", 0.1f, 0.4f, 1.5f);
        else if (id == 1) SetStats("장검", 0.2f, 0.3f, 2.0f);
        else SetStats("도끼", 0.3f, 0.2f, 3.0f);

        logDisplay.text = $"{weaponName} 장착! (레벨 초기화)";
        UpdateUI();
    }

    private void SetStats(string _name, float _stdDev, float _critRate, float _critMult)
    {
        weaponName = _name; stdDevMult = _stdDev; critRate = _critRate; critMult = _critMult;
    }

    public void LevelUp()
    {
        // 레벨업 시에는 기존 통계만 초기화하고 레벨을 올립니다.
        ResetStatsOnly();
        level++;
        baseDamage = level * 20f;
        logDisplay.text = $"레벨업! 현재 레벨: {level}";
        UpdateUI();
    }
    #endregion

    #region 공격 및 시뮬레이션
    public void OnSimulate1000()
    {
        ResetStatsOnly(); // 시뮬레이션 시작 전 통계 초기화
        for (int i = 0; i < 1000; i++)
        {
            ProcessAttack(true);
        }

        // Log 칸에 1000회 요약 결과 표시
        logDisplay.text = $"약점 공격 {weakPointHits}회, 명중 실패 {missHits}회, 전체 크리티컬 {critHits}회,\n최대 데미지 {maxDamageRecorded:F4}";
        UpdateUI();
    }

    public void OnAttack() => ProcessAttack(false);

    private void ProcessAttack(bool isSimulating)
    {
        float sd = baseDamage * stdDevMult;
        float normalDamage = GetNormalStdDevDamage(baseDamage, sd);

        bool isWeak = false; bool isMiss = false;

        // 2σ 판정
        if (normalDamage > baseDamage + (2 * sd)) { isWeak = true; weakPointHits++; normalDamage *= 2f; }
        else if (normalDamage < baseDamage - (2 * sd)) { isMiss = true; missHits++; normalDamage = 0f; }

        // 치명타 판정
        bool isCrit = Random.value < critRate;
        if (isCrit) { critHits++; normalDamage *= critMult; }

        float finalDamage = normalDamage;
        attackCount++;
        totalDamage += finalDamage;
        if (finalDamage > maxDamageRecorded) maxDamageRecorded = finalDamage;

        if (!isSimulating)
        {
            string tags = (isMiss ? "<color=gray>[실패]</color> " : "") +
                          (isWeak ? "<color=yellow>[약점]</color> " : "") +
                          (isCrit ? "<color=red>[치명타!]</color> " : "");
            logDisplay.text = $"{tags}데미지: {finalDamage:F1}";
            UpdateUI();
        }
    }
    #endregion

    #region UI 및 수학
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
    #endregion
}