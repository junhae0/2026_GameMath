using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssignmentManager : MonoBehaviour
{
    [Header("--- [설정] 기본 스탯 ---")]
    public float playerAtk = 30f;
    public float targetCritRate = 0.3f; // 30% 목표
    public float enemyMaxHp = 300f;
    private float enemyCurrentHp;

    [Header("--- [데이터] 치명타 보정 (실습 3) ---")]
    public int totalHits = 0;
    public int critHits = 0;

    [Header("--- [데이터] 전리품 확률 및 개수 ---")]
    private float probLegend = 5.0f;
    private float probRare = 15.0f;
    private float probHigh = 30.0f;
    private float probNormal = 50.0f;

    private int countLegend, countRare, countHigh, countNormal;

    [Header("--- [UI 연결] ---")]
    public TextMeshProUGUI leftStatsText;   // 왼쪽: 공격/치명타 정보
    public TextMeshProUGUI rightProbText;    // 오른쪽 위: 현재 아이템 확률
    public TextMeshProUGUI rightCountText;   // 오른쪽 아래: 획득 아이템 개수
    public TextMeshProUGUI hpText;           // 중앙 상단: 체력 텍스트
    public Slider hpSlider;                  // 중앙 상단: HP 바

    void Start()
    {
        enemyCurrentHp = enemyMaxHp;
        UpdateUI();
    }

    // [공격 버튼 클릭 이벤트]
    public void OnClickAttack()
    {
        // 1. 치명타 여부 결정 (이미지의 엄격한 보정 로직 적용)
        bool isCrit = RollCritStrict();

        // 2. 데미지 계산 (치명타 시 2배)
        float damage = isCrit ? playerAtk * 2 : playerAtk;
        enemyCurrentHp -= damage;

        // 3. 적 사망 체크
        if (enemyCurrentHp <= 0)
        {
            enemyCurrentHp = 0;
            UpdateUI();
            HandleEnemyDeath();
        }
        else
        {
            UpdateUI();
        }
    }

    // [치명타 보정 로직] 이미지의 "강제 발생/강제 일반" 요구사항 반영
    private bool RollCritStrict()
    {
        totalHits++;
        float currentRate = totalHits > 1 ? (float)critHits / totalHits : 0f;

        // A. 치명타가 안 뜰 경우 강제로 발생 (비율이 목표보다 낮고, 한 번 더 터져도 목표 이내일 때)
        if (currentRate < targetCritRate && (float)(critHits + 1) / totalHits <= targetCritRate)
        {
            Debug.Log("Forced Critical Hit!");
            critHits++;
            return true;
        }

        // B. 너무 발생 많이 해도 강제로 일반공격 (비율이 목표보다 높을 때)
        if (currentRate > targetCritRate && (float)critHits / totalHits >= targetCritRate)
        {
            Debug.Log("Forced Normal Hit!");
            return false;
        }

        // C. 그 외의 경우 기본 확률 30% 주사위
        if (Random.value < targetCritRate)
        {
            critHits++;
            return true;
        }

        return false;
    }

    // [적 사망 시 처리]
    private void HandleEnemyDeath()
    {
        // 아이템 획득 시뮬레이션
        float r = Random.Range(0f, 100f);

        if (r < probLegend) // 전설 획득
        {
            countLegend++;
            ResetItemProbs(); // 확률 초기화
        }
        else // 전설 획득 실패 시 확률 보정
        {
            if (r < probLegend + probRare) countRare++;
            else if (r < probLegend + probRare + probHigh) countHigh++;
            else countNormal++;

            UpdateItemProbs(); // 전설 1.5% 증가, 나머지 0.5% 감소
        }

        // 새로운 적 등장
        enemyCurrentHp = enemyMaxHp;
        UpdateUI();
    }

    // 전설 획득 실패 시 확률 조정
    private void UpdateItemProbs()
    {
        probLegend += 1.5f;
        probRare -= 0.5f;
        probHigh -= 0.5f;
        probNormal -= 0.5f;
    }

    // 전설 획득 시 확률 초기화
    private void ResetItemProbs()
    {
        probLegend = 5.0f;
        probRare = 15.0f;
        probHigh = 30.0f;
        probNormal = 50.0f;
    }

    private void UpdateUI()
    {
        // 왼쪽 통계
        float actualRate = totalHits > 0 ? ((float)critHits / totalHits) * 100f : 0f;
        leftStatsText.text = $"전체 공격 횟수 : {totalHits}\n" +
                             $"발생한 치명타 횟수 : {critHits}\n" +
                             $"설정된 치명타 확률 : {targetCritRate * 100:F2}%\n" +
                             $"실제 치명타 확률 : {actualRate:F2}%";

        // 중앙 HP
        hpText.text = $"체력 : {enemyCurrentHp}/{enemyMaxHp}";

        // 오른쪽 상단 현재 아이템 확률
        rightProbText.text = $"현재 아이템 확률\n" +
                             $"일반 : {probNormal:F1}%\n" +
                             $"고급 : {probHigh:F1}%\n" +
                             $"희귀 : {probRare:F1}%\n" +
                             $"전설 : {probLegend:F1}%";

        // 오른쪽 하단 드롭된 아이템 개수
        rightCountText.text = $"현재 드롭된 아이템\n" +
                              $"일반 : {countNormal}\n" +
                              $"고급 : {countHigh}\n" +
                              $"희귀 : {countRare}\n" +
                              $"전설 : {countLegend}";
    }
}