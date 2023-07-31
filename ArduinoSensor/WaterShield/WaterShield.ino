#include <Stepper.h> //외부전력

#define BTN 8

const int shield = 2048;
Stepper s_Stepper(shield, 2, 3, 4, 5);

bool isClockwise = true;
int flag = 0;

void setup() {
  Serial.begin(9600);
  s_Stepper.setSpeed(15);
  
  pinMode(BTN, INPUT_PULLUP);
}

void loop() {
  Serial.println(digitalRead(BTN));
  if (digitalRead(BTN) == LOW) {
    if (isClockwise) {
      Serial.println("LEFT");
      s_Stepper.step(shield*3);
    } else {
      Serial.println("RIGHT");
      s_Stepper.step(-shield*3);
    }
    delay(500);
    isClockwise = !isClockwise;

    // 버튼이 눌린 상태에서 버튼이 떼질 때까지 대기
    while (digitalRead(BTN) == LOW) {
      delay(100);
    }
  }
}