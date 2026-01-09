using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateCalendarEmployeeDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
       
        public Calendar Calendar { get; set; } = new(); 
    }
}
