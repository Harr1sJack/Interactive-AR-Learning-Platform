# AR-Based Interactive Learning Platform

An Android Augmented Reality (AR) application built using **Unity** and **AR Foundation (ARCore)** that renders interactive 3D educational models by scanning predefined textbook images.  
The system integrates backend-driven metadata retrieval, AI-based image services, and a live chatbot for learning assistance.

---

## Features

- Render interactive **3D educational models** in AR using predefined image targets  
- Gesture-based interaction:
  - Rotate  
  - Zoom  
  - Inspect components  
- Component-level information display  
- AI Vision module to identify scanned objects  
- Fetch similar images from the internet  
- AI-based image generation via backend integration  
- Live chatbot connected to Node.js server  
- Dynamic metadata retrieval using REST APIs  
- Modular architecture for adding new educational modules  

---

## Tech Stack

### AR Application
- Unity Engine  
- AR Foundation (ARCore)  
- C#  

### Backend
- Node.js  
- RESTful APIs  
- MongoDB  

### Development Tools
- Visual Studio / VS Code  
- Git & GitHub  

---

## System Architecture

The platform follows a **client–server architecture**:

### Unity Mobile Application
- Handles AR rendering  
- Manages user interaction  
- Sends API requests to backend  

### Node.js Backend
- Processes chatbot queries  
- Handles AI image generation  
- Fetches and serves model metadata  

### MongoDB Database
- Stores structured educational content  
- Maintains component-level descriptions  

---

## Workflow

1. User launches the application.  
2. The camera scans a predefined textbook image.  
3. The corresponding 3D model is rendered in AR.  
4. User interacts with the model using gestures.  
5. Backend services are called when metadata or AI features are required.  

---

## Target Platform

- Android devices supporting ARCore
