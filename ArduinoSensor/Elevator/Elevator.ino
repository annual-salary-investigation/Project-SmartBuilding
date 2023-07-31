#include <Stepper.h>
#include <time.h>

int led_pin[] = {6, 7, 8, 9, 10, 11, 12};  // A,B,C,D,E,F,G

// 각 숫자에 대한 LED 설정 값을 정의합니다.
// 숫자에 매칭되는 LED의 로직레벨을 LOW(0) 상태로 설정합니다.
int set_number[4][7] = {
    {0,1,1,0,0,0,0}, //1
    {1,1,0,1,1,0,1}, //2
    {1,1,1,1,0,0,1}, //3
    {0,1,1,0,0,1,1}, //4
};

int stepsPerRev = 2048;
Stepper stepper (stepsPerRev, A0,A1,A2,A3); // ( IN4,IN2,IN3,IN1)
int btn1 = 2;
int btn2 = 3;
int btn3 = 4;
int btn4 = 5;
int floors = 4; //현재 위치
int move = 0;
int num = 0;



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
    digitalWrite(led_pin[i], set_number[3][i]);
  }
}

void loop() {
  boolean btn1HL = digitalRead(btn1);
  boolean btn2HL = digitalRead(btn2);
  boolean btn3HL = digitalRead(btn3);
  boolean btn4HL = digitalRead(btn4);

  if (btn1HL == LOW) { // 버튼을 누를때 0
    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[0][i]);
    }
    move = floors - 1;    // 움직인다 = 현제층에서 - 1
    Serial.print(move*-1);
    Serial.println("개층 이동합니다.");
    Serial.println(stepsPerRev*move*-10);
    stepper.step(stepsPerRev*move*-7);  // 스탭모터움직임(1바퀴) X 움직인다 X 7
    floors=1;
    btn1HL==HIGH;
    delay(1000);
    Serial.print(floors);
    Serial.println("층입니다.");
    delay(500);
  }

  else if (btn2HL == LOW) {
    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[1][i]);
    }
    move = floors - 2;
    Serial.print(move*-1);
    Serial.println("개층 이동합니다.");
    Serial.println(stepsPerRev*move*-10);

    stepper.step(stepsPerRev*move*-7);
    delay(500);

    floors=2;
    btn2HL==HIGH;
    delay(1000);
    Serial.print(floors);
    Serial.print("층입니다");
    delay(500);
   
  }

  else if (btn3HL == LOW)
  {
    for (int i=0; i < 8; i++)
    {
      digitalWrite(led_pin[i], set_number[2][i]);
    }
    move = floors - 3;
    Serial.print(move*-1);
    Serial.println("개층 이동합니다.");
    Serial.println(stepsPerRev*move*-10);

    stepper.step(stepsPerRev*move*-7);
    floors=3;
    btn3HL==HIGH;
    delay(1000);
    Serial.print(floors);
    Serial.print("층입니다");
    delay(500);

  }

  else if (btn4HL == LOW)
  {
    for (int i=0; i < 8; i++)
    {  // 7segments 동작
      digitalWrite(led_pin[i], set_number[3][i]);
    }
    move = floors - 4;
    Serial.print(move*-1);
    Serial.println("개층 이동합니다.");
    Serial.println(stepsPerRev*move*-10);

    stepper.step(stepsPerRev*move*-7);
    floors=4;
    btn4HL==HIGH;
    delay(1000);
    Serial.print(floors);
    Serial.print("층입니다");
    delay(500);
  }

}
//   선입선출x 정렬o. (층을 누르고 진행중에 누르는 버튼을 누르면 배엘에 다음 동작을 추가하는식 [1, 4, 2, 3]) 버블정렬.
//   현제 깃허브 업로드가 안된다. 23년 7 월 5일 9시 45분 오류가 뜬다.



//앞으로 나아가야할 방향.
//  아두이노 -> 라즈베리파이 -> wpf  통신을 할 수 있어야 한다.
//  왜냐하면 엘리베이터를 집에서나, 마트에서나, 캠핑장에서나, 미리 호출 할 수 있어야 하기 때문이다.
//  특히 집이 가장 중요한데, 출근할때 미리 호출해서 바쁜출근시간을 단축시킨다는 컨셉이다.
//  비슷한 컨셉으로 마트가 다음으로 중요하다. 캠핑장에서 또는 집, 사무실에서 주문한 식자제들 배달할때 상품을 담는동안 엘리베이터를 미리 호출하는 컨셉이다.    

// 당장 내일 해야할 것
// 1. 오류수정
// 2. 파이썬에서 wpf(C#)으로 데이터 보내는 방법을 알아야 한다.(+SQL <-> 파이 연동)
// 만약 2번이 되면 UI와 하드디스크적인 부분 해결만 남았다고 볼 수 있다.