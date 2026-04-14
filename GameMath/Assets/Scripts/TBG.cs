using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용
using System.Text;

public class TBG : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI resultText; // 상단 '전투 결과'용
    [SerializeField] private TextMeshProUGUI itemText;   // 하단 '획득한 아이템'용

    [Header("Settings")]
    [SerializeField] float critChance = 0.2f;
    [SerializeField] float meanDamage = 20f;
    [SerializeField] float stdDevDamage = 5f;
    [SerializeField] float enemyHP = 100f;
    [SerializeField] float poissonLambda = 2f;
    [SerializeField] float hitRate = 0.6f;
    [SerializeField] float critDamageRate = 2f;
    [SerializeField] int maxHitsPerTurn = 5;

    // --- 데이터 집계 변수 ---
    int turn = 0;
    bool rareItemObtained = false;
    float currentRareChance = 0.05f;

    int totalSpawnedEnemies = 0;
    int totalKilledEnemies = 0;
    int totalAttackCount = 0;
    int totalHitCount = 0;
    int totalCritCount = 0;
    float maxDamage = 0f;
    float minDamage = float.MaxValue;

    int potionCount = 0, goldCount = 0;
    int weaponNormal = 0, weaponRare = 0;
    int armorNormal = 0, armorRare = 0;

    string[] rewards = { "Gold", "Weapon", "Armor", "Potion" };

    // 버튼에 연결할 함수
    public void StartSimulation()
    {
        ResetData();

        // 레어 아이템 획득 시까지 반복
        while (!rareItemObtained)
        {
            turn++;
            SimulateTurn();
            currentRareChance = Mathf.Min(currentRareChance + 0.05f, 1.0f); // 턴당 5% 상승
        }

        UpdateUI();
    }

    void ResetData()
    {
        rareItemObtained = false;
        turn = 0;
        currentRareChance = 0.05f;
        totalSpawnedEnemies = 0; totalKilledEnemies = 0;
        totalAttackCount = 0; totalHitCount = 0; totalCritCount = 0;
        maxDamage = 0f; minDamage = float.MaxValue;
        potionCount = 0; goldCount = 0;
        weaponNormal = 0; weaponRare = 0;
        armorNormal = 0; armorRare = 0;
    }

    void SimulateTurn()
    {
        int enemyCount = SamplePoisson(poissonLambda);
        totalSpawnedEnemies += enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            int hits = SampleBinomial(maxHitsPerTurn, hitRate);
            totalAttackCount += maxHitsPerTurn;
            totalHitCount += hits;

            float totalDamage = 0f;
            for (int j = 0; j < hits; j++)
            {
                float damage = SampleNormal(meanDamage, stdDevDamage);
                if (Random.value < critChance)
                {
                    damage *= critDamageRate;
                    totalCritCount++;
                }

                if (damage > maxDamage) maxDamage = damage;
                if (damage < minDamage) minDamage = damage;
                totalDamage += damage;
            }

            if (totalDamage >= enemyHP)
            {
                totalKilledEnemies++;
                string reward = rewards[Random.Range(0, rewards.Length)];
                ProcessReward(reward);
            }
        }
    }

    void ProcessReward(string reward)
    {
        if (reward == "Potion") potionCount++;
        else if (reward == "Gold") goldCount++;
        else if (reward == "Weapon")
        {
            if (Random.value < currentRareChance) { weaponRare++; rareItemObtained = true; }
            else weaponNormal++;
        }
        else if (reward == "Armor")
        {
            if (Random.value < currentRareChance) { armorRare++; rareItemObtained = true; }
            else armorNormal++;
        }
    }

    void UpdateUI()
    {
        float finalHitRate = (totalAttackCount > 0) ? (float)totalHitCount / totalAttackCount * 100f : 0;
        float finalCritRate = (totalHitCount > 0) ? (float)totalCritCount / totalHitCount * 100f : 0;

        // 1. 전투 결과 텍스트 정렬
        StringBuilder sbResult = new StringBuilder();
        sbResult.AppendLine("<color=#FFFF00><size=130%>전투 결과</size></color>\n");
        sbResult.AppendLine($"총 진행 턴 수 : {turn}");
        sbResult.AppendLine($"발생한 적 : {totalSpawnedEnemies}");
        sbResult.AppendLine($"처치한 적 : {totalKilledEnemies}");
        sbResult.AppendLine($"공격 명중 결과 : {finalHitRate:F2}%");
        sbResult.AppendLine($"발생한 치명타 결과 : {finalCritRate:F2}%");
        sbResult.AppendLine($"최대 데미지 : {maxDamage:F2}");
        sbResult.AppendLine($"최소 데미지 : {minDamage:F2}");

        if (resultText != null) resultText.text = sbResult.ToString();

        // 2. 획득한 아이템 텍스트 정렬
        StringBuilder sbItem = new StringBuilder();
        sbItem.AppendLine("<color=#FFFF00><size=130%>획득한 아이템</size></color>\n");
        sbItem.AppendLine($"포션 : {potionCount}개");
        sbItem.AppendLine($"골드 : {goldCount}개");
        sbItem.AppendLine($"무기 - 일반 : {weaponNormal}개");
        sbItem.AppendLine($"무기 - 레어 : {weaponRare}개");
        sbItem.AppendLine($"방어구 - 일반 : {armorNormal}개");
        sbItem.AppendLine($"방어구 - 레어 : {armorRare}개");

        if (itemText != null) itemText.text = sbItem.ToString();

        // 콘솔에도 통합 출력
        Debug.Log(sbResult.ToString() + "\n" + sbItem.ToString());
    }

    // --- 분포 샘플 함수 ---
    int SamplePoisson(float lambda)
    {
        int k = 0; float p = 1f; float L = Mathf.Exp(-lambda);
        while (p > L) { k++; p *= Random.value; }
        return k - 1;
    }

    int SampleBinomial(int n, float p)
    {
        int success = 0;
        for (int i = 0; i < n; i++) if (Random.value < p) success++;
        return success;
    }

    float SampleNormal(float mean, float stdDev)
    {
        float u1 = Random.value; float u2 = Random.value;
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);
        return mean + stdDev * z;
    }
}