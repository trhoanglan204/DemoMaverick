using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Models.ViewModels
{
    public class LoginVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập user name")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Username không được chứa khoảng trắng")]
        public string? Username { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập password")]
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
