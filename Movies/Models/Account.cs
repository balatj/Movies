using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        [Index(IsUnique = true)]
        [MaxLength(450)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }
    }
}