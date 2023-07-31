import serial
import time
import json
from threading import Thread, Timer
import paho.mqtt.client as mqtt

'''
라즈베리파이 -> 윈도우 MQTT broker 연결 설정
1. mosquitto/mosquitto.conf 에서 listener 1883 / allow anonymous true 로 바꿔주기
2. 윈도우 방화벽 설정 확인 : 인바운드 규칙 추가해서  TCP 1883 포트 허용
'''

# 아두이노와 시리얼 통신 설정 - 포트 열기
ser = serial.Serial('/dev/ttyACM0', 9600, timeout=1) 
    # /dev/ttyACM0 : Serial device name for the Arduino
    # 9600 : Baud rate => 아두이노랑 같아야함!!

# 아두이노로 신호 보내서 센서 제어
def control_LED(data):
    ser.write(data.encode()) # 아두이노로 데이터 전송
    time.sleep(1)

def control_FAN(data):
    ser.write(data.encode())
    time.sleep(1)
    

# MQTT - WPF로 데이터 보내는 객체
class publisher(Thread):
    def __init__(self):
        Thread.__init__(self) # 스레드 초기화
        self.host = '210.119.12.77'
        self.port = 1883
        self.clientID = 'test_PUB'
        print('publisher 스레드 시작')
        self.client = mqtt.Client(client_id=self.clientID) 

    def run(self):
        self.client.connect(self.host, self.port)
        self.publish_data_auto()

    def publish_data_auto(self):
        while True:
            try:
                response = ser.readline().decode('UTF-8').strip() # 아두이노로부터 응답받기
                print(response)                
                temp_part = response.split('|')[0]
                humid_part = response.split('|')[1]
                fire_part = response.split('|')[2]
                temp = float(temp_part)
                humid = float(humid_part)
                fire = int(fire_part)

                origin_data = {"Temp": temp, "Humid": humid, "Fire": fire} # 아두이노에서 받은 데이터 제이슨 형태로 저장
                pub_data = json.dumps(origin_data) # 제이슨 변환

                self.client.publish(topic='pknu/rpi/control/', payload=pub_data) # pknu/rpi/contro/ 토픽으로 전송 - WPF 에서도 이 토픽으로 구독해야 값 받을 수 있음!
                print('Data published')                

            except Exception as e:
                print(f'Other Error : {e.args}')
           

# MQTT - WPF에서 데이터 받아오는 객체
class subscriber(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.host = '210.119.12.77'
        self.port = 1883
        self.clientId = 'test_SUB'
        self.topic = 'pknu/rpi/control/sensor' # pknu/rpi/contror/sensor 토픽 구독 - WPF 에서 이 토픽으로 발행하는 값 받아올 수 있음!
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

    def onMessage(self, mqttc, obj, msg):   # MQTT 구독
        message = msg.payload.decode("utf-8")
        print(message)
        if message == "0":
            control_LED('0\n')  # LED1 끄기
        elif message == "1":
            control_LED('1\n')   # LED1 켜기
        elif message == "2":
            control_LED('2\n')   # LED2 끄기
        elif message == "3":
            control_LED('3\n')   # LED2 켜기
        elif message == "4":
            control_LED('4\n')   # LED3 끄기
        elif message == "5":
            control_LED('5\n')   # LED3 켜기
        
        elif message == "6":    # 팬모터 켜기
            control_FAN('6\n')
        elif message == "7":    # 팬모터 끄기
            control_FAN('7\n')

if __name__ == '__main__':      
    thPub = publisher() # publisher 객체 생성
    thSub = subscriber() # subscriber 객체생성
    thPub.start() # run() 자동실행
    thSub.start()
