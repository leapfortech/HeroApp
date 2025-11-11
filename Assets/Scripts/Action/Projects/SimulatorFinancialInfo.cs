using System;

public class SimulatorFinancialInfo
{
    public double ProfitabilityAmount { get; set; }
    public double TotalAmount { get; set; }
    public double CapitalGainAmount { get; set; }
    public double AccumulatedCapitalGainAmount { get; set; }
    public double DueAmount { get; set; }

    public SimulatorFinancialInfo()
    {

    }

    public SimulatorFinancialInfo(double profitabilityAmount, double totalAmount, double capitalGainAmount,
                                  double accumulatedCapitalGainAmount, double dueAmount)
    {
        ProfitabilityAmount = profitabilityAmount;
        TotalAmount = totalAmount;
        CapitalGainAmount = capitalGainAmount;
        AccumulatedCapitalGainAmount = accumulatedCapitalGainAmount;
        DueAmount = dueAmount;
    }
}