# FSharp.ChordParser

I created this for my dad to allow him to transpose chords and change lyrics to uppercase (and it was also a great opportunity for me to learn FParsec).

## Fabulous UI
This simple UI was created using [Fabulous](https://fsprojects.github.io/Fabulous/) which uses Xamarin.Forms. It currently targets WPF for Windows, but Fabulous also supports iOS and Android.
![image](https://user-images.githubusercontent.com/1030435/144350640-2917ffd6-0341-4c59-8ce0-ec11bee3f9ac.png)

## Console
**Command line synax:**
- ChordParser `{path to .txt file}`
- ChordParser `{path to .txt file} {transpose semitones (int)} {preferred accidental (#|b)}`

To try it out against the sample txt file (Song.txt), just run the app; it will output "Song ChordParser.txt". 

![image](https://user-images.githubusercontent.com/1030435/140804949-24957862-9ab6-41f4-bd22-8cdc51356d03.png)
