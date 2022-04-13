Hey!

You might wonder what this folder is about.
Some of our low poly packs contain these shaders as a free bonus :)

These shaders are optimized for our models. Most of our models store their colors in a colorscheme-file. By default Unity reads the colors once per pixel on the screen. These shaders read the colors once per vertex, which results in way less texture look-ups.

Our models work just fine with Unity's default and other shaders (just assign the colorscheme-texture).