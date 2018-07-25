using System;
using System.Linq;
using Models;

//TODO [CR RT] Move class to BLL
//TODO [CR RT] Rename class

public class SyncProgressProvider
{
    public event EventHandler<bool> ProgressUpdate;

    //TODO [CR RT] Please make this method privare
    public void VerifySyncProgress(Verdicts verdicts)
    {
        while (verdicts.FinalizedSyncProccesses.Any(verdict => verdict == false))
        {
        }

        ProgressUpdate?.Invoke(this, true);
    }

    //TODO [CR RT] Rename method
    public void Operation(SyncProgressProvider syncProgressProvider, Verdicts verdicts)
    {
        syncProgressProvider.VerifySyncProgress(verdicts);
    }
}