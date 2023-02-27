using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models.Domain
{
    [Keyless]
    public class UserEmailOptions
    {
        public int Id { get; set; }
        public List<string> ToEmails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        [NotMapped]
        public List<KeyValuePair<string, string>> PlaceHolders { get; set; }
    }
}
