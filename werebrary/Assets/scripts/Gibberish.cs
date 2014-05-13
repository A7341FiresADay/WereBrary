using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Gibberish {
	private string allWords;
	const int MAX_SENT = 16;	// Maximum length of gibberish sentences
	string finalReturnGibberish;

	public string FinalReturnGibberish{ get { return finalReturnGibberish; } }
	public Gibberish(string gibberishPool, int lengthOfGibberish)
	{
		allWords = gibberishPool;
		finalReturnGibberish = string.Empty;
		Markov markovChain = new Markov();		// Set up the Markov chain object
		//Console.Write("Enter the .txt file to Gibberfy: ");
		//string fileName = Console.ReadLine();
		string filePath = gibberishPool + ".txt";
		
		markovChain.CreateGraph(filePath);		// Create the chain from the corpus
		
		/*Console.Write("\nHow many lines? ");
		string reply = Console.ReadLine();
		int nLines = Convert.ToInt32(reply);
		Console.WriteLine("");*/
		for (int i = 0; i < lengthOfGibberish; i++)		// Generate 1-4 lines of gibberish
		{
			// Generate a sentence of gibberish and print it to the console
			string[] gibSent = markovChain.GenGibSent(MAX_SENT);
			for (int j = 0; j < gibSent.Length; j++)
			{
				if (gibSent[j] != null)
					finalReturnGibberish = finalReturnGibberish+ "..." + gibSent[j];
			}
		}
	}
	// Use this for initialization
	void Start () {
	
	}

	public static string StringFromPool(string gibberishPool)
	{
		string aw = gibberishPool;
		Markov markovChain = new Markov();		// Set up the Markov chain object
		//Console.Write("Enter the .txt file to Gibberfy: ");
		//string fileName = Console.ReadLine();
		string filePath = gibberishPool + ".txt";
		
		markovChain.CreateGraph(filePath);		// Create the chain from the corpus
		
		/*Console.Write("\nHow many lines? ");
		string reply = Console.ReadLine();
		int nLines = Convert.ToInt32(reply);
		Console.WriteLine("");*/
		string toReturn = "";
		for (int i = 0; i < Random.Range (1, 4); i++)		// Generate 1-4 lines of gibberish
		{
			// Generate a sentence of gibberish and print it to the console
			string[] gibSent = markovChain.GenGibSent(MAX_SENT);
			
			string firstChar = gibSent[0].Substring(0, 1).ToUpper();	// Cap first word
			string capitalized = firstChar
				+ gibSent[0].Substring(1, gibSent[0].Length-1);
			gibSent[0] = capitalized;
			
			for (int j = 1; i < gibSent.Length; j++)	// Print rest of words
			{
				if (gibSent[j] == null)
				{
					return toReturn;
				}
				else
					toReturn += " " + gibSent[i];
			}
			toReturn += ".";						// Put period at the end
		}

		return toReturn;
	}
		
	static string PrintSent(string[] sentWords)
	{
		string firstChar = sentWords[0].Substring(0, 1).ToUpper();	// Cap first word
		string capitalized = firstChar
			+ sentWords[0].Substring(1, sentWords[0].Length-1);
		sentWords[0] = capitalized;
		string outputFinal = string.Empty;
		
		for (int i = 1; i < sentWords.Length; i++)	// Print rest of words
		{
			if (sentWords[i] == null)
			{
				return null;
			}
			else
				string.Concat(outputFinal, ".....");
				//Debug.Log(" " + sentWords[i]);
		}
		return outputFinal;
	}
}
/* Markov Chain implements a level-one Markov chain of word bi-grams
	 * 
	 */
public class Markov
{
	Graph biGrams;		// Graph attribute holds the Markov Chain
	
	public Markov ()
	{
		biGrams = new Graph();	// Declare the Graph but don't initialize it yet
	}
	
