import serial
import time

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


if __name__ == '__main__':    
    while True:
        command = input("Enter command (1: ON, 0: OFF) : ")

        if command == '0':
            control_LED('0\n')
        elif command == '1':
            control_LED('1\n')
        elif command == '2':
            control_LED('2\n')
        elif command == '3':
            control_LED('3\n')        
        else:
            print("0, 1, 2, 3만 입력 가능")