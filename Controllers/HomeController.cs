﻿using DataMashUp.DTO;
using DataMashUp.Models;
using DataMashUp.Repo;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;


namespace DataMashUp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		private readonly IWebHostEnvironment _hostingEnvironment;

		private readonly string _Prescription;
		private readonly IRepository _repository;

		public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, IRepository repository)
		{
			_logger = logger;
			_hostingEnvironment = hostingEnvironment;
			_repository = repository;
		}

		public async Task<IActionResult> Index()
		{
			var news = await _repository.GetBreakingNews();
			var viewModel = new IndexDTO
			{
				Locations = await GetLOcation(),
				Articles = news.Articles.Take(5).ToList()

			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Index(IndexDTO viewModel)
		{
			if(ModelState.IsValid)
			{
				viewModel.Locations = await GetLOcation();
				return View(viewModel);
			}
			viewModel.Locations = await GetLOcation();
			var param = new { healthCondition = viewModel.HealthCondition, age = viewModel.Age, gender = viewModel.Gender };

			return RedirectToAction(nameof(Prescription), param);
		}

		public async Task<IActionResult> Prescription(string healthCondition, int age, string gender )
		{

			var prescription = await _repository.GetPrescription(healthCondition, age.ToString());
			prescription.Age = age;
			prescription.Gender = gender;
			return View(prescription);
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		[NonAction]
		public async Task<List<SelectListItem>> GetLOcation()
		{

			var PhoneAbsolutePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Static/location.json");
			using (var fs = System.IO.File.OpenText(PhoneAbsolutePath))
			{

				var jsonData = JsonConvert.DeserializeObject<List<LocationData>>(await fs.ReadToEndAsync());

				var locations = jsonData.Select(x => new SelectListItem
				{
					Text = x.city ,
					Value = x.city,
				}).ToList();
				//locations.Insert(0, new SelectListItem { Text = "Select location", Value = null });

				return locations;


			}


		}
	}
}