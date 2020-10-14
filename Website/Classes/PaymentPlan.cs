using System;

namespace Website.Classes
{
    public struct PaymentPlan
    {
        public string RebillStatus { get; set; }
        public string RebillFrequency { get; set; }
        public double RebillAmount { get; set; }
        public int PaymentsProcessed { get; set; }
        public int PaymentsRemaining { get; set; }
        public string NextPaymentDate { get; set; }

    }
}
