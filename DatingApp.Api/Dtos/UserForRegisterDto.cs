namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {

        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="You must specify between 8 and 4")]

        public string Password{ get; set;}
    }
}