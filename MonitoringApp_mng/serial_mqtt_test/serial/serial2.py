import serial
import time

if __name__ == '__main__':
    ser = serial.Serial('/dev/ttyACM0', 9600, timeout=1) 
    # /dev/ttyACM0 : Serial device name for the Arduino
    # 9600 : Baud rate => 아두이노랑 같아야함!!
    ser.reset_input_buffer() 
    # serial.Serial()에서 리턴한 값을 ser 변수에 저장하고 reset_input_buffer()로  통신 초기의 이상한 데이터가 아닌 input buffer에 있는 값만 가져옴

    while True:
        ser.write(b"Hello from Raspberry Pi!\n")
        line = ser.readline().decode('utf-8').rstrip()
        print(line)
        time.sleep(1)