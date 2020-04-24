using System.Collections.Generic;
using System.Threading;

namespace fidelizPlus_back.Payment
{
    public class PaymentMonitor
    {
        private Dictionary<int, Thread> WaitingPayments { get; set; }

        public PaymentMonitor()
        {
            WaitingPayments = new Dictionary<int, Thread>();
        }

        public bool IsMonitored(int purchaseId)
        {
            return WaitingPayments.ContainsKey(purchaseId);
        }

        public void Add(int purchaseId, Thread waitingThread)
        {
            WaitingPayments[purchaseId] = waitingThread;
        }

        public void Remove(int purchaseId)
        {
            if (IsMonitored(purchaseId))
            {
                WaitingPayments[purchaseId].Interrupt();
                WaitingPayments.Remove(purchaseId);
            }
        }
    }
}
