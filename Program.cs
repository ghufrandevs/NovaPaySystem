using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace NovaPaySystem
{
    
    internal class Program
    {
        static public void ShowMenu()
        {
            Console.WriteLine("========== NovaPay Banking System ==========");

            Console.WriteLine("1. Open Savings Account");
            Console.WriteLine("2. Open Current Account");
            Console.WriteLine("3. Open Fixed Deposit Account");
            Console.WriteLine("4. Deposit");
            Console.WriteLine("5. Withdraw");
            Console.WriteLine("6. Print Account Statement");
            Console.WriteLine("7. Apply Interest (Savings Only)");
            Console.WriteLine("8. Bank Summary");
            Console.WriteLine("0. Exit");
            Console.WriteLine("choice option :");
        }
        static public void OpenSavingAccount(Bank bank)
        {
            Console.WriteLine("Enter Owner Name: ");
            string name = (Console.ReadLine() ?? string.Empty).Trim();
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty. Please re-enter:");
                name = (Console.ReadLine() ?? string.Empty).Trim();
            }
            SavingsAccount savingsAccount = new SavingsAccount(name);
            bank.OpenAccount(savingsAccount);
        }
        static public void OpenCurrentAccount(Bank bank)
        {
            Console.WriteLine("Enter Owner Name: ");
            string name = (Console.ReadLine() ?? string.Empty).Trim();
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty. Please re-enter:");
                name = (Console.ReadLine() ?? string.Empty).Trim();
            }
            Console.WriteLine("Enter Overdraft Limit: ");

            double overdraftLimit = Convert.ToDouble(Console.ReadLine());

            CurrentAccount currentAccount = new CurrentAccount(name, overdraftLimit);

            bank.OpenAccount(currentAccount);
        }
        public static void OpenFixedDepositAccount(Bank bank)
        {
            Console.WriteLine("Enter Owner Name: ");

            string name = (Console.ReadLine() ?? string.Empty).Trim();

            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty. Please re-enter:");

                name = (Console.ReadLine() ?? string.Empty).Trim();
            }

            Console.WriteLine("Enter Deposit Amount: ");

            double depositAmount = Convert.ToDouble(Console.ReadLine());

            FixedDepositAccount fixedAccount = new FixedDepositAccount(name, depositAmount);

            bank.OpenAccount(fixedAccount);
        }
        public static void Deposit(Bank bank)
        {
            Console.WriteLine("Enter Account Number: ");

            int accountNumber = Convert.ToInt32(Console.ReadLine());

            BankAccount account = bank.FindAccount(accountNumber);
            if(account !=null)
            {
                Console.WriteLine("Enter Deposit Amount: ");

                double amount = Convert.ToDouble(Console.ReadLine());

                bank.ProcessDeposit(account, amount);
            }
            else
            {
                Console.WriteLine("Account not found.");
            }

            
        }
        public static void Withdraw(Bank bank)
        {
            Console.WriteLine("Enter Account Number: ");

            int accountNumber = Convert.ToInt32(Console.ReadLine());

            BankAccount account = bank.FindAccount(accountNumber);

            if (account != null)
            {
                Console.WriteLine("Enter Withdrawal Amount: ");

                double amount = Convert.ToDouble(Console.ReadLine());

                bank.ProcessWithdrawal(account, amount);
            }

            else
            {
                Console.WriteLine("Account not found");

                }
        }
        public static void PrintAccountStatement(Bank bank)
        {
            Console.WriteLine("Enter Account Number: ");

            int accountNumber = Convert.ToInt32(Console.ReadLine());

            bank.PrintAccountStatement(accountNumber);
        }
        public static void ApplyInterest(Bank bank)
        {
            Console.WriteLine("Enter Savings Account Number: ");

            int accountNumber = Convert.ToInt32(Console.ReadLine());

            BankAccount account = bank.FindAccount(accountNumber);

            if (account != null)
            {
                if (account is SavingsAccount savings)
                {
                    savings.ApplyInterest();

                    Console.WriteLine("Interest applied successfully.");
                }
                else
                {
                    Console.WriteLine("This is not a Savings Account.");
                }
            }
            else
            {
                Console.WriteLine("Account not found.");
            }
        }
        public static void BankSummary(Bank bank)
        {
            bank.DisplaySummary();
        }
        public static bool ExitSystem()
        {
            Console.WriteLine("Are you sure you want to exit the system? (yes/no)");
            string inputs = (Console.ReadLine() ?? string.Empty).ToLower();
            if (inputs == "no")
            {
                return false;
            }
            else if (inputs == "yes")
            {
                Console.WriteLine("Thank you for using the system --Exit--");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid option");
                return false;
            }
        }
        static void Main(string[] args)
        {
            Bank bank=new Bank("NovaPay");
            int option = 0;
            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                while (!int.TryParse(Console.ReadLine(), out option))
                {
                    Console.WriteLine("Invalid input. Please choose a number from 0 to 11");
                }

                switch (option)
                {
                    case 1://Open Savings Account
                        OpenSavingAccount(bank);
                        break;

                        case 2://Open Current Account
                        OpenCurrentAccount(bank);
                        break;

                        case 3://Open Fixed Deposit Account
                        OpenFixedDepositAccount(bank);
                        break;

                        case 4://Deposit
                        Deposit(bank);
                        break;

                        case 5:
                        Withdraw(bank);
                        break;

                        case 6://Print Account Statement
                        PrintAccountStatement(bank);
                        break;

                        case 7:
                        ApplyInterest(bank);
                        break; 

                    case 8:
                        BankSummary(bank);
                        break;
                    
                    case 0:
                        exit = ExitSystem();
                        break;
                }
                if (!exit)
                {
                    Console.ReadLine();
                    Console.Clear();
                }
            }

        }
    }
    interface IDepositable
    {
        public void Deposit(double amount);
    }
    interface IWithdrawable
    {
        public void Withdraw(double amount);
               

    }
    interface IStatementPrintable
    {
        public void PrintStatement();
    }

    abstract class BankAccount : IDepositable, IWithdrawable, IStatementPrintable 
    {
        private static int nextAccountNumber = 1001;
        protected static int totalTransactionsProcessed;
        protected double balance;
        protected List<Transaction> transactions;
        public int AccountNumber { get; }
        public string OwnerName { get; }
        public double Balance
        { get { return balance; } }
        public string AccountType { get; protected set; }
        public BankAccount(string ownerName)
        {
            OwnerName = ownerName;
            AccountNumber = nextAccountNumber++;
            balance = 0;
            transactions = new List<Transaction>();      
        }
        public static int GetTotalTransactions()
        {
            return totalTransactionsProcessed;
        }
        public void Deposit(double amount)
        {
            if(amount>0)
            {
                balance += amount;
                Transaction transaction = new ("Deposit",amount);
                transactions.Add(transaction);
            }
        }

        public void Deposit(double amount, string note)
        {
            if (amount > 0)
            {
                balance += amount;

                Transaction transaction = new("Deposit", amount, note);

                transactions.Add(transaction);
                totalTransactionsProcessed++;
            }
        }
        public abstract void Withdraw(double amount);

        public virtual void PrintStatement()
        {
            Console.WriteLine($"Owner Name : {OwnerName}");
            Console.WriteLine($"AccountNumber : {AccountNumber}");
            Console.WriteLine($"AccountType : {AccountType}");   
            Console.WriteLine($"Balance : {Balance}");
            foreach( Transaction transaction in transactions )
            {
                transaction.DisplayInfo();
            }

        }

    }
    
    class  SavingsAccount : BankAccount

    {
        private double interestRate;
        private static double MinBalance = 100;
        public SavingsAccount(string ownerName, double interestRate = 0.03) : base(ownerName)
        {
            AccountType = "Savings";
            this.interestRate = interestRate;

        }
        public override void Withdraw(double amount)
        {
            //Checks: amount > 0 AND (balance - amount) >= MinBalance.
            //If valid: subtracts from balance, creates a Transaction("Withdrawal", amount),
            //adds to transactions list, increments totalTransactionsProcessed.
            //Otherwise prints the specific error.

            if(amount>0 && ((Balance-amount)>=MinBalance))
                { 
                balance-= amount;
                Transaction transaction = new("Withdrawal", amount);
                transactions.Add(transaction);
                totalTransactionsProcessed++;
                
            }
            else
            {
                Console.WriteLine("Cannot withdraw. Minimum balance must remain 100.");
            }
        }
        public override void PrintStatement()

        {
            base .PrintStatement();
            Console.WriteLine($"interestRate : {interestRate}");

        }
        public void ApplyInterest()
        {
            double interest = Balance*interestRate;
            Deposit(interest, "Interest Credit");
        }


    }

    class CurrentAccount : BankAccount
    {
        private double overdraftLimit;
        public double OverdraftLimit
        {
            get { return overdraftLimit; }
        }
        public  CurrentAccount(string ownerName, double overdraftLimit) : base (ownerName)

        {
            AccountType = "Current";
            if(overdraftLimit>=0)
            {
                this.overdraftLimit = overdraftLimit;   
            }
        }
        public override void Withdraw(double amount)
        {
            //Checks: amount > 0 AND (balance - amount) >= -overdraftLimit. If valid: subtracts from balance,
            //logs Transaction, increments counter. Otherwise prints "Withdrawal exceeds overdraft limit."
            if(amount> 0 && ((balance-amount)>=-overdraftLimit))
            {
                balance -= amount;
                Transaction transaction = new("Withdrawal", amount);
                transactions.Add(transaction);
                totalTransactionsProcessed++;
            }
            else
            {
                Console.WriteLine("Withdrawal exceeds overdraft limit");
            }
           
        }
        public sealed override void PrintStatement()
        {
            base.PrintStatement();
            Console.WriteLine($"Overdraft Limit :{overdraftLimit}");
        }


    }

       class FixedDepositAccount: BankAccount
    {
        private double lockedAmount;
        public FixedDepositAccount(string ownerName, double depositAmount) :base(ownerName)
        {
            AccountType = "Fixed Deposit";
            //Calls base(ownerName). Sets AccountType = "Fixed Deposit". Validates depositAmount > 0.
            //Stores it in lockedAmount, then calls the overloaded Deposit(depositAmount, "Initial Fixed Deposit")
            //to set the balance and log the transaction.
            if(depositAmount>0)
            {
                this.lockedAmount= depositAmount;
                Deposit(depositAmount, "Initial Fixed Deposit");
            }
            else
            {
                Console.WriteLine("Invalid deposit amount.");
            }
        }

        public override void Withdraw(double amount)

        {
            Console.WriteLine("Fixed Deposit accounts cannot be withdrawn before maturity.");

        }
        public override void PrintStatement()
        {
            base .PrintStatement();
            Console.WriteLine($"lockedAmount : {lockedAmount}");

        }


    }
    class Transaction
    {
        private string type;
        private double amount;
        private DateTime date;
        private string note;


        public Transaction(string type, double amount, string note = "")
        {
            this.type = type;
            this.amount = amount;
            this.note = note;
            date = DateTime.Now;
        }
        public void DisplayInfo()
        {
            if (note != "")
            {
                Console.WriteLine($"{date.ToShortDateString()} {type} +{amount:F2} {note}");
            }
            else
            {
                Console.WriteLine($"{date.ToShortDateString()} {type} +{amount:F2} ");

            }

        }

    }
    class Bank
    {
        public string BankName { get; private set; }
        private List<BankAccount> accounts;
        public Bank(string name)
        {
            BankName = name;

            accounts = new List<BankAccount>();
        }

        public void OpenAccount(BankAccount account)
        {
            accounts.Add(account);
            Console.WriteLine($"New Account Number: {account.AccountNumber}");

        }
        public BankAccount FindAccount(int accountNumber)
        {
            foreach (BankAccount account in accounts)
            {
                if(account.AccountNumber == accountNumber)
                {
                    return account; 
                }
                
            }
            return null;
            
        }
        public void ProcessDeposit(IDepositable account, double amount)
        {
            account.Deposit(amount);

        }
        public void ProcessWithdrawal(IWithdrawable account, double amount)
        {
            account.Withdraw(amount);
        }
        public void PrintAccountStatement(int accountNumber)
        {
            BankAccount account = FindAccount(accountNumber);

            if (account != null)
            {
                IStatementPrintable printable = account;

                printable.PrintStatement();
            }

            else
            {
                Console.WriteLine("Account not found.");
            }
        }
        public void DisplaySummary()
        {
            int savingsCount = 0;
            int currentCount = 0;
            int fixedCount = 0;

            double totalBalance = 0;

            foreach (BankAccount account in accounts)
            {
                totalBalance += account.Balance;

                if (account is SavingsAccount)
                {
                    savingsCount++;
                }

                else if (account is CurrentAccount)
                {
                    currentCount++;
                }

                else if (account is FixedDepositAccount)
                {
                    fixedCount++;
                }
            }

            Console.WriteLine($"Bank Name: {BankName}");

            Console.WriteLine($"Total Accounts: {accounts.Count}");

            Console.WriteLine($"Savings Accounts: {savingsCount}");

            Console.WriteLine($"Current Accounts: {currentCount}");

            Console.WriteLine($"Fixed Deposit Accounts: {fixedCount}");

            Console.WriteLine($"Total Balance: {totalBalance}");

            Console.WriteLine($"Total Transactions: {BankAccount.GetTotalTransactions()}");
        }


    }

}
