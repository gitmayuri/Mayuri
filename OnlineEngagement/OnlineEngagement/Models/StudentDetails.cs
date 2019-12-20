using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using System.Configuration;


namespace OnlineEngagement.Models
{
   
    public class StudentDetails
    {
        [Display(Name = "Lead Type")]
        public string LeadType { get; set; }
        public int Id { get; set; }
        [Display(Name ="F Name")]
        public string fname { get;set;}
        [Display(Name ="L Name")]
        public string lname { get; set; }
        public string Email { get; set; }
        [Display(Name ="Mobile")]
        public string contactNo { get; set; }
        [Display(Name = "Gender:")]
        public string Gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "D.O.B:")]
        public DateTime dob { get; set; }
        public string birth { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime regDateTime { get; set; }       
        public string finaldate { get; set; }
        [Display(Name = "Pincode:")]
        public string pincode { get; set; }
        [Display(Name = "Address:")]
        public string address { get; set; }
        [Display(Name = "UserStatus:")]
        public string userStatus { get; set; }
        [Display(Name = "City")]
        public string city { get; set; }
        public string productStatus { get; set; }
        public string TestStatus { get; set; }
        public string SessionStatus { get; set; }
        public string CDFAssignedStatus { get; set; }
        public string ShadowCDFAssignedStatus { get; set; }
        public string  CDFAcceptanceStatus { get; set; }





    }




}