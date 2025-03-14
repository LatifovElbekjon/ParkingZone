using Microsoft.AspNetCore.Mvc;
using ParkingZone.ViewModels.SlotVMs;
using ParkingZone.Services;
using ParkingZone.Entities;
using ParkingZone.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ParkingZone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SlotsController : Controller
    {
        private readonly ISlotService _slotService;
        private readonly IParkingZoneService _parkingZoneService;

        public SlotsController(ISlotService slotService, IParkingZoneService parkingZoneService)
        {
            _slotService = slotService;
            _parkingZoneService = parkingZoneService;
        }

        // GET: Admin/Slots
        public IActionResult Index(Guid zoneId, string sortByTariff = "All", string sortByStatus = "All")
        {
            var zone = _parkingZoneService.GetById(zoneId);
            if (zone == null)
            {
                return BadRequest("Zone does not exist");
            }

            var slotsVMs = _slotService.GetByZoneId(zoneId).Select(n => new SlotsItemViewModel(n)).ToList();

            // Store current sorting state
            ViewData["CurrentSortByTariff"] = sortByTariff;
            ViewData["CurrentSortByStatus"] = sortByStatus;

            // Filter by Status first, if selected
            if (!string.IsNullOrEmpty(sortByStatus) && sortByStatus != "All")
            {
                if (sortByStatus == "Available")
                {
                    slotsVMs = slotsVMs.Where(s => s.IsAvailableForBooking).ToList();
                }
                else if (sortByStatus == "NotAvailable")
                {
                    slotsVMs = slotsVMs.Where(s => !s.IsAvailableForBooking).ToList();
                }
            }

            // Then sort by Tariff if selected
            if (!string.IsNullOrEmpty(sortByTariff) && sortByTariff != "All")
            {
                switch (sortByTariff)
                {
                    case "Econom":
                        slotsVMs = slotsVMs.Where(s => s.Tariff == ESlotCategory.Econom).ToList();
                        break;
                    case "Business":
                        slotsVMs = slotsVMs.Where(s => s.Tariff == ESlotCategory.Business).ToList();
                        break;
                    case "Super":
                        slotsVMs = slotsVMs.Where(s => s.Tariff == ESlotCategory.Super).ToList();
                        break;
                }
            }

            // Now, after filtering, we can order the slots by Number or any other criteria
            slotsVMs = slotsVMs.OrderBy(s => s.Number).ToList();  // Assuming sorting by number after filtering

            ViewData["ZoneId"] = zone.Id;
            ViewData["ZoneName"] = zone.Name;

            return View(slotsVMs);
        }

        [HttpGet]
        public IActionResult Create(Guid zoneId)
        {
            var zone = _parkingZoneService.GetById(zoneId);
            if (zone == null)
            {
                return NotFound("Zone not exist");
            }

            var createSlotVM = new CreateSlotViewModel()
            {
                ZoneId = zoneId
            };
            ViewData["ZoneName"] = zone.Name;
            return View(createSlotVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateSlotViewModel createSlotVM)
        {
            var zone = _parkingZoneService.GetById(createSlotVM.ZoneId);
            if (zone == null)
            {
                return NotFound("Zone not exist");
            }

            if (_slotService.SlotWithThisNumberExists(createSlotVM.ZoneId, createSlotVM.Number))
            {
                ModelState.AddModelError("Number", "This number already exist");
            }

            if(createSlotVM.Number <= 0)
            {
                ModelState.AddModelError("Number", "Number must be greater than 0");
            }

            if (!ModelState.IsValid)
            {
                return View(createSlotVM);
            }

            var slot = createSlotVM.MapToModel();
            _slotService.Add(slot);
            return RedirectToAction(nameof(Index), new { createSlotVM.ZoneId });
        }

        [HttpGet]
        public IActionResult Edit(Guid slotId)
        {
            var slot = _slotService.GetById(slotId);    
            if (slot == null)
            {
                return BadRequest("Slot not exist");
            }

            if (AreThereReservationsInUse(slot))
            {
                ViewBag.Warning = "this slot has using reservations, so you can't change it";
            }

            var editSlotVM = new EditSlotViewModel(slot);

            return View(editSlotVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditSlotViewModel editSlotVM)
        {
            var zone = _parkingZoneService.GetById(editSlotVM.ZoneId);
            if (zone == null)
            {
                return NotFound("Zone not exist");
            }

            var slot = _slotService.GetById(editSlotVM.Id);
            if (slot == null)
            {
                return NotFound("Slot not exist");
            }

            if (AreThereReservationsInUse(slot))
            {
                ModelState.AddModelError("", "This slot has using reservations, so you can't change it");
            }

            if (slot.Number != editSlotVM.Number)
            {
                if (_slotService.SlotWithThisNumberExists(editSlotVM.ZoneId, editSlotVM.Number))
                {
                    ModelState.AddModelError("Number", "This number already exist");
                }
            }
            
            if (editSlotVM.Number <= 0)
            {
                ModelState.AddModelError("Number", "Number must be greater than 0");
            }

            if (!ModelState.IsValid)
            {
                return View(editSlotVM);
            }

            editSlotVM.MapToModel(slot);
            _slotService.Update(slot);
            return RedirectToAction(nameof(Index), new { editSlotVM.ZoneId });
        }

        [HttpGet]
        public IActionResult Details(Guid slotId)
        {
            var slot = _slotService.GetById(slotId);
            if (slot == null)
            {
                return NotFound("Slot not exist");
            }

            var zone = _parkingZoneService.GetById(slot.ZoneId);
            if (zone == null)
            {
                return NotFound("Zone not exist");
            }
            ViewData["ZoneName"] = zone.Name;
            if (AreThereReservationsInUse(slot))
            {
                ViewBag.Warning = "this slot has using reservations, so you can't change it";
            }

            var detailsSlotVM = new DetailsSlotViewModel(slot);
            return View(detailsSlotVM);
        }

        [HttpGet]
        public IActionResult Delete(Guid slotId)
        {
            var slot = _slotService.GetById(slotId);
            if (slot == null)
            {
                return NotFound("Slot not exist");
            }

            var zone = _parkingZoneService.GetById(slot.ZoneId);
            if (zone == null)
            {
                return NotFound("Zone not exist");
            }

            if (AreThereReservationsInUse(slot))
            {
                ViewBag.Warning = "This slot has using reservations, so you can't change it";
            }

            ViewData["ZoneName"] = zone.Name;

            var detailsSlotVM = new DetailsSlotViewModel(slot);
            return View(detailsSlotVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(Guid slotId)
        {
            var slot = _slotService.GetById(slotId);
            if (slot != null)
            {
                if (AreThereReservationsInUse(slot))
                {
                    return RedirectToAction(nameof(Delete), new { slotId = slot.Id });
                }
                _slotService.Delete(slot);
            }
            else
            {
                return NotFound("Slot not exist");
            }           

            return RedirectToAction(nameof(Index), new { zoneId = slot.ZoneId });
        }

        private bool AreThereReservationsInUse(Slot slot)
        {
            return slot.Reservations.Any(c => c.Status == Enums.ReservationStatus.InProgress || c.Status == Enums.ReservationStatus.Pending);
        }
    }
}
