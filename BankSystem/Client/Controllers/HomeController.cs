﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using BankService;
using Client.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Overview()
        {
            var bankService = new BankServiceClient();
            var sessionId = await HttpContext.Authentication.GetSessionId();
            var accounts = await bankService.GetAccountsAsync(sessionId);

            return View(accounts);
        }
        public async Task<IActionResult> History(string id)
        {
            var bankService = new BankServiceClient();     
            var operations = await bankService.GetAccountHistoryAsync(id);

            return View(operations);
        }

        public IActionResult Transfer(string id)
        {
            ViewData["Message"] = "Transfer page.";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(string id, TransferViewModel transferViewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(transferViewModel);
            }

            var bankService = new BankServiceClient();

            var operation = new Operation
            {
                SourceId = id,
                DestinationId = transferViewModel.DestinationId,
                Amount = transferViewModel.Amount,
                Title = transferViewModel.Title,
                OperationType = Operation.OperationTypes.Transfer
            };
            
            try
            {
                await bankService.ExecuteOperationAsync(operation);
            }
            catch (FaultException exception)
            {
                ViewData["Error"] = exception.Message;

                return View();
            }        

            return RedirectToAction("Overview");
        }

        public IActionResult Payment(string id, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Message"] = "Your contact page.";
            ViewData["Title"] = "Payment";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string id, PaymentViewModel paymentViewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["Title"] = "Payment";
                return View(paymentViewModel);
            }

            var bankService = new BankServiceClient();
            var operation = new Operation
            {
                Amount = paymentViewModel.Amount,
                DestinationId = id,
                SourceId = "Payment",
                Title = "Payment",
                OperationType = Operation.OperationTypes.Payment
            };

            try
            {
                await bankService.ExecuteOperationAsync(operation);
            }
            catch (FaultException exception)
            {
                ViewData["Title"] = "Payment";
                ViewData["Error"] = exception.Message;

                return View("Payment");
            }
            
            return RedirectToAction("Overview");
        }

        public IActionResult Withdraw(string id)
        {
            ViewData["Message"] = "Your contact page.";
            ViewData["Title"] = "Withdraw";

            return View("Payment");
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(string id, PaymentViewModel paymentViewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["Title"] = "Withdraw";
                return View("Payment", paymentViewModel);
            }

            var bankService = new BankServiceClient();
            var operation = new Operation
            {
                Amount = paymentViewModel.Amount,
                SourceId = id,
                DestinationId = "Withdraw",
                Title = "Withdraw",
                OperationType = Operation.OperationTypes.Withdraw
            };
            
            try
            {
                await bankService.ExecuteOperationAsync(operation);
            }
            catch (FaultException exception)
            {
                ViewData["Title"] = "Withdraw";
                ViewData["Error"] = exception.Message;

                return View("Payment");
            }

            return RedirectToAction("WithdrawSuccess");
        }

        public IActionResult WithdrawSuccess()
        {
            ViewData["Message"] = "Your contact page.";
            ViewData["Title"] = "Withdraw";

            return View("WithdrawSuccess");
        }

        public async Task<IActionResult> CreateAccount()
        {
            var bankService = new BankServiceClient();
            var result = await bankService.CreateAccountAsync(await HttpContext.Authentication.GetSessionId());

            return RedirectToAction("Overview");
        }

        public async Task<IActionResult> DeleteAccount(string id)
        {
            var bankService = new BankServiceClient();
            var result = await bankService.DeleteAccountAsync(id);

            return RedirectToAction("Overview");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

