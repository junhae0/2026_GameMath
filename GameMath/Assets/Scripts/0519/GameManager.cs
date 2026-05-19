using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // UI 시스템(Text)을 사용하기 위해 필수
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Turn { Player1, Player2 }

    [Header("게임 상태 관리")]
    public Turn currentTurn = Turn.Player1;
    public int p1Score = 0;
    public int p2Score = 0;

    [Header("화면 표시용 UI 텍스트")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI scoreText;

    [Header("씬에 있는 모든 공 등록 (4개)")]
    public List<BilliardsBall> allBalls;

    private bool isBallMoving = false;

    // 중복 충돌을 방지하기 위해 이번 샷에 부딪힌 목적구(BilliardsBall)들을 저장하는 해시셋
    private HashSet<BilliardsBall> hitTargets = new HashSet<BilliardsBall>();

    // 공이 멈춰있을 때만 마우스 입력을 받도록 체크
    public bool CanPlay => !isBallMoving;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 UI 및 카메라 타겟 초기화
        UpdateGameUI();
    }

    // 현재 치려고 하는 공이 내 턴에 맞는 공인지 확인
    public bool IsMyBall(BilliardsBall.BallType type)
    {
        if (currentTurn == Turn.Player1 && type == BilliardsBall.BallType.Player1) return true;
        if (currentTurn == Turn.Player2 && type == BilliardsBall.BallType.Player2) return true;
        return false;
    }

    // 마우스로 공을 쳤을 때 발동
    public void OnBallShot()
    {
        isBallMoving = true;
        hitTargets.Clear(); // 샷을 날릴 때마다 부딪힌 공 목록을 깨끗하게 비웁니다.
        StartCoroutine(CheckBallsStoppedRoutine());
    }

    // 공과 공이 부딪혔을 때 호출되는 함수
    public void RecordCollision(BilliardsBall.BallType activeBall, BilliardsBall.BallType hitBall, BilliardsBall hitBallComponent)
    {
        if (!isBallMoving) return;

        // 💡 [점수 감점 파울 완벽 제거] 상대방 공을 맞추더라도 아무런 패널티가 없습니다.

        // 목적구(Target 흰공)와 부딪혔을 때만 처리
        if (hitBall == BilliardsBall.BallType.Target && hitBallComponent != null)
        {
            // 중복 입력을 알아서 걸러주는 HashSet에 충돌된 흰공 스크립트를 추가합니다.
            hitTargets.Add(hitBallComponent);
        }
    }

    // 모든 공이 완전히 멈췄는지 감시하는 코루틴
    private IEnumerator CheckBallsStoppedRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            bool allStopped = true;
            foreach (var ball in allBalls)
            {
                if (ball != null && !ball.IsStopped())
                {
                    allStopped = false;
                    break;
                }
            }

            // 모든 공이 멈추면 루프 탈출
            if (allStopped) break;
            yield return new WaitForSeconds(0.1f);
        }

        EvaluateTurnResult();
    }

    // 점수를 계산하고 턴을 정산하는 함수
    private void EvaluateTurnResult()
    {
        isBallMoving = false;
        bool isScored = false;

        // 서로 다른 흰공 2개를 모두 맞췄는지 체크 (Count == 2)
        if (hitTargets.Count >= 2)
        {
            if (currentTurn == Turn.Player1) p1Score++;
            else p2Score++;

            isScored = true; // 💡 득점 성공 상태 저장
            Debug.Log("득점 성공! 서로 다른 두 개의 흰공을 모두 맞췄습니다. 연속 기회 획득!");
        }
        else
        {
            Debug.Log("득점 실패! 다음 플레이어에게 턴이 넘어갑니다.");
        }

        // 점수 UI 최신화
        UpdateGameUI();

        // 5점 도달 시 게임 종료
        if (p1Score >= 5 || p2Score >= 5)
        {
            string winner = p1Score >= 5 ? "1P" : "2P";
            if (turnText != null) turnText.text = $"게임 종료! 승리자: {winner} 🏆";
            return;
        }

        // 💡 [규칙 유지] 점수를 내지 못했을 때만(!isScored) 다음 플레이어로 턴 변경
        if (!isScored)
        {
            currentTurn = (currentTurn == Turn.Player1) ? Turn.Player2 : Turn.Player1;
        }

        // UI 및 카메라 타겟 업데이트
        UpdateGameUI();
    }

    // UI 최신화 및 카메라 Target 변경
    private void UpdateGameUI()
    {
        if (turnText != null)
        {
            string turnName = (currentTurn == Turn.Player1) ? "1P (흰색공)" : "2P (빨강공)";
            turnText.text = $"현재 턴: {turnName}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"[ 점수 ]  1P: {p1Score}점  |  2P: {p2Score}점";
        }

        if (Camera.main != null)
        {
            CameraOrbit cameraScript = Camera.main.GetComponent<CameraOrbit>();
            if (cameraScript != null)
            {
                foreach (var ball in allBalls)
                {
                    if (ball != null && IsMyBall(ball.type))
                    {
                        cameraScript.target = ball.transform; //
                        break;
                    }
                }
            }
        }
    }
}