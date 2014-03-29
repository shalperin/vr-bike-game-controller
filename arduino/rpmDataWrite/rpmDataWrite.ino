//BLUM tutorial 2

int switchPin = 8;
int ledPin = 13;
boolean lastButton = LOW;
boolean ledOn = false;
boolean currentButton = LOW;
long lastTime = 0;
long INF = 99999999;

void setup() {
  Serial.begin(9600);
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

long getDeltaTimeAndUpdateTimer() {
  long currentTime = millis();
  long deltaTime = currentTime - lastTime;
  lastTime = currentTime;
  return deltaTime;
}

  
void loop() {
  long currentTime = millis();

  currentButton = debounce(lastButton);
  if (lastButton == LOW && currentButton == HIGH) {
    digitalWrite(ledPin, HIGH);
    Serial.println(getDeltaTimeAndUpdateTimer());
    delay(200);
    digitalWrite(ledPin, LOW);
  }  else if (currentTime - lastTime > 2500) {
    Serial.println(INF);
    lastTime = currentTime;
  }
  
  lastButton = currentButton;
}


