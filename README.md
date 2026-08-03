# SteelMongers

Formerly developed under the name, JJ&Son.

An archive of my final project for my studies at Raffles University's Diploma in Digital Game Art for showcasing purposes.
* https://raffles-university.edu.my/portfolio/steel-mongers/

[<img width="300" src="https://github.com/user-attachments/assets/8e0637f8-b7aa-4cb9-8ef1-099162744a9f">](https://jeremiah67.itch.io/steelmongers)

Screenshots:
--------------
Gameplay: https://youtu.be/zCvGOXacDqY

<img width="200" src="https://github.com/user-attachments/assets/87b2296a-9cb1-4b20-8bcf-6c40d54bb347" />
<img width="200" src="https://github.com/user-attachments/assets/44a83bce-ab02-4481-a8f9-8916e303f953" />
<img width="200" src="https://github.com/user-attachments/assets/d39d78fc-a2ca-44ee-b62a-90d3d02dd094" />
<img width="200" src="https://github.com/user-attachments/assets/b6aa2bb9-26d3-4550-b952-4924b7a597c3" />


Project Portfolio:
--------------
To anyone who prefers looking into visuals you may check out from this Behance page.

How to play through Unity's Editor:
--------------
- Recommended version: 2021.3.33f
  - This is because new versions of Unity are showing log errors that were not present in earlier versions for some reason.
* Open the "MainMenu" scene, this can be accessed through Assets/Scenes/Levels within the project's file path.

Controls:
--------------
<details>
  <summary>Main menu</summary>

- WD or Left / Right Arrows (Change button selection)
- Enter (Select desired button / Close credits)
</details>

<details>
  <summary>Gameplay</summary>

- WASD / Arrow Keys (Movement)
- Left Mouse Click (Shoot)
- Hold Right Mouse (Slow aim)
- Space
  - Press (Jump)
  - Hold (Fly)
- ESC (Pause)
</details>

<details>
  <summary>Debug (Gameplay)</summary>

- Enter + Backspace - (Unlock / Lock debug mode)
  - E - Skip mission (This will not work during the boss fight)
  - I - Kill Player
  - L - Invincibility (May need to restart level if you want it off)
  - T - Debug camera / Play camera
    - Y - Cycle debug camera positions
</details>

<details>
  <summary>(Unfinished) Xbox controls</summary>

- Xbox controls (Only works during gameplay)
  - Left Joystick (Movement)
  - Button A (Shoot)
</details>

Bugs:
--------------
* Boosting will play a particle animation and an associated sound effect regardless of what state Enum is in (i.e when Enum is not moving / when Enum plays a hard landing animation).
- Enum's Inverse Kinematics problems
  - Enum's right arm will either go through himself at certain angles.
  - Enum's right arm will move weirdly when shooting at a certain angle.
* P's neck will look up higher everytime Enum gets closer to it.
* There are frame rate spikes from time to time, my guess was its either the 3D Canvas or the reflection probes.

Credits
--------------
* Music: [Fesliyan Studios](https://www.fesliyanstudios.com/)
* Menu SFX: [jsfxr](https://sfxr.me/) browser port by [Eric Fredricksen](https://github.com/grumdrig) & [Chris McCormick](https://github.com/chr15m)
* Additional SFX: [Pixabay](https://pixabay.com/)
* Fonts: [Typodermic Fonts](https://typodermicfonts.com/) : [Venus Rising](https://www.dafont.com/venus-rising.font) - [Stormfaze](https://www.dafont.com/stormfaze.font) - [Neuropol](https://www.dafont.com/neuropol.font) - [Unispace](https://www.dafont.com/unispace.font)

Tools used:
--------------
* Game developed using the [Unity Engine](https://unity.com/)
* Models created through [Blender](https://www.blender.org/)
* [Mixamo.com](https://www.mixamo.com/) for sample animations
* Programmed using [VSCode](https://code.visualstudio.com/)
* [GIMP](https://www.gimp.org/) for image manipulation

Contact
--------------
If you need a Unity C# programmer or a Blender3D modeller, feel free to contact me at darccstars@protonmail.com!
