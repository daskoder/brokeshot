#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

#define OLED_RESET    -1
Adafruit_SSD1306 display(OLED_RESET);

String incomingData = "";  // String to store incoming serial data

void setup() {
  // Initialize Serial communication
  Serial.begin(9600);

  // Initialize OLED display
  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
    Serial.println(F("SSD1306 allocation failed"));
    while (true);
  }
  
  // Clear the display and set up initial settings
  display.clearDisplay();
  display.setTextSize(1);
  display.setTextColor(SSD1306_WHITE);
  display.setCursor(0, 0);
  display.print(F("Waiting for data..."));
  display.display();
}

void loop() {
  // Check if there's data available from the serial port
  if (Serial.available()) {
    incomingData = Serial.readStringUntil('\n');  // Read incoming data until newline
    displayDataOnOLED(incomingData);  // Display data on OLED screen
  }
}

void displayDataOnOLED(String data) {
  display.clearDisplay();
  display.setCursor(0, 0);
  display.setCursor(0, 10);
  display.print(data);  // Display the data (e.g., TEMP:45.5)
  display.display();
}
