# MR-Drones-And-CV
#### <<<<<Project files will be uploaded here soon.>>>>>
A seamless integration of Quarotor Simulation and Detection and Tracking Model: YOLOv8.


#### Quadrotor Simulation and Map:

This project contains a realistic, physics-based drone simulation. There are different models and controlling variables to control the drone pilot system.
The map of the world contains different objects like Terrain, Road maps, Football goals, cones, dominos, Buses, Humans, Animals (cows), trucks, a farmhouse, and some random stuff too for realism.

#### Quadcopter in the simulation:
![YOLO detection and tracking results](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Quadrotor_Simulation.gif)

#### Computer Vision and AI:
The project is using the latest version of [Sentis](https://unity.com/products/sentis) 1.4.0-pre.3.
Sentis is an experimental package introduced by Unity for the integration of AI models and the Unity engine.
The model used for this project is [YOLOv8](https://github.com/ultralytics/ultralytics) Nano Version.
Detection Results:
![YOLO detection and tracking results](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/YOLO_OUTPUT.gif)

#### Detection and Tracking Integration within Simulated World:
DetandTrack.cs file is the main code for the model inference during runtime. The same file can be used for other versions of the YOLOv8.
There are a lot of customization options to work around for your model in that script.

![Detection and Tracking Script Variables](https://github.com/AbdullahMushtaq78/MR-Drones-And-CV/blob/main/Results/Detection_and_Tracking_Script_Variables.png)

In this script, you can change:
| **Section**           | **Variable Name**     | **Description**                                                                                     |
|-----------------------|-----------------------|-----------------------------------------------------------------------------------------------------|
| **Controls**          | Run Model             | Dynamically turn the model ON/OFF. (Key "Y" is assigned)                                             |
|                       | Limit Frame Rate      | Limit the framerates for performance-related concerns. Another text field will appear for the frame limit. |
| **Model**             | Model Asset           | The version of the model of YOLOv8.                                                                  |
|                       | Iou Threshold         | Defines the minimum score for each detection to display.                                             |
|                       | Score Threshold       | Defines the minimum confidence score for each detection.                                             |
|                       | Max Output Boxes      | Defines the maximum bounding boxes to show for performance-related concerns.                         |
| **Camera and Display**| Camera                | The main source camera view used for detection. Can be changed to any other camera.                  |
|                       | Display Image         | A Raw Image to display the detection results. Currently, only capable of showing results on Raw Image. |
|                       | Frame Background      | A black background that gives a frame-like feel when the model is run.                               |
|                       | Target RT             | The targeted render texture to draw the output. Should be kept None as it is assigned at runtime.    |
|                       | Capture Width         | The camera's current view's width dimension.                                                         |
|                       | Capture Height        | The camera's current view's height dimension.                                                        |
|                       | Image Width           | The target result's width dimension. 640 is the standard output dimension of YOLOv8.                 |
|                       | Image Height          | The target result's height dimension. 640 is the standard output dimension of YOLOv8.                |
| **Bounding Boxes**    | Border Sprite         | A sprite created during runtime.                                                                     |
|                       | Border Texture        | A small black block image.                                                                           |
|                       | Labels Asset          | A .txt file containing names of all YOLO classes.                                                    |
|                       | Font                  | The font type used for the class names in the bounding boxes.                                        |
