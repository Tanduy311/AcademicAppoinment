using AcademicAppoinment.DTOs;
using AcademicAppoinment.Models;
using System.Collections.Generic;

namespace AcademicAppoinment.Services.Lecturers
{
    public interface ILecturerService
    {
        IEnumerable<Lecturer> SearchLecturers(string? name, string? department, string? specialization, int page = 1, int pageSize = 20);
        Lecturer? GetLecturerById(int id);
        IEnumerable<AvailabilitySlot> GetFutureAvailableSlots(int lecturerId);
    }
}