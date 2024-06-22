# MR-Drones-And-CV
#### <<<<<Project files will be uploaded here soon.>>>>>
A seamless integration of Quarotor Simulation and Detection and Tracking Model: YOLOv8.


## Quadrotor Simulation and Map:

This project contains a realistic, physics-based drone simulation. There are different models and controlling variables to control the drone pilot system.
The map of the world contains different objects like Terrain, Road maps, Football goals, cones, dominos, Buses, Humans, Animals (cows), trucks, a farmhouse, and some random stuff too for realism.

#### Quadcopter in the simulation:
(might take a while to load)

![YOLO detection and tracking results](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Quadrotor_Simulation.gif)

## Computer Vision and AI:
The project is using the latest version of [Sentis](https://unity.com/products/sentis) 1.4.0-pre.3.
Sentis is an experimental package introduced by Unity for the integration of AI models and the Unity engine.
The model used for this project is [YOLOv8](https://github.com/ultralytics/ultralytics) Nano Version.

#### Detection Results 
(might take a while to load)

![YOLO detection and tracking results](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/YOLO_OUTPUT.gif)

## Detection and Tracking Integration within Simulated World:
[DetAndTrack.cs](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Detection%20and%20Tracking/DetAndTrack.cs) file is the main code for the model inference during runtime. The same file can be used for other versions of the YOLOv8.
There are a lot of customization options to work around for your model in that script.

![Detection and Tracking Script Variables](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Detection_and_Tracking_Script_Variables.png)

In this script, you can change:
| **Section**           | **Variable Name**     | **Description**                                                                                     |
|-----------------------|-----------------------|-----------------------------------------------------------------------------------------------------|
| **Controls**          | Run Model             | Dynamically turn the model ON/OFF. (Key "Y" is assigned)                                             |
|                       | Limit Frame Rate      | Limit the framerates for performance-related concerns. Another text field will appear for the frame limit. |
| **Model**             | [Model Asset](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Detection%20and%20Tracking/yolov8n.onnx)           | The version of the model of YOLOv8 in ONNX format.|
|                       | Iou Threshold         | Defines the minimum score for each detection to display.                                             |
|                       | Score Threshold       | Defines the minimum confidence score for each detection.                                             |
|                       | Max Output Boxes      | Defines the maximum bounding boxes to show for performance-related concerns.                         |
| **Camera and Display**| Camera                | The main source camera view used for detection. Can be changed to any other camera.                  |
|                       | Display Image         | A Raw Image to display the detection results. Currently, only capable of showing results on Raw Image. |
|                       | Frame Background      | A black background that gives a frame-like feel when the model is run.                               |
|                       | Target RT             | The targeted render texture to draw the output. Should be kept as None as it is assigned at runtime.    |
|                       | Capture Width         | The camera's current view's width dimension.                                                         |
|                       | Capture Height        | The camera's current view's height dimension.                                                        |
|                       | Image Width           | The target result's width dimension. 640 is the standard output dimension of YOLOv8.                 |
|                       | Image Height          | The target result's height dimension. 640 is the standard output dimension of YOLOv8.                |
| **Bounding Boxes**    | Border Sprite         | A sprite created during runtime.                                                                     |
|                       | Border Texture        | A small block image.                                                                                 |
|                       | [Labels Asset](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Detection%20and%20Tracking/classes.txt)          | A ".txt" file containing names of all YOLO classes.|
|                       | Font                  | The font type used for the class names in the bounding boxes.                                        |





## System Designs

System designed followed through out the project.
#### Simulation System Design

![Simulation System Design](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Simulation_System_Design.png)

#### Simulation Controls

![Simulation Controls](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Simulation_Controls.png)

#### DJI Tello and Unity System Design
As part of this project, we also implemented a framework in which we designed the entire VR system to be used with an actual drone: DJI Tello EDU. In this system, we implemented a comprehensive library in C# for real-time communication with the drone. We also employed a Python-based microservice architecture for receiving video streaming frames and sending them to Unity, as Unity does not natively support the encoding and decoding of PNG files. This part of the project will also be shared soon.

![Tello and Unity System Design](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/TELLO_Unity_System_Design.png)


## Other images of the simulation:
| **Description**               | **Image**                                                                                                             | **Description**               | **Image**                                                                                                             |
|-------------------------------|-----------------------------------------------------------------------------------------------------------------------|-------------------------------|-----------------------------------------------------------------------------------------------------------------------|
| **Full Simulated World**      | <img src="https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Simulated_Farm.png" width="320" height="180"> | **Farm**                      | <img src="https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/FARM.png" width="320" height="180"> |
| **Third Person Drone View**   | <img src="https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Third_Person_Drone_View.png" width="320" height="180"> | **First Person Drone View**   | <img src="https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/First%20_Person_Drone_View.png" width="320" height="180"> |
| **Animals in the Farm**       | <img src="https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/FARM_with_Animals.png" width="320" height="180"> | **Animals Detection**         | <img src="https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Animals_Detection.png" width="320" height="180"> |
