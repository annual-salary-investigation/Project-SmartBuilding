# 입차코드
from Preprocess import *
import pytesseract
import pymysql
import time
import RPi.GPIO as GPIO
from picamera2 import Picamera2, Preview
import numpy as np
import Adafruit_SSD1306
import smbus
import re

# 카메라 설정
picam2 = Picamera2() # 실시간 카메라 동작: 'Pinamera2' 클래스의 인스턴스 생성-> 라즈베리파이가 카메라 모듈을 제어
preview_config = picam2.create_preview_configuration(main={"size": (800, 600)}) # 'create_preview_configuration' 메서드 사용하여 미리보기/ 'main' 매개변수를 통해 미리보기 크기 지정(가로 800픽셀, 세로600픽셀)
picam2.configure(preview_config) # 설정한 미리보기 구성을 'Picamera2' 인스턴스에 적용
picam2.start_preview(Preview.QTGL) # 카메라 미리보기 시작/ 'Preview.QTGL': 미리보기 창 활성화 
picam2.start() # 실제 카메라 동작 시작 -> 카메라가 영상을 캡처하고, 미리보기에 실시간으로 표시되는 동작 수행

# I2C 버스 초기화(조도)
bus = smbus.SMBus(1) # SMBus 객체를 생성하여 I2C 통신을 초기화하고, Raspberry Pi의 GPIO 핀 중 I2C 통신을 위해 사용되는 핀을 초기화하고, I2C 통신을 관리하는 인스턴스를 생성
# PCF8591T의 I2C 주소(조도)
address = 0x48 # PCF8591T 모듈의 I2C 주소를 설정/ PCF8591T 모듈의 I2C 주소를 0x48로 설정
# 조도센서 A0 채널
light_channel = 0 # PCF8591T 모듈에 연결된 조도센서의 채널 번호를 설정/ PCF8591T 모듈은 여러 개의 아날로그 입력 채널을 가지고 있는데, 조도센서가 연결된 채널 번호를 0으로 설정

# 부저 핀 번호
buzzer_pin = 17
pcf8591_channel = 0 # PCF8591T 모듈의 조도센서에 연결된 채널 번호 (0에서 3 사이의 값) => 조도센서가 PCF8591T 모듈의 0번 채널에 연결되어있음

# 부저 GPIO 설정
GPIO.setmode(GPIO.BCM)
GPIO.setup(buzzer_pin, GPIO.OUT, initial=GPIO.LOW)  # 초기값을 LOW(끈 상태)로 설정

# LED 모듈의 빨간색, 녹색에 대응하는 GPIO핀 설정
red_pin = 6
green_pin = 5
GPIO.setmode(GPIO.BCM)
GPIO.setup(green_pin, GPIO.OUT, initial=GPIO.HIGH)
GPIO.setup(red_pin, GPIO.OUT, initial=GPIO.LOW)

# 초음파센서 GPIO 핀 설정
trigger_pin = 24
echo_pin = 23
GPIO.setmode(GPIO.BCM)
GPIO.setup(trigger_pin, GPIO.OUT)
GPIO.setup(echo_pin, GPIO.IN)

# 서보모터 GPIO 핀 설정
servo_pin = 18
GPIO.setmode(GPIO.BCM) # GPIO 핀들의 번호를 지정하는 규칙 설정
GPIO.setup(servo_pin, GPIO.OUT) # 출력으로 설정

# 서보모터 초기화
servo = GPIO.PWM(servo_pin, 50) # 서보핀을 PWM 모드 50Hz로 사용
servo.start(0) # 초기값 0

# DB에서 필요한 변수
conn = pymysql.connect(host='210.119.12.54', user='root', password='12345', db='miniproject', charset='utf8') #db연결
now = time.strftime('%Y-%m-%d %H:%M:%S')

# 전역 변수로 result_chars 선언
result_chars = ''

