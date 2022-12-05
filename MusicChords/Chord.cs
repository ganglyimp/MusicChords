using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MusicChords
{
    //TODO: Allow for sharps. Store more info about scale position (2nd, 5th, flat 7, etc)

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
        public string name { get; }
        public List<int> notes { get; }
        public double beat { get; }

        public Chord(string n, double beat)
        {
            this.beat = beat;
            
            name = n;
            notes = new List<int>();

            int root;
            string scale;
            int topNote;
            List<string> mods;
            int bassNote;

            if(NoteToNum(n) != -1)
            {
                //Simple Triad
                root = NoteToNum(name);
                scale = "dom";
                topNote = 5;
                mods = new List<string>();
                bassNote = -1;

                SetNotesInChord(root, scale, topNote, mods, bassNote);

                return;
            }

            //GET ROOT
            root = GetRoot(n);

            // Note: root length can be one or two chars (ex. C and Cb)s
            n = (NumToNote(root).Length == 2) ? n.Substring(2) : n.Substring(1);

            //CHECK FOR SCALE
            scale = GetScale(n);

            if (scale.Equals("6/9"))
                n = n.Substring(n.IndexOf('9') + 1);
            else if(!scale.Equals("dom"))
                n = n.Substring(scale.Length);

            //CHECK FOR TOP NOTE
            topNote = GetTopNote(n);

            int modIndex = n.IndexOf('(');
            if (modIndex != -1)
                n = n.Substring(modIndex);

            //CHECK FOR MODS
            mods = GetMods(n);

            if(mods.Count > 0)
                n = n.Substring(n.IndexOf(')'));

            //SLASH CHORD?
            int slashIndex = n.IndexOf('/');

            if (slashIndex != -1)
                bassNote = NoteToNum(n.Substring(slashIndex + 1));
            else
                bassNote = -1;

            //GET NOTES IN CHORD
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
        private int GetRoot(string chord)
        {
            string rootName = (chord[1].Equals('b')) ? chord.Substring(0, 2) : chord.Substring(0, 1);

            return NoteToNum(rootName);
        }

        private string GetScale(string chord)
        {
            int nextNum = chord.IndexOfAny(new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '(', '/' });

            //There is no top note or other modifications specified in chord.
            if (nextNum == -1)
                return chord;
            //Scale specified
            else if (nextNum != 0)
                return chord.Substring(0, nextNum);
            //chord[0] is a number
            else
            {
                //Check if 6/9 chord
                if (chord.StartsWith("69") || chord.StartsWith("6/9") || chord.StartsWith("6-9"))
                    return "6/9";
                //Dominant chord (no scale specified)
                else
                    return "dom";
            }
        }

        private int GetTopNote(string chordSymbol)
        {
            int index = chordSymbol.IndexOfAny(new char[] { '(', '/' });

            if (index == -1)
            {
                int topNote;
                if (Int32.TryParse(chordSymbol, out topNote))
                    return topNote;
            }
            else if (index > 0)
            {
                int topNote;
                if (Int32.TryParse(chordSymbol.Substring(0, index), out topNote))
                    return topNote;
            }
                

            return -1;
        }

        private List<string> GetMods(string chordSymbol)
        {
            List<string> mods = new List<string>();

            int startInd = chordSymbol.IndexOf('(');
            int endInd = chordSymbol.IndexOf(')');

            if (startInd == -1 || endInd == -1)
                return mods;

            chordSymbol = chordSymbol.Substring(startInd+1, endInd-1);

            string[] parts = Regex.Split(chordSymbol, @"(?=b|#|add)");
            foreach (string part in parts)
                if(part.Length > 0)
                    mods.Add(part);

            return mods;

        }

        private void SetNotesInChord(int root, string scale, int topNote, List<string> mods, int bassNote)
        {
            List<string> chordPositions;

            //PARSE SCALE
            switch(scale)
            {
                // Major
                case "M":
                    chordPositions = new List<string> { "1", "3", "5" };
                    break;
                // Minor
                case "m":
                    chordPositions = new List<string> { "1", "b3", "5" };
                    break;
                // Diminished
                case "dim":
                case "o":
                case "°":
                    chordPositions = new List<string> { "1", "b3", "b5"};
                    if (topNote == -1) { chordPositions.Add("6"); }
                    break;
                // Half-Diminished
                case "ø":
                case "O":
                case "0":
                    chordPositions = new List<string> { "1", "b3", "b5", "b7" };
                    break;
                // Augmented
                case "aug":
                case "+":
                    chordPositions = new List<string> { "1", "3", "#5" };
                    break;
                // Major 7
                case "t":
                case "Δ":
                case "^":
                    chordPositions = new List<string> { "1", "3", "5", "7" };
                    break;
                // 6/9
                case "6/9":
                    chordPositions = new List<string> { "1", "3", "5", "6", "9" };
                    break;
                // Suspended
                case "sus":
                    chordPositions = new List<string> { "1", "4", "5" };
                    break;
                default:
                    chordPositions = new List<string> { "1", "3", "5" };
                    break;
            }

            //PARSE TOP NOTE
            if(topNote != -1)
            {
                String lastItem = chordPositions[chordPositions.Count-1];
                int tempInd = lastItem.IndexOfAny(new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9'});
                lastItem = lastItem.Substring(tempInd);

                int currPosition;
                if (topNote == 6)
                    chordPositions.Add("6");
                else if (Int32.TryParse(lastItem, out currPosition))
                {
                    while (currPosition < topNote)
                    {
                        currPosition += 2;
                        chordPositions.Add(currPosition.ToString());
                    }
                }
            }

            //PARSE MODS
            if (!scale.Equals("M") && chordPositions.Contains("7"))
                mods.Add("b7");

            if(mods.Count > 0)
            {
                foreach(string mod in mods)
                {
                    //If it's an "add" mod, append to end
                    int addInd = mod.LastIndexOf('d');
                    if(addInd != -1)
                    {
                        string posi = mod.Substring(addInd + 1);
                        chordPositions.Add(posi);
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
            string[] twelveTones = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

            for (int i = 0; i < 12; i++)
                if (note.Equals(twelveTones[i]))
                    return i;

            return -1;
        }

        private string NumToNote(int num)
        {
            string[] twelveTones = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

            if (num < 0 || num > 11)
                return "NULL";

            return twelveTones[num];
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
                note = (note - 1 < 0) ? (12 - note) : note-1;
            else if (flatSharp.Equals("#"))
                note = (note + 1) % 12;

            return note;
        }
    }
}