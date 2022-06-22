# Lost Ark Logger
 This project enables you to research and analyze combat actions by parsing packets
 
# How does this work
 This project parses TZSP Packets sent from Mikrotik and maybe other Routers, and outputs them into a log file parsable by the included ACT plugin.

# Usage
 1. Modify fileName to the path to write to
 2. Set mikrotik router to sniff packets to/from TCP 6040 and stream to the machine this will be logging on
 3. Build and run server
 4. Select ACT plugin LostArk_ACT_Plugin.cs in ACT
 5. Select log file in Options->Miscellaneous

# Misc shit
 Running this on linux requires the oo2net library to be built for linux, good luck with that
 
 probably a bunch of other issues as well
 
# WARNING
This is not endorsed by Smilegate or AGS. Usage of this tool isn't defined by Smilegate or AGS. I do not save your personal identifiable data. Having said that, the .pcap generated can potentially contain sensitive information (specifically, a one-time use token)
  