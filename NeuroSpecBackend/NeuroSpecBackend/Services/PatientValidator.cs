using FluentValidation;
using NeuroSpec.Shared.Models.DTO;

namespace NeuroSpecBackend.Services
{
    public class PatientValidator:AbstractValidator<Patient>
    {
        public PatientValidator()
        {
            RuleFor(patient => patient.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(5).WithMessage("Username must be at least 5 characters long");

            RuleFor(patient => patient.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long");

            RuleFor(patient => patient.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(patient => patient.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\d{11}$").WithMessage("Phone number must be of 11 digits");

            RuleFor(patient => patient.FirstName)
                .NotEmpty().WithMessage("First name is required");

            RuleFor(patient => patient.LastName)
                .NotEmpty().WithMessage("Last name is required");

            RuleFor(patient => patient.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past");

            RuleFor(patient => patient.Height)
                .GreaterThan(0).WithMessage("Height must be a positive number");

            RuleFor(patient => patient.Weight)
                .GreaterThan(0).WithMessage("Weight must be a positive number");
        }

    }
}
