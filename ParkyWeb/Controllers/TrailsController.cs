using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ParkyWeb.Controllers
{
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;

        public TrailsController(INationalParkRepository npRepo, ITrailRepository trailRepo)
        {
            _npRepo = npRepo;
            _trailRepo = trailRepo;
        }

        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        public async Task<IActionResult> Upsert(int? Id)
        {
            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

            TrailsViewModel objVM = new TrailsViewModel()
            {
                NationalParkList = npList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()
            };

            if (Id == null)
            {
                //this will be true for Insert/Create
                return View(objVM);
            }

            //Flow will come here for update
            objVM.Trail = await _trailRepo.GetAsync(SD.TrailAPIPath, Id.GetValueOrDefault());
            if(objVM == null)
            {
                return NotFound();
            }

            return View(objVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsViewModel obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Trail.Id == 0)
                {
                    await _trailRepo.CreateAsync(SD.TrailAPIPath, obj.Trail);
                } 
                else
                {
                    await _trailRepo.UpdateAsync(SD.TrailAPIPath + obj.Trail.Id, obj.Trail);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailAPIPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            var status = await _npRepo.DeleteAsync(SD.TrailAPIPath, Id);
            if (status)
            {
                return Json(new { success = true, message = "Delete Successful" });
            }

            return Json(new { success = false, message = "Delete Unsuccessful" });
        }
    }
}