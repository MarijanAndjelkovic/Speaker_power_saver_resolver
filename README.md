# Speaker Keep-Alive Utility 🔊

A lightweight, unobtrusive Windows background application designed to prevent external speakers and audio interfaces from entering sleep or standby mode.

## 🛑 The Problem
Many modern speakers and audio amplifiers have an auto-standby feature that turns off the hardware after a few minutes of silence to save power. When a new sound plays, the hardware takes 1-2 seconds to "wake up". This results in the first few seconds of audio being cut off, often accompanied by an annoying "pop" or "crack" sound.

## ✅ The Solution
This utility plays a continuous, entirely unhearable white noise audio loop in the background. This keeps the audio channel constantly active, preventing the hardware from ever going to sleep, ensuring your audio plays instantly every time.

## 🌟 Key Features

* **System Tray Operation:** The app is designed to stay out of your way. Clicking the standard "X" window button simply minimizes the application to the system tray. 
* **Quick Access:** Double-click the tray icon to bring up the control panel.
* **Volume Calibration:** Includes a numeric control to adjust the volume (+ or - dB). This allows you to find the exact minimum volume required to keep your specific speakers awake without the noise becoming audible.
* **Launch on Boot:** Features a simple checkbox to add the application to your PC's startup sequence. Set it once, and forget it.
* **Dedicated Exit:** A specific "Exit the app fully" button ensures the background process is completely terminated when you actually want to close it[cite: 5].

## 🛠️ Tech Stack
* C#
* .NET Framework / Windows Forms
* Background Task Management

## 📸 Interface

<img width="463" height="386" alt="image" src="https://github.com/user-attachments/assets/19c26993-2b18-44e0-aae3-39d7c06f153e" />
