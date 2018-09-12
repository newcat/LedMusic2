# LedMusic

LedMusic is a software for creating music visualizations primarily for LED strips (e. g. WS2812b).
However, due to the flexibility of the software, it can also be used for visualizations on different hardware.

## Architecture
The software is divided into three parts.
```
┌──────────┐                ┌───────────────────────┐                         ┌───────────────────┐
│          │                │                       │                         │                   │
│   GUI    │  <───────────> │  State & Calculation  │ <─────────────────────> │    VST Plugin     │
│  Vue.js  │    WebSocket   │          C#           │   UDP & Shared Memory   │  Run in VST host  │
│          │                │                       │                         │                   │
└──────────┘                └───────────────────────┘                         └───────────────────┘
```

## Screenshots
![Node Editor](documentation/screenshots/1.png)
