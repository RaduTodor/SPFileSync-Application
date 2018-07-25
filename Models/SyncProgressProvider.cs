
using System;
using System.Linq;
using System.Threading;

public class SyncProgressProvider
{
    public event EventHandler<bool> ProgressUpdate;

    public void VerifySyncProgress(Verdicts verdicts)
    {
        while (verdicts.FinalizedSyncProccesses.Any(verdict => verdict == false))
        {

        }
        ProgressUpdate?.Invoke(this, true);
    }

    public void Operation(SyncProgressProvider syncProgressProvider, Verdicts verdicts)
    {
        syncProgressProvider.VerifySyncProgress(verdicts);
    }
}