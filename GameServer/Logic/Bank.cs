using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class Bank
    {
        private int moneyInBank;

        public int AddMoneyToBank(int amount)
        {
            moneyInBank += amount;
            return moneyInBank;
        }

        public int LandOnBank()
        {
            var temp = moneyInBank;
            moneyInBank = 0;
            return temp;
        }

        public int MoneyInBank { get => moneyInBank; }
    }
}
