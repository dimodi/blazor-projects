namespace TelerikBlazorEF.Data
{
    public class NameGenerator
    {
        private readonly List<string> Vowels = new List<string>() { "a", "e", "i", "o", "u", "y" };
        private readonly List<string> Consonants = new List<string>() { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z" };

        private const int UpperCaseStart = 65;
        private const int UpperCaseEnd = 90;
        private const int LowerCaseStart = 97;
        private const int LowerCaseEnd = 122;

        private const int DefaultMinWordLength = 3;
        private const int DefaultMaxWordLength = 7;

        public string Word(int minLenth = DefaultMinWordLength, int maxLength = DefaultMaxWordLength, bool capitalFirstLetter = true)
        {
            var result = "";
            var wordLength = Random.Shared.Next(minLenth, maxLength + 1);

            result += Letter(capitalFirstLetter);

            var isFirstLetterVowel = IsVowel(result);

            for (int i = 1; i < wordLength; i++)
            {
                result += Letter(i == 1, i % 2 == 1 && !isFirstLetterVowel || i % 2 == 0 && isFirstLetterVowel);
            }

            return result;
        }

        private string Vowel()
        {
            return Vowels.ElementAt(Random.Shared.Next(0, Vowels.Count));
        }

        private bool IsVowel(string letter)
        {
            return Vowels.Any(x => x.ToUpper() == letter.ToUpper());
        }

        private string Consonant()
        {
            return Consonants.ElementAt(Random.Shared.Next(0, Consonants.Count));
        }

        private string Letter(bool upperCase = false, bool? vowel = null)
        {
            if (!vowel.HasValue)
            {
                var minAsciiCode = upperCase ? UpperCaseStart : LowerCaseStart;
                var maxAsciiCode = upperCase ? UpperCaseEnd : LowerCaseEnd;

                return ((char)Random.Shared.Next(minAsciiCode, maxAsciiCode + 1)).ToString();
            }
            else if (vowel.Value)
            {
                return Vowel();
            }
            else
            {
                return Consonant();
            }
        }
    }
}
