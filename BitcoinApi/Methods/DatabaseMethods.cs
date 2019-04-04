using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinApi.Database;
using BitcoinApi.Models;

namespace BitcoinApi.Methods
{
    public class DatabaseMethods
    {
        public static Response SaveTransaction(Transaction information)
        {
            using (var context = new BitcoinApiContext())
            {
                try
                {
                    context.Transactions.Add(information);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    var exceptionResponse = new Response
                    {
                        IsSuccessful = false,
                        Message = e.Message
                    };
                    return exceptionResponse;
                }

                return new Response {IsSuccessful = true, Message = "Success"};
            }
        }

        public static List<TransactionInformation> GetLastTransactions()
        {
            using (var context = new BitcoinApiContext())
            {
                var transactions = context.Transactions.Where(t => t.Confirmations < 3).Select(t =>
                    new TransactionInformation
                    {
                        Address = t.Wallet.Address,
                        Amount = t.Amount,
                        Confirmations = t.Confirmations,
                        Date = t.Date
                    }).ToList();
                return transactions;
            }
        }

        public static Response SaveWallet(string address)
        {
            using (var context = new BitcoinApiContext())
            {
                try
                {
                    var wallet = new Wallet {Address = address, Balance = null};
                    context.Wallets.Add(wallet);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    var exceptionResponse = new Response
                    {
                        IsSuccessful = false,
                        Message = e.Message
                    };
                    return exceptionResponse;
                }

                return new Response {IsSuccessful = true, Message = "Success"};
            }
        }
    }
}