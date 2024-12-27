using System.Security.Claims;
using Application.Commands.Role;
using Application.Interfaces;
using Application.Queries.Role;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class RoleAppService : BaseAppService<CreateRoleCommand, RoleQuery, Role, IRoleService>, IRoleAppService
{
    private readonly RoleManager<Role> _roleManager;
    public RoleAppService(
        IMapper mapper, 
        IRoleService service,
        RoleManager<Role> roleManager) : base(mapper, service)
    {
        _roleManager = roleManager;
    }
    
    public async Task AddRole(CreateRoleCommand command)
    {
        var role = await _roleManager.FindByNameAsync(command.Name);
        if(role is not null) throw new Exception("Role already exists");
        
        role = new Role
        {
            Name = command.Name,
            CreatedAt = DateTime.Now,
            UpdateAt = DateTime.Now
        };

        var result = await _roleManager.CreateAsync(role);
        if(!result.Succeeded) throw new Exception("Error creating role" + string.Join(", ", result.Errors.Select(e => e.Description)));
        
        foreach (var claim in command.Claims)
        {
            var claimResult = await _roleManager.AddClaimAsync(role, new Claim(claim.Model, claim.Value));
            if(!claimResult.Succeeded) throw new Exception("Error adding claim" + string.Join(", ", claimResult.Errors.Select(e => e.Description)));
        }
    }

    public async Task UpdateRole(Guid id, CreateRoleCommand command)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if(role is null) throw new Exception("Role not found");
        
        role.Name = command.Name;
        role.UpdateAt = DateTime.Now;
        var result = await _roleManager.UpdateAsync(role);
        if(!result.Succeeded) throw new Exception("Error updating role" + string.Join(", ", result.Errors.Select(e => e.Description)));

        foreach (var claim in command.Claims)
        {
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            if (existingClaims.Any(c => c.Type == claim.Model && c.Value == claim.Value)) continue;
            var claimResult = await _roleManager.AddClaimAsync(role, new Claim(claim.Model, claim.Value));
            if(!claimResult.Succeeded) throw new Exception("Error adding claim" + string.Join(", ", claimResult.Errors.Select(e => e.Description)));
        }
    }
}