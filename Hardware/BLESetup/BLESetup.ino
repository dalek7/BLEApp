#include <SoftwareSerial.h>

SoftwareSerial BTSerial(4, 5); 

void setup()
{
  Serial.begin(9600);
  BTSerial.begin(9600);
  Serial.println("Hello! 20190801");
  BTSerial.write("AT?NAME");
}

void loop()
{
  
  if (BTSerial.available()) {
    Serial.write(BTSerial.read());
  }
  
  
  if (Serial.available()) {
    BTSerial.write(Serial.read());
  }
}
