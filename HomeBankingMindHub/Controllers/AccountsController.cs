using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authorization;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implements;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]

    [ApiController]

    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var accountsDTO = _accountService.GetAllAccountsDTO();

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

                var account = _accountService.GetAccountDTOByIdAndClientEmail(id, email);
                if (account == null)
                {
                    return Forbid();
                }

                return Ok(account);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

    }

}

