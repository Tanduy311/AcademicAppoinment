using Microsoft.EntityFrameworkCore;
using AcademicAppoinment.DTOs;
using AcademicAppoinment.Models;

namespace AcademicAppoinment.Services.Lecturers
{
    public class AvailabilitySlotService : IAvailabilitySlotService
    {
        private readonly AppDbContext _context;

        public AvailabilitySlotService(AppDbContext context)
        {
            _context = context;
        }

        public AvailabilitySlot CreateSlot(int userId, CreateAvailabilitySlotRequest request, out string error)
        {
            error = string.Empty;

            // Find lecturer for current user
            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found for current user.";
                return null!;
            }

            // BR-04: Validate time - no past times
            if (request.StartTime < DateTime.Now)
            {
                error = "Cannot create slot in the past.";
                return null!;
            }

            // BR-04: Validate time - start must be before end
            if (request.StartTime >= request.EndTime)
            {
                error = "Start time must be before end time.";
                return null!;
            }

            // BR-05: Check for overlapping slots for same lecturer
            var overlapping = _context.AvailabilitySlots
                .Where(s => s.LecturerId == lecturer.LecturerId)
                .Any(s => s.StartTime < request.EndTime && s.EndTime > request.StartTime);

            if (overlapping)
            {
                error = "This time slot overlaps with an existing slot.";
                return null!;
            }

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturer.LecturerId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                MeetingType = request.MeetingType,
                LocationOrLink = request.LocationOrLink,
                IsAvailable = true,
                CreatedAt = DateTime.Now
            };

            _context.AvailabilitySlots.Add(slot);
            _context.SaveChanges();

            return slot;
        }

        public IEnumerable<AvailabilitySlotResponse> GetMySlots(int userId, out string error)
        {
            error = string.Empty;

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found for current user.";
                return Enumerable.Empty<AvailabilitySlotResponse>();
            }

            var slots = _context.AvailabilitySlots
                .Where(s => s.LecturerId == lecturer.LecturerId)
                .OrderBy(s => s.StartTime)
                .ToList()
                .Select(s => new AvailabilitySlotResponse
                {
                    AvailabilitySlotId = s.AvailabilitySlotId,
                    LecturerId = s.LecturerId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MeetingType = s.MeetingType,
                    LocationOrLink = s.LocationOrLink,
                    IsAvailable = s.IsAvailable,
                    CreatedAt = s.CreatedAt
                });

            return slots;
        }

        public AvailabilitySlot? UpdateSlot(int userId, int slotId, UpdateAvailabilitySlotRequest request, out string error)
        {
            error = string.Empty;

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found for current user.";
                return null;
            }

            var slot = _context.AvailabilitySlots.FirstOrDefault(s => s.AvailabilitySlotId == slotId);
            if (slot == null)
            {
                error = "Slot not found.";
                return null;
            }

            // Ownership check: slot must belong to current lecturer
            if (slot.LecturerId != lecturer.LecturerId)
            {
                error = "You can only update your own slots.";
                return null;
            }

            // BR-04: Validate time - no past times
            if (request.StartTime < DateTime.Now)
            {
                error = "Cannot set slot start time in the past.";
                return null;
            }

            // BR-04: Validate time - start must be before end
            if (request.StartTime >= request.EndTime)
            {
                error = "Start time must be before end time.";
                return null;
            }

            // BR-05: Check for overlapping slots (excluding the current slot)
            var overlapping = _context.AvailabilitySlots
                .Where(s => s.LecturerId == lecturer.LecturerId && s.AvailabilitySlotId != slotId)
                .Any(s => s.StartTime < request.EndTime && s.EndTime > request.StartTime);

            if (overlapping)
            {
                error = "Updated time slot would overlap with another existing slot.";
                return null;
            }

            slot.StartTime = request.StartTime;
            slot.EndTime = request.EndTime;
            slot.MeetingType = request.MeetingType;
            slot.LocationOrLink = request.LocationOrLink;

            _context.SaveChanges();
            return slot;
        }

        public bool DeleteSlot(int userId, int slotId, out string error)
        {
            error = string.Empty;

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found for current user.";
                return false;
            }

            var slot = _context.AvailabilitySlots.FirstOrDefault(s => s.AvailabilitySlotId == slotId);
            if (slot == null)
            {
                error = "Slot not found.";
                return false;
            }

            // Ownership check
            if (slot.LecturerId != lecturer.LecturerId)
            {
                error = "You can only delete your own slots.";
                return false;
            }

            // Check if slot has appointments
            var hasAppointments = _context.Appointments
                .Any(a => a.AvailabilitySlotId == slotId && a.Status != "Cancelled" && a.Status != "Rejected");

            if (hasAppointments)
            {
                error = "Cannot delete slot that has active appointments.";
                return false;
            }

            _context.AvailabilitySlots.Remove(slot);
            _context.SaveChanges();
            return true;
        }
    }
}
