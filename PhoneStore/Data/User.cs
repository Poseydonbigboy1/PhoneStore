using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Data
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public String? Name { get; set; }
        [Required]
        public String Login { get; set; }
        [Required]
        public String Password { get; set; }
        public ERole Roles { get; set; }
    }

    public enum ERole
    {
        CUSTOMER,
        MANAGER
    }
}
