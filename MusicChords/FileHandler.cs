using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicChords
{
    class FileHandler 
    {
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