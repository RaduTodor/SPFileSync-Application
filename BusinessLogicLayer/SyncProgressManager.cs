namespace BusinessLogicLayer
{
    using System;
    using System.Linq;
    using Models;

    public class SyncProgressManager
    {
        public event EventHandler<bool> ProgressUpdate;

        private void VerifySyncProgress(Verdicts verdicts)
        {
            while (verdicts.FinalizedSyncProccesses.Any(verdict => verdict == false))
            {
            }

            ProgressUpdate?.Invoke(this, true);
        }

        public void CheckSyncProgress(SyncProgressManager syncProgressProvider, Verdicts verdicts)
        {
            syncProgressProvider.VerifySyncProgress(verdicts);
        }
    }
}