# PCF8591T에서 조도값을 읽는 함수
def read_light():
    bus.write_byte(address, 0x40 | light_channel)  # A/D 변환 채널 선택
    light_value = bus.read_byte(address)  # 조도값 읽기
    return light_value

# 부저를 울리는 함수
def sound_buzzer(state, light_value):
    GPIO.output(buzzer_pin, GPIO.HIGH)
    time.sleep(1)
    GPIO.output(buzzer_pin, GPIO.LOW)

# 지정차량이 주차되어 있을 때 LED 녹색불 켜는 함수
def turn_on_green():
    state = False

    query = 'SELECT CarId FROM parking'
    cur = conn.cursor()
    cur.execute(query)
    conn.commit()
    car_id = cur.fetchall()
    
    car_id_list = [result[0] for result in car_id]

    for car_id in car_id_list:
        if car_id != None: # 등록Id가 있으면 초록불
            GPIO.output(red_pin, GPIO.HIGH)
            GPIO.output(green_pin, GPIO.LOW)
            state = False

        else:
            GPIO.output(red_pin, GPIO.LOW)
            GPIO.output(green_pin, GPIO.HIGH)
            state = True
    return state

# 초음파 센서를 사용하여 거리를 측정하는 함수
def measure_distance():
    GPIO.output(trigger_pin, True)
    time.sleep(1)
    GPIO.output(trigger_pin, False)
   
    pulse_start = time.time()
    pulse_end = time.time()
   
    while GPIO.input(echo_pin) == 0:
        pulse_start = time.time()
       
    while GPIO.input(echo_pin) == 1:
        pulse_end = time.time()
       
    pulse_duration = pulse_end - pulse_start
    distance = pulse_duration * 17150
    distance = round(distance, 2)
   
    return distance


# 전처리 된 이미지에서 글자를 추출하는 함수
def CarplateDetect(img) :   
    global result_chars # 번호판 글자를 담을 변수
    result_chars = ''  # result_chars 초기화
   
    chars = pytesseract.image_to_string(img, lang='kor', config='--psm 7 --oem 0')    
    # --psm 7: 이미지 안의 글자가 한줄로 놓여있다 / --oem :0번 엔진(문자 그대로 한글자 한글자 읽음) 쓰겠다
   
    for c in chars:
        if ord('가') <= ord(c) <= ord('힣') or c.isdigit(): # 한글 잘 판별하도록 + 특수문자들 제거
            result_chars += c

    # 추출된 글자가 2자리 숫자, 1자리 한글, 4자리 숫자 순서로 맞는지 확인
    pattern = r'^\d{2}[가-힣]\d{4}$'
    match = re.match(pattern, result_chars)

    if match:
        print("번호판 인식 성공:", result_chars)
        return True
    else:
        print("유효하지 않은 번호판 형식:", result_chars)
        return False

print(result_chars) # 최종 결과

# 입차 시 DB 처리하는 함수
def CarEntrance():
    registered_car = False # 초기값 False / 등록된 차량 여부 판단하기 위한 변수로 사용 -> 서보모터 동작여부 결정 
    # carDB에서 조회후 일치하는 값이 있는지(등록된 차량인지) 비교  
    query = 'SELECT id, name FROM car'
    cur = conn.cursor()
    cur.execute(query)

    for row in cur:
        if result_chars == row[1]:
            query = f"INSERT INTO parking(CarName, EntranceTime, Fee, CarId, IsExit) VALUES('{row[1]}', '{now}', 0, '{row[0]}', 1)"
            registered_car = True # 등록된 차량일 경우 True
            break
            
        else:  # 등록되지 않은 차량일 경우
            if parking_full == True: # 만차
                query = f"INSERT INTO parking(CarName, EntranceTime, IsExit, Reason) VALUES('{result_chars}', '{now}', 0, '만차')"    
            elif parking_full == False:
                query = f"INSERT INTO parking(CarName, EntranceTime, IsExit) VALUES('{result_chars}', '{now}', 1)"
            
            registered_car = False # 등록되지 않은 차량일 경우
            
    cur.execute(query)
    conn.commit()

    return registered_car # 만차일 때 값을 반환하여 서보모터 동작을 방지
    
