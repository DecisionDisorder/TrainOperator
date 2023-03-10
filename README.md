

# 지하철 운영 게임 (TrainOperator)
구글 플레이에 업로드 되어있는 지하철 회사를 운영하는 안드로이드 플랫폼의 모바일 게임 프로젝트입니다.

[Developer]
- 김현종 (guswhd990507@naver.com)

## 프로젝트 소개
### 게임 설명
한국의 지하철 노선들을 구매하고 확장하며 수익을 거두면서 지하철 운영 회사를 키워나가는 게임입니다.  
터치형 수익을 향상시켜 화면의 일정 영역을 반복적으로 터치하며 수익을 거둘수도 있고, 시간형 수익을 향상시켜 가만히 기다려서 수익을 거둘수도 있습니다.  ㄴ
노선과 관련하여 구매, 확장을 통해 성장할 수 있는 콘텐츠는 다음과 같습니다.
- 열차 구매: 승객 수 증가로 인한 터치형 수익 향상시킵니다.
- 차량기지 구매: 구매할 수 있는 열차 개수 증가시킵니다.
- 역 구매: 노선 및 회사의 확장성 확대합니다.
- 확장권 구매: 새로운 구간이나 노선으로 확장 가능하도록 합니다.
- 노선 연결: 구매한 역들을 구간 단위로 연결하며 시간형 수익이 향상됩니다.
- 열차 확장: 일반 노선에 대해 배차마다의 열차의 길이를 늘려서 승객 수를 추가로 확보하여 터치형 수익이 향상됩니다.
- 경전철 노선 관리: 경전철 노선의 자동 운행 시스템을 개선하여 승객 수를 향상시킵니다.
- 스크린도어 설치: 고객의 만족도가 상승하여 수익 효율이 상승합니다.

노선 외에 지하철을 운영하는데에 필요한 기능을 다음과 같이 제공합니다.
- 기관사 고용: 기관사를 고용하여 감당 가능한 승객의 수를 확장시킵니다. 단, 기관사의 등급과 인원 수에 비례하여 일정한 주기로 월급이 지출됩니다.
- 지하철 관리: 위생과 안전을 관리하여 미니게임이나 위생 감사에 유리한 결과를 이끌어냅니다.
- 시설 임대: 확장한 노선이나 역에 비례하여 시설 임대를 내어 시간형 수익을 추가할 수 있습니다.
- 은행&복권: 예금을 하여 이자를 거두거나, 복권을 구매하여 크게 한방을 노릴 수 있습니다.

위의 성장요소와 함께 수익의 효율을 끌어올리는 시스템 2가지가 있습니다.
- 레벨: 터치형 수익을 거두면서 터치를 할 때 경험치를 획득합니다. 경험치를 모아 레벨업을 할 수 있으며, 이 레벨에 비례하여 시간형 수익의 효율이 상승합니다.
- 고객만족도: 스크린도어를 설치하면 고객 만족도를 상승시킬 수 있으며, 고객 만족도의 수치와 단계에 따라서 전체 수익의 효율이 상승합니다.

게임에 구현된 노선은 다음과 같습니다.
- 수도권 1~9호선
- 의정부 경전철
- 김포골드라인
- 부산 1~4호선
- 부산김해 경전철
- 동해선
- 대구 1~3호선
- 우이신설선
- 신분당선
- 수인분당선
- 에버라인
- 인천 1,2호선
- 경의중앙선
- 경춘선
- 경강선
- 신림선
- 공항철도
- 서해선
- 광주 1호선
- 대전 1호선

### 게임 플레이 시작
본 게임은 안드로이드 배포를 기준으로 개발되어 iOS 빌드에 대한 테스트는 진행되지 않았습니다.

게임의 최신 정식 버전은 [구글 플레이 스토어](https://play.google.com/store/apps/details?id=com.saejong25.TrainOperator)에서 다운로드 받을 수 있습니다.

또는, Unity 엔진의 버전을 2020.3.34f1에 맞추고 Build에서 Android 플랫폼으로 Switch하면 직접 디버깅용 APK 빌드가 가능합니다.

### 스크린샷
<img src="/Screenshots/ScreenShot1.png" width=50% height=50%/>
<img src="/Screenshots/ScreenShot2.png" width=50% height=50%/>
<img src="/Screenshots/ScreenShot3.png" width=50% height=50%/>
<img src="/Screenshots/ScreenShot4.png" width=50% height=50%/>
<img src="/Screenshots/ScreenShot5.png" width=50% height=50%/>
<img src="/Screenshots/ScreenShot6.png" width=50% height=50%/>
<img src="/Screenshots/ScreenShot7.png" width=50% height=50%/>
<img src="/Screenshots/ScreenShot8.png" width=50% height=50%/>


## 기술 관련 소개

### 개발 환경 및 기술 스택
- Unity 2020.3.31f1 + C#
- Google Play Game Service SDK 0.10.14
- Google Mobile Ads SDK 7.0.2
- Meta(Facebook) Audience SDK 6.4.0
- VCS: Unity Collaborate → GitHub


### 개발 이슈
1. '경' 이상 단위의 변수 체계의 필요성  
[관련 이야기 보기](https://velog.io/@dedi/%EB%8D%94-%ED%81%B0-%EC%88%AB%EC%9E%90%EB%A5%BC-%EB%8B%B4%EC%95%84%EB%82%B4%EB%B3%B4%EC%9E%90)  ([관련 소스코드: LargeVariable](https://github.com/DecisionDisorder/TrainOperator/blob/master/Assets/Script/MoneyUnitTranslator.cs))
3. 일부 코드 리팩토링  
[관련 이야기 보기](https://velog.io/@dedi/%EA%B3%BC%EA%B1%B0%EC%9D%98-%EB%82%98%EB%A5%BC-%EB%B0%98%EC%84%B1%ED%95%98%EA%B2%8C-%ED%95%98%EB%8A%94-%EC%BD%94%EB%93%9C%EB%93%A4)

4. 반복작업 자동화  
게임 특성상 UI 오브젝트의 위치를 일정한 간격으로 배치할 일이 많이 있었습니다.  
이를 자동화 하기 위해 유니티에서 제공하는 EditorGUILayout을 이용하여 interval, orientation, direction의 값을 설정할 수 있는 에디터용 UI를 만들었고, 일괄 배치할 오브젝트를 선택한 후에 'Reposition' 버튼을 누르면 일괄 재배치가 되도록 구현하였습니다.  
 규칙으로는 선택한 오브젝트 중 Hierarchy 상으로 가장 위에 있는 오브젝트 위치 기준으로 자동 배치 합니다.
([관련 소스코드](https://github.com/DecisionDisorder/TrainOperator/blob/master/Assets/Script/Tool/Reposition.cs))
