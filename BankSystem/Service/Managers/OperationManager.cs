﻿using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Service.Models;

namespace Service.Managers
{
    public class OperationManager
    {
        public void ExecuteInternalTransfer(Operation operation)
        {
            //var sourceAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.SourceId);
            //var destinationAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.DestinationId);
            //var operationInDestinationView = operation.Clone();

            //operation.BalanceBefore = sourceAccount.Balance;
            //operationInDestinationView.BalanceBefore = destinationAccount.Balance;

            //sourceAccount.Balance -= operation.Amount;
            //destinationAccount.Balance += operation.Amount;

            //operation.BalanceAfter = sourceAccount.Balance;
            //operationInDestinationView.BalanceAfter = destinationAccount.Balance;

            //DAL.Instance.Operations.Add(operation);
            //DAL.Instance.Operations.Add(operationInDestinationView);

            //sourceAccount.OperationsHistory.Add(operation.Id);
            //destinationAccount.OperationsHistory.Add(operationInDestinationView.Id);

            //DAL.Instance.Accounts.Update(sourceAccount);
            //DAL.Instance.Accounts.Update(destinationAccount);

            ExecuteExpenseOperation(operation.Clone());
            ExecuteIncomeOperation(operation.Clone());
        }

        public async Task ExecuteExternalTransfer(Operation operation, string credentials)
        {
            using (var client = new HttpClient())
            {
                var externalOperation = new ExternalOperation(operation);

                //TODO Needs mapping from BankId to IP
                //const string pcIp = "192.168.1.11";
                var externalIp = ConfigurationManager.AppSettings["ExternalIp"];
                var myOwnBankBaseAdress = "http://" + externalIp + "/BankService/web";
                var url = myOwnBankBaseAdress + "/accounts/" + operation.DestinationId;

                var content = new StringContent(externalOperation.ToJson(), Encoding.UTF8, "application/json");

                //Todo
                //httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);

                //var json = externalOperation.ToJson();
                //var content = new FormUrlEncodedContent(externalOperation);
                //client.DefaultRequestHeaders.Authorization

                client.DefaultRequestHeaders.Add("Authorization", credentials);
                var response = await client.PostAsync(url, content);
                
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    //var sourceAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.SourceId);

                    //operation.BalanceBefore = sourceAccount.Balance;
                    //sourceAccount.Balance -= operation.Amount;
                    //operation.BalanceAfter = sourceAccount.Balance;

                    //DAL.Instance.Operations.Add(operation);

                    //sourceAccount.OperationsHistory.Add(operation.Id);

                    //DAL.Instance.Accounts.Update(sourceAccount);

                    ExecuteExpenseOperation(operation);
                }

                var responseString = await response.Content.ReadAsStringAsync();
            }
        }

        //public void ExecuteOutcomingPayment(Operation operation)
        //{
        //    var sourceAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.SourceId);

        //    operation.BalanceBefore = sourceAccount.Balance;
        //    sourceAccount.Balance -= operation.Amount;
        //    operation.BalanceAfter = sourceAccount.Balance;

        //    DAL.Instance.Operations.Add(operation);

        //    sourceAccount.OperationsHistory.Add(operation.Id);

        //    DAL.Instance.Accounts.Update(sourceAccount);
        //}

        //public void ExecuteIncomingPayment(Operation operation)
        //{
        //    var destinationAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.DestinationId);

        //    operation.BalanceBefore = destinationAccount.Balance;
        //    destinationAccount.Balance += operation.Amount;
        //    operation.BalanceAfter = destinationAccount.Balance;

        //    DAL.Instance.Operations.Add(operation);

        //    destinationAccount.OperationsHistory.Add(operation.Id);

        //    DAL.Instance.Accounts.Update(destinationAccount);
            
        //}

        //public void ReceiveExternalTransfer(Operation operation)
        //{
        //    var destinationAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.DestinationId);

        //    ExecuteOperation(operation, destinationAccount, true);

        //    //operation.BalanceBefore = destinationAccount.Balance;
        //    //destinationAccount.Balance += operation.Amount;
        //    //operation.BalanceAfter = destinationAccount.Balance;

        //    //DAL.Instance.Operations.Add(operation);

        //    //destinationAccount.OperationsHistory.Add(operation.Id);

        //    //DAL.Instance.Accounts.Update(destinationAccount);
        //}

        public void ExecuteIncomeOperation(Operation operation)
        {
            var destinationAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.DestinationId);
            ExecuteOperation(operation, destinationAccount, true);
        }

        public void ExecuteExpenseOperation(Operation operation)
        {
            var sourceAccount = DAL.Instance.Accounts.Single(account => account.Id == operation.SourceId);
            ExecuteOperation(operation, sourceAccount, false);
        }

        private void ExecuteOperation(Operation operation, Account account, bool isIncomeOperation)
        {
            operation.BalanceBefore = account.Balance;

            if (isIncomeOperation)
            {
                account.Balance += operation.Amount;
            }
            else
            {
                account.Balance -= operation.Amount;
            }
            
            operation.BalanceAfter = account.Balance;

            DAL.Instance.Operations.Add(operation);

            account.OperationsHistory.Add(operation.Id);

            DAL.Instance.Accounts.Update(account);
        }
    }
}
