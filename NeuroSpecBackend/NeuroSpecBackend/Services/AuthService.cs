using System;
using System.Security.Claims;
using System.Text;
using MongoDB.Driver;
using NeuroSpec.Shared.Models.DTO;
using System.Threading.Tasks;
using NeuroSpecBackend.Model;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace NeuroSpecBackend.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<Patient> _patients;
        private readonly string _jwtSecret;
        private readonly int _jwtLifespan;

        public AuthService(NeuroDbContext dbContext, IConfiguration configuration)
        {
            _patients = dbContext.Patients;
            _jwtSecret = configuration["Jwt:Secret"];
            _jwtLifespan = int.Parse(configuration["Jwt:Lifespan"]);
        }

        public async Task<string?> SignUp(Patient newPatient)
        {
            var validator = new PatientValidator();
            var validationResult = validator.Validate(newPatient);
            if (!validationResult.IsValid)
            {
                return string.Join("; ", validationResult.Errors);
            }

            var existingPatient = await _patients.Find(p => p.Username == newPatient.Username || p.Email == newPatient.Email).FirstOrDefaultAsync();
            if (existingPatient != null)
            {
                return "Username or Email already exists";
            }

            newPatient.Password = BCrypt.Net.BCrypt.HashPassword(newPatient.Password);
            await _patients.InsertOneAsync(newPatient);
            return null;
        }
        //only use when necessary
        //do no use!!!!!!!!!!!!!
        public async Task RehashPasswords()
        {
            var patients = await _patients.Find(_ => true).ToListAsync();
            foreach (var patient in patients)
            {
                patient.Password = BCrypt.Net.BCrypt.HashPassword(patient.Password);
                await _patients.ReplaceOneAsync(p => p.Id == patient.Id, patient);  
            }
        }


        public async Task<string?> SignIn(string username, string password)
        {
            var patient = await _patients.Find(p => p.Username == username).FirstOrDefaultAsync();
            if (patient == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, patient.Password);
            if (isPasswordValid)
            {
                return null;
            }

            return GenerateJwtToken(patient);
        }

        private string GenerateJwtToken(Patient patient)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, patient.PatientID.ToString()),
                new Claim(ClaimTypes.Name, patient.Username),
                new Claim(ClaimTypes.Email, patient.Email ?? string.Empty)
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtLifespan),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
