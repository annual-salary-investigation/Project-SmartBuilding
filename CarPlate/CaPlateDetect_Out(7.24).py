# 출차코드
from Preprocess import *
import pytesseract
import pymysql
import cv2
import sys
import time
import RPi.GPIO as GPIO
from picamera2 import Picamera2, Preview
import numpy as np
# from PIL import Imagelkhujjuhy1
import threading

# 카메라 설정
picam2 = Picamera2() # 실시간 카메라 동작
preview_config = picam2.create_preview_configuration(main={"size": (800, 600)})
picam2.configure(preview_config)
picam2.start_preview(Preview.QTGL)
picam2.start()

# 이미지 전처리
# test = Preprocess('test1.jpg') # metadata = picam2.capture_file("test1.jpg")에 있는 test1.jpg 사용
# test.run()

# LED의 GPIO 핀
led_pin = 25
GPIO.setmode(GPIO.BCM)
GPIO.setup(led_pin, GPIO.OUT)
GPIO.output(led_pin, GPIO.LOW)  # LED 끄기

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
    # longest_idx, longest_text = -1, 0
    # plate_chars = []
   
    global result_chars # 번호판 글자를 담을 변수
    result_chars = ''
    # has_digit = False
   
    chars = pytesseract.image_to_string(img, lang='kor', config='--psm 7 --oem 0')    
    # --psm 7: 이미지 안의 글자가 한줄로 놓여있다 / --oem :0번 엔진(문자 그대로 한글자 한글자 읽음) 쓰겠다
   
    for c in chars:
        if ord('가') <= ord(c) <= ord('힣') or c.isdigit(): # 한글 잘 판별하도록 + 특수문자들 제거
            # if c.isdigit():
            #     has_digit = True
            result_chars += c

    print(result_chars) # 최종 결과

# 출차 시 DB 처리하는 함수
def CarExit():
    registered_car = False # 초기값 False / 등록된 차량 여부 판단하기 위한 변수로 사용 -> 서보모터 동작여부 결정 
    # 입차된 차량인지 확인
    query = 'SELECT CarName, CarId FROM parking'
    cur = conn.cursor()
    cur.execute(query)

    for row in cur:
        if result_chars == row[0]:
            if row[1] is not None: # 등록된 차량이면
                query = f"UPDATE parking SET ExitTime = '{now}', IsExit = {0} WHERE CarName = '{result_chars}'"
                registered_car = True # 등록된 차량일 경우 True
            
            elif row[1] is None: # 일반 차량이면
                query = f"UPDATE parking SET ExitTime = '{now}', IsExit = {0} WHERE CarName = '{result_chars}'"
                registered_car = False # 등록되지 않은 차량일 경우 False
        
    cur.execute(query)
    conn.commit()

    # if count >= 4:  # 일반주차구역이 만차이면
    #     return registered_car
    # else:
    return registered_car # 만차일 때 값을 반환하여 서보모터 동작을 방지

# 주차요금을 계산하는 함수(일단 1분+100원)
def GetFee():
    fee = 0

    query = f'SELECT TIMEDIFF(ExitTime, EntranceTime) FROM parking WHERE ExitTime IS NOT NULL'
    cur = conn.cursor()
    cur.execute(query)
    used_time = str(cur.fetchone()[0]).split(':')

    if int(used_time[0]) != 0: # 시
        for i in range(int(used_time[0])):
            fee += 6000

    elif int(used_time[1]) != 00: # 분
        for i in range(int(used_time[1])):
            fee += 100

    query = f'UPDATE parking SET Fee = {fee} WHERE ExitTime IS NOT NULL'
    cur.execute(query)
    conn.commit()

    print(f'주차요금 : {fee}원')


# DB 정리하는 함수
def Clear():
    query = 'DELETE FROM parking WHERE IsExit = 0;' # 만차
    cur = conn.cursor()
    cur.execute(query)

    conn.commit()

    
# 서보모터 동작 함수
def ServoMotor(duty):
    duty = 0 / 18 + 2

    GPIO.setup(servo_pin,GPIO.OUT)
    servo.ChangeDutyCycle(3.0)
    time.sleep(0.3)
    GPIO.setup(servo_pin,GPIO.IN)
    time.sleep(1)

    GPIO.setup(servo_pin,GPIO.OUT)
    servo.ChangeDutyCycle(10.0)
    time.sleep(0.3)
    GPIO.setup(servo_pin,GPIO.IN)
    time.sleep(1)

    # return duty

# 메인루프(입차)
try:
    is_car_detected = False  # 등록된 차량이 감지되었는지 여부를 저장하는 변수

    while True:      
        distance = measure_distance() # 초음파 센서를 사용하여 거리 측정
        print(distance)

        if distance < 8 : # 8보다 작은 경우
           
            metadata = picam2.capture_file("test1.jpg") # 카메라에서 이미지 캡쳐 후 test1.jpg 파일로 저장
            time.sleep(1)

            test = Preprocess("test1.jpg") # metadata = picam2.capture_file("test1.jpg")에 있는 test1.jpg 사용
            test.run()

            CarplateDetect(test.img_result) # 이미지에서 번호판 글자 추출

            if (len(result_chars) == 7 or len(result_chars) == 8): # 추출된 글자가 자동차 번호판 글자수와 일치하면
                registered_car = CarExit() # DB 데이터 UPDATE

                if registered_car and not is_car_detected:  # 등록된 차량이 감지되었을 때만 서보모터 동작
                    ServoMotor(0) # 서보모터 동작
                    is_car_detected = True
                    
                elif not registered_car and not is_car_detected: # 일반 차량이 감지되면 요금부과
                    GetFee() # 요금부과
                    is_car_detected = False
                    
                    continue # 요금부과 할 때 까지 DB에 존재

                Clear()
                
            else:
                is_car_detected = False # 등록된 차량이 아니므로 변수 초기화
        else:
            is_car_detected = False  # 차량이 감지되지 않았으므로 변수 초기화
            continue

except KeyboardInterrupt:
    print("사용자에 의해 측정이 중지되었습니다")
    GPIO.output(led_pin, GPIO.LOW)  # LED 끄기
    GPIO.cleanup()