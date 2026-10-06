# The Last Broadcast: Archivist

**A psychological horror game built in Unity3D and released for Windows and macOS.**

[Play on itch.io](https://mossmangames.itch.io/tlb-archivist)

## Overview

*The Last Broadcast: Archivist* is a first-person psychological horror game set inside a decaying records facility. The player takes the role of an archivist tasked with processing an endless stream of documents while following instructions delivered through an old radio system.

The core gameplay combines **document management, radio-frequency interaction, environmental systems, and resource management**. As the player progresses, increasingly distorted radio transmissions introduce uncertainty into what should otherwise be a routine administrative task.

The project was developed in **Unity3D using C#** and released as a commercial game for both Windows and macOS.

## Technical Highlights

* Developed in **Unity3D / C#**
* Built and shipped commercial releases for **Windows and macOS**
* Interactive first-person environment with mouse-based interaction
* Custom radio-frequency interaction system
* Dynamic document-processing gameplay loop
* Environmental interaction and state-based systems
* In-game electrical/fuse system used for environmental progression
* Audio-driven gameplay and narrative systems
* UI and interaction systems designed around diegetic environmental elements
* Designed and implemented gameplay systems from prototype through release

## Core Gameplay Systems

### Radio Frequency System

The radio is the central gameplay mechanic. Players tune between frequencies using the keyboard, with transmissions providing instructions that determine how documents should be processed.

The system connects player input, frequency state, audio feedback, and gameplay progression, making the radio more than a conventional menu or dialogue system.

### Document Processing

Documents arrive continuously and must be evaluated and processed by the player.

The system creates a repeating gameplay loop:

1. Receive a document.
2. Examine its contents.
3. Interpret information received through the radio.
4. Determine the appropriate destination or action.
5. Process the document.
6. Continue while managing increasing environmental and cognitive pressure.

This turns a simple administrative task into the primary gameplay mechanic.

### Environmental Power System

The facility contains interactive electrical systems that can affect the player's ability to operate within the environment.

Fuse interactions are integrated directly into gameplay rather than presented as a conventional menu, requiring the player to physically interact with the environment to restore functionality.

## Engineering Focus

The project was designed around a simple principle: **make ordinary systems feel interactive and meaningful.**

Instead of relying exclusively on traditional HUDs and menus, many interactions are presented through the game world itself. This required coordinating gameplay state, player interaction, UI, audio, environmental objects, and progression systems.

The result is a project that demonstrates experience building interconnected gameplay systems rather than isolated prototypes.

## Development Goals

The project was also an opportunity to explore how a relatively small set of mechanics can support an entire gameplay experience.

The primary development challenges included:

* Creating a compelling gameplay loop around a non-combat activity
* Making radio interaction intuitive while maintaining uncertainty
* Connecting environmental state to gameplay progression
* Building systems that could support increasing complexity without overwhelming the player
* Maintaining a consistent visual and interaction language across the game
* Packaging and deploying the project across multiple desktop platforms

## Release

*The Last Broadcast: Archivist* was released commercially for:

* **Windows**
* **macOS**

The project is part of the larger *The Last Broadcast* universe, an ongoing survival-horror project developed by Moss Games.

## Technology

**Engine:** Unity3D
**Language:** C#
**Platforms:** Windows, macOS
**Genre:** Psychological Horror / Survival Horror

## Project Purpose

While *The Last Broadcast: Archivist* is a game, the project demonstrates broader software-development skills including **object-oriented programming, interactive system design, input handling, state management, UI development, audio integration, cross-platform deployment, and iterative problem solving**.

The project represents my experience taking a concept from an initial gameplay prototype through development and into a publicly released product.
