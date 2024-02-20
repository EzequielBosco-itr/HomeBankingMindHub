﻿using HomeBankingMindHub.dtos;

using HomeBankingMindHub.Models;

using HomeBankingMindHub.Repositories;

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]

    [ApiController]

    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;



        public AccountsController(IAccountRepository accountRepository)

        {

            _accountRepository = accountRepository;

        }



        [HttpGet]

        public IActionResult Get()

        {

            try

            {

                var accounts = _accountRepository.GetAllAccounts();



                var accountsDTO = new List<AccountDTO>();



                foreach (Account account in accounts)

                {

                    var newAccountDTO = new AccountDTO

                    {

                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(transaction => new TransactionDTO
                        {
                            Id = transaction.Id,
                            Type = transaction.Type.ToString(),
                            Amount = transaction.Amount,
                            Description = transaction.Description,
                            Date = transaction.Date
                        }).ToList()

                    };

                    accountsDTO.Add(newAccountDTO);

                }


                return Ok(accountsDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }



        [HttpGet("{id}")]

        public IActionResult Get(long id)

        {

            try

            {

                var account = _accountRepository.GetAccountById(id);

                if (account == null)

                {

                    return Forbid();

                }



                var accountDTO = new AccountDTO

                {

                    Id = account.Id,

                    Number = account.Number,

                    CreationDate = account.CreationDate,

                    Balance = account.Balance,

                    Transactions = account.Transactions.Select(transaction => new TransactionDTO
                    {
                        Id = transaction.Id,
                        Type = transaction.Type.ToString(),
                        Amount = transaction.Amount,
                        Description = transaction.Description,
                        Date = transaction.Date
                    }).ToList()

                };


                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

    }

}
