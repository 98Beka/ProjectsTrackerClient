﻿using ProjectsTrackerClient.Interfaces;

namespace ProjectsTracker.Models {
    public class Employee : IModel {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? MidleName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public List<Project>? Projects { get; set; } = new();
        public List<Project>? ProjectsAsLead { get; set; } = new();
    }
}
