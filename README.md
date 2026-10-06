# Multiplayer Games Framework

A C# and MonoGame multiplayer networking project exploring client-server communication, TCP/UDP networking and synchronisation of game objects.

## Overview

This project was developed using **C# and MonoGame** to explore multiplayer networking and the systems required to communicate and synchronise game objects between clients.

The project includes a client-server architecture using TCP and UDP, a custom packet system, JSON serialisation, client management, game-state synchronisation and encrypted communication.

## Features

* C# client-server architecture
* TCP networking using sockets and `NetworkStream`
* UDP networking for position updates
* Client connection and disconnection handling
* Unique client ID assignment
* Custom packet-based communication system
* JSON serialisation and deserialisation
* Player and object position synchronisation
* Game-state and score synchronisation
* Multithreading and asynchronous networking
* Thread-safe collections for connected clients
* RSA public-key encryption
* MonoGame-based multiplayer game

## Controls

* **W** — Move paddle up
* **S** — Move paddle down

The game supports two players, with each client controlling either the left or right paddle depending on their assigned client ID.

## Technologies

**Language**

* C#

**Frameworks & Libraries**

* MonoGame
* .NET
* Aether Physics 2D

**Networking**

* TCP
* UDP
* C# Sockets
* `TcpListener`
* `UdpClient`
* `NetworkStream`

**Other**

* JSON serialisation
* RSA encryption
* Multithreading
* Asynchronous programming
* Visual Studio
* GitHub

## Networking

The project uses both TCP and UDP for different types of communication.

TCP is used for reliable communication such as messages, client ID assignment and game-state updates.

UDP is used for frequently updated positional information, such as paddle and ball movement, where low-latency communication is more important.

## Packet System

A custom packet system was developed to structure and serialise different types of network messages.

Packet types include:

* Messages
* Client ID assignment
* Object positions
* UDP position updates
* Game-state updates
* Encrypted packets
* Public key exchange

Packets are serialised to JSON before being transmitted and deserialised when received.

## Client & Server

The server manages connected clients and assigns each client a unique ID.

The client establishes both TCP and UDP connections to the server and processes incoming network messages asynchronously.

Connected clients are managed using thread-safe collections, while separate processing is used for TCP and UDP communication.

## Encryption

The project also explores encrypted communication using RSA public-key encryption.

The server provides its public key to the client, which can then use the key to encrypt messages before sending them to the server.

## What I Worked On

I worked on the networking systems used to connect clients and synchronise game objects.

This included working with C# sockets, TCP and UDP communication, `NetworkStream`, JSON serialisation, custom packet structures, client management and asynchronous networking.

I also worked with game-state synchronisation, including player positions, ball movement, scores and win conditions.

## What I Learned

This project gave me practical experience with:

* C# programming
* Client-server architecture
* TCP and UDP networking
* Socket programming
* JSON serialisation
* Multithreading and asynchronous programming
* Networked object synchronisation
* Encryption
* Debugging network communication
* Working with a multiplayer game architecture

## Status

This is a learning project developed to explore multiplayer networking and client-server communication using C# and MonoGame.
