# FSharp.ChordParser

I created this for my dad to allow him to transpose chords and change lyrics to uppercase (and it was also a great opportunity for me to learn FParsec).
It is currently a command line utility, but please feel free to contribute a GUI of your choice!

## Command line syntax:
- ChordParser `{path to .txt file}`
- ChordParser `{path to .txt file} {transpose semitones (int)} {preferred accidental (#|b)}`

To try it out against the sample txt file (Song.txt), just run the app; it will output "Song ChordParser.txt". 
