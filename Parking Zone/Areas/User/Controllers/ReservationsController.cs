using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingZone.Services;
using ParkingZone.ViewModels.ReservationVMs;
using System.Security.Claims;

namespace ParkingZone.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ISlotService _slotService;

        public ReservationsController(ISlotService slotService, IReservationService reservationService)
        {
            _slotService = slotService;
            _reservationService = reservationService;
        }

        // GET: ReservationsController
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservationsVM = _reservationService.GetReservationsByUserId(userId!).Select(f => new ReservationsItemViewModel(f)).ToList();
            return View(reservationsVM);
        }

        [HttpGet]
        public IActionResult DeleteReservations(string filter = "All")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = _reservationService.GetReservationsByUserId(userId!).ToList();

            if (filter == "Finished")
            {
                // Delete finished reservations
                var finishedReservations = reservations.Where(r => r.Status == ParkingZone.Enums.ReservationStatus.Finished).ToList();
                foreach (var reservation in finishedReservations)
                {
                    _reservationService.Delete(reservation);
                }
            }
            else
            {
                // Delete all reservations
                foreach (var reservation in reservations)
                {
                    _reservationService.Delete(reservation);
                }
            }

            // After deletion, redirect to the Index action
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Prolong(Guid reservationId)
        {
            var reservation = _reservationService.GetById(reservationId);
            if (reservation == null)
            {
                return NotFound("Reservation not found");
            }
            var prolongVM = new ProlongViewModel(reservation);
            return View(prolongVM);
        }

        [HttpPost]
        public IActionResult Prolong(ProlongViewModel prolongVM)
        {
            var reservation = _reservationService.GetById(prolongVM.ReservationId);
            if (reservation == null)
            {
                return NotFound("Reservation not found");
            }

            var slot = _slotService.GetById(reservation.SlotId);
            if (slot == null)
            {
                return NotFound("Slot not found");
            }

            if (!_slotService.IsSlotFree(slot, reservation.StartTime.AddHours(reservation.Duration), prolongVM.AdditionalHours))
            {
                ModelState.AddModelError("AdditionalHours", "Slot not free this time");
            }

            if (!ModelState.IsValid)
            {
                prolongVM.ZoneAddress = reservation.Slot.Zone.Address;
                prolongVM.SlotNumber = reservation.Slot.Number;
                prolongVM.EndDate = reservation.StartTime.AddHours(reservation.Duration).ToString();
                return View(prolongVM);
            }

            _reservationService.Prolong(reservation, prolongVM.AdditionalHours);

            return RedirectToAction("Index", "Reservations", new { area = "User" });
        }
    }
}
