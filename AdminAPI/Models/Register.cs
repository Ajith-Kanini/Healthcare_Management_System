using System.ComponentModel.DataAnnotations;

namespace AdminAPI.Models
{
    public class Register
    {
        [Key]
        public int UserId { get; set; }
        public string UserName  { get; set; }=string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string UseRole { get; set; } = string.Empty;
    }
}
