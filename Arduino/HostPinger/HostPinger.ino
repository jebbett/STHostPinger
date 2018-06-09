/*
 * HostPinger ESP8266 Edition V2.0
 * Created By Jake Tebbett (jebbett) - Copyright 2018
 * ## VERSION CONTROL ##
 * v1.1 - Set wifi to station mode
 * v1.2 - added flashing LED while connecting WiFi
 * v2.0 - Format changes to support ST and Hubitat and added HeartBeat
 */

#include <ESP8266WiFi.h>
#include <ESP8266Ping.h> //https://github.com/dancol90/ESP8266Ping
#include <WiFiClientSecure.h>

// SETTINGS
const char* ssid          = "YOURSSID";                                                        // WiFi SSID
const char* password      = "YOURWIFIPASSWORD";                                                // WiFi Password
const char* remote_host[] = {"192.168.1.1","www.google.com"};                                  // Hosts to check
const char* host          = "graph-eu01-euwest1.api.smartthings.com";                          // Home automation API URL
String url                = "/api/smartapps/installations/YOURAPPID";                          // Home automation API URL suffix
String accToken           = "YOURACCESSTOKEN";                                                 // Home automation API Access Token

int delaySeconds          = 10;                                       // Number of seconds for next loop after checking all hosts
bool advancedDebug        = false;                                    // When true this will take longer as it waits for a response
bool heartBeat            = false;                                    // When true this will send current state with every loop

// DO NOT EDIT BELOW
const int httpsPort = 443;
int hostCount = sizeof(remote_host)/sizeof(char *); //array size
char* activeClients[sizeof(remote_host)];

// Get disconnecct event
WiFiEventHandler disconnectedEventHandler;

//+=============================================================================
// Gets called when device loses connection to the accesspoint
//
void lostWifiCallback (const WiFiEventStationModeDisconnected& evt) {
  Serial.println("Lost Wifi");
  // reset and try again, or maybe put it to deep sleep
  ESP.reset();
  delay(1000);
}

void setup() {
  Serial.begin(115200);
  delay(10);
  pinMode(LED_BUILTIN, OUTPUT);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.println("Connecting to WiFi");
  disconnectedEventHandler = WiFi.onStationModeDisconnected([](const WiFiEventStationModeDisconnected& event)
  {
    Serial.println("Lost Wifi");
    ESP.reset();
    delay(1000);
  });
  
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) {
    digitalWrite(LED_BUILTIN, LOW);
    Serial.print(".");
    delay(250);
    Serial.print(".");
    digitalWrite(LED_BUILTIN, HIGH);
    delay(250);
  }
  Serial.println();
  Serial.print("WiFi connected with ip ");  
  Serial.println(WiFi.localIP());
}

void loop() {
  // Loop through hosts and check state
  for (int thisHost = 0; thisHost < hostCount; thisHost++) {
    
    Serial.print("[Pinging Host] ");
    Serial.print(remote_host[thisHost]);
  
    if(Ping.ping(remote_host[thisHost], 1)) {
      if(activeClients[thisHost] == "online"){
        if(heartBeat){
          if(sendToST(remote_host[thisHost], "online")){
            activeClients[thisHost] = "online";
          }
        }
        Serial.println(" -> ALREADY ONLINE");
      }else{
        Serial.println(" -> ONLINE");
        if(sendToST(remote_host[thisHost], "online")){
          activeClients[thisHost] = "online";
        }
      }
    } else {
      if(activeClients[thisHost] == "offline"){
        if(heartBeat){
          if(sendToST(remote_host[thisHost], "offline")){
            activeClients[thisHost] = "online";
          }
        }
        Serial.println(" -> ALREADY OFFLINE");
      }else{
        Serial.println(" -> OFFLINE");
        if(sendToST(remote_host[thisHost], "offline")){
          activeClients[thisHost] = "offline";
        }
      }
    }
  }
  Serial.print("Wait ");
  Serial.print(delaySeconds);
  Serial.println(" seconds...");
  delay(delaySeconds * 1000); 
}

bool sendToST(const char* pingHost, String state){
    
  // Use WiFiClientSecure class to create TLS connection
  WiFiClientSecure client;
  Serial.print("connecting to ");
  Serial.println(host);
  if (!client.connect(host, httpsPort)) {
    Serial.println("connection failed");
    return false;
  }
  String url2 = url + "/statechanged/" + state + "?access_token=" + accToken + "&ipadd=" + pingHost;

  Serial.print("requesting URL: ");
  Serial.println(url2);

  client.print(String("GET ") + url2 + " HTTP/1.1\r\n" +
               "Host: " + host + "\r\n" +
               "User-Agent: BuildFailureDetectorESP8266\r\n" +
               "Connection: close\r\n\r\n");

  Serial.println("request sent");
  if(advancedDebug){
    while (client.connected()) {
      String line = client.readStringUntil('\n');
      if (line == "\r") {
        Serial.println("headers received");
        break;
      }
    }
    String line = client.readString();
    Serial.println("reply below (if blank this usually means successful:");
    Serial.println("==========");
    Serial.println(line);
    Serial.println("==========");
    Serial.println("closing connection");
  }
  return true;
}
