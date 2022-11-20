using System;
using System.Collections.Generic;

namespace MusicChords
{
    class FileHandler 
    {
        /* EXPECTED FILE FORMAT:
         *  ___________________
         * || 32 4/4 Fm        || Line 1: NumMeasures | Time Signature | Key Signature
         * || Gm7 - FM7 -      || Line 2-EndOfFile: Chord progression, 1 line per measure
         * || EbM7 - Dm7 -     || NOTE: Assumes songs are in 4/4 time
         * || Gm7 - - FM7      ||
         * || [...]            ||
         */

        public int numBars { get; }
        public Tuple<int, int> timeSig { get; }
        public Chord keySig { get; }
        public List<string> barList { get; }

        public FileHandler(string filepath)
        {
            // OPEN/READ FILE
            List<string> lines = System.IO.File.ReadAllLines(filepath).ToList<String>();


            // PARSING HEADER LINE
            // 0 : NumBars
            string[] headerLine = lines[0].Split(' ');
            int temp;
            if(!Int32.TryParse(headerLine[0], out temp))
            {
                throw new TypeAccessException("Incorrect heading format. Measure count should be a number.");
            }
            numBars = temp;

            // 1 : TimeSig X/X
            int top, bottom;
            if(!Int32.TryParse(headerLine[1].Substring(0, 1), out top) || !Int32.TryParse(headerLine[1].Substring(2, 1), out bottom)) 
            {
                throw new TypeAccessException("Incorrect heading format. Time signature should contain two numbers.");
            }
            timeSig = new Tuple<int, int>(top, bottom);

            // 2 : KeySig
            keySig = new Chord(headerLine[2]);


            // REST OF FILE: CHORDS
            lines.RemoveAt(0);
            barList = new List<string>();
            barList.AddRange(lines);
            
            checkNumBars();
        }

        private void checkNumBars() {
            int count = 0;

            barList.ForEach((item) => {
                if(item != "")  count++;
            });

            if(count != numBars) {
                throw new IndexOutOfRangeException($"Number of bars in file does not match given bar count. File contains {count} bars.");
            }
        }
    }
}