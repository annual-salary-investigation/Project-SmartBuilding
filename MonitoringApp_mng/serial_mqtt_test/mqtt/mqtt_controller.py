# MQTT 패키지 설치 - paho-mqtt
# sudo pip install paho-mqtt
# 동시에 publish / subscribe

from threading import Thread, Timer
import time
import json
import datetime as dt

import paho.mqtt.client as mqtt

# DHT11 온습도센서용 라이브러리
import Adafruit_DHT as dht
# GPIO
import RPi.GPIO as GPIO

#DHT11, GPIO 관련 설정
sensor = dht.DHT11
rcv_pin = 10
servo_pin = 18
green = 22

GPIO.setwarnings(False)
GPIO.setmode(GPIO.BCM)
GPIO.setup(green, GPIO.OUT)
GPIO.setup(servo_pin, GPIO.OUT) 
GPIO.output(green, GPIO.HIGH) # == True 쓰는거랑 같음

pwm = GPIO.PWM(servo_pin, 100) # 속도
pwm.start(3) # 각도 0도 DutyCycle 3 ~20


# 데이터 보내는 객체
class publisher(Thread):
    def __init__(self):
        Thread.__init__(self) # 스레드 초기화
        self.host = '210.119.12.77'
        self.port = 1883
        self.clientID = 'IOT77'
        print('publisher 스레드 시작')
        self.client = mqtt.Client(client_id=self.clientID) #설계대로

    def run(self):
        self.client.connect(self.host, self.port)
        # self.client.username_pw_set() # id/pwd로 로그인 할 때 필요
        self.publish_data_auto()

    def publish_data_auto(self):
        humid, temp = dht.read_retry(sensor, rcv_pin)
        curr = dt.datetime.now().strftime('%Y-%m-%d %H:%M:%S') # 2023-06-14 10:40:12 형태의 포맷
        origin_data = { 'DEV_ID' : self.clientID,
                        'CURR_DT' : curr,
                        'TYPE' : 'TEMPHUMID',
                        'STAT' : f'{temp}|{humid}' } # sample data 
        pub_data = json.dumps(origin_data) # json 변환
        self.client.publish(topic='pknu/rpi/control/', payload=pub_data)
        print('Data published')
        Timer(2.0, self.publish_data_auto).start()

# 데이터 받아오는 객체
class subscriber(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.host = '210.119.12.77'
        self.port = 1883
        self.clientId = 'IOT77_SUB'
        self.topic = 'pknu/monitor/control/'
        print('subscriber 스레드 시작')
        self.client = mqtt.Client(client_id=self.clientId)

    def run(self):
        self.client.on_connect = self.onConnect # 접속 성공 시그널 처리
        self.client.on_message = self.onMessage # 접속 후 메세지가 수신되면 처리
        self.client.connect(self.host, self.port)
        self.client.subscribe(topic=self.topic)
        self.client.loop_forever()

    def onConnect(self, mqttc, obj, flags, rc):
        print(f'subscriber 연결됨 rc > {rc}')

    def onMessage(self, mqttc, obj, msg):
        rcv_msg = str(msg.payload.decode('utf-8'))
        # print(f'{msg.topic} / {rcv_msg}')
        data = json.loads(rcv_msg) # json 데이터로 형변환
        stat = data['STAT']
        print(f'현재 STAT : {stat}')
        if (stat == 'OPEN'):
            GPIO.output(green, GPIO.LOW)
            pwm.ChangeDutyCycle(12) # 90도
        elif (stat == 'CLOSE'):
            GPIO.output(green, GPIO.HIGH)
            pwm.ChangeDutyCycle(3) # 0도
        time.sleep(1.0)

if __name__ == '__main__':
    thPub = publisher() # publisher 객체 생성
    thSub = subscriber() # subscriber 객체생성
    thPub.start() # run() 자동실행
    thSub.start()