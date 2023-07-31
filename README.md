# FinalProject
스마트 빌딩(Smart Building) 프로젝트

## 차양막(차수막) [팀전체] 7.26
- 봉이 두개임, 손은 모터라생각하시면됨
<img src="https://github.com/annual-salary-investigation/Project-SmartBuilding/blob/main/image/%EC%B0%A8%EC%88%98%EB%A7%89.gif?raw=true" width="700">

## 세이프가드 [팀전체] 7.27
- 봉이 두개임, 손은 모터라생각하시면됨
<img src="https://github.com/annual-salary-investigation/Project-SmartBuilding/blob/main/image/safe_guard.gif?raw=true" width="700">



## 엘리베이터, 3층 wpf제작 (자인 동훈)
### 7.4, 7.5
- 엘리베이터 스텝모터, 스위치 구현	
### 7.6
- 엘리베이터 스텝모터, 스위치, 세븐 세그먼트 구현
	- 층 이동 및 층 표시
### 7.7
- 시리얼 통신(아두이노, 라즈베리파이), mqtt 통신(라즈베리파이, wpf)
### ~7.12
- 엘리베이터 : 드라이버 추가구매
- wpf : 기본 템플릿 ui 50%완료
### 7.13
- 버튼 ToggleSwitch로 대체, 
- ui 구조 조정, 거실 화장실 온도 lvc:Gauge / 습도 lvc:AngularGauge 더미데이터로 동작 (아직 mqtt, serial 통신 x) [ViewModel.cs]
- 마트 주문 DB연결
### ~7.26
- 엘리베이터 실물제작


## 센서 제어(예서, 자인, 동훈)

### 7월 4일 ~ 5일
- 온습도 센서 , 펜모터 제어 => 온도 올라가면 펜돌아가게
- 초음파센서, OLED => 가까워지면 OLED 화면 켜짐
- 화재감지센서 => 불꽃켜지면 LED 켜짐
- RFID => 지정된 카드찍히면 서보모터 움직임
- 조도센서 => 밝아지면 스텝모터 돌아가기 => 블라인드

### 7월 6일
- 아두이노 라즈베리파이 시리얼 통신
- 라즈베리파이에서 아두이노 실행

### 7월 7일
- 아두이노 메가 사용
- 온습도센서 DHT 22사용
- FanMotor 사용
- MQTT 연결

### ~7월 27일
- 차양막 (스텝모터 사용)제작 (버튼)
- 안전 가드 제작(스텝모터, 초음파센서, 부저)
  

## 자동차 번호판인식 (소영 지은)
### 1일차(7.3)
- 자동차 도로 제작
- 차량 차단기 모형 제작
- 차단기 바 모터와 연결하여 제작

### 2일차(7.4)
- 자동차 번호판 인식
  - PiCamera 로 번호판(이미지)을 인식
  - 번호판을 인식하면 모터가 움직임
- 라즈베리파이 MySQL DB 연결
  - DB의 차량번호 데이터를 라즈베리파이에서 조회

### 3일차(7.5)
- 자동차 번호판 인식
  - 모터 움직임 세부조정 -> 차량기가 90도 움직임
  - 인식된 번호판의 차량번호를 읽을 수 있도록
- 라즈베리파이에서 MySQL에 데이터를 INSERT 가능한지 확인

### 4일차(7.6)
- 자동차 번호판 인식
  - tesseract OCR 모듈을 이용해 번호판 이미지에서 글자 추출
  - 테스트용 이미지로 주피터노트북에서 결과 확인
  - 주피터노트북의 코드를 Python으로 작성(함수 정의)

### 5일차(7.7)
- 자동차 번호판 인식
  - Python 코드 정리 완료 (클래스 + 메인코드)
- 차량 차단기 구체적 실현 방안 논의
  - ① 초음파 센서로 일정거리안에 사물이 들어오면 카메라 인식
  - ② 카메라 찍히는 화면을 캡쳐하여 이미지로 저장
  - ③ 이미지의 번호판을 인식하여 글자 추출
  - ④ DB에 있는 차량번호와 일치하면 서보모터 동작(번호판 모형 or 실제 번호판 출력 => 인식 잘되는 것으로 선택)

### 6일차(7.11)
- 자동차 번호판 인식
  - 이미지 & DB 번호판 인식 연동 완료
  - DB 번호판 비교 및 서보모터 동작 완료

### 7일차(7.12)
- 자동차 번호판 인식
  - 실시간 카메라 정상 출력 및 DB 내 일치 번호판 식별 후 서보모터 동작 완료
 
