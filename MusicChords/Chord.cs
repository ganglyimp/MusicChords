using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MusicChords
{
    /* Representation of Notes
     *  |   |   | |   |   |   |   | |   | |   |   |
     *  |   |   | |   |   |   |   | |   | |   |   |
     *  |   | 1 | | 3 |   |   | 6 | | 8 | | 10|   |
     *  |   |___| |___|   |   |___| |___| |___|   |
     *  |  0  |  2  |  4  |  5  |  7  |  9  |  11 |
     *  |_____|_____|_____|_____|_____|_____|_____|___ __ _
     *  
     *  C = 0; Db = 1; D = 2; etc...
     *  
     *  Major: M || Minor: m
     *  Diminished: dim, o, °
     *  Half-Dim: ø, O, 0
     *  Augmented: aug, +
     *  Major 7: t, Δ, ^
     *  6/9: 69, 6-9, 6/9
     *  Suspended: sus
     *  Add[number]: add
     *  
     *  Any chord mods must be placed within parentheses ex. CbM7(b5#11)
     */

    class Chord
    {
        private enum TwelveTones { C, Db, D, Eb, E, F, Gb, G, Ab, A, Bb, B }; 
        private const string CHORD_REGEX = @"([A-G]{1}[b#]?)((?:M|m|dim|o|°|ø|O|0|aug|\+|t|Δ|\^|6\/9)?[1-9]{0,2})(\((?:(?:b|#|add|sus)[1-9]{1,2})*\))?(\/[A-G]{1}[b#]?)?";
        private Regex SCALE_REGEX = new Regex(@"(M|m|dim|o|°|ø|O|0|aug|\+|t|Δ|\^|6\/9)?([1-9]{0,2})");
        private Dictionary<string, List<string>> MUSIC_SCALES = new Dictionary<string, List<string>>
        {
            // Major
            { "M", new List<string> {"1", "3", "5"} },
            // Minor
            { "m", new List<string> {"1", "b3", "5"} },
            // Diminished
            { "dim", new List<string> {"1", "b3", "b5"} },
            { "o", new List<string> {"1", "b3", "b5"} },
            { "°", new List<string> {"1", "b3", "b5"} },
            // Half-Diminished
            { "ø", new List<string> {"1", "b3", "b5", "b7"} },
            { "O", new List<string> {"1", "b3", "b5", "b7"} },
            { "0", new List<string> {"1", "b3", "b5", "b7"} },
            // Augmented
            { "aug", new List<string> {"1", "3", "#5"} },
            { "+", new List<string> {"1", "3", "#5"} },
            // Major 7
            { "t", new List<string> {"1", "3", "5", "7"} },
            { "Δ", new List<string> {"1", "3", "5", "7"} },
            { "^", new List<string> {"1", "3", "5", "7"} },
            // 6/9
            { "6/9", new List<string> {"1", "3", "5", "6", "9"} },
            { "6-9", new List<string> {"1", "3", "5", "6", "9"} },
            // Dom - default
            { "dom", new List<string> {"1", "3", "5"}}
        };

        public string name { get; }
        public List<int> notes { get; }
        public double beat { get; }

        public Chord(string name, double beat)
        {
            Regex chordRegex = new Regex(CHORD_REGEX);
            if(!chordRegex.IsMatch(name)) throw new ArgumentException($"Could not parse given symbol: {name}");

            this.beat = beat;            
            this.name = name;
            this.notes = new List<int>();

            List<string> splitSymbols = chordRegex.Split(name).Where(s => s != String.Empty).ToList();

            int root = NoteToNum(splitSymbols[0]);
            string scale = "dom";
            int topNote = 5;
            List<string> mods = new List<string>();
            int bassNote = -1;

            // Chord symbol is a simple triad
            if(splitSymbols.Count < 2)
            {
                SetNotesInChord(root, scale, topNote, mods, bassNote);
                return;
            }

            for(int i = 1; i < splitSymbols.Count; i++) 
            {
                string currToken = splitSymbols[i];

                // Token is a mod
                if(currToken.StartsWith('(')) 
                {
                    mods = GetMods(currToken);
                }
                // Token is a slash symbol
                else if(currToken.StartsWith('/')) 
                {
                    bassNote = NoteToNum(currToken.Substring(1));
                }
                // Token is a scale
                else 
                {
                    List<string> splitScale = SCALE_REGEX.Split(currToken).Where(s => s != String.Empty).ToList();

                    if(splitScale.Count > 1) 
                    {
                        scale = splitScale[0];
                        Int32.TryParse(splitScale[1], out topNote);
                    }
                    else 
                    {
                        // Check whether we have a top note or a scale
                        if(Int32.TryParse(splitScale[0], out topNote)) continue;
                        scale = splitScale[0];
                    }
                }
            }

            SetNotesInChord(root, scale, topNote, mods, bassNote);
        }

        public override string ToString()
        {
            string chord = $"{name} ({beat}): ";
            foreach (int note in notes)
                chord += NumToNote(note) + " ";

            return chord;
        }


        //HELPER FUNCTIONS
        private List<string> GetMods(string modList)
        {
            List<string> mods = new List<string>();

            int startInd = modList.IndexOf('(');
            int endInd = modList.IndexOf(')');

            // No valid mod list in symbol. Return empty list. 
            if (startInd == -1 || endInd == -1) return mods;

            string modsWithoutParens = modList.Substring(startInd+1, endInd-1);

            string[] parts = Regex.Split(modsWithoutParens, @"(?=b|#|add|sus)");
            foreach (string part in parts)
                if(part.Length > 0)
                    mods.Add(part);

            return mods;
        }

        private void SetNotesInChord(int root, string scale, int topNote, List<string> mods, int bassNote)
        {
            List<string> chordPositions = (MUSIC_SCALES.ContainsKey(scale)) ? MUSIC_SCALES[scale] : MUSIC_SCALES["dom"];

            //PARSE TOP NOTE
            if(topNote != -1)
            {
                string lastItem = chordPositions[chordPositions.Count-1];
                if(lastItem.Length > 1) lastItem = lastItem.Substring(1);

                int currPosition;
                if (topNote == 6)
                    chordPositions.Add("6");
                else if (Int32.TryParse(lastItem.ToString(), out currPosition))
                {
                    while (currPosition < topNote)
                    {
                        currPosition += 2;
                        chordPositions.Add(currPosition.ToString());
                    }
                }
            }

            //PARSE MODS
            if (!scale.Equals("M") && chordPositions.Contains("7")) //Special case: dom7 chord
                mods.Add("b7");

            foreach(string mod in mods)
            {
                //If it's an "add" mod, append to end
                int addInd = mod.LastIndexOf('d');
                if(addInd != -1)
                {
                    string position = mod.Substring(addInd + 1);
                    chordPositions.Add(position);
                    continue;
                }

                //If it's a "sus" mod, replace the third (if present) with the sus note
                int susInd = mod.LastIndexOf('s');
                if(susInd != -1) 
                {
                    string position = mod.Substring(susInd + 1);
                    int theThird = chordPositions.FindIndex(n => n.IndexOf('3') != -1);
                    if(theThird != -1) chordPositions[theThird] = position;
                    else chordPositions.Add(position);
                    continue;
                }

                //See if note already in chordPositions list : modify / add
                string pos = mod.Substring(1);
                bool posAdded = false;
                for(int i = 0; i < chordPositions.Count; i++)
                {
                    if (pos.Equals(chordPositions[i]))
                    {
                        chordPositions[i] = mod;
                        posAdded = true;
                    }
                }
                if (!posAdded)
                    chordPositions.Add(mod);
            }

            //CONVERT RELATIVE NOTES TO ACTUAL NOTE VALUES
            foreach (string note in chordPositions)
            {
                string flatSharp = "-";
                string theNote = note;
                if(note.IndexOfAny(new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' }) != 0)
                {
                    flatSharp = note.Substring(0, 1);
                    theNote = note.Substring(1);
                }

                int relPos = -1;
                if (Int32.TryParse(theNote, out relPos))
                    notes.Add(GetNoteFromPos(root, relPos, flatSharp));
            }

            //ADD SLASH CHORD
            if(bassNote != -1)
            {
                notes.Insert(0, bassNote);
            }
        }

        private int NoteToNum(string note)
        {
            TwelveTones noteNumber;
            if(Enum.TryParse(note, true, out noteNumber))
                return (int)noteNumber;
            
            // If not an edge case # or b, root note is likely invalid.
            if(note.Length < 2 && (note[1] != '#' || note[1] != 'b'))
                return -1;

            char root = note[0];
            bool isSharp = note[1] == '#';

            int rootValue = NoteToNum(root.ToString());
            if(rootValue == -1) return rootValue;
            return ((isSharp) ? ++rootValue : --rootValue) % 12;
        }

        private string NumToNote(int num)
        {
            if(!Enum.IsDefined(typeof(TwelveTones), num))
                return "NULL";

            return ((TwelveTones)num).ToString();
        }

        private int GetNoteFromPos(int root, int position, string flatSharp)
        {
            int[] majorScaleSteps = { 2, 2, 1, 2, 2, 2, 1 }; //Whole-step = 2; half-step = 1
            int note = root;

            int index = 0;
            int count = 1;

            while(count < position)
            {
                note = (note + majorScaleSteps[index]) % 12; 

                count++;
                index = (index + 1) % 7;
            }

            if (flatSharp.Equals("b"))
                note = (note - 1 < 0) ? 11 : (note - 1) % 12;
            else if (flatSharp.Equals("#"))
                note = (note + 1) % 12;

            return note;
        }
    }
}