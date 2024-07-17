using System.ComponentModel.DataAnnotations.Schema;

namespace BirdiTMS.Models.ViewModels.FromClient
{
    public class ClBirdiTask
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }

        public DateTime DueDate { get; set; }
    }
}
