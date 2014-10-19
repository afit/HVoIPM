Hardware VoIP Monitor: desktop VoIP and PBX monitoring
======================================================

What is HVoIPM?
---------------
HVoIPM is a small desktop application that can be used to monitor one or more heterogenous VoIP or PBX systems. Users can choose from a range of device monitors (or build their own) to provide telephony data to be logged or displayed on screen. For instance, the software could be used to log all calls made by a certain device, or to flash up a warning or launch a process when a VoIP device's registration state goes offline.

Which devices does HVoIPM support?
----------------------------------

+--------------------------------------+------------------------------------------------+----------------------------+
| Device                               | Supported                                      | Supported by               |
|======================================|================================================+============================+
| Linksys PAP2 (2.0.12(LS), 3.1.3(LS)) | Yes, since 0.1                                 | LinksysPAP2DeviceMonitor   |
| Linksys PAP2 (3.1.7(LSd))            | Yes, since 0.5.3                               | LinksysPAP2DeviceMonitor   |
| Nortel BCM                           | Yes, since 0.3                                 | TAPIDeviceMonitor          |
| *Any TAPI-compliant device*          | Theoretically, since 0.3                       | TAPIDeviceMonitor          |
| Sipura SPA-3000                      | Yes, since 0.5.3                               | SipuraSPA3000DeviceMonitor |
| Netgear TA612V                       | Theoretically, see extending HVoIPM.           |                            |
| Sipura SPA-2000                      | Theoretically, see extending HVoIPM.           |                            |
| Sipura SPA-2002                      | Theoretically, see extending HVoIPM.           |                            |
| Linksys RTP300                       | Theoretically, see extending HVoIPM.           |                            |
| Grandstream 486                      | Theoretically, see extending HVoIPM.           |                            |
| Cisco ATA 186                        | Theoretically, see extending HVoIPM.           |                            |
| Netcomm V100                         | Theoretically, see extending HVoIPM.           |                            |
| AVM Fritz Box 7050                   | Theoretically, see extending HVoIPM.           |                            |
| Speedtouch 190 ATA                   | No. Apparently these don't expose enough data. |                            |
| Linksys WRT54GP2                     | No. Apparently these don't expose enough data. |                            |
+--------------------------------------+------------------------------------------------+----------------------------+

If your device isn't on the list, please see extending HVoIPM.

What does it look like?
-----------------------
That depends what's happening with your device. Sometimes it might look like these screenshots.

.. image:: http://s.reincubate.com/res/i/labs/HVoIPM-Systray.gif
   :alt: Hardware VoIP Monitor: System tray

.. image:: http://s.reincubate.com/res/i/labs/HVoIPM-Main.gif
   :alt: Hardware VoIP Monitor: Main window

.. image:: http://s.reincubate.com/res/i/labs/HVoIPM-Log.gif
   :alt: Hardware VoIP Monitor: Call records

Where can I get it?
-------------------
You will need the `Microsoft .NET Framework Version 2.0 <http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5&displaylang=en>`_ installed before you can run this application. Download links are below:

 `HVoIPM 0.5.31 Alpha </res/labs/HVoIPM-0.5.31.zip>`_ (11th February 2007)

* Configuration file update with extra mappings

 `HVoIPM 0.5.3 Alpha </res/labs/HVoIPM-0.5.3.zip>`_ (2nd January 2006)

* No longer prevents system shutdown
* Sipura SPA-3000 support added
* Added "load at startup" option

 `HVoIPM 0.5.2 Alpha </res/labs/HVoIPM-0.5.2.zip>`_ (30th December 2005)

* Retry option after device monitor errors

 `HVoIPM 0.5.1 Alpha </res/labs/HVoIPM-0.5.1.zip>`_ (30th December 2005)

* Configuration file bugfix

 `HVoIPM 0.5 Alpha </res/labs/HVoIPM-0.5.zip>`_ (30th December 2005)

* Main window is now resizable
* Can check availability of upgrades
* Device monitor errors handled non-fatally
* Behaviour configuration split for greater flexibility
* State lookups are broken out into configuration and handled more safely
* Known issues: Call logging is a bit hit-and-miss

 `HVoIPM 0.4 Alpha </res/labs/HVoIPM-0.4.zip>`_ (29th December 2005)

* Support for launching external processes
* Safely handles more PAP2 ring tones
* Application version now displayed in application
* Known issues: Call logging is a bit hit-and-miss

 `HVoIPM 0.3 Alpha </res/labs/HVoIPM-0.3.zip>`_ (28th December 2005)

* TAPI monitor confirmed working
* Flashing systray notification fixed
* Support for password protected PAP2s fixed
* Safely handles for PAP2 "reorder" and "off hook warning" ring tones

 `HVoIPM 0.2 Alpha </res/labs/HVoIPM-0.2.zip>`_ (28th December 2005)

* Documentation embedded in configuration file
* Better error handling with faulty monitors
* Known issues: TAPI implementation largely untested, flashing system tray notification broken

 `HVoIPM 0.1 Alpha </res/labs/HVoIPM-0.1.zip>`_ (23rd December 2005)

