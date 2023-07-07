#!/usr/bin/env python3
import serial
import paho.mqtt.client as mqtt
import json
import time
import datetime as dt
import uuid
from collections import OrderedDict

count = 0
datas = []
dev_id = 'HOME01'
broker_address = '210.119.12.85'
port = 1883
topic = 'building/home/data/'

try:
    client2 = mqtt.Client('HomeClient')
    client2.connect(host=broker_address, port=port)
    print('MQTT 연결 성공~!')
except:
    client2 = None
    print('MQTT 연결 실패~ ㅜㅜ')


if __name__ == '__main__':
    ser = serial.Serial('/dev/ttyACM0', 9600, timeout=1)
    ser.reset_input_buffer()

    while True:
        if ser.in_waiting > 0:
            line = ser.readline().decode('utf-8').rstrip()
            if (line.strip() != ''):
                datas.append(line)                

                if (count % 2 != 0):
                    print(datas)
                    # mqtt 전송하면 됨
                    currtime = dt.datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                    humid, temp = (datas[0], datas[1])
                    raw_data = OrderedDict()
                    raw_data['dev_id'] = dev_id
                    raw_data['time'] = currtime
                    raw_data['temp'] = temp
                    raw_data['humid'] = humid

                    pub_data = json.dumps(raw_data, ensure_ascii=False, indent='\t')
                    print(dev_id, pub_data)
                    datas.clear()
                    client2.publish(topic, pub_data)                    

                count += 1
