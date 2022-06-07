# ImageGenerator

Use it if you want to ugly your images. :)

## Generator In Action

Program can generate image based on target, but with given pattern.

For example we have this target:

<img src="https://sun9-23.userapi.com/s/v1/ig2/R8VYayd56MMRbZvCtv-DodacX0QfcwoIE0S8FHLPazWBtDCzy_A_f2fWYRZUzG3a8ph1B0GNaITrxr42yUxKqgdO.jpg?size=1024x768&quality=95&type=album" width="250">

And our pattern is just a square.

So we get this result:

<img src="https://sun9-6.userapi.com/s/v1/ig2/9Wuc6veLxNOSepJvCa17PZf8sRGOozh6sJa_gMX_Uf7Jb1MZgGmMxEJ6GFkaVr3uylCd4UTidchhequYvMv3biKY.jpg?size=1024x768&quality=95&type=album" width="250">

Lets try something more interesting, like hearts.
Same target, but pattern is a heart.

And we have this result:

<img src="https://sun9-52.userapi.com/s/v1/ig2/pHwPlYvKdPjS4ezXrz6fvczNHkSPc2UtVbrdFwpntXsdwzf0hwXGddJpIshYnHZD3Jkp6dUG_jS9OchfS4Xjkfjw.jpg?size=1024x768&quality=96&type=album" width="250">

Actually, you may notice, that images don't have small details, but the reason of it is time limitation and settings. In this case of patterns max and min scale (you can set up them in settings).

# Doing You Own

If you want to do your own, you need patterns and target (you can add image to start from, but it isn't necessarily).

Patterns can be anything, but i recommend you select gray-scale images.

Put patterns in Images/Patterns, and target in Images/target.png (if you want you can change path to target in settings).

ALL images need to be same format (.jpg, .png, .bmp, etc), otherwise it may lead to errors.