* First release of HVoIPM
* Known issues: TAPI implementation broken, flashing system tray notification broken

How do I make it work?
----------------------
If you have a Linksys PAP2 device, all you will need to do to get HVoIPM working is edit the configuration file ("Hardware VoIP Monitor.exe.config"), changing the LinksysPAP2DeviceMonitor:Hostname value to that of your device's URL, including the http:// prefix.

If it doesn't work, read the configuration file. If that doesn't help, refer to our forum threads ( `1 <http://forums.whirlpool.net.au/forum-replies.cfm?t=447063>`_ , `2 <http://bbs.adslguide.org.uk/showthreaded.php?Cat=&Board=voip&Number=2199351&page=0&view=collapsed&sb=5&o=0>`_ ). When reporting problems, include a copy of your "system.xml" log file with any report.

Alternately, if HVoIPM doesn't quite do what you want, have a look at the `Sipura 3000 System Tray Monitor <http://www.clacy.com/sipura/>`_ .

Future goals for HVoIPM
-----------------------

* Providing a set of documentation for configuration (there's some documentation in the configuration file, currently)
* A setup wizard to help users easily configure HVoIPM for their devices
* Integration with telephony billing software and address book services, so users can take better advantage of call logs
* Extensions to the monitoring structure to allow for balance monitoring with common VoIP services
* A Mono build for Linux \& OS X users
* Internationalisation: please let us know if you'd like to see HVoIPM in your language

How can HVoIPM be extended?
---------------------------
Support for further devices is provided by writing simple device monitor plugins. These are simple to write, and we'd love to receive any contributions. Below is a snippet of an explanatory message, along with some example source code showing how it can be done.

*It should be possible to get HVoIPM working against your Sipura SPA-2000. As you've asked for the source code, I'll explain roughly how. Basically, there are two ways to figure out what a VoIP device is doing: the first is to packet sniff and examine the RTP/VoIP traffic that's going back and forth, and the second is to introspect the device itself, over whichever interfaces it exposes.*

*The first technique is almost foolproof -- it'll work with any VoIP-compliant device to return a fairly limited set of base information. The problem it that in order for this to work, the machine that you're running the sniffing software on either needs to have it's network adapter running in promiscuous mode, or be sitting on a monitor port on your switch. (Because if you have a switch rather than a hub, like most people, your SPA-2000 won't route traffic to your computer.) There are a few tools that take this approach. `Vomit <http://vomit.xtdnet.nl/>`_ , for instance.*

*This won't really work very well for a lot of small-end consumer VoIP devices of the sort that HVoIPM works with, otherwise I'd port some of the GPLed Linux C to .NET. (It'd work well with Softphones and Skype clones, though.)*

*HVoIPM uses the second approach. That is to say, it tries to get whatever data it can from the devices by looking at the interfaces they have. Unfortunately, most of the first generation consumer VoIP boxes we're seeing now are shit. Some of them offer absolutely no decent access at all. Whilst most professional equipment provides a TAPI interface, most cheapo new stuff only provide little web interfaces. The Linksys PAP2 is one of the best in this regard, and when configured to work with one of these devices, HVoIPM basically just screen-scrapes the HTML from the PAP2 device. The SPA-3000 provides a great interface for this, too, and I wouldn't be surprised if the SPA-2000 and SPA-2002s did as well.*

*If your SPA-2000 doesn't have a decent enough web interface (or other interface), possibly it could be flashed with the PAP2 firmware. (If it really is the same device inside.). Alternately, HVoIPM could pull data out of its telnet interface, if it has one, or from any SNMP flags it has.*

*If you load up Visual Studio, and create a new project with these files you should be able to build your own device monitor classes, which can then be specified in the HVoIPM configuration. It's possible for HVoIPM to report on additional properties as well as the basic ones that it does now, too. It should be easy enough to adapt the code to work with an SPA-2000.*

*I think I've included most of the relevant source files here -- you'll probably need to comment out a few references to get it all to build, but you should get the picture if you're familiar with C#. Excuse the poor code -- I know that the LinksysPAP2DeviceMonitor class is extremely suboptimal in many ways, especially with all of the String creation & disposal it does.*

* `IDeviceMonitor.cs <https://github.com/afit/HVoIPM/blob/master/Monitor/IDeviceMonitor.cs>`_ 
* `DeviceMonitor.cs <https://github.com/afit/HVoIPM/blob/master/Monitor/DeviceMonitor.cs>`_ 
* `AbstractWebDeviceMonitor.cs <https://github.com/afit/HVoIPM/blob/master/Monitor/AbstractWebDeviceMonitor.cs>`_ 
* `LinksysPAP2DeviceMonitor.cs <https://github.com/afit/HVoIPM/blob/master/Monitor/Impl/LinksysPAP2DeviceMonitor.cs>`_ 

Contributors
------------

* Mark Lerno: invaluable help debugging the LinksysPAP2DeviceMonitor
* Ian Worthington: suggestions for improvements and bug-spotting
* Tim Boorman: bug-spotting and 3.1.7(LSd) testing
* NutCracker: most of the SPA-3000 state mappings
