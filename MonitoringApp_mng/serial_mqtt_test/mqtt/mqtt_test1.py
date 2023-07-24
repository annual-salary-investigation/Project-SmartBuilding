import paho.mqtt.client as mqtt

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("연결")
    else:
        print(f"연결 실패 : {rc}")


# MQTT클라이언트 생성
client = mqtt.Client()

# 브로커 연결 상태 콜백 함수 설정
client.on_connect = on_connect

# 브로커에 연결 시도
client.connect("192.168.124.102", 1883)

# 연결처리 네트워크 루프 실행
client.loop_forever()