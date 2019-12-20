using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace OnlineEngagement.Models
{
    public class AdminModel
    {
        [Required]  
        [Display(Name ="Email")]   
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
    public class UserDetail
    {
        public List<SelectListItem> Cities { get; set; }
        public string GraphApproved { get; set; }

        [Display(Name = "Test :")]
        public List<SelectListItem> AllTest { get; set; }

        public List<SelectListItem> AllTestId { get; set; }

        public string ShadowCDFAvailableMsg { get; set; }
        public string CDFSesDate { get; set; }
        public string CDFAvailableMsg { get; set; }
        public string CDFLevels { get; set; }
        public string CDFAcceptance { get; set; }
        public string ProPrice1 { get; set; }
        public DateTime ShadowDate { get; set; }
        public string ShadowAssignDate { get; set; }
        public string ShadowCDF { get; set; }
        public int sessionAreaId { get; set; }
        public int PageNumber { get; set; }
        public int RegStudentCount { get; set; }
        public int StudID { get; set; }
        [Display(Name = "Name")]
        public string NameCDF { get; set; }
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Display(Name = "Venue")]
        public string SesstionVenue { get; set; }

        //[Display(Name="Pin Code")]
        //public string PinCode { get; set; }

        [Display(Name = "Area")]
        public string Area { get; set; }


        [Display(Name = "Session Time")]
        public string SesstionTime { get; set; }

        [Display(Name = "CDF Response")]
        public string CDFResponse { get; set; }

        //[Display(Name = "Email")]
        //public string CDFEmail { get; set; }

        //[Display(Name = "City")]
        //public string CDFCity { get; set; }

        [Display(Name = "User Status   ")]
        public string UserStatus { get; set; }
        [Display(Name = "Test Reassign")]
        public string TestReassign { get; set; }
        [Display(Name = "Test Approval")]
        public string TestApproval { get; set; }

        [Display(Name = "First Name")]
        public string Fname { get; set; }
        [Display(Name = "Last Name")]
        public string Lname { get; set; }
        public string ParentFb { get; set; }

        public int Id { get; set; }

        public int UserTypeId { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        [Display(Name = "Email ID")]
        [EmailAddress(ErrorMessage = "Plz Enter Email")]
        public string EmailId { get; set; }
        [Display(Name = "Contact No")]
        public string Contact { get; set; }

        public string Password { get; set; }

        public int CityId { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Pincode")]
        public string Pincode { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }

        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}")]
        [Display(Name = "RegDate")]
        public DateTime RegDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public string UserType { get; set; }

        [Display(Name = "Test Status")]
        public string TestStatus { get; set; }

        [Display(Name = "Test Complete Date")]

        public DateTime TestCompDate { get; set; }

        [Display(Name = "Assign CDF")]
        public string AssignCDF { get; set; }

        [Display(Name = "Id")]
        public int CDFId { get; set; }

        [Display(Name="Id")]
        public int ShadowId { get; set; }

        [Display(Name="Name")]
        public string ShadowName { get; set; }

        [Display(Name = "Name")]
        public string CDFName { get; set; }

        [Display(Name = "Contact")]
        public string CDFContact { get; set; }

        [Display(Name ="Contact")]
        public string ShadowContact { get; set; }

        [Display(Name = "Email")]
        public string CDFEmail { get; set; }

        [Display(Name ="Email")]
        public string ShadowEmail { get; set; }

        [Display(Name = "City")]
        public string CDFCity { get; set; }

        [Display(Name = "Gender")]
        public string CDFGender { get; set; }

        [Display(Name ="Gender")]
        public string ShadowGender { get; set; }

        [Display(Name = "Level")]
        public int CDFLevel { get; set; }

        [Display(Name ="Level")]
        public string ShadowLevel { get; set; }

        [Display(Name = "Session Id")]
        public int SessionId { get; set; }


        [Display(Name = "Session Date")]
        [DataType(DataType.Date)]
        public DateTime? SessionDate { get; set; }

        //[DataType(DataType.Time)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Display(Name = "Session Time")]
        public TimeSpan SessionTime { get; set; }

        [Display(Name = "Session Status")]
        public string SessionStatus { get; set; }

        [Display(Name = "CDF Responce")]
        public string SesCDFResponce { get; set; }

        [Display(Name = "Session Address")]
        public string SesAddress { get; set; }

        
        [Display(Name = "DOB")]
        public string BirthDate { get; set; }

        [Display(Name = "Address")]
        public string CDFAddress { get; set; }

        [Display(Name = "PinCode")]
        public string CDFPinCode { get; set; }

        [Display(Name = "Total Session")]
        public int CDFSessionCnt { get; set; }
        public SelectList sessionArea { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int ProductPrice { get; set; }

        [Display(Name = "Student Education")]
        public string education { get; set; }

    }

    public class FeedBackDetail
    {
        public int Qid { get; set; }
        public int Qno { get; set; }
        public string Questiontxt { get; set; }
        public string Answertxt { get; set; }

        public string ParentName { get; set; }
        public string ContactNo { get; set; }
        public string emailId { get; set; }
    }

    public class SPP_FBQuestionSet
    {
        public string ParentName { get; set; }
        public string ContactNo { get; set; }
        public string emailId { get; set; }

        public int qno1 { get; set; }
        public string questext1 { get; set; }
        public string anstext1 { get; set; }


        
        public int qno2 { get; set; }
        public string questext2 { get; set; }
        public string anstext2 { get; set; }

        
        public int qno3 { get; set; }
        public string questext3 { get; set; }
        public string anstext3 { get; set; }

        
        public int qno4 { get; set; }
        public string questext4 { get; set; }
        public string anstext4 { get; set; }

        
        public int qno5 { get; set; }
        public string questext5 { get; set; }
        public string anstext5 { get; set; }

       
        public int qno6 { get; set; }
        public string questext6 { get; set; }
        public string anstext6 { get; set; }
    }
    public class Authcode
    {
        //name	count	date	createdBy	comment
        //table Authcode

        public int id { get; set; }
        public string prodName { get; set; }
        public string authcode { get; set; }
        public DateTime date { get; set; }

        public string adminUser { get; set; }
        public string status { get; set; }

        public int promotorId { get; set; }
        public int validity { get; set; }

        //Table Promoter

        public string name { get; set; }
        public int count { get; set; }
        [Display(Name = "Created Id")]
        public int createdBy { get; set; }
        public string comment { get; set; }
        public string EmailId { get; set; }

    }



    public class CDFDetails
    {

        [Display(Name = "CDFId")]
        public int CDFId { get; set; }

        [Display(Name = "Name")]
        public string CDFName { get; set; }
        [Display(Name = "Contact")]
        public string CDFContact { get; set; }

        [Display(Name = "Email")]
        public string CDFEmail { get; set; }

        [Display(Name = "City")]
        public string CDFCity { get; set; }
    }

    public class SessionDetails
    {
        public int CDFPinCode { get; set; }
        public string ShadowName { get; set; }
        public string ShadowContact { get; set; }
        public string ShadowSessionDate { get; set; }
        public string CDFContact { get; set; }
        public string CDFName { get; set; }
        [Display(Name = "Session Time")]
        public string FinalTime { get; set; }
        public string SessionTimeIn { get; set; }
        public int LoginClientId { get; set; }
        public int CandId { get; set; }
        public string StudID { get; set; }
        public string CDFId { get; set; }
        [Display(Name = "Venue :")]
        public string sessionVenue { get; set; }
        [Display(Name = "Pin Code :")]
        public string sessionPinCode { get; set; }
        [Display(Name = "Area :")]
        public List<SelectListItem> sessionArea { get; set; }

        public List<SelectListItem> sessionAreaId { get; set; }

        [Display(Name = "Session Date")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime sessionDate { get; set; } 
       
        public string SesDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH:mm:ss}")]

        //  [DisplayFormat(DataFormatString = "{hh\\:mm tt}")]
        [DataType(DataType.Time)]
        public DateTime SesTime1 { get; set; }


        [Display(Name = "Session Time")]
        public TimeSpan SessionTime { get; set; }
        public int AreaId { get; set; }
        public int ShadowCDF { get; set; }

        [Display(Name = "State :")]
        public List<SelectListItem> sessionState { get; set; }

        public List<SelectListItem> sessionStateId { get; set; }

        [Display(Name = "City :")]
        public List<SelectListItem> sessionCity { get; set; }

        public List<SelectListItem> sessionCityId { get; set; }
    }

    public class ViewModelDashboard
    {
        public IEnumerable<UserDetail> UserDetail { get; set; }
        public SessionDetails SessionDetails { get; set; }
        public SPP_FBQuestionSet SPP_FBQuestionSet { get; set; }
        public FeedBackDetail FeedbackDetails { get; set; }
    }

    public class ViewModel
    {
        public SessionDetails SessionDetails { get; set; }
        public IEnumerable<UserDetail> userDtl { get; set; }
        public UserDetail UserDetail { get; set; }
        public StudentDetails studentdetails { get; set; }
        public IEnumerable<CDFDetails> CDFDetails { get; set; }
    }

    public class ViewModel1
    {
        public UserDetail UserDetail1 { get; set; }
        public UserDetail UserDetail2 { get; set; }
    }

    public class StudentTracking
    {
        public DateTime SesDate { get; set; }
        public TimeSpan SesTime { get; set; }
        public string SesStatus { get; set; }
        public string CDFName { get; set; }
        public string TestCompDate { get; set; }
        public string ProdPurchDate { get; set; }

    }
    public class TestReassign
    {
        [Display(Name = "To Date:")]
        public DateTime ToDate { get; set; }

        [Display(Name ="From Date:")]
        public DateTime FromDate { get; set; }
        [Display(Name = "Test Name:")]
        public String TestName { get; set; }
        public int CandId { get; set; }
        [Display(Name = "Candidate Name:")]
        public string CandName { get; set; }
    }
    public class authCode
    {
        public object userId { get; set; }

        [Display(Name = "Product Name")]
        public List<SelectListItem> ProductName { get; set; }

        public int ProductId { get; set; }

        //public int ProductId { get; set; }

        //[Display(Name ="Product Name")]
        //public string ProductName { get; set; }
        
        [Display(Name ="Created For")]
        public string CreatedFor { get; set; }
        
        [Display(Name ="No Codes")]
        public int NoOfCodes { get; set; }
        [Display(Name ="Validity In Days")]
        public string ValidityInDays { get; set; }
        [Display(Name ="Comment")]
        public string Comment { get; set; }            
    }

    public class CommnetAndRating
    {
        public int Id { get; set; }
        public int UserTypeId { get; set; }
        //  public int User_Id { get; set; }
        public string userTypeName { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string Name { get; set; }
        public string gender { get; set; }
        // public DateTime dob { get; set; }
        [Display(Name = "DOB")]
        public string BirthDate { get; set; }
        public string contactNo { get; set; }
        public string email { get; set; }
        // public DateTime CommentDate { get; set; }
        [Display(Name = "CommentDate")]
        public string CommentDate { get; set; }

        public string Comment { get; set; }
        public int Rating { get; set; }




    }
}
