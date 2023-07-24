import serial
import time
import json
from threading import Thread, Timer
import paho.mqtt.client as mqtt

# 아두이노와 시리얼 통신 설정
ser = serial.Serial('/dev/ttyACM0', 9600, timeout=1) 
    # /dev/ttyACM0 : Serial device name for the Arduino
    # 9600 : Baud rate => 아두이노랑 같아야함!!

# 아두이노로 신호 보내서 LED 제어
def control_LED(data):
    ser.write(data.encode()) # 아두이노로 데이터 전송
    time.sleep(1)
    response = ser.readline().decode().strip() # 아두이노로부터 응답받기
    print(f"Response: {response}")

'''
라즈베리파이 -> 윈도우 MQTT broker 연결 설정
1. mosquitto/mosquitto.conf 에서 listener 1883 / allow anonymous true 로 바꿔주기
2. 윈도우 방화벽 설정 확인 : 인바운드 규칙 추가해서  TCP 1883 포트 허용
'''

# WPF로 데이터 보내는 객체
class publisher(Thread):
    def __init__(self):
        Thread.__init__(self) # 스레드 초기화
        self.host = '192.168.124.102'
        self.port = 1883
        self.clientID = 'test_PUB'
        print('publisher 스레드 시작')
        self.client = mqtt.Client(client_id=self.clientID) #설계대로

    def run(self):
        self.client.connect(self.host, self.port)
        self.publish_data_auto()

    def publish_data_auto(self):
        response = ser.readline().decode().strip() # 아두이노로부터 응답받기 
        pub_data = json.dumps(response) # json 변환
        self.client.publish(topic='pknu/rpi/control/', payload=pub_data)
        print('Data published')
        Timer(2.0, self.publish_data_auto).start() # Timer로 2초에 한 번씩 publish_data_auto 실행해서 MQTT broker로 데이터 전송

# WPF에서 데이터 받아오는 객체
class subscriber(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.host = '192.168.124.102'
        self.port = 1883
        self.clientId = 'test_SUB'
        self.topic = 'pknu/rpi/control/'
        print('subscriber 스레드 시작')
        self.client = mqtt.Client(client_id=self.clientId)

    def run(self):
        self.client.on_connect = self.onConnect # 접속 성공 시그널 처리
        self.client.on_message = self.onMessage # 접속 후 메세지가 수신되면 처리
        self.client.connect(self.host, self.port)
        self.client.subscribe(topic=self.topic)
        self.client.loop_forever() # 현재 스레드에서 무한루프 돌면서 MQTT 브로커와 통신 유지, 새로운 메세지 도착하면 콜백함수 호출해서 해당 메세지 처리

    def onConnect(self, mqttc, obj, flags, rc):
        print(f'subscriber 연결됨 rc > {rc}')

    def onMessage(self, mqttc, obj, msg):
        message = msg.payload.decode("utf-8")
        if message == "1":
            control_LED('1\n')  # LED 켜기
        elif message == "0":
            control_LED('0\n')   # LED 끄기

if __name__ == '__main__':    
    thPub = publisher() # publisher 객체 생성
    thSub = subscriber() # subscriber 객체생성
    thPub.start() # run() 자동실행
    thSub.start()
    