# Lynx Auto Arm
Small helper app for FinishLynx to automatically arm the camera for capture after the timer starts.

## What it does
This app receives data from Lynx via a scoreboard script. After the timer starts, and a configurable delay expires, it sends "Alt-t" keystroke to FinishLynx to toggle the capture mode. Presumably, capture would not already be enabled, so this would arm the camera. Note that the only hotkey available is "toggle", and there is no feedback whether the capture is enabled or not. So this could potentially disable capture if capture had already been enabled.

## Requirements
This does require the NCP plug-in so that a scoreboard can be configured using network settings rather than a COM port.

## Set-up
In FinishLynx, add a new scoreboard with the following settings:
- Script: JSON.lss (included in Lynx Scoreboard Script folder)
- Serial Port: Network (UDP)
- Port: 8113
- IP Address: 127.0.0.1
- Running Time: Normal
- Running Time Options: None
- Results: Off

## Running
- Double click AutoArm.exe
- Set the value of "Delay". Capture toggle will occur when the running time is greater than or equal to this value.
- Set the UDP port. This must match the value configured in the scoreboard settings. Default is 8113.
- Click "Enable"

That's it!

## Problems / Contributing
Feel free to write issues or submit pull requests in GitHub.
