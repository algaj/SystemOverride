using Godot;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceThing
{
	public class NameGenerator
    {
		RandomNumberGenerator _random = new RandomNumberGenerator();

        char[] _vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
        char[] _consonants = { 'q', 'w', 'r', 't', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm' };

        public NameGenerator()
        {
			GD.Randomize();
		}

		public string GenerateFirstName()
        {
			return GenerateRandomName(3, 4);
        }

		public string GenerateLastName()
        {
			return GenerateRandomName(4, 6);
        }

        /// <summary>
        /// Generates a random name consisting of consonants and vowels.
        /// </summary>
        /// <param name="minSymbols">The minimum number of symbols in the name.</param>
        /// <param name="maxSymbols">The maximum number of symbols in the name.</param>
        /// <returns>A randomly generated name.</returns>
        string GenerateRandomName(int minSymbols, int maxSymbols)
        {
            // Chance for a name to start with a vowel
            var nameStartingWithVowelChance = 0.2f;

            // Chance for a consonant to be followed by another consonant
            var consonantFollowingConsonantChance = 0.05f;

            // Get a random number of symbols for the name
            var symbolCount = _random.RandiRange(minSymbols, maxSymbols);

            // StringBuilder for building the name
            var builder = new StringBuilder();

            // Keep track of whether the last letter was a consonant
            bool wasConsonantLast;

            // Start with a capital letter
            if (_random.Randf() > nameStartingWithVowelChance)
            {
                builder.Append(GetRandomCapitalLetter(_consonants));
                wasConsonantLast = true;
            }
            else
            {
                builder.Append(GetRandomCapitalLetter(_vowels));
                wasConsonantLast = false;
            }

            // Add the remaining symbols to the name
            for (int i = 0; i < symbolCount - 1; i++)
            {
                // Determine whether to add a consonant or vowel
                if (wasConsonantLast && (_random.Randf() > consonantFollowingConsonantChance))
                {
                    builder.Append(GetRandomLetter(_vowels));
                    wasConsonantLast = false;
                }
                else
                {
                    builder.Append(GetRandomLetter(_consonants));
                    wasConsonantLast = true;
                }
            }

            // Return the generated name
            return builder.ToString();
        }

        /// <summary>
        /// Gets a random letter from the specified array of letters.
        /// </summary>
        /// <param name="letters">The array of letters to choose from.</param>
        /// <returns>A random letter.</returns>
        char GetRandomLetter(char[] letters)
        {
            return letters[_random.RandiRange(0, letters.Length - 1)];
        }

        /// <summary>
        /// Gets a random capital letter from the specified array of letters.
        /// </summary>
        /// <param name="letters">The array of letters to choose from.</param>
        /// <returns>A random capital letter.</returns>
        char GetRandomCapitalLetter(char[] letters)
        {
            return char.ToUpper(GetRandomLetter(letters));
        }


    }

	public partial class StoryEngine : Node
	{
		List<Planet> _planets = new List<Planet>();

		RandomNumberGenerator _random = new RandomNumberGenerator();
		NameGenerator _nameGenerator = new NameGenerator();

		public override void _Ready()
		{
			var planetNodes = GetTree().GetNodesInGroup("planets");

			foreach (var node in planetNodes)
            {
				_planets.Add(node as Planet);
            }

			Debug.Assert(_planets.Count > 0, "_planets.Length > 0");

			GD.Randomize();

			BeginStory();

			for (int i = 1; i <= 100; i++)
            {
				ProgressStory(i);
            }
		}

		public void ProgressStory(int wave)
        {

			if (wave == 1)
			{
				GD.Print("Scouts arived");
			}

			if (wave == 5)
            {
				GD.Print("First collonisation units arived.");
            }

			if (wave == 6)
            {
				GD.Print("Player struggles");
				GD.Print("<SOLUTION>");
            }

			if (wave == 10)
            {
				GD.Print("Enemies discover new power.");
				GD.Print("Player struggles");
            }

			if (wave == 15)
            {
				GD.Print("Player starts fortifying the planet");
            }

			if (wave == 20)
            {
				GD.Print("Enemies find ways to get around it.");
            }

			if (wave == 25)
            {
				GD.Print("Exploration of the rest of the universe explored.");
            }

			if (wave == 30)
            {
				GD.Print("Player can recruit ally spaceships");
            }

			if (wave == 35)
            {
				GD.Print("Another faction joins the war.");
            }

			if (wave == 40)
            {
				GD.Print("Player can now unlock better ship.");
            }

			if (wave == 45)
            {
				GD.Print("Enemies become much stronger");
            }

			if (wave == 50)
            {
				GD.Print("<PROBLEM>");
				GD.Print("50 waves to resolve it");
            }

			if (wave == 55)
            {
				GD.Print("<PROBLEM RESOLVED>");
            }
        }

		void BeginStory()
        {
			GD.Print("There was a ", _nameGenerator.GenerateFirstName(), " ", _nameGenerator.GenerateLastName());

			GD.Print("who lived on ", _planets[_random.RandiRange(0, _planets.Count - 1)].Name);

			GD.Print("<MOTIVATION>");

			GD.Print("<DECISION>");

			GD.Print("Start of the game");
        }
	}
}