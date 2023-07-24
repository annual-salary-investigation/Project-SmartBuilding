int red = 9;
int green = 11;
int blue = 10;


void setup() {
  
  pinMode(red, OUTPUT);
  pinMode(green, OUTPUT);
  pinMode(blue, OUTPUT);
  
}

void loop() {
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
