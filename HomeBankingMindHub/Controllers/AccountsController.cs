using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authorization;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Linq;
using HomeBankingMindHub.DTOs;

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
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()

        {

            try

            {

                var accounts = _accountRepository.GetAllAccounts();



                var accountsDTO = new List<AccountDTO>();



                foreach (Account account in accounts)

                {

                    var newAccountDTO = new AccountDTO(account);

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
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get(long id)

        {

            try

            {
                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                var account = _accountRepository.GetAccountByIdAndClientEmail(id, email);
                if (account == null)
                {
                    return Forbid();
                }

                var accountDTO = new AccountDTO(account);

                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

    }

}

