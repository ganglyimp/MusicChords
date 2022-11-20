using System;
using System.Collections.Generic;

namespace MusicChords
{
    class Song 
    {
        
        public struct Measure
        {
            public Chord[] bar;
            int measureNum;

            public Measure(int timeSig, int num)
            {
                bar = new Chord[timeSig];
                measureNum = num;
            }

            public override string ToString()
            {
                string output = "Measure " + measureNum + ": \n";

                foreach(Chord chord in bar)
                    output += "\t" + chord.ToString() + "\n";

                return output;
            }
        }

        // Group measures into sections
        public Measure[] leadSheet { get; }

        public Song(FileHandler fileData)
        {
            leadSheet = new Measure[fileData.numBars];
            string[,] chordList = fileData.chordArr;

            for(int i = 0; i < fileData.numBars; i++)
            {
                Measure currMeasure = new Measure(fileData.timeSig, (i+1));

                for(int j = 0; j < fileData.timeSig; j++)
                {
                    Chord currChord = new Chord(chordList[i, j]);
                    currMeasure.bar[j] = currChord;
                }

                leadSheet[i] = currMeasure;
            }
        }

        //Measures start counting from 1
        public Measure GetMeasure(int measure)
        {
            return leadSheet[measure-1];
        }

        public override string ToString()
        {
            string output = "";

            foreach (Measure measure in leadSheet)
                output += measure.ToString();

            return output;
        }
    }
}