	/* CreateGraph method
		 * Makes two passes through the text file whose path is passed in.
		 * First pass creates the Vertex array in the graph and counts number of different
		 * words in the text file.
		 * Second pass fills the transition matrix with counts of occurrances of row word
		 * followed by column word in the corpus.
		 * A special non-word "BEGIN-END" represents sentence boundaries,
		 * i.e., the first word in a sentence follows BEGIN-END, and the last word in a
		 * sentence is followed by BEGIN-END.
		 */
	public void CreateGraph(string filePath)
	{
		StreamReader input = null;
		try
		{
			// Get ready to read the corpus data file
			input = new StreamReader(filePath);
			
			// Read until out of data (first pass through the corpus)
			string line = "";
			char[] delimiters = { ' ' };
			
			biGrams.LoadVertex("BEGIN-END");	// Load up the sentence boundary token
			
			while ((line = input.ReadLine()) != null)	// Read file line by line
			{
				// Convert the letters to all lower case
				line = line.ToLower();
				
				// Handle punctuation by replacing it with whitespace
				line = line.Replace('.', ' ');
				line = line.Replace(',', ' ');
				line = line.Replace(';', ' ');
				line = line.Replace(':', ' ');
				line = line.Replace('?', ' ');
				line = line.Replace('!', ' ');
				line = line.Replace('"', ' ');
				line = line.Replace('(', ' ');
				line = line.Replace(')', ' ');
				line = line.Replace("--", " ");
				//line = line.Replace('-', ' ');	// Leave - in for hyphenated words
				
				// Split the line of text up
				string[] myWords = line.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);

				// Add words in the line to the vertex array (hash table)
				for (int i = 0; i < myWords.Length; i++)
				{
					biGrams.LoadVertex(myWords[i]);
				}
			}
			
			biGrams.FixBeginEndCount();
			
			//Debug.Log("Loaded " + biGrams.NumVertices + " words");
		}
		//catch (Exception ex)]
		catch(UnityException ex)
		{
			Debug.Log("Exception: " + ex.Message);
		}
		finally
		{
			if (input != null)
			{
				input.Close();
			}
		}
		
		biGrams.CreateMatrix();  // Now know how many vertices, so create transition matrix
		
		// Now ready for second pass to fill the matrix with bi-gram counts
		
		try
		{
			// Read in the data file again (second pass)
			input = new StreamReader(filePath);
			
			// Read until out of data
			string line = "";
			char[] delimiters = { ' ' };
			string oldWord = "BEGIN-END";
			string newWord = null;
			
			while ((line = input.ReadLine()) != null)
			{
				// Convert the string to all lower case
				line = line.ToLower();
				
				// Handle punctuation differently in this pass
				line = line.Replace(',', ' ');	// Remove , ; : "
				line = line.Replace(';', ' ');
				line = line.Replace(':', ' ');
				line = line.Replace('"', ' ');
				line = line.Replace(".", "! ");	// Use '!' as universal end of sentence char
				line = line.Replace("?", "! ");	// Map . and ? to !      ! stays put
				line = line.Replace("--", " ");	// Remove --
				line = line.Replace("(", "");	// Eat paraentheses (not white space)
				line = line.Replace(")", "");
				//line = line.Replace('-', ' ');	// Leave - in for hyphenated words
				
				// Split the line of text up into words
				string[] myWords = line.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);
				
				// Add bi-gram pairs to the transition matrix to count them
				for (int i = 0; i < myWords.Length; i++)
				{
					bool isLastWord = false;
					newWord = myWords[i];
					if (LastWordInSent (newWord))	// Handle special end of sentence "word"
					{
						isLastWord = true;			// Map ! to setting Boolean true
						newWord = newWord.Replace("!", "");	// Then remove !
					}
					biGrams.LoadEdge (oldWord, newWord);	// Add/update graph edge
					oldWord = newWord;						// Update old word
					if (isLastWord)
					{
						newWord = "BEGIN-END";				// Handle end of sentence edge
						biGrams.LoadEdge(oldWord, newWord);	// using the special non-word
						oldWord = newWord;
					}
				}
			}
		}
		catch (UnityException ex)
		{
			Debug.Log("Exception: " + ex.Message);
		}
		finally
		{
			if (input != null)
			{
				input.Close();
			}
		}
		//Debug.Log(" and " + biGrams.NumSents() + " sentences.");
		//biGrams.DumpMatrix();		// Only upper left of matrix for debugging purposes
	}
	
	bool LastWordInSent(string word)	// Returns true if "word" is end of sentence (!)
	{
		return word.Contains("!");
	}
	
	public string [] GenGibSent(int maxSent)
	{
		string[] gibSent = new string[maxSent];		// Maximum maxSent-word sentence
		string prevWord = "BEGIN-END";				// Start with sentence boundary
		for (int i = 0; i < gibSent.Length; i++)
		{
			string nextWord = biGrams.GenNextWord(prevWord);	// Get next word
			if (nextWord.Equals("BEGIN-END"))					// If end of sentence
				return gibSent;									// Return words so far
			else
			{
				gibSent[i] = nextWord;				// Store the word
				prevWord = nextWord;				// Update previous word
			}
		}
		return gibSent;
	}
	
}
/* Word class implements a Word for the Graph class
	 * Contains all the stuff a word needs to work in the Graph
	 */
class Word
{
	private string wordSt;		// The word as a string
	private int count = 1;		// The count of this word in corpus
	private int offset;			// Offset of the word in the graph arrays
	
	public Word(string st, int off)
	{
		wordSt = st;
		offset = off;
		count = 1;
	}
	
	public string WordSt
	{
		get { return wordSt; }
		set { wordSt = value; }
	}
	
	public int Offset
	{
		get { return offset; }
		set { offset = value; }
	}
	
	public int Count
	{
		get { return count; }
		set { count = value; }
	}
	
