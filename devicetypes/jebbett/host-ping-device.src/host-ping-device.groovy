/**
 *  Host Ping Device
 *
 *  Copyright 2016 Jake Tebbett
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 *  in compliance with the License. You may obtain a copy of the License at:
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software distributed under the License is distributed
 *  on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License
 *  for the specific language governing permissions and limitations under the License.
 *
 * 	V1.1 - Added Presence
 *	V1.2 - Attribute update 10/01/18
 */

metadata {
	definition (name: "Host Ping Device", namespace: "jebbett", author: "jebbett") {
		capability "switch"
        capability "presenceSensor"
        attribute "switch", "string"
	}

	tiles(scale: 2) {
        standardTile("switch", "device.switch", width: 6, height: 6, canChangeIcon: true) {
    		state "off", label: 'Offline', icon: "st.Electronics.electronics18", backgroundColor: "#ff0000"
    		state "on", label: 'Online', icon: "st.Electronics.electronics18", backgroundColor: "#79b821"
		}
    	main("switch")
        details(["switch"])
    }
}

def on() {
    sendEvent(name: "switch", value: "on");
    sendEvent(name: "presence", value: "present");
    log.debug "Online"
}

def off() {
    sendEvent(name: "switch", value: "off");
    sendEvent(name: "presence", value: "not present");
    log.debug "Offline"
}
