/*

  OnAir is an ESP32 app that should allow the OnAir sign to switch on and off. Since the device is very simple 
  implementation is done on the basis of switching the device on into a known state then being able to 
  activate the manual switch to two mode - full on or pulsing.   We implemt on the basis of a simple REST based call 
  once connected to the WIFI. Can also be used as a direct connection from the app.
  
*/

#include <Arduino.h>

#include <WiFiManager.h> // https://github.com/tzapu/WiFiManager

#include <WebServer.h>

#include <ArduinoJson.h>

// constants won't change. They're used here to set pin numbers:
const int ledPin = 13; // the number of the LED pin
const int onPin = 21; // the number of the LED pin
const int switchPin = 12; // the number of the LED pin
#define BOUNCE_DELAY 500 // The time between emulation of the push switch 

int incomingByte = 0; // for incoming serial data
boolean wifiConnected = false;

WebServer server(80);

//WiFiManager, Local intialization. Once its business is done, there is no need to keep it around
WiFiManager wm;

// JSON data buffer
StaticJsonDocument < 250 > jsonDocument;
char buffer[250];

void setup() {

  WiFi.mode(WIFI_STA); // explicitly set mode, esp defaults to STA+AP
  // it is a good practice to make sure your code sets wifi mode how you want it.

  // initialize the hardware items - two GPIO pins - relay and push switch controls.   
  pinMode(ledPin, OUTPUT);
  pinMode(onPin, OUTPUT);
  pinMode(switchPin, OUTPUT);

  digitalWrite(ledPin, LOW);
  digitalWrite(onPin, LOW);
  digitalWrite(switchPin, LOW);

  Serial.begin(115200); // opens serial port, sets data rate to 9600 bps

  // reset settings - wipe stored credentials for testing
  // these are stored by the esp library
  //wm.resetSettings();

  // Automatically connect using saved credentials,
  // if connection fails, it starts an access point with the specified name ( "AutoConnectAP"),
  // if empty will auto generate SSID, if password is blank it will be anonymous AP (wm.autoConnect())
  // then goes into a blocking loop awaiting configuration and will return success result

  bool res;
  // res = wm.autoConnect(); // auto generated AP name from chipid
  // res = wm.autoConnect("AutoConnectAP"); // anonymous ap
  res = wm.autoConnect("OnAirWifi", "password"); // password protected ap

  if (!res) {
    Serial.println("Failed to connect- serial comms only");
    // ESP.restart();
  } else {
    //if you get here you have connected to the WiFi    
    Serial.println("OnAir connected to external Wifi :)");
    wifiConnected = true; // We have a connection so we can be both a serial device as well as a WIFI simple REST server
    setup_routing();
  }

}

void setup_routing() {
  server.on("/setOn", setOn);
  server.on("/setOff", setOff);
  server.on("/setPulse", setPulse);
  server.on("/reset", handleReset);

  // start server    
  server.begin();
}

void create_json(char * value, char * message) {
  jsonDocument.clear();
    jsonDocument["result"] = value;
  jsonDocument["message"] = message;
  serializeJson(jsonDocument, buffer);
}

void add_json_object(char * value, char * message) {
  JsonObject obj = jsonDocument.createNestedObject();
  obj["result"] = value;
  obj["message"] = message;
}

void handleReset() {
  Serial.println("Reset the WIFI");

  jsonDocument.clear();
  create_json("OK", "WIFI Has been reset - need to log in via smartphone or other device");

  server.send(200, "application/json", buffer);

  wm.resetSettings();
  ESP.restart();
}

void setOff() {
  Serial.println("Set the device off");
  create_json("OK", "Device has been switched off");
  server.send(200, "application/json", buffer);
  digitalWrite(ledPin, LOW);
  digitalWrite(onPin, LOW);
  digitalWrite(switchPin, LOW);
}

void setOn() {
  Serial.println("Set the device on");

  create_json("OK", "Device has been switched on full");

  server.send(200, "application/json", buffer);

  digitalWrite(onPin, LOW);
  delay(100);
  digitalWrite(onPin, HIGH);
  digitalWrite(ledPin, HIGH);

  delay(BOUNCE_DELAY);
  digitalWrite(switchPin, HIGH);
  delay(BOUNCE_DELAY);
  digitalWrite(switchPin, LOW);

}
void setPulse() {
  Serial.println("Set the device to pulse");

  create_json("OK", "Device has been switched to pulse");
  server.send(200, "application/json", buffer);

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

}

void loop() {

  // Only try this is we actually on the IOT
  if (wifiConnected)
    server.handleClient();

  if (Serial.available() > 0) {
    // read the incoming byte:
    incomingByte = Serial.read();
  } else {
    return;
  }

  // We must have something... 
  if (incomingByte == 48) {
    // Switch off 0
    Serial.println("OK");
    digitalWrite(ledPin, LOW);
    digitalWrite(onPin, LOW);
    digitalWrite(switchPin, LOW);
  } else if (incomingByte == 49) {
    Serial.println("OK");
    digitalWrite(onPin, LOW);
    delay(100);
    digitalWrite(onPin, HIGH);
    digitalWrite(ledPin, HIGH);

    delay(BOUNCE_DELAY);
    digitalWrite(switchPin, HIGH);
    delay(BOUNCE_DELAY);
    digitalWrite(switchPin, LOW);

  } else if (incomingByte == 50) {

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
  } else if (incomingByte == 57) {

    // Alive - do nuffin
    Serial.println("OK");

  } else {
    Serial.println("ERROR");
  }

}
