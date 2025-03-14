using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingZone.Services;
using ParkingZone.ViewModels.ZoneVMs;

namespace ParkingZone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ParkingZonesController : Controller
    {
        private readonly IParkingZoneService _zoneService;

        public ParkingZonesController(IParkingZoneService zoneservice)
        {
            _zoneService = zoneservice;
        }

        // GET: Admin/ParkingZones
        public IActionResult Index()
        {
            var parkingZoneListItemVMs = _zoneService.GetAll().Select(x => new ParkingZonesListItemVM(x)).ToList();
            return View(parkingZoneListItemVMs);
        }

        // GET: Admin/ParkingZones/Details/5
        public IActionResult Details(Guid zoneId)
        {
            var parkingZone = _zoneService.GetById(zoneId);
            if (parkingZone == null)
            {
                return NotFound();
            }
            var parkingZoneDetailsVM = new DetailsParkingZoneViewModel(parkingZone);

            return View(parkingZoneDetailsVM);
        }

        // GET: Admin/ParkingZones/Create
        public IActionResult Create()
        {
            var createParkingZoneVM = new CreateParkingZoneViewModel()
            {
                Address = string.Empty,
                Name = string.Empty
            };

            return View(createParkingZoneVM);
        }

        // POST: Admin/ParkingZones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateParkingZoneViewModel createParkingZoneVM)
        {
            if (ModelState.IsValid)
            {
                var newZone = createParkingZoneVM.MapToModel();
                _zoneService.Add(newZone);
                return RedirectToAction(nameof(Index));
            }
            return View(createParkingZoneVM);
        }
        
        // GET: Admin/ParkingZones/Edit/5
        public IActionResult Edit(Guid zoneId)
        {
            var existingParkingZone = _zoneService.GetById(zoneId);
            if (existingParkingZone == null)
            {
                return NotFound();
            }
            var editZoneVM = new EditParkingZoneViewModel()
            {
                Id = existingParkingZone.Id,
                Name = existingParkingZone.Name,
                Address = existingParkingZone.Address
            };
            return View(editZoneVM);
        }

        // POST: Admin/ParkingZones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid Id, EditParkingZoneViewModel editParkingZoneVM)
        {
            if (Id != editParkingZoneVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingParkingZone = _zoneService.GetById(editParkingZoneVM.Id);
                if (existingParkingZone == null)
                {
                    return NotFound();
                }
                editParkingZoneVM.MapToModel(existingParkingZone);
                _zoneService.Update(existingParkingZone);

                return RedirectToAction(nameof(Index));
            }
            return View(editParkingZoneVM);
        }

        // GET: Admin/ParkingZones/Delete/5
        public IActionResult Delete(Guid zoneId)
        {
            var existingParkingZone = _zoneService.GetById(zoneId);
            if (existingParkingZone == null)
            {
                return NotFound();
            }
            var detailsParkingZoneVM = new DetailsParkingZoneViewModel(existingParkingZone);
            return View(detailsParkingZoneVM);
        }

        // POST: Admin/ParkingZones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid Id)
        {
            var existingParkingZone = _zoneService.GetById(Id);
            if (existingParkingZone != null)
            {
                _zoneService.Delete(existingParkingZone);
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
