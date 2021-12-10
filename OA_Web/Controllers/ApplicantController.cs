using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OA_DAL.Models;
using OA_Service.AppServices;
using OA_Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OA_Web.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly ApplicantAppService applicantAppService;
        private readonly IHttpClientFactory httpClientFactory;
        public string ErrorMessage { get; private set; }

        public ApplicantController(ApplicantAppService _applicantAppService,IHttpClientFactory _httpClientFactory)
        {
            applicantAppService = _applicantAppService;
            httpClientFactory = _httpClientFactory;
        }
        public IActionResult Index()
        {
            return View(applicantAppService.GetAllApplicants());
        }
        public async Task<List<SelectListItem>> DisplayALLCountriesAsync(string ErrorMessage) 
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://restcountries.com/v2/all?fields=name");
            var client = httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var resposeresult = await response.Content.ReadAsStreamAsync();
                var result = JsonSerializer.Deserialize<List<CountryNameModel>>(resposeresult);
                List<SelectListItem> countries = new List<SelectListItem>();
                foreach (var c in result)
                {
                    countries.Add(new SelectListItem() { Text = c.name, Value = c.name });
                }
                return countries;
            }
            else
            {
                ErrorMessage = $"error in Api https://restcountries.com/v2/all?fields=name: {response.ReasonPhrase}";
                return new List<SelectListItem>();
            }
            
        }
        public async Task<IActionResult> AddApplicant()
        {
            ViewBag.countries = DisplayALLCountriesAsync(ErrorMessage).Result;
            if (ErrorMessage != null) 
            {
                return Content(ErrorMessage);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddApplicant(ApplicantViewModel applicantViewModel)
        {
            ViewBag.countries = DisplayALLCountriesAsync(ErrorMessage).Result;
            if (ErrorMessage != null)
            {
                return Content(ErrorMessage);
            }
            if (!ModelState.IsValid)
            {
                return View(applicantViewModel);
            }
            applicantAppService.SaveNewApplicant(applicantViewModel);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult UpdateApplicant(int id)
        {
            ViewBag.countries = DisplayALLCountriesAsync(ErrorMessage).Result;
            if (ErrorMessage != null)
            {
                return Content(ErrorMessage);
            }
            return View(applicantAppService.GetById(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateApplicant(Applicant applicant) 
        {
            if (!ModelState.IsValid)
            {
                return View(applicant);
            }
            //var applicant = applicantAppService.GetByModelId(applicantViewModel.ID);
            //applicant.CountryOfOrigin = applicantViewModel.CountryOfOrigin;
            ViewBag.countries = DisplayALLCountriesAsync(ErrorMessage).Result;
            if (ErrorMessage != null)
            {
                return Content(ErrorMessage);
            }
            applicantAppService.UpdateApplicantMyModel(applicant);
            return RedirectToAction("Index","Applicant");
        }
        public IActionResult Hired(int id) 
        {
            var Applicant = applicantAppService.GetByModelId(id);
            applicantAppService.ChangeHiredStatus(Applicant);
            return RedirectToAction("Index");
        }
        public IActionResult unHired(int id)
        {
            var Applicant = applicantAppService.GetByModelId(id);
            applicantAppService.ChangeHiredStatus(Applicant);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteApplicant(int id) 
        {
            applicantAppService.DeleteApplicant(id);
            return RedirectToAction("Index");
        }
        public IActionResult ApplicantDetails(int id) 
        {
            return View(applicantAppService.GetById(id));
        }
    }
}
