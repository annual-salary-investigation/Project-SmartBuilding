#include "DHT.h"
#include <Stepper.h>

// 온습도 센서 
#define DHTTYPE DHT22
#define DHTPIN 2 
DHT dht(DHTPIN, DHTTYPE);

// LED 
#define RED1 22
#define BLUE1 23
#define GREEN1 24
#define RED2 25
#define BLUE2 26
#define GREEN2 27
#define RED3 28
#define BLUE3 29
#define GREEN3 30

// 팬모터
#define INA 3
#define INB 4

// 조도센서
#define ILLUMI A0

// 화재감지센서
#define FLAME 5

// 부저
#define BUZ 6

int flag = 0;
int flag1 = 0;
const int stepsPerRevolution = 2048;
Stepper myStepper(stepsPerRevolution, 8, 9, 10, 11);
uint8_t success;

void LED1powerOff()
{ // LED common anode라서 high가 비활성화 low가 활성화임 
  digitalWrite(RED1, HIGH);
  digitalWrite(BLUE1, HIGH);
  digitalWrite(GREEN1, HIGH);
}

void LED2powerOff()
{ // LED common anode라서 high가 비활성화 low가 활성화임 
  digitalWrite(RED2, HIGH);
  digitalWrite(BLUE2, HIGH);
  digitalWrite(GREEN2, HIGH);
}

void LED3powerOff()
{ // LED common anode라서 high가 비활성화 low가 활성화임 
  digitalWrite(RED3, HIGH);
  digitalWrite(BLUE3, HIGH);
  digitalWrite(GREEN3, HIGH);
} 

void setup()
{
  Serial.begin(9600); // 라즈베리파이랑 시리얼 통신

  dht.begin();

  myStepper.setSpeed(14);

  pinMode(FLAME, INPUT);
  pinMode(BUZ, OUTPUT);

  pinMode(RED1, OUTPUT);
  pinMode(RED2, OUTPUT);
  pinMode(RED3, OUTPUT);
  pinMode(BLUE1, OUTPUT);
  pinMode(BLUE2, OUTPUT);
  pinMode(BLUE3, OUTPUT);
  pinMode(GREEN1, OUTPUT);
  pinMode(GREEN2, OUTPUT);
  pinMode(GREEN3, OUTPUT);
  
  pinMode(INA, OUTPUT);
  pinMode(INB, OUTPUT);

  // LED 전부 끄고 시작
  LED1powerOff();
  LED2powerOff();
  LED3powerOff();
}

void loop()
{  
  float humidity = dht.readHumidity();
  float temperature = dht.readTemperature();
  float f = dht.readTemperature(true);

  int sensor = analogRead(ILLUMI);
  
  int state = digitalRead(FLAME);
  noTone(BUZ);

  // 온습도
  if (isnan(temperature) || isnan(humidity))
  {
    Serial.println("0.0|0.0");
  }
  else
  {
    Serial.print(temperature);
    Serial.print("|");
    Serial.print(humidity);
    Serial.print("|");    
  }

  // 25도 이상이면 팬모터 자동 작동
  if(flag = 0 && temperature >= 25)
  {
    FanpowerOn();
    flag=1;
  }

  
  // LED
  if (Serial.available() > 0) 
  {
    String command = Serial.readStringUntil('\n');
    
    if (command == "0") {
      LED1powerOff();
    }
    else if (command == "1") {
      digitalWrite(RED1, LOW);
      digitalWrite(BLUE1, LOW);
      digitalWrite(GREEN1, LOW);
    }
    else if (command == "2") {
      LED2powerOff();
    }
    else if (command == "3") {
      digitalWrite(RED2, LOW);
      digitalWrite(BLUE2, LOW);
      digitalWrite(GREEN2, LOW);
    }
    else if (command == "4") {
      LED3powerOff();
    }
    else if (command == "5") {
      digitalWrite(RED3, LOW);
      digitalWrite(BLUE3, LOW);
      digitalWrite(GREEN3, LOW);
    }
    else if(command == "6")
    {
      FanpowerOff();
    }
    else if(command == "7")
    {
      FanpowerOn();
    }
  }

  //화재
  if (state == 1)
  {
    digitalWrite(RED1, LOW);
    digitalWrite(RED2, LOW);
    digitalWrite(RED3, LOW);
    digitalWrite(BLUE1, HIGH);
    digitalWrite(BLUE2, HIGH);
    digitalWrite(BLUE3, HIGH);
    digitalWrite(GREEN1, HIGH);
    digitalWrite(GREEN2, HIGH);
    digitalWrite(GREEN3, HIGH);
    
    Serial.println("1");
    for(int i = 0; i<5; i++)
    {
      tone(BUZ, 500, 100);
      delay(1000);    
    }
  }
  else
  {
    Serial.println("0");
    
    noTone(BUZ);
  }

  // 블라인드
  if(flag == 0 and sensor < 300)
  {
    myStepper.step(stepsPerRevolution);
    flag = 1;
  }
  else if(flag == 1 and sensor > 300)
  {
    myStepper.step(-stepsPerRevolution);
    flag = 0;
  }
 

  delay(2000);
}

// LOW 활성화 HIGH 비활성화
void FanpowerOn()
{
  digitalWrite(INA, LOW);
  digitalWrite(INB, HIGH);
  flag = 1;
}

void FanpowerOff()
{
  digitalWrite(INA, HIGH);
  digitalWrite(INB, HIGH);
  flag = 1; // 팬모터 자동 켜짐 조건이 flag 0 이 있어서 여기서 flag 0으로 해버리면 자동 작동 중에 버튼으로 끄더라도 다시 켜짐
  // 이거는 동작시켜보고 더 적절한 방식으로~ 개인적으로는 강제로 끌 수 있어야 한다고 생각해서 1로 했음!!
}
