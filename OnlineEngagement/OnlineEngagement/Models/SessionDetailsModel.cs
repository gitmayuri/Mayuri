using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineEngagement.Models
{
    public class ViewDetails
    {
        public SessionDetailsModel SessionDetailsModel { get; set; }
        public SPP_FBQuestionSetDtl SPP_FBQuestionSetDtl { get; set; }
        public FeedBackDetailDtl FeedBackDetailDtl { get; set; }
        public CommentsAndRatingDtl CommentsAndRating { get; set; }
        public Parent_FBQuestionSetDtl Parent_FBQuestionSetDtl { get; set; }
        public ParentFeedBackDetail ParentFeedBackDetail { get; set; }
    }
    public class SessionDetailsModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string Fname { get; set; }

        [Display(Name = "Last Name")]
        public string Lname { get; set; }
        public string FullName { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Pincode")]
        public string Pincode { get; set; }

        public int CDFId { get; set; }
        public string CDFName { get; set; }
        public string CDFGender { get; set; }
        public string CDFContact { get; set; }
        public string CDFEmail { get; set; }
        public string CDFLevels { get; set; }


        public int ShadowId { get; set; }
        public string ShadowName { get; set; }
        public string ShadowGender { get; set; }
        public string ShadowContact { get; set; }
        public string ShadowEmail { get; set; }
        public string ShadowLevel { get; set; }



        public int SessionId { get; set; }

        [Display(Name = "Session Date")]
        [DataType(DataType.Date)]
        public DateTime? SessionDate { get; set; }

        [Display(Name = "Session Time")]
        public string SesstionTime { get; set; }

        public string SessionStatus { get; set; }
        public string SesCDFResponce { get; set; }
        public string SesAddress { get; set; }

        public string message { get; set; }
    }
    public class SPP_FBQuestionSetDtl
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

        public string message { get; set;}
    }
    public class FeedBackDetailDtl
    {
        public int Qid { get; set; }
        public int Qno { get; set; }
        public string Questiontxt { get; set; }
        public string Answertxt { get; set; }

        public string ParentName { get; set; }
        public string ContactNo { get; set; }
        public string emailId { get; set; }

        public string message { get; set; }
    }
    public class CommentsAndRatingDtl
    {
        public int UId { get; set; }
        public int UserTypeId { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public string message { get; set; }
    }
    public class Parent_FBQuestionSetDtl
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

        public string message { get; set; }
    }
    public class ParentFeedBackDetail
    {
        public int Qid { get; set; }
        public int Qno { get; set; }
        public string Questiontxt { get; set; }
        public string Answertxt { get; set; }

        public string ParentName { get; set; }
        public string ContactNo { get; set; }
        public string emailId { get; set; }

        public string message { get; set; }
    }
}