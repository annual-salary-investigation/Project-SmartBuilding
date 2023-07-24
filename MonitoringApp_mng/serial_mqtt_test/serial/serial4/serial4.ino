int red = 9;
int green = 11;
int blue = 10;

void powerOffAllLEDs()
{
  digitalWrite(red, LOW);
  digitalWrite(green, LOW);
  digitalWrite(blue, LOW);
}

void setup() {

  Serial.begin(9600);

  powerOffAllLEDs();
  
  pinMode(red, OUTPUT);
  pinMode(green, OUTPUT);
  pinMode(blue, OUTPUT);

  
}

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available() > 0 ) {
    
    powerOffAllLEDs();
    
    String data = Serial.readStringUntil('\n');
    Serial.print("You sent me: ");
    Serial.println(data);

    if(data.toInt() == 1) {
      digitalWrite(red, HIGH);
      delay(2000);
      digitalWrite(red, LOW);
      delay(2000);
    
      digitalWrite(green, HIGH);
      delay(2000);
      digitalWrite(green, LOW);
      delay(2000);
      
      digitalWrite(blue, HIGH);
      delay(2000);
      digitalWrite(blue, LOW);
      delay(2000);
    }
    else if(data.toInt() == 0) { powerOffAllLEDs(); };
  }
}
