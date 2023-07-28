#include <Stepper.h>
const int stepsPerRevolution = 560;
Stepper myStepper(stepsPerRevolution, 3, 4, 5, 6);
int flag = 0;

#define TRIG 12 //TRIG 핀 설정 (초음파 보내는 핀)
#define ECHO 13 //ECHO 핀 설정 (초음파 받는 핀)
#define BUZ 8 // 부저를 연결한 핀 번호

void setup() {
  myStepper.setSpeed(15);
  Serial.begin(9600);
  pinMode(TRIG, OUTPUT);
  pinMode(ECHO, INPUT);
  pinMode(BUZ, OUTPUT);
}
void loop()
{
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
j
  if(distance <10)j
  {j
    tone(BUZ, 1000, 3000);
  }

  if(flag == 0 && distance < 10)
  {
     myStepper.step(stepsPerRevolution);
     tone(BUZ, 1000, 3000);
     flag = 1;
  }

  if(flag == 1 && distance > 10)
  {
    myStepper.step(-stepsPerRevolution);
    flag = 0;
  }
}

