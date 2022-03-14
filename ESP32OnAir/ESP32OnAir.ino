/*

  OnAir is an ESP32 app that should allow the OnAir sign to switch on and off. Since the device is very simple 
  implementation is done on the basis of switching the device on into a known state then being able to 
  activate the manual switch to two mode - full on or pulsing.   We implemt on the basis of a simple REST based call 
  once connected to the WIFI. Can also be used as a direct connection from the app.
  
*/

// constants won't change. They're used here to set pin numbers:
const int buttonPin = 2;     // the number of the pushbutton pin
const int ledPin =  13;      // the number of the LED pin
const int onPin =  21;      // the number of the LED pin
const int switchPin =  12;      // the number of the LED pin
#define BOUNCE_DELAY 500

// variables will change:
int buttonState = 0;         // variable for reading the pushbutton status

int incomingByte = 0; // for incoming serial data

void setup() {
  // initialize the LED pin as an output:
  pinMode(ledPin, OUTPUT);
  pinMode(onPin, OUTPUT);
  pinMode(switchPin, OUTPUT);
  // initialize the pushbutton pin as an input:
  pinMode(buttonPin, INPUT);

  Serial.begin(115200); // opens serial port, sets data rate to 9600 bps
  digitalWrite(ledPin, LOW);
  digitalWrite(onPin, LOW);
  digitalWrite(switchPin, LOW);
  
}

void loop() {

    if (Serial.available() > 0) {
      // read the incoming byte:
      incomingByte = Serial.read();

      if (incomingByte == 48 ) {
        // Switch off 0
        Serial.println("OK");  
        digitalWrite(ledPin, LOW);
        digitalWrite(onPin, LOW);
        digitalWrite(switchPin, LOW);
      }
      else if (incomingByte == 49 ) {
        Serial.println("OK");  
        digitalWrite(onPin, LOW);
        delay(100);
        digitalWrite(onPin, HIGH);
        digitalWrite(ledPin, HIGH);
 
        delay(BOUNCE_DELAY);
        digitalWrite(switchPin, HIGH);
        delay(BOUNCE_DELAY);
        digitalWrite(switchPin, LOW);

        
        
      }
       else if (incomingByte == 50 ) {

        Serial.println("OK");  
        digitalWrite(onPin, LOW);
        delay(BOUNCE_DELAY);
        digitalWrite(onPin, HIGH);
        digitalWrite(ledPin, HIGH);
 
        delay(BOUNCE_DELAY);
        digitalWrite(switchPin, HIGH);
        delay(BOUNCE_DELAY);
        digitalWrite(switchPin, LOW);

        delay(BOUNCE_DELAY);
        digitalWrite(switchPin, HIGH);
        delay(BOUNCE_DELAY);
        digitalWrite(switchPin, LOW);
       } else if (incomingByte == 57 ) {

        // Alive - do nuffin
        Serial.println("OK");  
        
       } else
       {
        Serial.println("ERROR");  
        }
       
    }
    // read the state of the pushbutton value:
    
  
}
