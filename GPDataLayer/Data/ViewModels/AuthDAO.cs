using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GPDataLayer.Data.ViewModels
{
    public class AuthDAO
    {
		[Required(ErrorMessage = "Username is Required.")]
		public string username { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string password { get; set; }
	}
}