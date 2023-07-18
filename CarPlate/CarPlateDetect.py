from Preprocess import *
import pytesseract
import pymysql
import cv2
import sys
import time
import RPi.GPIO as GPIO
from picamera2 import Picamera2, Preview
import numpy as np
from PIL import Image
import threading

#pytesseract.pytesseract.tesseract_cmd = r'C:/Dev/Tools/Tesseract-OCR/tesseract.exe'

# 카메라 설정
picam2 = Picamera2() # 실시간 카메라 동작

preview_config = picam2.create_preview_configuration(main={"size": (800, 600)})
picam2.configure(preview_config)
picam2.start_preview(Preview.QTGL)
picam2.start()

# 이미지 전처리
test = Preprocess('test1.jpg') # metadata = picam2.capture_file("test1.jpg")에 있는 test1.jpg 사용
test.run()

# [초음파센서 동작]
# 초음파 센서 GPIO핀
trigger_pin = 24
echo_pin = 23

# 초음파 GPIO 핀 설정
GPIO.setmode(GPIO.BCM)
GPIO.setup(trigger_pin, GPIO.OUT)
GPIO.setup(echo_pin, GPIO.IN)

# [서보모터 동작]
# GPIO 핀 설정
servo_pin = 13
GPIO.setmode(GPIO.BCM) # GPIO 핀들의 번호를 지정하는 규칙 설정
GPIO.setup(servo_pin, GPIO.OUT) # 출력으로 설정

# 서보 모터 초기화
servo = GPIO.PWM(servo_pin, 50) # 서보핀을 PWM 모드 50Hz로 사용
servo.start(0) # 초기값 0


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

# picam2.close()
# img_ori=cv2.imread('plate1.png')65노0887

# 전처리 된 이미지에서 글자를 추출하는 함수
def CarplateDetect() :
    # longest_idx, longest_text = -1, 0
    # plate_chars = []
   
    result_chars = ''
    # has_digit = False
   
    chars = pytesseract.image_to_string(test.img_result, lang='kor', config='--psm 7 --oem 0')    
    # --psm 7: 이미지 안의 글자가 한줄로 놓여있다 / --oem :0번 엔진(문자 그대로 한글자 한글자 읽음) 쓰겠다
   
    for c in chars:
        if ord('가') <= ord(c) <= ord('힣') or c.isdigit(): # 한글 잘 판별하도록 + 특수문자들 제거
            # if c.isdigit():
            #     has_digit = True
            result_chars += c

    conn = pymysql.connect(host='210.119.12.54', user='root', password='12345', db='miniproject', charset='utf8') #db연결
    query = 'SELECT name FROM car' #쿼리문
    cur = conn.cursor()
    cur.execute(query)
   
    # result = cur.fetchall()
    # 서보모터 동작 
    for row in cur:
        if result_chars == row[0]:
            duty = 0 / 18 + 2
            # print(True)
            # GPIO.output(servo_pin, True)
            # servo.ChangeDutyCycle(duty)
            # time.sleep(1)
            # GPIO.output(servo_pin, False)
            # servo.ChangeDutyCycle(0)

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

            start_time = time.time()
   
        else:
            continue

    print(result_chars) # 최종 결과

    # 입차시 DB INSERT
    # if (len(result_chars) == 7 or len(result_chars) == 8):
    #     now = time.strftime('%Y-%m-%d %H:%M:%S')
    #     query = f"INSERT INTO parking(CarName, EntranceTime) VALUES ('{result_chars}', '{now}')"
    #     cur.execute(query)

    #     conn.commit()
    # else:
    #     print('번호판 인식 에러!')

    # 출차시 DB DELETE
    # query = f"DELETE FROM parking WHERE CarName = '{result_chars}'" #쿼리문
    # cur = conn.cursor()
    # cur.execute(query)

    # conn.commit()


# 메인루프
try:
    while True:       
        distance = measure_distance() # 초음파 센서를 사용하여 거리 측정
        print(distance)

        if distance < 10 and distance >= 5: # 10보다 작은 경우
            metadata = picam2.capture_file("test1.jpg")
            print(metadata)
            CarplateDetect() # CarplateDetect 함수 실행
            time.sleep(2) # 1초 동안 대기
       
        else:
            continue

except KeyboardInterrupt:
    print("사용자에 의해 측정이 중지되었습니다")
    GPIO.cleanup()