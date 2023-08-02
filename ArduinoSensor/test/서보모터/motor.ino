#include <ArduinoJson.h>
#include <Servo.h>

Servo MyServo; // 서보 모터 객체 생성
int pos = 90;
void setup() {
  Serial.begin(9600); // 시리얼 통신 시작 (보레이트: 9600)
  pinMode(9,OUTPUT);
  MyServo.attach(9);
  MyServo.write(pos); // 서보 모터를 초기 위치로 설정 (90)

}

void loop() {
  if (Serial.parseInt()==1)
  {
    for (pos = 190; pos >= 90; pos -= 10)    // 서보모터릴 180도에서 90도로 이동
    {
      if(pos == 90){
        pos = 90;
        MyServo.write(pos); // 서보모터 180도로 고정
        delay(10);
      }
    }

  }
  else if(Serial.parseInt()==2)
  {
    for (pos = 90; pos <= 190; pos += 10)    // 위에 변수를 선언한 pos는 0, 180도보다 작다면 , 1도씩 더하고
    {
      if(pos == 180){
        pos = 180; 
        MyServo.write(pos); // 서보모터 90도로 고정
        delay(10);
      }
    }
  }
}






