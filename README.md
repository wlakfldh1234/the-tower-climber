# 🔥 더 타워 클라이머 (The Tower Climber)

차오르는 용암을 피해 탑을 오르는 2D 생존 플랫포머

![Unity](https://img.shields.io/badge/Unity-6.0-black?style=flat-square&logo=unity)
![C#](https://img.shields.io/badge/C%23-9.0-239120?style=flat-square&logo=c-sharp)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

---

## 📖 게임 소개

**더 타워 클라이머**는 밑에서 차오르는 용암을 피해 탑 정상까지 올라가는 2D 생존 플랫포머입니다.

스테이지를 클리어할 때마다 골드를 획득하고, 상점에서 능력치를 강화해 더 높은 탑에 도전할 수 있습니다.

> **"차오르는 용암, 멈추면 끝이다"**

---

## 🎮 게임 플레이

### 핵심 루프

### 주요 시스템
- **생존 플랫포머** - 시간 안에 정상까지 도달
- **성장 시스템** - HP / 이동속도 / 점프력 / 보호막 강화
- **환불 기능** - 잘못 산 아이템 되돌리기 (LIFO)
- **상태창** - 현재 능력치 및 보유 아이템 확인

---

## 🕹️ 조작법

| 키 | 기능 |
|---|---|
| `A` / `D` 또는 `←` / `→` | 좌우 이동 |
| `Space` | 점프 |
| `I` | 상태창 열기 / 닫기 |
| `Esc` | 일시정지 |

### 시연용 치트키 (개발자 모드)
| 키 | 기능 |
|---|---|
| `F1` | 즉시 스테이지 클리어 |
| `F2` | 골드 +500 |
| `F3` | HP 풀 회복 |
| `F4` | 즉시 게임 클리어 |

---

## 🛠️ 개발 환경

- **엔진** : Unity 6000.3.9f1
- **언어** : C#
- **입력** : Unity Input System
- **개발 기간** : 2주 3일
- **개발 인원** : 1인 개발

---

## 🏗️ 프로젝트 구조

### 핵심 매니저 (Singleton Pattern)
- **GameManager** - 스테이지 진행, 골드, 플레이어 스탯 저장 (DontDestroyOnLoad)
- **SoundManager** - BGM / SFX 재생 및 볼륨 관리
- **ShopManager** - 상점 시스템, 자료구조 4종 관리

### 컴포넌트 분리 (플레이어)
- `PlayerMove2D` - 이동 / 점프 물리 처리
- `PlayerHp` - 체력 관리 및 데미지 처리
- `PlayerStats` - 능력치 적용 / 원복
- `PlayerInputReader` - 입력 감지

---

## 💡 자료구조 활용

상점 & 인벤토리 시스템 하나에 **Array, Queue, Stack, List** 4가지 자료구조를 의도적으로 분리해 사용했습니다.

| 자료구조 | 역할 | 선택 이유 |
|---|---|---|
| **Array** | 상점 카탈로그 (`ItemData[] shopItems`) | 고정 크기 데이터, O(1) 인덱스 접근 |
| **Queue** | 구매 요청 대기열 (`Queue<int>`) | FIFO - 요청 발생과 처리 분리 |
| **Stack** | 구매 이력 / 환불 (`Stack<ItemData>`) | LIFO - 가장 최근 구매부터 취소 |
| **List** | 보유 아이템 (`List<ItemData>`) | 동적 추가 / 삭제, foreach 순회로 UI 반영 |

---

## 🐛 트러블슈팅

### Issue 1. Retry 후 잘못된 골드가 지급되는 문제
- **원인** : `StageClear()` 와 `Retry()` 양쪽에서 `currentStage` 를 변경하면서 상태 추적 꼬임
- **해결** : 상태 변경 책임을 한 곳(`NextStage()`)에 집중

### Issue 2. 씬 전환 시 인벤토리가 초기화되는 문제
- **원인** : 씬 단위 오브젝트인 `ShopManager` 가 새로 생성되며 List 초기화
- **해결** : 인벤토리 데이터 소유권을 `GameManager` (DontDestroyOnLoad) 로 이관

---

## 📸 스크린샷

> 추후 게임 플레이 스크린샷 추가 예정

---

## 🎬 시연 영상

> 추후 시연 영상 링크 추가 예정

---

## 📁 폴더 구조

```
Assets/
├── Scenes/
│   ├── Title.unity
│   ├── Stage1 ~ Stage10.unity
│   └── GameClear.unity
├── Scripts/
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   ├── SoundManager.cs
│   │   └── ShopManager.cs
│   ├── Player/
│   │   ├── PlayerMove2D.cs
│   │   ├── PlayerHp.cs
│   │   ├── PlayerStats.cs
│   │   └── PlayerInputReader.cs
│   ├── UI/
│   │   ├── TitleManager.cs
│   │   ├── StatusUIManager.cs
│   │   ├── CountdownManager.cs
│   │   └── GameClearManager.cs
│   └── Objects/
│       ├── Lava.cs
│       ├── Portal.cs
│       ├── GoalPlatform.cs
│       └── MovingPlatform.cs
├── Prefabs/
├── Animations/
├── Sprites/
└── Audio/
```
---

## 👤 개발자

**구동현**
- 1인 개발 포트폴리오 프로젝트

---

## 📝 라이선스

MIT License
