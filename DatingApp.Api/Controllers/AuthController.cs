using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo=repo;
            _config=config;
            
        }
        [HttpPost("Register")]
        public  async Task<IActionResult> Rgister(UserForRegisterDto userForRegisterDto)
        {
           userForRegisterDto.username=userForRegisterDto.username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.username))
            return BadRequestMade("Username already exists");

            var userToCreate = new User{
                Username= userForRegisterDto.Username
            };

           var createdUser= await _repo.Register(userToCreate,userForRegisterDto.pssword);
           return StatusCode(201);
        }

        [HttpPost("Login")]
        public  async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(),userForLoginDto.Password);

            if(userFromRepo==null)
            return UnAuthorized();

            var claims =new[]
            {
              new claim(ClaimTypes,userFromRepo.Id.ToString()),
              new claim(ClaimTypes,userFromRepo.Username)
            };

            var key= new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(CLaims),
                Expires = DateTimeNow.AddDays(1),
                SigningCredentials = creds

            };
            var tokenHandler= new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token= tokenHandler.WriteToken(token)
            });


        }

    }
}