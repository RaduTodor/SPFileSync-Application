namespace Models
{
    public class Verdicts
    {
        public Verdicts()
        {
            FinalizedSyncProccesses = new bool[0];
        }

        public bool[] FinalizedSyncProccesses { get; set; }
    }
}