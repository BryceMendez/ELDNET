using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELDNET.Models
{
    public class LockerRequest
    {
        /* public int Id { get; set; }


         [Required]
         public string? StudentId { get; set; }

         [Required]
         public string? Name { get; set; }

         [Required]
         [Display(Name = "ID Number")]
         public string? IdNumber { get; set; }

         [Required]
         [Display(Name = "Locker Number")]
         public string? LockerNumber { get; set; }

         [Required]
         public string? Semester { get; set; }

         [Required]
         [Display(Name = "Contact Number")]
         public string? ContactNumber { get; set; }



         public string? Status { get; set; } = "Pending";

         [Required]
         public string? StudyLoadPath { get; set; }
         [Required]
         public string? RegistrationPath { get; set; }


         [NotMapped]
         [Required(ErrorMessage = "Please upload your Study Load.")]
         [Display(Name = "Study Load")]
         public IFormFile? StudyLoadFile { get; set; }

         [NotMapped]
         [Required(ErrorMessage = "Please upload your Registration Form.")]
         [Display(Name = "Registration Form")]
         public IFormFile? RegistrationFile { get; set; }

         [Required]
         [Display(Name = "Accept Terms and Conditions")]
         public bool AcceptTerms { get; set; }

         [Required]
         public bool IsChangedByApplicant { get; set; } = false;

         public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
         public string Approver1Status { get; set; } = "Pending";
         public string FinalStatus { get; set; } = "Pending";*/



        public int Id { get; set; }


   
        public string? StudentId { get; set; }

      
        public string? Name { get; set; }

        [Display(Name = "ID Number")]
        public string? IdNumber { get; set; }

      
        [Display(Name = "Locker Number")]
        public string? LockerNumber { get; set; }

    
        public string? Semester { get; set; }

       
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }



        public string? Status { get; set; } = "Pending";

      
        public string? StudyLoadPath { get; set; }
     
        public string? RegistrationPath { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        [NotMapped]
        
        [Display(Name = "Study Load")]
        public IFormFile? StudyLoadFile { get; set; }

        [NotMapped]
        
        [Display(Name = "Registration Form")]
        public IFormFile? RegistrationFile { get; set; }

        
        [Display(Name = "Accept Terms and Conditions")]
        public bool AcceptTerms { get; set; }

    
        public bool IsChangedByApplicant { get; set; } = false;

        public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";
        public string FinalStatus { get; set; } = "Pending";
    }
}