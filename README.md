# Flappy-Astro

## 랭킹 작업 머지 안내

랭킹 관련 스크립트는 `Assets/3.Script/Ranking` 폴더에 있습니다.

- `ScoreManager`가 점수 기록, TOP 10 제한, JSON 저장과 불러오기를 담당함.
- 기록은 점수 내림차순으로 직접 구현한 선택 정렬을 사용합니다. 동점이면 먼저 달성한 기록이 앞에 유지.
- 기록 데이터는 `Application.persistentDataPath/personal_top_10.json`에 저장.
- `ScoreUI`는 최고 점수와 TOP 10 목록을 표시합니다.

게임 오버 시 최종 점수를 랭킹에 적용하려면 게임 오버 처리 코드에서 아래처럼 호출

```csharp
ScoreManager.Instance.AddScore(finalScore);
```

`AddScore` 호출 후에는 기록 저장과 TOP 10 UI 갱신이 자동으로 처리 기존 JSON 호환을 위해 `ScoreRecord.score`, `ScoreRecord.achievedAtUnixMilliseconds`, `ScoreData.records`의 이름은 변경하지않음.
