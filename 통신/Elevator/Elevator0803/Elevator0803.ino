#include <Stepper.h>
#include <time.h>

int stepsPerRev = 2048;
Stepper stepper (stepsPerRev, A0,A1,A2,A3); // ( IN4,IN2,IN3,IN1)
int btn1 = 2;
int btn2 = 3;
int btn3 = 4;
int btn4 = 5;
int floors = 1; //현재 위치
int move = 0;
int num = 0;

int led_pin[] = {6, 7, 8, 9, 10, 11, 12};  // A,B,C,D,E,F,G

// 각 숫자에 대한 LED 설정 값을 정의합니다.
// 숫자에 매칭되는 LED의 로직레벨을 LOW(0) 상태로 설정합니다.
int set_number[4][7] = {
    {0,1,1,0,0,0,0}, //1
    {1,1,0,1,1,0,1}, //2
    {1,1,1,1,0,0,1}, //3
    {0,1,1,0,0,1,1}, //4
};

void callelev()
{
  move = floors - 3;

    stepper.step(stepsPerRev*move*+7);
    floors=3;
    delay(1000);
   
    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[2][i]);
    }
   
    delay(500);
    Serial.println("3 실행");
}

void setup() {
  Serial.begin(9600);
  stepper.setSpeed(15);

  for (int i = 0 ; i < 8; i ++)
  {
    pinMode(led_pin[i], OUTPUT);
  }

  pinMode(btn1, INPUT_PULLUP);    //raspberry pi 에서는 GPIO 역활이다.
  pinMode(btn2, INPUT_PULLUP);
  pinMode(btn3, INPUT_PULLUP);
  pinMode(btn4, INPUT_PULLUP);

  for (int i=0; i < 8; i++)
  {
    digitalWrite(led_pin[i], set_number[0][i]);
  }
}

void loop()
{
  boolean btn1HL = digitalRead(btn1); // 1층
  boolean btn2HL = digitalRead(btn2);
  boolean btn3HL = digitalRead(btn3);
  boolean btn4HL = digitalRead(btn4);

  if (btn1HL == LOW)
  { // 1층 버튼
    move = floors - 1;    // 현재층 -1 => 4층 일때 -1  3층만큼 움직임
   
    stepper.step(stepsPerRev*move*+7);
    floors=1;
    //btn1HL==HIGH;
    delay(1000);
   
    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[0][i]);
    }
   
    delay(500);
    Serial.println("1 실행");
  }

  if (btn2HL == LOW)
  { // 2층 버튼
   
    move = floors - 2;
   
    stepper.step(stepsPerRev*move*+7);
    delay(500);

    floors=2;
    //btn2HL==HIGH;
    delay(1000);
   
    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[1][i]);
    }
     
    delay(500);
    Serial.println("2 실행");
  }

  if (btn3HL == LOW)
  {
    move = floors - 3;

    stepper.step(stepsPerRev*move*+7);
    floors=3;
    delay(1000);
   
    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[2][i]);
    }
   
    delay(500);
    Serial.println("3 실행");
  }

  if (btn4HL == LOW)
  {
    move = floors - 4;

    stepper.step(stepsPerRev*move*+7);
    floors=4;
    delay(1000);

    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[3][i]);
    }
   
    delay(500);
    Serial.println("4 실행");
  }
 
  Serial.println(floors);
  delay(1000);

  if (Serial.available() > 0)
  {
    String command = Serial.readStringUntil('\n');
    Serial.println("호출");
    Serial.println(command);
   
    if (command == "8")
    {
      callelev();
    }
  }
}