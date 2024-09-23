using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.ViewModels
{
    public class ItemDAO
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string @class { get; set; }
        [Required]
        public string lifeCycle_Phase { get; set; }
        [Required]
        public string uom { get; set; }
        [Required]
        public string part_category { get; set; }
        [Required]
        public string short_description { get; set; }

        public string campus { get; set; }

        public string customer_gp_id { get; set; }

        public string kit { get; set; }

        public string locn_code { get; set; }

        public string shipping_weight_in_lbs { get; set; }

        public string shipping_length_in_inches { get; set; }

        public string shipping_width_in_inches { get; set; }

        public string shipping_height_in_inches { get; set; }

        public string notes_comments { get; set; }

        public string calculate_mrp { get; set; }

        public string item_fullfilment_method { get; set; }

        public string price_shed { get; set; }

        public string make_buy_code { get; set; }

        public string replenishment { get; set; }

        public string phantom { get; set; }

        public string item_class_code { get; set; }

        public string buyer_Id { get; set; }

        public string planner_code { get; set; }

        public string planner_lead_code { get; set; }

        public string primary_vendor { get; set; }

        public string initial_cost { get; set; }

        public string mfg_fixed_lead_time { get; set; }

        public string warranty_days { get; set; }

        public string floor_stock { get; set; }

        public string default_route { get; set; }

        public string number_of_days_period_qty { get; set; }
        public string order_increment { get; set; }
        public string min_order_qty { get; set; }
        public string max_order_qty { get; set; }
        public string planning_lead_time_canton { get; set; }
        public string planning_lead_time_plano { get; set; }
        public string planning_lead_time_galway { get; set; }
        public string uplifted_support { get; set; }
        public string contract_type { get; set; }
        public string offset_date_attr { get; set; }
        public string wty_in_months { get; set; }
        public string customer_platform { get; set; }

        public string order_policy { get; set; }

        public string eccn { get; set; }

        public string schb { get; set; }

        public string liscence_exception_symbol { get; set; }

        public string ccats { get; set; }
        public string queue_time { get; set; }
        public string labor_time { get; set; }

        public string customer_group { get; set; }

        public string platform { get; set; }

        public string pafr { get; set; }

        public string regmodel { get; set; }

        public string eccn_eu { get; set; }
        public string schb_eu { get; set; }
        public string eu_ccats_ern { get; set; }
        public string eu_vendor_product_code { get; set; }
        public string warranty_name { get; set; }
        public string terms { get; set; }
        public string parts { get; set; }
        public string labor { get; set; }
        public string onsite { get; set; }
        public string coverage { get; set; }
        public string labor_vendor { get; set; }
        public string parts_vendor { get; set; }
        public string main_region { get; set; }

        public string rev_level { get; set; }

        public List<Bom> bom { get; set; }
    }
}
