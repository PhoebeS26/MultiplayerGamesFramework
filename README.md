# C# Multiplayer Games Framework

A C# multiplayer networking project exploring client-server communication, TCP/UDP networking, packet handling, game-state synchronisation and encrypted communication.

## Overview

This project was developed to explore how multiplayer game networking can be implemented using C# and .NET networking APIs.

The project uses a client-server architecture where clients connect to a central server. The server manages connected clients and handles different types of network packets, including player/object positions and game-state information.

The project also includes a separate UDP communication path for position updates and an RSA-based encryption experiment.

## Features

* TCP client-server communication using `TcpListener` and sockets
* UDP communication using `UdpClient`
* Multiple connected clients
* Unique client ID assignment and ID reuse
* Thread-safe client management
* Packet-based network communication
* JSON serialisation and deserialisation
* Custom packet type handling
* Player/object position synchronisation
* Game-state synchronisation
* Asynchronous UDP communication
* RSA public-key encryption/decryption experiment
* Connection and disconnection handling
* Error handling and network debugging

## Technologies

**Language**

* C#

**Networking**

* TCP
* UDP
* Sockets
* `NetworkStream`

**Other**

* JSON serialisation
* Multithreading
* Asynchronous programming
* RSA encryption
* Git / GitHub

## Networking Architecture

The server manages TCP connections using `TcpListener`. Each connected client is assigned a unique ID and represented by a `ConnectedUser` object.

TCP communication is used for reliable messages and game-state information, while UDP is used for position updates where lower communication overhead is useful.

```text
Client
  │
  ├── TCP ───────────────┐
  │                      │
  └── UDP ───────────────┤
                         ▼
                    Game Server
                         │
                 ┌───────┴───────┐
                 │               │
             Client 1         Client 2
```

## Packet System

Network messages are represented using different packet types derived from a common `Packet` class.

Examples include:

* `MessagePacket`
* `AssignClientIDPacket`
* `PositionPacket`
* `UdpPositionPacket`
* `GameStatePacket`
* `EncryptPacket`
* `PublicKeyPacket`

Packets are serialised to JSON before being transmitted. The receiving side uses the packet type contained in the JSON data to deserialise it into the appropriate packet class.

## Client Management

The server maintains connected clients using a thread-safe `ConcurrentDictionary`.

Each client receives a unique ID when connecting. When a client disconnects, its ID is placed into a queue so that it can be reused by a future connection.

The server also stores each client's UDP endpoint so that UDP position updates can be forwarded to other connected clients.

## Encryption Experiment

The project includes an experiment with RSA encryption.

When a client connects, the server generates an RSA key pair and sends its public key to the client. Encrypted packets can then be received by the server and decrypted using its private key before being converted back into the original packet.

This was implemented to explore how encrypted data could be incorporated into a networked application.

## What I Learned

This project helped me develop practical experience with:

* Client-server architecture
* TCP and UDP networking
* Socket programming
* Serialisation and structured network data
* Multithreading and asynchronous programming
* Managing multiple connected clients
* Handling network errors and disconnections
* Basic encrypted communication
* Designing reusable packet-based systems

## Project Status

This project was developed as a learning project to explore multiplayer networking concepts. It is not intended to be a production-ready networking solution.
