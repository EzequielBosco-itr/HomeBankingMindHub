namespace HomeBankingMindHub.Utils
{
    public class RandomNumberUtils
    {
        public static int GenerateRandomNumber(int min, int max)
        {
            var random = new Random();
            int newRandom = random.Next(min, max);

            return newRandom;
        }
    }
}
