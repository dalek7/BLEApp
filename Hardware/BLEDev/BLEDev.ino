#include <SoftwareSerial.h>
SoftwareSerial BTSerial(4, 5); 

int cnt;
void setup() {
  cnt = 0;
  BTSerial.begin(9600);
  Serial.begin(9600);
  pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {

  if (BTSerial.available()) {
    char c1 = BTSerial.read();
    Serial.write(c1);

      if (c1=='1')
      {
        Serial.println("On");
        BTSerial.print('1');
        digitalWrite(LED_BUILTIN, HIGH);
        cnt = 0;
      }
      else if (c1=='0')
      {
        Serial.println("Off");
        BTSerial.print('0');
        digitalWrite(LED_BUILTIN, LOW);
        cnt = 0;
      }
  }
  char buf1[10];
  sprintf(buf1, "%c", cnt++);
  BTSerial.print(buf1);

  if(cnt>255)
    cnt = 0;
  
  delay(10);
    
}