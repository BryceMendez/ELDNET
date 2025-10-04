using System;
using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class ReservationRoom
    {
        /*[Required]
        public int Id { get; set; }
        [Required]
        // 🔹 Link to Student
        public string? StudentId { get; set; } // Foreign key to StudentAccount.StudentId
        [Required]
        [Display(Name = "Organization Name")]
        public string? OrganizationName { get; set; }
        [Required]
        [Display(Name = "Activity Title")]
        public string? ActivityTitle { get; set; }
        [Required]
        [Display(Name = "Activity Date")]
        public DateTime ActivityDate { get; set; }
        [Required]
        public string? Speaker { get; set; }
        [Required]
        public string? Venue { get; set; }
        [Required]
        [Display(Name = "Purpose/Objective")]
        public string? PurposeObjective { get; set; }
        [Required]
        [Display(Name = "Date Needed")]
        public DateTime? DateNeeded { get; set; }

        [Required]
        [Display(Name = "Time From")]
        public TimeSpan? TimeFrom { get; set; }

        [Required]
        [Display(Name = "Time To")]
        public TimeSpan? TimeTo { get; set; }

        [Required]
        public string? Participants { get; set; }

        [Required]
        [Display(Name = "Equipment/Facilities Needed")]
        public string? EquipmentFacilities { get; set; }

        [Required]
        [Display(Name = "Nature of Activity")]
        public string? NatureOfActivity { get; set; }

        [Required]
        [Display(Name = "Source of Funds")]
        public string? SourceOfFunds { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }

        [Required]
        [Display(Name = "Reserved By")]
        public string? ReservedBy { get; set; }

        
        // 🔹 Overall status
        public string? Status { get; set; } = "Pending";

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsChangedByApplicant { get; set; } = false;

        // 🔹 Approval Workflow Fields (5 approvers)
        public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";

        public string Approver2Name { get; set; } = "Ms. Nitcy Milagros (Department Head/ Dean)";
        public string Approver2Status { get; set; } = "Pending";

        public string Approver3Name { get; set; } = "Otelia G. Moho (Campus Director)";
        public string Approver3Status { get; set; } = "Pending";

        public string Approver4Name { get; set; } = "Engr. Jessir Flores (Building Officer)";
        public string Approver4Status { get; set; } = "Pending";

        public string Approver5Name { get; set; } = "Eduardo Hona (DSU Head Guard)";
        public string Approver5Status { get; set; } = "Pending";

        public string FinalStatus { get; set; } = "Pending";*/
    
    
 


        
            
            public int Id { get; set; }
            
            // 🔹 Link to Student
            public string? StudentId { get; set; } // Foreign key to StudentAccount.StudentId
        
            [Display(Name = "Organization Name")]
            public string? OrganizationName { get; set; }
  
            [Display(Name = "Activity Title")]
            public string? ActivityTitle { get; set; }
          
            [Display(Name = "Activity Date")]
            public DateTime ActivityDate { get; set; }
        
            public string? Speaker { get; set; }
          
            public string? Venue { get; set; }
        
            [Display(Name = "Purpose/Objective")]
            public string? PurposeObjective { get; set; }
      
            [Display(Name = "Date Needed")]
            public DateTime? DateNeeded { get; set; }

          
            [Display(Name = "Time From")]
            public TimeSpan? TimeFrom { get; set; }

           
            [Display(Name = "Time To")]
            public TimeSpan? TimeTo { get; set; }

            
            public string? Participants { get; set; }

           
            [Display(Name = "Equipment/Facilities Needed")]
            public string? EquipmentFacilities { get; set; }

         
            [Display(Name = "Nature of Activity")]
            public string? NatureOfActivity { get; set; }

          
            [Display(Name = "Source of Funds")]
            public string? SourceOfFunds { get; set; }

       
            [Display(Name = "Room Number")]
            public string? RoomNumber { get; set; }

         
            [Display(Name = "Reserved By")]
            public string? ReservedBy { get; set; }


            // 🔹 Overall status
            public string? Status { get; set; } = "Pending";

           
            public DateTime Date { get; set; }

            
            public bool IsChangedByApplicant { get; set; } = false;

            // 🔹 Approval Workflow Fields (5 approvers)
            public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
            public string Approver1Status { get; set; } = "Pending";

            public string Approver2Name { get; set; } = "Ms. Nitcy Milagros (Department Head/ Dean)";
            public string Approver2Status { get; set; } = "Pending";

            public string Approver3Name { get; set; } = "Otelia G. Moho (Campus Director)";
            public string Approver3Status { get; set; } = "Pending";

            public string Approver4Name { get; set; } = "Engr. Jessir Flores (Building Officer)";
            public string Approver4Status { get; set; } = "Pending";

            public string Approver5Name { get; set; } = "Eduardo Hona (DSU Head Guard)";
            public string Approver5Status { get; set; } = "Pending";

            public string FinalStatus { get; set; } = "Pending";
        }
    }




