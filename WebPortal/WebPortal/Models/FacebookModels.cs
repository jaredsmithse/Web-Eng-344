using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebPortal.Models{

    public class FacebookFeedModel
    {
        [Required]
        [Display(Name = "Friend's name")]
        public string Name { get; set; }

        public string Message { get; set; }

        public int Likes { get; set; }
    }
}
