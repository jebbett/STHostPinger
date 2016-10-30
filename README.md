# SmartThings Host Pinger

**Using an EXE running on any PC - Ping any IP, host name or URL and pass this to a switch in SmartThings!**

**Code and Program Location:**
https://github.com/jebbett/STHostPinger

## Requirements

**- STHostPinger.exe & config.config** - Application (Windows / Mac&Linux via Mono) - config.config needs to be configured with details as described below and can be running on any PC connected to the internet.

**- Host Pinger SmartApp** - This passes the output of the exe to the custom device types.

**- Host Ping Device Type (optional)** - This device type revieves the online/offline event and translates this in to an on/off switch or presence sensor.

## Version Control

 * 28/10/16 - 1.0 - Release Version
 * 29/10/16 - 1.1 - Added child app and triggering of external switches
 * 30/10/16 - 1.2 - Removed direct triggering of child devices, Child app now includes delay on 'Offline'

## How To Install:

### 1. Install the Smart App and Custom Device Type

Go to your IDE, then...

#### My SmartApps:

A. Create New SmartApp

B. Select “From Code”

C. Paste App source code with the code from the file SmartApp_HostPinger.txt.

D. Save and publish the app for yourself.

E. Enable OAuth in IDE by going in to IDE > My Smart Apps > [App Name] > App Settings > Enable OAuth.

F. Get the Token and API Endpoint values via one of the below methods:

* EASY OPTION: Enable debugging, open live logging in IDE and then open the app again and press done and values will be returned in Live Logging.
* Open the SmartApp and click API Information (this will involve manually typing the codes)

#### My Device Handlers (optional):

A. Create New Device Handler

B. Select “From Code”

C. Paste App source code with the code from the file DeviceType_HostPinger.txt.

D. Save and publish the device for yourself.


### 2. Configure STHostPinger.exe & config.config

(Mac and Linux users can launch this exe by downloading Mono from the Mono-Project and running "mono STHostPinger.exe")

A. Download the exe and config.config file. 

B. Open the config.config file.

C. In config/smartThingsEndpoints fill in your API token and add the APP ID to the endpoint urls from the previous section.

ENTITY accessToken "XXXXXX FROM SMARTTHINGS XXXXXXXXX"
ENTITY appId "XXXXXX FROM SMARTTHINGS XXXXXXXXX"
ENTITY ide "https://graph-eu01-euwest1.api.smartthings.com"

D. Be sure to also check that your IDE URL matches the URL in config.config, if you have the URL from the app then this should be correct, if you were unable to get this from the app then you willl need to copy from IDE, it'll be something like "graph-na02-useast1.api.smartthings.com"

E. Configure all of the IPs / Hosts or Addresses you want to ping.

item HOST="google.com"
item HOST="192.168.1.1"
item HOST="SERVER1"

G. The polling interval, timeout and debugging can also be configured to your liking or leave as the default values.

### 3. Run The Exe

You can now run the exe and and this should push the updates to your ST Hub via the cloud.

If anything obvious isn't working you will recieve errors in the console, you can enable extra debugging by setting to 2 in config.config, or have no updates by setting to 0. 


### 4. Configure the Smart App

Configuration should be self explanatory however come back here with any questions.

Add a new device give it the same name you have specified as the host in config.config, you can open the device later and change to a different name if you like!


### 5. Errors / Debugging

**EXE - SendGetRequest: the remote server returned and error: (403) Forbidden**
There is an issue with the URL in config.config re-check section 2.D

**No error in the EXE and no "Event" present in the App**
This is because SmartThings is not reciving an event, this is usually because the App ID or Token are incorrect, if you re-install the app these values will change and need to be setup again.

**"Event" in the app, but hasn't triggered the switch to change.**
This is likely to that the Child Device has been configured with a name not matching the host in config.config.

**Live Logging - java.lang.NullPointerException: Cannot get property 'authorities' on null object @ line xx**
You have not enabled OAuth in the SmartApp