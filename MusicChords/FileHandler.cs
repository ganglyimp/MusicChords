using System;
using System.Collections.Generic;
using System.Linq;

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

        public Tuple<int, int> timeSig { get; }
        public Chord keySig { get; }
        public List<string> songLines { get; }

        public FileHandler(string filepath)
        {
            if(filepath == "")
                throw new FileLoadException("Filepath is null. Please input a valid filepath.");

            songLines = new List<string>();
            List<string> lines = System.IO.File.ReadLines(filepath).ToList<String>();

            // PARSING HEADER LINE
            string[] headerLine = lines[0].Split(' ');
            
            // Time Signature
            int top, bottom;
            if(!Int32.TryParse(headerLine[0].Substring(0, 1), out top) || !Int32.TryParse(headerLine[0].Substring(2, 1), out bottom)) 
            {
                throw new TypeAccessException("Incorrect heading format. Time signature should contain two numbers.");
            }
            timeSig = new Tuple<int, int>(top, bottom);

            // Key Signature
            keySig = new Chord(headerLine[1], 0.0);

            // REST OF THE FILE
            lines.RemoveAt(0);
            songLines.AddRange(lines);
        }
    }
}