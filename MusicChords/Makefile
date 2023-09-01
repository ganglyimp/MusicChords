.PHONY: MusicChords.cs 

all: run

MusicChords.exe: MusicChords.cs FileHandler.cs Song.cs Chord.cs
	@csc MusicChords.cs FileHandler.cs Song.cs Chord.cs

clean:
	@rm -f MusicChords.exe 

run: MusicChords.exe 
	@mono MusicChords.exe