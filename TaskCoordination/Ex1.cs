using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskCoordination
{
    class Ex1
    {
        static void MainEx1(string[] arg)
        {
            object padlock = new();

            Task MainTask = new Task(() =>
            {
                Console.WriteLine("Main task initial: \n");

                List<BankAccount> bankList = new List<BankAccount>();
                for (int i = 0; i < 10; i++)
                {
                    bankList.Add(new BankAccount(100, i));
                }

                var depositTask = new Task(() =>
                {
                    for (int i = 0; i < bankList.Count(); i++)
                    {
                        //throw new Exception(); // Catches exception 
                        lock (padlock)
                        {
                            bankList[i].Deposit(10);
                        }
                        Console.WriteLine("Deposit 10 by task: subTask");
                    }
                });
                depositTask.Start();

                var withdrawTask = new Task(() =>
                {
                    for (int i = 0; i < bankList.Count(); i++)
                    {
                        //throw new Exception(); // Catches exception 
                        lock (padlock)
                        {
                            bankList[i].Withdraw(10);
                        }
                        Console.WriteLine($"Withdraw 10 by task: secondSubTask");
                    }
                });
                withdrawTask.Start();


                var FailCheck = depositTask.ContinueWith(t =>
                {
                    Console.WriteLine($"\nOops, task {t.Id} crashed, Status: {t.Status}");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
                var SuccssesCheck = depositTask.ContinueWith(t =>
                {
                    Console.Write($"\nCongratulation, task {t.Id} finished successfully! ");

                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);


                var secondFailCheck = withdrawTask.ContinueWith(t =>
                {
                    Console.WriteLine($"\nOops, task {t.Id} crashed, Status: {t.Status}");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);

                var secondSuccssesCheck = withdrawTask.ContinueWith(t =>
                {
                    Console.Write($"\nCongratulation, task {t.Id} finished successfully! ");
                    int sumAmount = 0;

                    foreach (var item in bankList)
                    {
                        sumAmount += item.Balance;
                    }

                    Console.WriteLine($"Summery of all accounts balance: {sumAmount}");

                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            );
            MainTask.Start();

            try
            {
                MainTask.Wait();

                Console.WriteLine("\nMain task finished.");
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class BankAccount
    {
        public int Balance;
        public int Id;
        public BankAccount(int balance, int id)
        {
            Balance = balance;
            Id = id;
        }
        public void Deposit(int amount)
        {
            Balance += amount;
        }
        public void Withdraw(int amount)
        {
            Balance -= amount;
        }

    }

}