# 현재 일반주차구역에 주차되어 있는 차량 갯수를 구하는 함수
def Full():
    global parking_full # 일반차량이 만차인지 여부를 저장하는 변수

    query = 'SELECT count(IsExit) FROM parking WHERE CarId IS NULL;'
    cur = conn.cursor()
    cur.execute(query)
    count = cur.fetchone()[0]

    if count >= 4:
        parking_full = True
    else: parking_full = False

    return parking_full

# 서보모터 동작 함수
def ServoMotor(duty):
    duty = 0 / 18 + 2

    GPIO.setup(servo_pin,GPIO.OUT)
    servo.ChangeDutyCycle(3.0)
    time.sleep(0.3)
    GPIO.setup(servo_pin,GPIO.IN)
    time.sleep(2)

    GPIO.setup(servo_pin,GPIO.OUT)
    servo.ChangeDutyCycle(9.0)
    time.sleep(0.3)
    GPIO.setup(servo_pin,GPIO.IN)
    time.sleep(2)


# DB 정리하는 함수
def Clear():
    query = 'DELETE FROM parking WHERE IsExit = 0;' # 입차불가차량
    cur = conn.cursor()
    cur.execute(query)

    conn.commit()


# 메인루프(입차)
try:
    is_car_detected = False  # 등록된 차량이 감지되었는지 여부를 저장하는 변수
    light_value = 0

    while True:
        distance = measure_distance() # 초음파 센서를 사용하여 거리 측정
        print("거리:", distance)

        # 조도값 읽기
        light_value = read_light()  # read_light() 함수가 light_value를 반환하도록 수정  
        print("조도값:", light_value)

        state = turn_on_green()
        if state:
            light_value = read_light() # 조도센서 값을 읽음
            if is_car_detected and light_value >= 190:  # 차량이 감지되고, 조도센서 값이 190 이상일 때만 부저를 작동시킴
                print("부저가 울림")
                sound_buzzer(state, light_value)  # state와 light_value를 인자로 제공

        turn_on_green()

        if distance < 8 : # 8보다 작은 경우
            
            metadata = picam2.capture_file("test1.jpg") # 카메라에서 이미지 캡쳐 후 test1.jpg 파일로 저장
            time.sleep(2)

            # 이미지 전처리
            test = Preprocess("test1.jpg") # metadata = picam2.capture_file("test1.jpg")에 있는 test1.jpg 사용
            test.run()
            if test.isCarPlate():
                CarplateDetect(test.img_result) # 이미지에서 번호판 글자 추출

                if (len(result_chars) == 7): # 추출된 글자가 자동차 번호판 글자수(7자)와 일치하면
                    Clear() # DB 정리
                    Full() # 만차여부 확인
                    registered_car = CarEntrance() # DB에 데이터 INSERT

                    if registered_car and not is_car_detected:  # 등록된 차량이 감지되었을 때
                        ServoMotor(0) # 서보모터 동작
                        is_car_detected = True

                    elif not registered_car and not parking_full:
                        ServoMotor(0) # 서보모터 동작
                        is_car_detected = True # 일반차량이 감지되고, 만차가 아닐 경우에도 서보모터 동작

                else:
                    is_car_detected = False # 등록된 차량이 아니므로 변수 초기화

            else:
                is_car_detected = False  # 차량이 감지되지 않았으므로 변수 초기화
                continue

except KeyboardInterrupt:
    print("사용자에 의해 측정이 중지되었습니다")
    GPIO.output(red_pin, GPIO.HIGH)  # LED 끄기
    GPIO.output(green_pin, GPIO.HIGH)  # LED 끄기
    GPIO.output(buzzer_pin, GPIO.LOW)  # 프로그램 종료 시 부저 끄기
    GPIO.cleanup()