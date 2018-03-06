/*
 * HostPinger ESP8266 Edition V1.1
 * Created By Jake Tebbett (jebbett) - Copyright 2018
 * ## VERSION CONTROL ##
 * v1.1 - Set wifi to station mode
 */

#include <ESP8266WiFi.h>
#include <ESP8266Ping.h>
#include <WiFiClientSecure.h>

// SETTINGS
const char* ssid          = "YOURSSID";                               // WiFi SSID
const char* password      = "YOURWIFIPASSWORD";                       // WiFi Password
const char* remote_host[] = {"192.168.1.1","www.google.com"};         // Hosts to check
const char* host          = "graph-eu01-euwest1.api.smartthings.com"; // SmartThings IDE URL
String appID              = "YOURAPPID";                              // SmartThings AppID
String accToken           = "YOURACCESSTOKEN";                        // SmartThings Access Token

int delaySeconds          = 10;                                       // Number of seconds for next loop after checking all hosts
bool advancedDebug        = false;                                    // When true this will take longer as it waits for a response

// DO NOT EDIT BELOW
const int httpsPort = 443;
int hostCount = sizeof(remote_host)/sizeof(char *); //array size
char* activeClients[sizeof(remote_host)];

void setup() {
  Serial.begin(115200);
  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.println("Connecting to WiFi");
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(100);
    Serial.print(".");
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
        //do nothing
        Serial.println(" -> ALREADY ONLINE");
      }else{
        Serial.println(" -> ONLINE");
        if(sendToST(remote_host[thisHost], "online")){
          activeClients[thisHost] = "online";
        }
      }
    } else {
      if(activeClients[thisHost] == "offline"){
        //do nothing
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
  String url = "/api/smartapps/installations/" + appID + "/statechanged/" + state + "?access_token=" + accToken + "&ipadd=" + pingHost;

  Serial.print("requesting URL: ");
  Serial.println(url);

  client.print(String("GET ") + url + " HTTP/1.1\r\n" +
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
