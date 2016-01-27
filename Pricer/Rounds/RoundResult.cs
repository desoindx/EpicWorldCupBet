namespace Pricer.Rounds
{
    public class RoundResult
    {
        public int Point;
        public int ScoreFor;
        public int ScoreAgainst;

        public void Add(int point, int scoreFor, int scoreAgainst)
        {
            Point += point;
            ScoreFor += scoreFor;
            ScoreAgainst += scoreAgainst;
        }
    }
}
