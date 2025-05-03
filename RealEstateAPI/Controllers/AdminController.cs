using RealEstateAPI.DTOs.UserDTos.Administration;
using RealEstateAPI.DTOs.UserDTos.Login;
using RealEstateAPI.DTOs.UserDTos.Register;
using RealEstateAPI.Helpers;
using RealEstateAPI.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        #region Fields
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        #endregion

        #region Constructor
        public AdminController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
        #endregion

        #region Admin Functions

        [HttpGet("[action]")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> ListOfUsers()
        {
            var response = await _authService.ListOfUsers();
            return Ok(response);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> ActivateUser(UserActivation activation)
        {
            var response = await _authService.UserActivation(activation);
            if (response == "Not Exist") return BadRequest("No user with this ID!");
            return Ok(response);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> ApproveProperty(Guid propertyId)
        {
            var response = await _userService.ApproveProperty(propertyId);
            return Ok(response);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var response = await _userService.BlockUser(userId);
            return Ok(response);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var response = await _userService.UnblockUser(userId);
            return Ok(response);
        }

        #endregion
    }
}
