using System;

namespace MusicChords
{
    class FileHandler 
    {
        /* EXPECTED FILE FORMAT:
         *  ___________________
         * || 32               || Line 1: Number of measures
         * || Gm7 - FM7 -      || Line 2-EndOfFile: Chord progression, 1 line per measure
         * || EbM7 - Dm7 -     || NOTE: Assumes songs are in 4/4 time
         * || Gm7 - - FM7      ||
         * || [...]            ||
         */

        public int numBars { get; }
        public int timeSig { get; }
        public string[,] chordArr { get; }

        public FileHandler(string filepath)
        {
            // Open/Read File
            string[] lines = System.IO.File.ReadAllLines(filepath);

            // Header Line: NumBars | TimeSig | KeySig
            string[] headerLine = lines[0].Split(' ');

            // Rest of File: Chords
            string[,] tempChords = new string[numBars, 4];
            for(int i = 0; i < numBars; i++)
            {
                string curr = lines[i + 1];
                string[] split = curr.Split(' ');


                for(int j = 0; j < timeSig; j++)
                {
                    tempChords[i,j] = split[j];
                }
            }

            chordArr = (string[,])tempChords.Clone();
        }

        private void ParseHeader(string[] headerLine) 
        {
            // 0 : NumBars
            if(Int32.TryParse())
        }
    }
}