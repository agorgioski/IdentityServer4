using System.Text;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Models;
using Newtonsoft.Json;
using Common.Password;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Web.Controllers
{
    [Authorize("admin")]
    // [ClaimRequirement("role", "admin")]
    public class AdminController : Controller
    {
        #region Members
        private readonly string baseUrl = String.Empty;
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        private List<SelectListItem> roles = new List<SelectListItem>() {
            new SelectListItem {
                Text = "admin",
                Value = "admin"
            },
            new SelectListItem {
                Text = "user",
                Value = "user"
            },
        };

        public AdminController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _client = _clientFactory.CreateClient();
            _configuration = configuration;
            baseUrl = configuration.GetSection("URLs")["adminApiBaseUrl"];
        }

        #endregion Members

        #region Functions

        private async Task setAuthorizationHeaders() 
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            _client.SetBearerToken(accessToken);
        }

        private async Task<List<AppUser>> GetUsersAsync()
        {
            await setAuthorizationHeaders();
            List<AppUser> users = null;

            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
            request.Headers.Add("Accept", "application/json");

            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                users = await response.Content.ReadAsAsync<List<AppUser>>();
            }
            return users;
        }

        private async Task<AppUser> GetUserAsync(long id)
        {
            string path = baseUrl + "/" + id;

            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "application/json");

            HttpResponseMessage response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("User cannot be found, status code: " + response.StatusCode);
            }
            return await response.Content.ReadAsAsync<AppUser>();
        }
        private async Task<long> CreateUserAsync(AppUser user, IFormCollection form)
        {
            await setAuthorizationHeaders();
            //TODO: This needs to be moved to the so far nonexistent service layer
            string password = form["password"];
            string role = form["role"];

            user.Claims = new List<Claim>();
            switch (role)
            {
                case ("user"):
                    user.Claims.Add(new Claim(
                                    type: "role",
                                    value: "user"));
                    break;
                case ("admin"):
                    user.Claims.Add(new Claim(
                                    type: "role",
                                    value: "user"));
                    user.Claims.Add(new Claim(
                                    type: "role",
                                    value: "admin"));
                    break;
                default:
                    //shouldn't get to here if there's proper validation 
                    throw new Exception("Something went wrong with the dropdown menu");
            }
            user.Claims.Add(new Claim(
                            type: "username",
                            value: user.Username));
            user.PasswordSalt = PasswordHashing.PasswordSaltInBase64();
            user.PasswordHash = PasswordHashing.PasswordToHashBase64(password, user.PasswordSalt);

            // TODO: Change this id generation if it behaves unexpectedly(duplicates etc)
            user.SubjectId = PasswordHashing.PasswordSaltInBase64();

            HttpResponseMessage response = await _client.PostAsJsonAsync(baseUrl, user);
            response.EnsureSuccessStatusCode();

            string[] parts = response.Headers.Location.ToString().Split("/");
            return Convert.ToInt64(parts[parts.Length - 1]);
        }


        private async Task EditUserAsync(AppUser user)
        {
            await setAuthorizationHeaders();
            string path = baseUrl + "/" + user.Id;
            var request = new HttpRequestMessage(HttpMethod.Patch, path)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(user),
                    Encoding.UTF8,
                    "application/json"
                 )
            };
            request.Headers.Add("Accept", "application/json");

            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private async Task DeleteUserAsync(long id)
        {
            await setAuthorizationHeaders();
            string path = baseUrl + "/" + id;

            await _client.DeleteAsync(path);
        }

        #endregion Functions

        #region Methods
        // GET: Admin
        public IActionResult Index()
        {
            return View();
        }


        // GET: Admin/All
        public async Task<IActionResult> All()
        {
            var users = await GetUsersAsync();
            return View(users);
        }

        // GET: Admin/Details/{id}
        public async Task<IActionResult> Details(long id)
        {
            AppUser user = null;
            try
            {
                user = await GetUserAsync(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return View(user);
        }

        // GET: Admin/Edit/{id}
        public async Task<IActionResult> Edit(long id)
        {
            AppUser user = null;
            try
            {
                user = await GetUserAsync(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            ViewBag.Roles = roles;
            return View(user);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser user)
        {
            if (ModelState.IsValid)
            {
                await EditUserAsync(user);
                return RedirectToAction(nameof(All));
            }
            return View(user);
        }

        // GET Admin/Create
        public IActionResult Create()
        {
            ViewBag.Roles = roles;
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser user)
        {
            long createdUserId;
            try
            {
                createdUserId = await CreateUserAsync(user, Request.Form);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return RedirectToAction(nameof(Details), routeValues: new { id = createdUserId });
        }

        // GET: Case/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            AppUser user = null;
            try
            {
                user = await GetUserAsync(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return View(user);
        }

        // POST: Case/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await DeleteUserAsync(id);
            return RedirectToAction(nameof(All));
        }

        #endregion Methods
    }
}