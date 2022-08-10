<p align="center">
  <img width="500" alt="RocketManLogo" src="https://user-images.githubusercontent.com/62103572/183918141-3c87876e-402a-401d-ade0-80678f4e4155.png">
  </br>
  </br>
  <img alt="GitHub commit activity" src="https://img.shields.io/github/commit-activity/y/EliaFantini/RocketMan-a-VR-videogame-created-with-Unity">
  <img alt="GitHub last commit" src="https://img.shields.io/github/last-commit/EliaFantini/RocketMan-a-VR-videogame-created-with-Unity">
  <img alt="GitHub code size" src="https://img.shields.io/github/languages/code-size/EliaFantini/RocketMan-a-VR-videogame-created-with-Unity">
  <img alt="GitHub repo size" src="https://img.shields.io/github/repo-size/EliaFantini/RocketMan-a-VR-videogame-created-with-Unity">
  <img alt="GitHub follow" src="https://img.shields.io/github/followers/EliaFantini?label=Follow">
  <img alt="GitHub fork" src="https://img.shields.io/github/forks/EliaFantini/RocketMan-a-VR-videogame-created-with-Unity?label=Fork">
  <img alt="GitHub watchers" src="https://img.shields.io/github/watchers/EliaFantini/RocketMan-a-VR-videogame-created-with-Unity?label=Watch">
  <img alt="GitHub star" src="https://img.shields.io/github/stars/EliaFantini/RocketMan-a-VR-videogame-created-with-Unity?style=social">
</p>

The Virtual Reality escape-room videogame **RocketMan**, realized for **Oculus Quest** (1 and 2), takes you into an
apocalyptic world. The goal is to escape the Earth thanks to a rocket that you will first need to repair
and set up through various riddles. You can move by walking, teleporting, or by using the joystick of the
controllers. To solve the riddles, you have to use various tools that you can pick up using your hands or
through magnetic grabbing.

The game was realized in the frame of the course [CS-444 Virtual Reality](https://edu.epfl.ch/coursebook/en/virtual-reality-CS-444) and it was awarded as the **best game of the course** for year 2021/2022. 

## Authors
- [Elia Fantini](https://github.com/EliaFantini)
- [Olivier Lonneux](https://github.com/Olivier-Lonneux)
- [Gianni Lodetti](https://github.com/Gianniii)

## How to play
The game was built as an **.apk** file of 139 MB. Download the game from [here](https://drive.google.com/file/d/11GLue_6yvf3JiYeFwc-NR2a00tw4diwA/view?usp=sharing) (if the download link expires, please contact me). To install on your Oculus Quest an apk that does not come from the oculus quest store, you can follow one of the many tutorials on the web, like [this one](https://www.youtube.com/watch?v=WIbSYRc0ICk).

## Gameplay Trailer
The GIF and video that follow contain some gameplay scenes that might represent a **spoiler** of the solution of some riddles in the game. Be aware of this before watching them, if you want to play the game. Since GitHub doesn't let upload videos bigger than 10MB, the video had to be compressed and the quality is much lower than the original.  :(


https://user-images.githubusercontent.com/62103572/183925619-1bf638b2-4384-43f7-94e9-44aaac373e29.mp4


## Files description

```
├── CS-440                            # Folder containing all the Unity project files, to be opened by a Unity Editor
   ├── ...
   ├── Assets                         # Main folder of the project, it contains all our code, assets, prefabs, materials, sounds, scenes, videos and everything which was not automatically created by Unity
   ├── ...
├── report.pdf                        # The report of the project
├── VRProject_Guidelines_2022.pdf     # Pdf containing the project's guidelines and rules given by the course's professor
└── README.md                         # You are here
```


## How to open up the project on Unity Editor
This game was developed using **Unity Editor 2020.3.32f1**, we suggest you to download this version because different ones (especially older) might give problems.


Once you've downloaded the repository ( warning: it's 1.17GB) and extracted it into a folder, open up the folder CS-440 using the Unity Hub. If the version of the Unity Editor you're using to open the project is the correct one, no error should appear.

## Quick presentation slides
The following presentation is a summary of the main features. For a detailed explanations of the realisation of all aspects of the game, please read the file **report.pdf**.

<p align="center">
 <img width="auto" alt="RocketManGif" src="https://user-images.githubusercontent.com/62103572/183921705-d7e4aa2f-54de-4897-a907-c03d6069aba1.gif">
</p>

## Aesthetics and immersion design

The game was thought to be played on a portable device such as the Oculus Quest, so we decided to use
only low poly assets that could easily be rendered without a drop in the frames per second. That helped
us also to find much more assets from the web with a compatible graphic style, creating an environment
which is thematic consistent. In fact, the game uses nineteen different external assets’ packages, but the
whole scenes were made by placing manually every single prefab, recreating a rocket environment as we
imagined it since the very beginning, without using any pre-made scene.

To improve immersion, not only we tried to make interactions as natural as possible, but we also provided
each of them with a proper sound effect and some haptic feedback. The music in the background was
chosen to be relaxing while the player is reasoning on riddles. A big part of our attention was put in the
rocket and capsule launch animations, so that they could convey as much as possible the emotions and
feelings of a real launch. For them, every sound and particle effect was carefully designed.

## Play testing and feedbacks

The feedback we got from the play test was very valuable, as it allowed us to see what parts of the
game were not very intuitive. For example, we noticed that the flashlight on the second floor was too
hard to find, and people would forget they could interact with items with a button, so we decided to
have the flashlight on by default instead of toggling it with a button.

Moreover, many testers said that
the tutorial was long and not intuitive due to the amount of text. Hence, we replaced most of the text
with videos showing the interactions. Some people also forgot some parts of it, so we introduced an
animation at the end of it that moves the tablet showing the videos on the wall, where it will always
be available. We also had other minor comments, such as missing sounds indicating success or failure in
some riddles, so players didn’t know if they had successfully completed the riddle or not. 

Every comment
or minor bug encountered has been noted down and solved. Other than that, every participant said it
was a funny and immersive experience, without any kind of motion-sickness except for a player that felt
slightly destabilized by the climbing.

## 🛠 Skills

C#, Unity Engine, Oculus Framework, Blender, Photoshop, Adobe Premiere Pro. Game design, development of the game mechanics from scratch using only the Unity engine and the basic Oculus Framework for Virtual Reality interactions (the popular VRTK was not allowed, as it had most of the common vr-mechanics already coded), realisation of the whole game scenes from scratch, using Unity Store's assets but only of single objects, not whole scenes. Premiere Pro for the realisation of the gameplay trailer. Tons of debugging experience made through this project :). Playtesting. Manual realisation of small assets with Blender. Photoshop for the realisation of some game materials and for the game's logo.

## 🔗 Links
[![portfolio](https://img.shields.io/badge/my_portfolio-000?style=for-the-badge&logo=ko-fi&logoColor=white)](https://github.com/EliaFantini/)
[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/-elia-fantini/)
