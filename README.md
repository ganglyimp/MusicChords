# MusicChords

This is a very basic, barebones program that can read in a formatted txt file that contains a list of chord symbols (using lead sheet notation) and calculate what notes belong to each chord. Each note is given a numeric representation in the range from 0 - 11, with C being represented as 0. 

## Text File Format

```
32 4/4 EbM

EbM7 - Cm7 -
Gm7 - - Eb7
AbM7 - D7 -
Gm7 - C7 -

Fm7 - Db7 -
EbM7 - Gm7 C7 
Fm7 - Bb7 -
EbM7 - Fm7/Bb -
... 
```

The text file is split into two parts: the header line and chord sections.

### **Header Line**

The header line consists of three elements, delimited by a space. This line contains basic metadata about the file.

| Name           | Description                                          | Example |
|----------------|------------------------------------------------------|---------|
| Measure Count  | Total number of measures in the file.                | 32      |
| Time Signature | Follows music notation rules for time signatures.    | 4/4     |
| Key Signature  | Key signature for song. Only accepts major or minor. | EbM     |

### **Chord Sections**

An empty line marks the start of a new chord sections. A song consists of one or more chord sections. Each chord section contains a group of measures, each delimited by a new line. And each measure contains a combinaton of chord symbols and time spacers. The provided example above consists of two chord sections, each consisting of four measures.

---

## Time Spacers

Time spacers more or less function as a musical rest. Only one kind of time spacer may be used in each individual measure. The value of the time spacer also determines the "duration" of the chord symbol. For example, if you use a 1 beat spacer in a measure, then chord symbols will also all take up one beat. The value of the time spacers are dependant on the time signature provided in the header. In a 4/4 time signature, 1 beat spacers are worth a quarter note, but in a 6/8 time signature, they're worth an eighth note. All measure elements must add up to align to the time signature.

| Symbol | Description                                                               |
|--------|---------------------------------------------------------------------------|
| =      | 2 beat spacer. In a 4/4 time signature, it represents a half note.        |
| -      | 1 beat spacer. In a 4/4 time signature, it represents a quarter note.     |
| *      | 1/2 beat spacer. In a 4/4 time signature, it represents an eighth note.    |
| '      | 1/4 beat spacer. In a 4/4 time signature, it represents a sixteenth note. |

### Examples

| Time Signature | Markup                      | Music Notation                   |
|----------------|-----------------------------|----------------------------------|
| 4/4            | `Fm7 - Bb7 -`               | ![Example 1](/RM_images/Ex1.png) |
| 4/4            | `Gm7 * * * C7 * Ab Bb7`     | ![Example 2](/RM_images/Ex2.png) |
| 5/4            | `Ebm7 - - Gb7(#5) -`        | ![Example 3](/RM_images/Ex3.png) |
| 6/8            | `Eb7(#9) - - D7(b13#9) - -` | ![Example 4](/RM_images/Ex4.png) |

---

## Currently Supported Chord Symbols

Basic Format of a Chord Symbol: `[Root][Scale][Top Note]([Modifiers])/[Bass Note]`

All of these elements are optional except for the chord's root. When only a chord's root is inputted, the program will assume that the chord is a major triad. With the exception of modifiers, the program will only read in one of each element.

### **Definitions**

| Chord Portion   | Definition                                                                                                   | Example          |
| ----------------| -------------------------------------------------------------------------------------------------------------|------------------|
| Root            | Harmonic base of the chord; should be one of the 12 notes in the chromatic scale (sharps # not recognized)   | **C**_m7_, **Db**_9_ |
| Scale           | The scale the chord is based on (ex. minor, major, diminished, etc.)                                         | _C_**m**_7_, _E_**sus**_9_ |
| Top Note        | A number; denotes if the chord is a triad, 7th chord, or an extended chord                                   | _AbM_**7**, _Bdim_**9** |
| Modifiers       | Altered notes or added notes; must be enclosed in parentheses                                                | _C13_(**b9#11**), _Fm_(**add11**) |
| Bass Note       | A slash chord, denoted with a "/" followed by the note's name; denote if the bass note is something other than root | _Gsus/_**E**, _Cm/_**Ab** |

### **Supported Scale Symbols**

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

---

## Program Format

| Class           | Function                                                                            |
|-----------------|-------------------------------------------------------------------------------------|
| FileHandler     | Handles reading in the txt file and separating the chord symbols into a 2D array    |
| Song            | Serves as a container for Chord objects; contains an array of measures              |
| Chord           | Contains info about a chord, including notes in the chord represented as an integer |
| ChordsInterface | Simple terminal-based interface to run the program                                  |
