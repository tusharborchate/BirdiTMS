﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BirdiTMS.Models.Entities
{
    public class BirdiTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }
        public  DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }


    }
}