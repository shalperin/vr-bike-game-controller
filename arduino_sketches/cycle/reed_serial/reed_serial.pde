int ledPin = 13;
int switchPin = 2;
int val;

void setup() {
  beginSerial(9600);
  pinMode(ledPin, OUTPUT);
  pinMode(switchPin, INPUT);
}

void loop() {  
  val = digitalRead(switchPin);
  
  if (val == LOW) {
    digitalWrite(ledPin, LOW);
  }
  if (val == HIGH) {
    digitalWrite(ledPin, HIGH);
    Serial.println('1');  
  }
}
