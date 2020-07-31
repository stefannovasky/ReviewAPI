using System;
using System.Text;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Shared.Utils
{
    public class RandomCodeUtils : IRandomCodeUtils
    {
        public string GenerateRandomCode()
        {
            int length = 8;

            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double next = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * next));
                letter = Convert.ToChar(shift + 65);
                builder.Append(letter);
            }

            return builder.ToString().ToUpper();
        }
    }
}
