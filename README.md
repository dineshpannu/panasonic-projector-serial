# panasonic-projector-serial

This project aims to simplify controlling your serial connected Panasonic projector by providing a reliable MQTT based IP bridge to communicate over. It comes in 2 parts: a service exposing a MQTT server, and a command line client to send commands to the server. 

It has been designed for operation in conjunction with MadVR to control aspect ratio lens memories. In particular, it will buffer successive lens memory load commands while a lens zoom is in operation so that only the most recent aspect ratio change triggers a lens memory load.

Common use cases:
* Allow MadVR to change lens memory with aspect ratio changes
* Control projector from home automation systems over MQTT. Hubitat driver forthcoming.

## Getting Started
Download the latest package zip file. This contains both the service and the client. The service will need to be run on a PC connected to the projector. The client can be executed from any networked PC, but we assume it will be the same PC as is running the service.

### Prerequisites
* A Windows PC connected to your projector via serial cable. If a USB-serial adaptor is used, a FTDI based chipset is recommended.
* .NET 4.7.2

Linux and Mac OS support is possible if there is demand. The core library has been written in .NET Standard, so fronting it with a .NET Core application for other operating systems should be straightforward.

### Installing

* Unzip the package and copy all files to a suitable location. For example, `C:\Program Files\Panasonic Projector Serial\`. We will refer to this as **[InstallFolder]**

#### Server installation
* Open an administrator command prompt.
* Install the service by entering: 
  ```
  C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe [InstallFolder]\server\PanasonicSerialService.exe
  ```

### Configuration
#### Server Configuration
Edit ***[InstallFolder]**\server\PanasonicSerialService.exe.config*. Configurable items are under `<appSettings>`.


| Key | Purpose | Default |
| --- | ------- | --- |
| ServerPort | TCP port to run MQTT server on | 8889 |
| ComPort | Serial port of connected projector | COM1 |
| LensMemory1 | Aspect ratio of first lens memory slot | 0 |
| LensMemory2 | Aspect ratio of second lens memory slot | 0 |
| LensMemory3 | Aspect ratio of third lens memory slot | 0 |
| LensMemory4 | Aspect ratio of fourth lens memory slot | 0 |
| LensMemory5 | Aspect ratio of fifth lens memory slot | 0 |
| LensMemory6 | Aspect ratio of sixth lens memory slot | 0 |

LensMemory[1-6] can be configured by setting the aspect ratio it's intended to be used by. Aspect ratio can be provided in the following equivalent formats:
* 16x9, or
* 1.78:1. or
* 1.78

LensMemory configuration is optional. Once configured, the server will select the correct lens memory to load when given a target aspect ratio. If no aspect ratio is a perfect match, it will select the memory with the closest match. It makes no preference to undershoot or overshoot the target aspect ratio. If LensMemory[1-6] is not configured, the server can still be directed to load a memory by supplying the lens memory slot number to load.

#### Client Configuration
Edit ***[InstallFolder]**\client\PanasonicSerialClient.exe.config*. Configurable items are under `<appSettings>`. All settings can be overridden by command line switches.

| Key | Purpose | Default |
| --- | ------- | --- |
| Host | IP address of PC runnign server | localhost |
| ServerPort | TCP port of server to connect to | 8889 |


### Usage
#### Command Line Client Usage
With the server installed and operational, we can send it commands using the command line client.

```
[InstallFolder]\client\PanasonicSerialClient.exe [switches] [command] [command option]
```

Example to power off projector:
```
[InstallFolder]\client\PanasonicSerialClient.exe PowerOff
```

Example to load lens memory 3:
```
[InstallFolder]\client\PanasonicSerialClient.exe LensMemoryLoad 3
```

Example to load lens memory matching 2.00:1 aspect ratio:
```
[InstallFolder]\client\PanasonicSerialClient.exe LensMemoryLoad 2.00:1
```


##### Command Line Client Switches
| Switch | Purpose | Default |
| ------ | ------- | ------- |
| -d, --debug | Enable debug output to console and log file | false |
| -h, --host | IP address of PanasonicSerialServer | localhost |
| -p, --port | Port for server | 8889 |


#### MQTT Usage
Connect to MQTT server with no certificate, TLS, username or password. Send JSON in the following format to topic ```RequestAction```:
```
{
  "Command": "LensMemoryLoad",
  "Option": "2.35:1"
}
```

#### Commands
| Command | Command Option | Option Definition |
| ------- | -------------- | ------------------ |
| PowerOn | |
| PowerOff | |
| SwitchInput | CP1 | Component in |
| | SVD | S-Video in |
| | VID | Composite video in |
| | HD1 | HDMI 1 in |
| | HD2 | HDMI 2 in | 
| | HD3 | HDMI 3 in |
| | RG1 | PC in (RGB) |
| Menu | |
| Enter | |
| Return | |
| Up | |
| Down | |
| Left | |
| Right | |
| Lens | |
| Default | |
| Freeze | 0 | Off |
| | 1 | On |
| FunctionButton | |
| Blank | |
| Picture | |
| MemoryLoad | |
| Waveform | 0 | Off |
| | 1 | Full scan (Y) |
| | 2 | Full scan (R) |
| | 3 | Full scan (G) |
| | 4 | Full scan (B) |
| | 5 | Single scan (Y) |
| | 6 | Single scan (R) |
| | 7 | Single scan (G) |
| | 8 | Single scan (B) |
| Aspect | |
| PictureMode | NOR | Normal |
| | DYN | Dynamic |
| | 709 | REC709 |
| | DCN | D-Cinema |
| | CN1 | Cinema1 |
| | CN2 | Cinema2 |
| | GM1 | Game |
| PictureAdjust | |
| ColourManagement | |
| VieraLink | |
| SubMenu | |
| Keystone | |
| AdvancedMenu | |
| AutoSetup | |
| Sleep | 0 | Off |
| | 1 | 60 min |
| | 2 | 90 min |
| | 3 | 120 min |
| | 4 | 150 min |
| | 5 | 180 min |
| | 6 | 210 min |
| | 7 | 240 min |
| Settings3D | |
| LensMemoryLoad | [aspect ratio] | eg, 2.35:1 or 2.35. Will match to the closest lens memory using server configuration |
| | 1 | Lens memory 1 |
| | 2 | Lens memory 2 |
| | 3 | Lens memory 3 |
| | 4 | Lens memory 4 |
| | 5 | Lens memory 5 |
| | 6 | Lens memory 6 |
| GammaAdjustment | | See projector manual |
| Trigger1 | TROI0=+00000 | Low |
| | TROI0=+00001 | High |
| Trigger2 | TROI1=+00000 | Low |
| | TROI1=+00001 | High |
| InputFormat3D | DIFI1=+00000 | Auto |
| | DIFI1=+00001 | Native |
| | DIFI1=+00003 | Side by side |
| | DIFI1=+00004 | Top and bottom |




### Logging
#### Server logging
The service logs to Windows event log. Please use Event Viewer to inspect entries under Windows Logs -> Application.

#### Client logging
The command line client logs to %APPDATA%/PanasonicSerialClient.log

### Built With
* [CommandLineParser](https://github.com/commandlineparser/commandline)
* [MQTTnet](https://github.com/chkr1011/MQTTnet)
* [Newtonsoft.Json](https://www.newtonsoft.com/json)
* [Serilog](https://serilog.net/)

## Contributing
Contrinutions happily accepted. Please send a pull request.

## Authors
* **Dinesh Pannu** - *Initial work*

## License
This project is licensed under GPL - see the [LICENSE](LICENSE) file for details
