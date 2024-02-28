namespace HomeBankingMindHub.Utils
{
    public class RandomNumber
    {
        public static int GenerateRandomNumber(int min, int max)
        {
            var random = new Random();
            int newRandom = random.Next(min, max);

            return newRandom;
        }
    }
}
