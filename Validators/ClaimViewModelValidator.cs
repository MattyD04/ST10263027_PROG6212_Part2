﻿using FluentValidation;
using ST10263027_PROG6212_POE.Models;

public class ClaimViewModelValidator : AbstractValidator<ClaimViewModel>
{
    public ClaimViewModelValidator()
    {
        RuleFor(c => c.ClaimNum)
            .NotEmpty().WithMessage("Claim Number is required.");
        RuleFor(c => c.LecturerNum)
            .NotEmpty().WithMessage("Lecturer Number is required.");
        RuleFor(c => c.SubmissionDate)
            .NotEmpty().WithMessage("Submission Date is required.");
        RuleFor(c => c.HoursWorked)
            .GreaterThan(0).WithMessage("Hours Worked must be greater than 0.");
        RuleFor(c => c.HourlyRate)
            .GreaterThan(0).WithMessage("Hourly Rate must be greater than 0.");
        RuleFor(c => c.TotalAmount)
            .GreaterThan(0).WithMessage("Total Amount must be greater than 0.")
            .LessThanOrEqualTo(15000)
            .WithMessage("Total Amount is R15,000 or less, so approved automatically due to policy.")
            .When(c => c.TotalAmount <= 15000)
            .WithState(c => new { c.TotalAmount })
            .Must(totalAmount => totalAmount <= 15000)
            .WithMessage("Total Amount is greater than R15,000 and cannot be automatically approved.");
    }
}