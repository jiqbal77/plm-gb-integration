using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.ViewModels
{
    public class CODetail
    {
        [Required]
        public string ChangeId { get; set; }
        [Required]
        public string ChangeNotice { get; set; }
        [Required]
        public string ChangeType { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public List<ItemDAO> items { get; set; }
    }
}
