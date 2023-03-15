# DukeKLH
### Description ###
Duke KLH</b></color> (kaukolämpöhybridi, district heating hybrid) is a VR environment that can be used for  comparing the costs and emissions of different district heating hybrid solutions, such as geothermal heating or air-to-water heatpump heating.
The main focus of the environment is to help housing cooperatives, building managers and other interested parties in deciding what kind of district heating solution is the best option in their specific situation.


### Unity version ###
This project has been created using Unity 2021.3.8f1 and has been tested with Unity 2021.3.15f1. 


### Setting up the project ###
#### Packages ####
After opening the project for the first time you need to download the following packages from Unity's Package Manager, which can be opened from Unity's top ribbon under *Window/Package Manager*. Filter the packages by selecting *Packages: Unity Registry* and search for the following packages:
+ High Definition RP
+ XR Plugin Management
+ OpenXR Plugin
+ TextMeshPro
+ InputSystem

#### HDRP Wizard ####
After loading the packages you will likely see only pink materials. This means you need to convert the materials through HDRP Wizard, which can be opened from Unity's top ribbon under *Window/Rendering/HDRP Wizard*.

#### Curved UI text ####
Texts are likely not shown correctly (if at all) when the project is first loaded. This is caused by text meshes not being generated yet. Hitting Play in any Scene should solve this issue. 

#### Layers ####
Input system uses layers to detect interaction, but layers are not configured in this project. See the picture below for default Layer configuration.

![image](https://user-images.githubusercontent.com/125269615/225032758-92c1e1d8-6152-4b64-929b-453a92ef1f2e.png)



#### Build settings (Scenes) ####
Scenes list needs to be set before the project can be used. Here is the configuration used by this project.

![image](https://user-images.githubusercontent.com/125269615/225230745-35f1e052-a843-44a7-8965-6892f0a144cd.png)

