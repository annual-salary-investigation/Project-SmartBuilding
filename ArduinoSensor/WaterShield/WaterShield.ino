#include <Stepper.h>
const int stepsPerRevolution = 2048;
Stepper myStepper(stepsPerRevolution, 3, 4, 5, 6);
const int btnPin = 7;
const int buzzerPin = 8; // 부저를 연결한 핀 번호

bool isClockwise = true;

void setup() {
  myStepper.setSpeed(15);
  Serial.begin(9600);
  pinMode(btnPin, INPUT_PULLUP);
  pinMode(buzzerPin, OUTPUT); // 부저 핀을 출력으로 설정
}

void loop() {
  if (digitalRead(btnPin) == LOW) {
    if (isClockwise) {
      tone(buzzerPin, 1000, 1000); // 부저를 1000Hz 주파수로 0.1초 동안 울림
      Serial.println("LEFT");
      myStepper.step(stepsPerRevolution*4);
    } else {
      tone(buzzerPin, 1000, 1000); // 부저를 1000Hz 주파수로 0.1초 동안 울림
      Serial.println("RIGHT");
      myStepper.step(-stepsPerRevolution*4);
    }
    delay(500);
    isClockwise = !isClockwise;

    // 엘리자를 위한 알림 소리 울리기

    // 버튼이 눌린 상태에서 버튼이 떼질 때까지 대기
    while (digitalRead(btnPin) == LOW) {
      delay(10);
    }
  }
}