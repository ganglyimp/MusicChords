# MusicChords
This is a very basic, barebones program that can read in a formatted txt file that contains a list of chord symbols (using lead sheet notation) and calculate what notes belong to each chord. Each note is given a numeric representation in the range from 0 - 11, with C being represented as 0. 

## Text File Format

```
32
EbM7 - Cm7 -
Gm7 - - Eb7
AbM7 - D7 -
... 
```
The first line should contain the total number of measures in the song. Every line following that should contain a list of chords in each measure. Each line should only contain the chords in a single measure (with each chord separated with a space). This program currently assumes the inputted song is in 4/4 time, so there should only be 4 chords per measure. If a beat doesn't possess a chord change, use "-" to specify a blank chord.

### Currently Supported Chord Symbols
Basic Format of a Chord Symbol: `[Root][Scale][Top Note]([Modifiers])/[Bass Note]`

All of these elements are optional except for the chord's root. When only a chord's root is inputted, the program will assume that the chord is a major triad. With the exception of modifiers, the program will only read in one of each element.

#### Definitions:
| Chord Portion   | Definition                                                                                                   | Example          |
| ----------------| -------------------------------------------------------------------------------------------------------------|------------------|
| Root            | Harmonic base of the chord; should be one of the 12 notes in the chromatic scale (sharps # not recognized)   | **C**m7, **Db**9 |
| Scale           | The scale the chord is based on (ex. minor, major, diminished, etc.)                                         | C**m**7, E**sus**9 |
| Top Note        | A number; denotes if the chord is a triad, 7th chord, or an extended chord                                   | AbM**7**, Bdim**9** |
| Modifiers       | Altered notes or added notes; must be enclosed in parentheses                                                | C13(**b9#11**), Fm(**add11**) |
| Bass Note       | A slash chord, denoted with a "/" followed by the note's name; denote if the bass note is something other than root | Gsus/**E**, Cm/**Ab** |

#### Supported Scale Symbols
| Scale           | Symbols      |
|-----------------|--------------|
| Major           | M            |
| Minor           | m            |
| Diminished      | dim, o, °    | 
| Half-Diminished | ø, O, 0      |
| Augmented       | aug, +       |
| Major 7         | t, Δ, ^      |
| 6/9             | 69, 6-9, 6/9 |
| Suspended       | sus          |

## Program Format
| Class           | Function                                                                            |
|-----------------|-------------------------------------------------------------------------------------|
| FileHandler     | Handles reading in the txt file and separating the chord symbols into a 2D array    |
| Song            | Serves as a container for Chord objects; contains an array of measures              |
| Chord           | Contains info about a chord, including notes in the chord represented as an integer |
| ChordsInterface | Simple terminal-based interface to run the program                                  |
