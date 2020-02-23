using System.Linq;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

using Web.AutoMapper;

namespace Web.Controllers
{
    [Authorize("user")]  
    public class CaseController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        private readonly string baseUrl = String.Empty;


        public CaseController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _client = _clientFactory.CreateClient();
            _configuration = configuration;

            baseUrl = configuration.GetSection("URLs")["caseApiBaseUrl"];
            Mapper.initMapper();
        }

        private async Task setAuthorizationHeaders() 
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            _client.SetBearerToken(accessToken);
        }

        private async Task<List<Case>> GetCasesAsync()
        {
            await setAuthorizationHeaders();
            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
            request.Headers.Add("Accept", "application/json");

            List<Case> cases = null;
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                cases = await response.Content.ReadAsAsync<List<Case>>();
            }

            return cases;
        }

        private async Task<Case> GetCaseAsync(long id)
        {
            await setAuthorizationHeaders();
            string path = baseUrl + "/" +id;
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "application/json");

            Case @case = null;
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                @case = await response.Content.ReadAsAsync<Case>();
            }

            return @case;
        }
        
        private async Task<CaseKind> GetCaseKindByType(string type) 
        {
            await setAuthorizationHeaders();
            string path = baseUrl + "/casekind/" + type; 
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "application/json");

            CaseKind caseKind = null;
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                caseKind = await response.Content.ReadAsAsync<CaseKind>();
            }

            return caseKind;
        }


        private async Task<long> CreateCaseAsync(Case @case)
        {
            await setAuthorizationHeaders();
            HttpResponseMessage response = await _client.PostAsJsonAsync(
                baseUrl, @case);
            response.EnsureSuccessStatusCode();

            // Probably a better way to do this
            string [] parts = response.Headers.Location.ToString().Split("/");
            return Convert.ToInt64(parts[parts.Length-1]);
        }

        private async Task<bool> EditCaseAsync(Case @case)
        {
            await setAuthorizationHeaders();
            string path = baseUrl + "/" + @case.Id;
            var request = new HttpRequestMessage(HttpMethod.Patch, path)
             {
                 Content = new StringContent(
                    JsonConvert.SerializeObject(@case),
                    Encoding.UTF8,
                    "application/json"
                 )
             };
            request.Headers.Add("Accept", "application/json");
            
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return true;
        }

        private async Task<bool> DeleteCaseAsync(long id)
        {
            await setAuthorizationHeaders();
            string path = baseUrl + "/" + id;
            
            await _client.DeleteAsync(path);

            return true;
        }

        // GET: Case
        public IActionResult Index()
        {
            return View();
        }
        
        // GET: Case/All
        public async Task<IActionResult> All()
        {
            var cases = await GetCasesAsync();
            return View(cases);
        }

        // GET: Case/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var @case = await GetCaseAsync(id);

            return View(@case);
        }

        // GET: Case/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Case/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,CaseNumber,Kind,Status,Title")] Case @case)
        {
            if (ModelState.IsValid)
            {
                long createdCaseId = await CreateCaseAsync(@case);

                return RedirectToAction(nameof(Details), routeValues: new { id = createdCaseId });
            }
            return View(@case);
        }

        // GET: Case/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            Case @case = await GetCaseAsync(id);
            return View(@case);
        }

        // POST: Case/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Description,CaseNumber,Kind,Status,Title")] Case @case)
        {
            if (ModelState.IsValid) 
            {
                await EditCaseAsync(@case);
                return RedirectToAction(nameof(All));
            }
            return View(@case);
        }

        // GET: Case/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            Case @case = await GetCaseAsync(id);
            return View(@case);
        }

        // POST: Case/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await DeleteCaseAsync(id);
            return RedirectToAction(nameof(All));
        }

        #region React methods

        // GET: Case/React
        public IActionResult React()
        {
            return View();
        }

        // GET cases
        [Route("cases")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> Cases()
        {
            var cases = await GetCasesAsync();
            var caseModels = Mapper.mapper.Map<IEnumerable<Case>, IEnumerable<CaseModel>>(cases).ToList();
            return Json(caseModels);
        }
        
        [HttpPost]
        public async Task<ActionResult> EditCase(CaseModel caseModel)
        {
            var @case = Mapper.mapper.Map<CaseModel, Case>(caseModel);
            @case.Kind = await GetCaseKindByType(caseModel.Kind);
            await EditCaseAsync(@case);
            return Json("Success!");
        }

        [HttpPost]
        public async Task<ActionResult> CreateCase(CaseModel caseModel)
        {
            var @case = Mapper.mapper.Map<CaseModel, Case>(caseModel);
            @case.Kind = await GetCaseKindByType(caseModel.Kind);
            await CreateCaseAsync(@case);
            return Json("Success!");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCase([FromBody]String id)
        {
            await DeleteCaseAsync(Convert.ToInt64(id));
            return Json("Success!");
        }


        #endregion
    }
}
