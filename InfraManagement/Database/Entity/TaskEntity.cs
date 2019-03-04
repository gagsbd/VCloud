using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfraManagement.Database.Entity
{
    [Table("Task")]
    public class TaskEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       
        public int OrgId { get;  set; }
        public string TaskCode { get; set; }
        public string Name { get; set; }
        public string StatusUrl { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int TaskType { get; set; }
        public bool IsLRP { get; set; }

        public string Predecessor { get; set; }

    }
}