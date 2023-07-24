
#define RED1 22
#define BLUE1 23
#define GREEN1 24
#define RED2 25
#define BLUE2 26
#define GREEN2 27
#define RED3 28
#define BLUE3 29
#define GREEN3 30


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


  pinMode(RED1, OUTPUT);
  pinMode(RED2, OUTPUT);
  pinMode(RED3, OUTPUT);
  pinMode(BLUE1, OUTPUT);
  pinMode(BLUE2, OUTPUT);
  pinMode(BLUE3, OUTPUT);
  pinMode(GREEN1, OUTPUT);
  pinMode(GREEN2, OUTPUT);
  pinMode(GREEN3, OUTPUT);
}

void loop()
{
// LED
  if (Serial.available() > 0) 
  {
    LED1powerOff();
    LED2powerOff();
    LED3powerOff();
    
    String command = Serial.readStringUntil('\n');
    
    if (command == "0") {
      LED1powerOff();
      Serial.println("LED1 OFF");
    }
    else if (command == "1") {
      digitalWrite(RED1, LOW);
      digitalWrite(BLUE1, LOW);
      digitalWrite(GREEN1, LOW);
      Serial.println("LED1 ON");
    }
    else if (command == "2") {
      LED2powerOff();
      Serial.println("LED2 OFF");
    }
    else if (command == "3") {
      digitalWrite(RED2, LOW);
      digitalWrite(BLUE2, LOW);
      digitalWrite(GREEN2, LOW);
      Serial.println("LED2 ON");
    }
    else if (command == "4") {
      LED3powerOff();
      Serial.println("LED3 OFF");
    }
    else if (command == "5") {
      digitalWrite(RED3, LOW);
      digitalWrite(BLUE3, LOW);
      digitalWrite(GREEN3, LOW);
      Serial.println("LED3 ON");
    }
  } 
}
