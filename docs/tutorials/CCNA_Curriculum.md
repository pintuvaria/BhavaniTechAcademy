# CCNA (Cisco Certified Network Associate) - Complete Tutorial

## Module 1: Network Fundamentals
### 1.1 The OSI and TCP/IP Models
The Open Systems Interconnection (OSI) model conceptualizes network communication into 7 distinct layers:
1. **Physical Layer:** Bits, cables, hubs, and electrical signals.
2. **Data Link Layer:** Frames, MAC addresses, and switches.
3. **Network Layer:** Packets, IP addresses, and routers.
4. **Transport Layer:** Segments, TCP/UDP, and port numbers.
5. **Session Layer:** Establishing and terminating sessions.
6. **Presentation Layer:** Data formatting and encryption (SSL/TLS).
7. **Application Layer:** HTTP, FTP, DNS.

### 1.2 IPv4 Subnetting & VLSM
Subnetting borrows bits from the Host portion to create smaller network segments. 
- **Formula:** Usable Hosts = `2^h - 2` (Subtracting the Network and Broadcast addresses).
- **VLSM:** Variable Length Subnet Masking allows you to assign different subnet masks within the same network to prevent IP address waste (e.g., a `/30` mask for a point-to-point router link only provides exactly 2 usable IPs).

## Module 2: Switching and Routing
### 2.1 VLANs and 802.1Q Trunking
A Virtual LAN (VLAN) logically segments a physical switch. Devices in VLAN 10 cannot communicate with devices in VLAN 20 without a Layer 3 routing device (Router-on-a-Stick). **802.1Q** is the standard protocol that "tags" Ethernet frames with their VLAN ID as they cross trunk links.

### 2.2 Spanning Tree Protocol (STP)
Redundant links in a Layer 2 network will cause infinite broadcast storms. STP (IEEE 802.1D) prevents this by:
1. Electing a **Root Bridge** (Lowest Bridge ID).
2. Blocking redundant ports.
If a primary link fails, STP recalculates and unblocks the backup port.

### 2.3 OSPF (Open Shortest Path First)
OSPF is a Link-State routing protocol.
- Uses **Dijkstra's Shortest Path First (SPF)** algorithm based on interface bandwidth (cost).
- Requires a hierarchical design centered around **Area 0 (The Backbone)**. All other areas must connect to Area 0.

## Module 3: Network Security (ACLs)
Access Control Lists (ACLs) filter traffic.
- **Standard ACLs (1-99):** Filter ONLY by Source IP. Must be placed as close to the *destination* as possible.
- **Extended ACLs (100-199):** Filter by Source IP, Destination IP, Protocol, and Port. Must be placed as close to the *source* as possible.
- **Implicit Deny:** At the bottom of every ACL is a hidden `deny ip any any` rule.