### 8일차(7.13)
- 자동차 번호판 인식
  - 초음파 센서를 통해 특정거리에서 카메라로 번호판 캡쳐
  - 캡쳐 후 번호판 글자 출력 및 DB비교 후 서보모터 동작
  
### 9일차(7.17)
- 자동차 번호판 인식
  - 번호판 인식 후 DB 입출차시 입력 및 삭제 
  - 출차용 차단기 추가 제작
  
### 10일차(7.19)
- 자동차 번호판 인식
  - 지정 주차 입차 시 LED ON/OFF
  - 만차 시 지정주차X-> 서보모터 동작X 제어
 
### 11일차(7.20)
- 자동차 번호판 인식
  - 지정 주차 입차 시 R --> G 변화
  - DB 입차 시 IsExit, Reason 추가하여 만차, 번호판 오류 고려
  - 등록차량 및 일반차량에 따른 서보모터 구현 완료
    
< 해결 과제 >
- [x] 전처리 과정 코드 내용 학습 필요(어떤식으로 동작하는지, 왜 이렇게 되는지 등)
- [x] 번호판 출력값과 DB를 비교하는 코드 구상
- [x] 카메라 정상 출력 및 실시간 인식
- [x] 초음파 센서를 통해 어느정도의 거리에서 카메라 동작시킬지(캡쳐) 확인
- [x] DB에 번호판 입력 및 입출차시간 입력
- [x] 만차인 경우 고려
- [ ] 조도센서 및 부저 연동
- [ ] 출차 시 요금부과
- [ ] 번호판 전처리 정확성 높이기
      

## WPF 라즈베리파이 통신(가연 수민)
### 1일차(7.6)
- WPF 템플릿 제작
- 차량 관리 화면(진행 중)
### 2일차(7.7)
- 차량 관리 화면 완료
	- 차량 등록, 수정, 삭제 => MySQL DB 바인딩 완료
- 로그인 화면 완료
	- MySQL 관리자 DB 바인딩 완료
	- 앱 구현 마지막 단계에서 로드 이벤트 추가 예정
### 3일차(7.11)
- 센서 제어 대시보드 정리(진행 중)
- 날씨 화면 제작(진행 중)

### 4일차(7.12)
- 날씨 화면 제작(진행 중)
	- 기상청 API 연동 완료
	- 온도, 습도, 풍속 textblock에 바인딩 완료

### 5일차 (7.13)
- 날씨 화면 제작(진행 중)
	- 기상청 API 연동 완료
	- 온도, 습도, 풍속 textblock에 바인딩 완료
	- 값에 따른 날씨 이미지 변경 완료
	- 추가 진행사항 : 대시보드 정리

### 7일차 (7.15)
- 아두이노 <- 시리얼 통신 -> 라즈베리파이 <- MQTT -> WPF 앱 확인 완료
	- WPF 앱에서 토글버튼으로 아두이노에 연결된 LED 제어 가능

### 6일차 (7.14)
- 현재 단계에서 필요한 화면은 구현 완료
	- 라즈베리파이 <-> 아두이노 시리얼 통신 완료 
	- 라즈베리파이 <-> 윈도우(WPF 앱) MQTT 통신 확인 필요
	- 통신 완료시 나머지 화면 제작

### 8일차 (7.16)
- Timer 사용 -> 실행 중 현재 시간 실시간으로 바뀜
- 추가 작업은 센서 및 기능 구현 내용 정리 후 필요

### 9일차 (7.17)
- 기상청 홈페이지 크롤링 테스트
	- 기존 API 오류 잦아 홈페이지 크롤링 테스트
	- string으로 받아오는 값 double 형식으로 변환해야하는데 오류남... 확인 필요

### 10일차 (7.18)
- 기상청 홈페이지 크롤링 완료
	- 기상청 API 대신 웹크롤링해서 받아온 값으로 날씨영역 바인딩
- 대시보드 정리

### 11일차 (7.24)
- 사용자용 모니터링 앱 정리 진행 중
- MQTT 통신 테스트
  - 토글 버튼으로 LED 단순 제어 가능

### 12일차 (7.25)
- 사용자용 모니터링 앱 정리 진행 중
  - 온습도 센서 화면 구현 완료

### 13일차 (7.26)
- 사용자용 모니터링 앱 정리
- MQTT 통신
  - 온습도값 바인딩
  - LED 제어 가능

  
 < ToDoList >
- [ ] MQTT 통신 확인
  - 펜모터, 화재감지센서 등 전체 센서 제어 필요 
