#include <Stepper.h> //외부전력

#define BTN 10
#define TRIG 12
#define ECHO 13
#define BUZ 11

const int shield = 2048;
const int guard = 560;
Stepper s_Stepper(shield, 6, 7, 8, 9);
Stepper g_Stepper(guard, 2, 3, 4, 5);

bool isClockwise = true;
int flag = 0;

void setup() {
  Serial.begin(9600);
  s_Stepper.setSpeed(15);
  g_Stepper.setSpeed(15);
  
  pinMode(BTN, INPUT_PULLUP);
  pinMode(TRIG, OUTPUT);
  pinMode(ECHO, INPUT);
  pinMode(BUZ, OUTPUT);
}

void loop() {
  int s_Value =  digitalRead(BTN);
  long duration, distance;

  digitalWrite(TRIG, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG, LOW);
  duration = pulseIn (ECHO, HIGH); //물체에 반사되어돌아온 초음파의 시간을 변수에 저장합니다.
  distance = duration * 17 / 1000;

  Serial.println(duration ); //초음파가 반사되어 돌아오는 시간을 보여줍니다.
  Serial.print("\nDIstance : ");
  Serial.print(distance); //측정된 물체로부터 거리값(cm값)을 보여줍니다.
  Serial.println(" Cm");
  delay(1000); //1초마다 측정값을 보여줍니다.j

  if (s_Value == LOW) {
    if (isClockwise) {
      tone(BUZ, 1000, 100); // 부저를 1000Hz 주파수로 0.1초 동안 울림
      Serial.println("LEFT");
      s_Stepper.step(shield*3);
    } else {
      tone(BUZ, 1000, 100); // 부저를 1000Hz 주파수로 0.1초 동안 울림
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

// 안전벽
    if(distance <20)
  {
    tone(BUZ, 1000, 3000);
  }

  if(flag == 0 && distance < 20)
  {
     g_Stepper.step(guard);
     flag = 1;
  }

  if(flag == 1 && distance > 20)
  {
    g_Stepper.step(-guard);
    flag = 0;
  }  
}