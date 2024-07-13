using System.ComponentModel.DataAnnotations.Schema;

namespace BirdiTMS.Models.ViewModels.FromServer
{
    public class SrBirdiTask
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime DueDate { get; set; }

        public string UserId { get; set; }

        public SrUserViewModel User { get; set; }
    }
}
