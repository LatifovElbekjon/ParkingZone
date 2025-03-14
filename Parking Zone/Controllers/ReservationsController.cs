using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkingZone.Services;
using ParkingZone.ViewModels.ReservationVMs;
using ParkingZone.ViewModels.SlotVMs;
using System.Globalization;
using System.Security.Claims;

namespace ParkingZone.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ISlotService _slotService;
        private readonly IParkingZoneService _parkingZoneService;
        private readonly IReservationService _reservationService;   

        public ReservationsController(ISlotService slotService, 
                                      IParkingZoneService parkingZoneService, 
                                      IReservationService reservationService)
        {
            _slotService = slotService;
            _parkingZoneService = parkingZoneService;
            _reservationService = reservationService;
        }

        [HttpGet]
        public IActionResult FreeSlots()
        {
            var zones = _parkingZoneService.GetAll();
            if (zones != null && zones.Any())
            {
                ViewBag.Zones = zones.Select(z => new SelectListItem
                {
                    Value = z.Id.ToString(),
                    Text = z.Name
                }).ToList();
            }
            else
            {
                ViewBag.Zones = new List<SelectListItem>(); // Handle the case where no zones are available
            }
            return View(new CreateReservationViewModel() { StartTime = DateTime.Today});
        }

        [HttpPost]
        public IActionResult ShowFreeSlots(CreateReservationViewModel createReservationVM)
        {
            if(!ModelState.IsValid)
            {
                var zones = _parkingZoneService.GetAll();
                if (zones != null && zones.Any())
                {
                    ViewBag.Zones = zones.Select(z => new SelectListItem
                    {
                        Value = z.Id.ToString(),
                        Text = z.Name
                    }).ToList();
                }
                else
                {
                    ViewBag.Zones = new List<SelectListItem>(); // Handle the case where no zones are available
                }

                return View(nameof(FreeSlots), new CreateReservationViewModel() { StartTime = DateTime.Today });
            }
            var zone = _parkingZoneService.GetById(createReservationVM.ZoneId);
            if (zone == null)
            {
                return BadRequest("Zone not exist");
            }

            var slotsVMs = _slotService.GetFreeSlotsByZoneId(
                createReservationVM.ZoneId, 
                createReservationVM.StartTime, 
                createReservationVM.Duration)
                .Select(n => new SlotsItemViewModel(n)).ToList();
            var freeslotsVM = new FreeSlotsViewModel();
            freeslotsVM.FreeSlots = slotsVMs;   
            freeslotsVM.StartTime = createReservationVM.StartTime;  
            freeslotsVM.Duration = createReservationVM.Duration;    
            freeslotsVM.ZoneName = zone.Name;
            return View(freeslotsVM);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Reserve(Guid slotId, string startTime, int duration)
        {
            var slot = _slotService.GetById(slotId);
            if (slot == null)
            {
                return NotFound("Slot not found");
            }
            var zone = _parkingZoneService.GetById(slot.ZoneId);
            if (zone == null)
            {
                return NotFound("Zone not found");
            }
            var reserveVM = new ReservationViewModel(slot, startTime, duration);
            return View(reserveVM);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Reserve(ReservationViewModel reservationVM)
        {
            var slot = _slotService.GetById(reservationVM.SlotId);
            if (slot == null)
            {
                return NotFound("Slot not found");
            }

            DateTime startTime;
            var formats = new[] { "dd.MM.yyyy H:mm", "dd.MM.yyyy HH:mm" };
            var culture = new CultureInfo("ru-RU");

            // Try to parse the StartTime using the formats specified
            if (!DateTime.TryParseExact(reservationVM.StartTime, formats, culture, DateTimeStyles.None, out startTime))
            {
                ModelState.AddModelError("StartTime", "Invalid date format. Please use 'dd.MM.yyyy H:mm'.");
            }

            if (!_slotService.IsSlotFree(slot, DateTime.Parse(reservationVM.StartTime), reservationVM.Duration))
            {
                ModelState.AddModelError("StartTime", "Slot is not free with this time");
                ModelState.AddModelError("Duration", "Slot is not free with this duration");
            }

            if (!ModelState.IsValid)
            {
                return View(reservationVM);
            }

            var reservation = reservationVM.MapToModel();
            reservation.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            _reservationService.Add(reservation);
            reservationVM.ZoneName = slot.Zone.Name;
            ViewBag.SuccessMessage = "Successfully booked";
            return RedirectToAction("Index", "Reservations", new { area = "User" });
        }
    }
}
