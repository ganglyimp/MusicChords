using System;

namespace MusicChords
{
    class ChordsInterface 
    {
        static void Main(string[] args) 
        {
            //Console.Write("Input filepath: ");
            //string filename = Console.ReadLine();
            string filename = "sample-files/OverTheRainbow.txt";

            if(filename == null) return;
            FileHandler fileData = new FileHandler(filename);
            
            Song song = new Song(fileData);

            bool exitProgram = false;
            while (!exitProgram)
            {
                Console.WriteLine($"Currently loaded file: {filename}");

                Console.WriteLine();
                Console.WriteLine("[1] Display Entire Lead Sheet\n" +
                                  "[2] Display Chords From Specific Measure\n" +
                                  "[3] Load New File\n" +
                                  "[4] Exit\n");

                Console.Write("Input command #: ");
                int command = -1;
                try
                {
                    command = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nInvalid command.\n{e}\n");
                    continue;
                }


                if(command == 1)
                {
                    Console.WriteLine();
                    Console.WriteLine(song.ToString());
                }
                else if(command == 2)
                {
                    PrintChordFromMeasure(song);
                }
                else if(command == 3)
                {
                    Console.WriteLine();
                    Console.Write("Input filepath: ");
                    filename = Console.ReadLine() ?? "";

                    fileData = new FileHandler(filename ?? "");
                    song = new Song(fileData);
                }
                else if(command == 4)
                {
                    exitProgram = true;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid command.");
                }

                Console.WriteLine();
            }
        }

        private static void PrintChordFromMeasure(Song song) 
        {
            int command = -1;

            Console.WriteLine();
            Console.Write("Input measure #: ");

            try
            {
                command = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nInvalid number.\n{e}\n");
                return;
            }

            //Out of Range
            if(command < 1 || command > song.totalMeasures)
            {
                Console.WriteLine("\nNumber out of range.\n");
                return;
            }

            Console.WriteLine(song.GetMeasure(command).ToString());
        }
    }
}