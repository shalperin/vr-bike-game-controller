// Flash an LED when the reed switch triggers.
// based on Jeremy Blum tutorial 2: http://youtu.be/_LCCGFSMOr4
// and on Arduino button tutorial: http://arduino.cc/en/Tutorial/Button

int switchPin = 8;
int ledPin = 13;
boolean lastButton = LOW;
boolean ledOn = false;
boolean currentButton = LOW;

void setup() {
  pinMode(switchPin, INPUT);
  pinMode(ledPin, OUTPUT);
}


boolean debounce(boolean last) {
  boolean current = digitalRead(switchPin);
  if (last != current) {
    delay(5);
    current = digitalRead(switchPin);
  }
  return current;
}

void loop() {
  currentButton = debounce(lastButton);
  if (lastButton == LOW && currentButton == HIGH) {
    digitalWrite(ledPin, HIGH);
    delay(200);
    digitalWrite(ledPin, LOW);
  }  
  lastButton = currentButton;
}