	// override ToString
	public override string ToString()
	{
		return wordSt + " - Count: " + count + " Offset: " + offset;
	}
}
/* Graph class implements a directed, labeled graph, where the labels
	 * are integers and the vertices are Word objects
	 */
class Graph
{
	// attributes
	private int[,] matrix; 			// Adjacency matrix
	private Hashtable vertices; 	// Vertex array is hash table
	private int numVertices = 0;    // Max number of vertices in matrix
	private string[] wordString;
	//private Random rand = new Random();
	
	// Constructor only initializes vertices hash table and numVertices.
	// Have to create the adjacency matrix in the second pass of CreateGraph
	public Graph()
	{
		numVertices = 0;
		vertices = new Hashtable();
	}
	
	public int NumVertices
	{
		get { return numVertices; }
	}
	
	public void CreateMatrix ()		// Don't call until numVertices is set
	{
		matrix = new int[numVertices, numVertices];		// Declare 2D array
		wordString = new string[numVertices];
		wordString[0] = "BEGIN-END";
		//for (int i = 0; i < numVertices; i++)		// Not needed
		//    for (int j = 0; j < numVertices; j++)
		//        matrix[i, j] = 0;
	}
	
	// Add a vertex to the list of vertices
	public void LoadVertex(string word)
	{
		if (vertices.Contains(word))		// Word already in vertices
		{
			Word w = (Word) vertices[word];
			w.Count++;						// Increment word count
		}
		else
		{									// New word, add to vertices
			numVertices++;					// and count it as a new word
			Word w = new Word(word, numVertices - 1);
			vertices.Add(word, w);
		}
	}
	
	public void FixBeginEndCount()	// Correct overcount for BEGIN-END word
	{
		Word begEnd = (Word) vertices["BEGIN-END"];
		//Console.WriteLine("In fix count = " + begEnd.Count);
		int count = begEnd.Count;
		begEnd.Count = count - 1;
		//Console.WriteLine("In fix count = " + begEnd.Count);
	}
	
	public Word GetVertex(string word)	// Safe getter
	{
		if (vertices.Contains(word))
		{
			return (Word) vertices[word];
		}
		else
		{
			Debug.Log("Couldn't find " + word + ", so I returned null");
			return null;
		}
	}
	
	// Load an edge between two vertices
	// Assume a directed graph
	public void LoadEdge(string start, string end)
	{
		Word startWord = (Word) vertices[start];	// Look up the words
		Word endWord = (Word) vertices[end];
		int startOffset = (startWord == null) ? 1 : startWord.Offset;		// Look up the matrix offsets- added condition
		int endOffset = (endWord == null) ? 1 : endWord.Offset;				//because of null reference exception when startword and endword are null
		matrix[startOffset, endOffset]++;			// Increment the count
		
		if (startOffset == 0)
			startWord.Count++;
		
		if (wordString[endOffset] == null)			// Handle the last word
		{
			wordString[endOffset] = end;
		}
	}
	
	public int NumSents()	// Return number of sentences
	{
		Word bE = (Word) vertices["BEGIN-END"];
		return bE.Count;
	}
	
	public string GenNextWord(string prevWord)		// Generate next word of gibberish
	{
		Word prev = (Word) vertices[prevWord];
		int nextOffset = findNextWord(prev.Offset, prev.Count);
		return wordString[nextOffset];
	}
	
	int findNextWord(int prevOff, int prevCount)
	{
		int randCt = Random.Range (1, prevCount);//rand.Next(1, prevCount);
		int tot = 0;
		for (int i = 0; i < numVertices; i++)	// Walk across the matrix row
		{
			tot += matrix[prevOff, i];			// Until random stop point
			if (tot >= randCt)
				return i;						// Return where we landed
		}
		return 0;								// Shouldn't happen
	}
	
	// Dump out upper left corner of the transition matrix for debugging purposes
	public void DumpMatrix()
	{
		for (int i = 0; i < 14; i++)
		{
			Debug.Log("{0,12} " + wordString[i]);
			Word w = (Word) vertices[wordString[i]];
			Debug.Log(" " + w.Count + " ");
			for (int j = 0; j < 14; j++)
				Debug.Log(" " + matrix[i, j]);
			Debug.Log("");
		}
	}
	
	// Override ToString for debugging purposes
	public override string ToString()
	{
		// List the vertices
		string text = "Vertices: \n";
		for (int i = 0; i < numVertices; i++)
		{
			if (vertices[i] != null)
			{
				text += "vertex " + i + ": " +
					vertices[i].ToString() + "\n";
			}
		}
		
		// list the adjacency matrix
		text += "\nAdjacency:\n";
		for (int i = 0; i < numVertices; i++)
		{
			for (int j = 0; j < numVertices; j++)
			{
				if (matrix[i, j] != 0)
				{
					
					text += "vertex " + i +
						" is connected to vertex " + j + "\n";
				}
			}
		}
		return text;
	}
}