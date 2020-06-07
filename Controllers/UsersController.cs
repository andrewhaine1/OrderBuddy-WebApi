using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Models;
using Ord.WebApi.Services.Data.User;

namespace Ord.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IOrdUserService _userService;

        public UsersController(IOrdUserService ordUserService)
        {
            _userService = ordUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] OrdUserResourceParameters resourceParams)
        {
            var userEnts = await _userService.GetOrdUsersAsync(resourceParams);

            return Ok(userEnts);
        }

        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUsers(int userId)
        {
            var userEnt = await _userService.GetOrdUserAsync(userId);

            if (userEnt == null)
                return NotFound(new { Error = $"A user with an Id of '{userId}' could not be found." });

            return Ok(userEnt);
        }

        [Route("getbyoauthid/{oauthId}", Name = "GetUserByOauthId")]
        public async Task<IActionResult> GetUser(int oauthId)
        {
            var userEnt = await _userService.GetOrdUserByOAuthIdAsync(oauthId);

            if (userEnt == null)
                return NotFound(new { Error = $"A user with an OAuth Id of '{ oauthId }' could not be found." });

            return Ok(userEnt);
        }

        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser(OrdUser ordUserMod)
        {
            var user = await _userService.GetOrdUserByOAuthIdAsync(ordUserMod.OauthId);

            if (user != null)
                return Conflict(new { Error = $"A user with the Oath Id '{ordUserMod.OauthId}' already exists." });

            var userEnt = new Entities.OrdUser
            {
                OauthId = ordUserMod.OauthId,
                FullName = ordUserMod.FullName,
                MobileNumber = ordUserMod.MobileNumber,
                EmailAddress = ordUserMod.EmailAddress,
                CanPlaceOrders = true
            };

            _userService.CreateUser(userEnt);
            if (!await _userService.SaveChangesAsync())
                throw new Exception($"Could not create user '{ordUserMod.FullName}'.");

            return Ok(userEnt);
        }

        [HttpPatch("{userId}", Name = "PatchUpdateUser")]
        public async Task<IActionResult> PatchUpdateUser(int userId,
            [FromBody] JsonPatchDocument<OrdUser> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userEnt = await _userService.GetOrdUserAsync(userId);
            if (userEnt == null)
                return NotFound(new { Error = $"A user with an Id of '{userId}' could not be found." });

            var userMod = new OrdUser
            {
                OauthId = userEnt.OauthId,
                FullName = userEnt.FullName,
                MobileNumber = userEnt.MobileNumber,
                EmailAddress = userEnt.EmailAddress,
                DeviceToken = userEnt.DeviceToken
            };

            patchDoc.ApplyTo(userMod);

            TryValidateModel(userMod);

            // Validation
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            userEnt.FullName = userMod.FullName;
            userEnt.MobileNumber = userMod.MobileNumber;
            userEnt.EmailAddress = userMod.EmailAddress;
            userEnt.DeviceToken = userMod.DeviceToken;

            _userService.UpdateUser(userEnt);

            if (!await _userService.SaveChangesAsync())
                throw new Exception($"Error updating user '{userMod.FullName}'.");

            return NoContent();
        }

        [HttpGet("{userId}/canplaceorders", Name = "CanPlaceOrders")]
        public async Task<IActionResult> CanPlaceOrders(int userId)
        {
            var userEnt = await _userService.GetOrdUserAsync(userId);
            if (userEnt == null)
                return NotFound(new { Error = $"A user with an Id of '{userId}' could not be found." });

            return Ok(new { userEnt.CanPlaceOrders });
        }

        [AllowAnonymous]
        [HttpPost("{userId}/refreshtoken", Name = "RefreshToken")]
        public async Task<IActionResult> RefreshToken(int userId, RefreshToken refreshToken)
        {
            var userEnt = await _userService.GetOrdUserAsync(userId);
            if (userEnt == null)
                return NotFound(new { Error = $"A user with an Id of '{userId}' could not be found." });

            var values = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>
            {
                new System.Collections.Generic.KeyValuePair<string, string>("client_id", "orderbuddy_password"),
                new System.Collections.Generic.KeyValuePair<string, string>("client_secret", "7baeb4e4"),
                new System.Collections.Generic.KeyValuePair<string, string>("grant_type", "refresh_token"),
                new System.Collections.Generic.KeyValuePair<string, string>("refresh_token", refreshToken.Token)
            };

            var formEnCoded = new System.Net.Http.FormUrlEncodedContent(values);

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await httpClient.PostAsync("https://accounts.orderbuddy.co.za/connect/token", formEnCoded); // URL will change to accounts.orderbuddy.co.za

                if (response != null)
                {
                    var httpStringContent = await response.Content.ReadAsStringAsync();
                    var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(httpStringContent);

                    if (responseObject != null)
                    {
                        if (responseObject.error != null)
                            return BadRequest(new { Error = "Invalid token." });

                        var token = new
                        {
                            AccessToken = responseObject.access_token,
                            RefreshToken = responseObject.refresh_token,
                            ExpiresIn = responseObject.expires_in
                        };

                        return Ok(token);
                    }
                }
            }

            return BadRequest(new { Error = "Could not refresh token." });
        }
    }
}
