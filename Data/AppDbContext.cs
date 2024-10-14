﻿using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Models;
using System.Security.Claims;

namespace ST10263027_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public AppDbContext() { } //parameterless constructor for the AppDbContext class
        public DbSet<Lecturer> Lecturers { get; set; } // This represents the database table for Lecturer entities which links to the Lecturer
        public DbSet<ProgrammeCoordinator> ProgrammeCoordinators { get; set; } // This represents the database table for Programme Coordinators entities which links to ProgrammeCoordinators model
        public DbSet<AcademicManager> AcademicManagers { get; set; } // This represents the database table for Academic Managers entities which links to AcademicManager model
        public DbSet<ST10263027_PROG6212_POE.Models.Claim> Claims { get; set; } // This represents the database table for Claims entities which links to Claims model

    }
}
//-----------------------------------End of file--------------------------------------------//
