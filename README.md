# DukeKLH
### Description ###
Duke KLH</b></color> (kaukolämpöhybridi, district heating hybrid) is a VR environment that can be used for  comparing the costs and emissions of different district heating hybrid solutions, such as geothermal heating or air-to-water heatpump heating.
The main focus of the environment is to help housing cooperatives, building managers and other interested parties in deciding what kind of district heating solution is the best option in their specific situation.


### Unity version ###
This project has been created using Unity 2021.3.8f1 and has been tested with Unity 2021.3.15f1. 


### Setting up the project ###
Open Unity Hub after the cloning the files.
Select Open -> Open project from disk.
After the project first loads, you need to download the following packages from Unity's Package Manager:
+ High Definition RP
+ XR Plugin Management
+ OpenXT Plugin
+ TextMeshPro
+ InputSystem

After loading the packages you will likely see only pink materials. This means you need to convert the materials through HDRP Wizard, which can be found from Unity's top ribbon under *Window/Rendering/HDRP Wizard*.

Texts are likely not shown correctly (if at all) when the project is first loaded. This is caused by text meshes not being generated yet. Hitting Play in any Scene should solve this issue. 
