int red = 9;
int blue = 12;
int green = 13;

void powerOff()
{ // LED common anode라서 high가 비활성화 low가 활성화임 
  digitalWrite(red, HIGH);
  digitalWrite(blue, HIGH);
  digitalWrite(green, HIGH);
}
void setup() {
  Serial.begin(9600);

  powerOff();

  pinMode(red, OUTPUT);
  pinMode(blue, OUTPUT);
  pinMode(green, OUTPUT);

}

void loop() {
  
  if (Serial.available() > 0) 
  {
    powerOff();
    
    String command = Serial.readStringUntil('\n');
    
    if (command == "0") {
      powerOff();
      Serial.println("LED OFF");
    }
    else if (command == "1") {
      digitalWrite(red, LOW);
      digitalWrite(blue, HIGH);
      digitalWrite(green, HIGH);
      Serial.println("RED ON");
    }
    else if (command == "2") {
      digitalWrite(red, HIGH);
      digitalWrite(blue, LOW);
      digitalWrite(green, HIGH);
      Serial.println("BLUE ON");
    }
    else if (command == "3") {
      digitalWrite(red, HIGH);
      digitalWrite(blue, HIGH);
      digitalWrite(green, LOW);
      Serial.println("GREEN ON");
    }
  }

}
