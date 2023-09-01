using System;
using System.Collections.Generic;

namespace MusicChords
{
    // A Song consists of a group of sections, which contains a group of measures, which contains a group of chords.
    class Song 
    {
        public struct Section {
            public List<Measure> measures;
            int sectionNum;

            public Section(int sectionNum) {
                measures = new List<Measure>();
                this.sectionNum = sectionNum;
            }

            public override string ToString()
            {
                string output = $"Section {sectionNum}: \n";
                measures.ForEach((measure) => output += measure.ToString());

                return output;
            }
        }

        public struct Measure
        {
            public List<Chord> bar;
            int measureNum;

            public Measure(int num)
            {
                bar = new List<Chord>();
                measureNum = num;
            }

            public override string ToString()
            {
                string output = "\tMeasure " + measureNum + ": \n";

                foreach(Chord chord in bar)
                    output += "\t\t" + chord.ToString() + "\n";

                return output;
            }
        }

        private Dictionary<string, float> TIME_SPACERS = new Dictionary<string, float>() 
        {
            { "=", 2 }, { "-", 1 }, { "*", 0.5f }, { "'", 0.25f }
        };
         
        public List<Section> leadSheet { get; }
        public int totalMeasures { get; }
        public Tuple<int, int> timeSig { get; }
        public Chord keySig { get; }

        public Song(FileHandler fileData)
        {
            timeSig = fileData.timeSig;
            keySig = fileData.keySig;

            leadSheet = new List<Section>();

            int sectionCount = 0, measureCount = 0;
            Section tempSection = new Section(sectionCount);
            for(int i = 1; i < fileData.songLines.Count; i++)
            {
                string currLine = fileData.songLines[i];

                if(i != 0 && currLine == "")
                {
                    // End of previous section, start of new section
                    leadSheet.Add(tempSection);
                    
                    sectionCount++;
                    tempSection = new Section(sectionCount);
                }
                else 
                {
                    // Line is a measure
                    string[] measureTokens = currLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    float spacerValue = ParseTimeSpacer(measureTokens);

                    // Check if measure has correct number of tokens
                    if(measureTokens.Length != timeSig.Item1 / spacerValue)
                    {
                        throw new Exception($"In Measure {i}: expected {timeSig.Item1 / spacerValue} tokens." +
                                            $"Received {measureTokens.Length} tokens.");
                    }

                    Measure currMeasure = new Measure(measureCount);
                    currMeasure.bar.AddRange(ParseMeasure(measureTokens, spacerValue));
                    tempSection.measures.Add(currMeasure);
                    
                    measureCount++;
                }
            }

            // Add last section
            leadSheet.Add(tempSection);
            totalMeasures = measureCount;
        }

        public Section GetSection(int section)
        {
            // Sections start count from from 1
            return leadSheet[section-1];
        }
        
        public Measure GetMeasure(int measure)
        {
            int measureCount = 0;
            foreach(Section section in leadSheet) {
                int sectionEnd = measureCount + (section.measures.Count-1);
                if(measure <= sectionEnd)
                    return section.measures[measure - measureCount];
                
                measureCount += section.measures.Count; 
            };

            throw new IndexOutOfRangeException("Invalid measure number.");
        }

        public override string ToString()
        {
            string output = "";

            foreach (Section section in leadSheet)
                output += section.ToString();

            return output;
        }

        private float ParseTimeSpacer(string[] measureTokens)
        {
            float spacerValue = 0.0f;
            foreach(string token in measureTokens) 
            {
                if(!TIME_SPACERS.ContainsKey(token)) continue;

                // Mixed time spacers in a single measure
                if(spacerValue != 0.0f && spacerValue != TIME_SPACERS[token])
                    throw new Exception("Multiple time spacers detected in a single measure. Avoid mixing time spacers.");
                
                spacerValue = TIME_SPACERS[token];
            }

            // No valid spacer characters were found
            if(spacerValue == 0.0f) 
                throw new Exception("No valid spacer characters were found. Refer to the README file for a list of valid spacers."); 

            // Can't evenly divide measure
            if(spacerValue == 2.0f && timeSig.Item1 % 2 == 1)
                throw new Exception("2-beat spacer used with an odd time signature. Can't evenly divide measure. Please use a smaller time spacer.");    

            return spacerValue;
        }

        private List<Chord> ParseMeasure(string[] measureTokens, float spacerValue)
        {
            List<Chord> chordList = new List<Chord>();

            for(int i = 0; i < measureTokens.Length; i++)
            {
                if(IsParserCharacter(measureTokens[i])) continue;

                double beat = i * spacerValue;

                chordList.Add(new Chord(measureTokens[i], beat));
            }

            return chordList;
        }

        private bool IsParserCharacter(string item)
        {
            string[] spacerChars = {"=", "-", "*", "'"};

            return Array.Exists(spacerChars, (c) => c == item);
        }
    }
}