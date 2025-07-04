using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Models.Commands.Role;
using Models.Queries.Role;

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

    public async Task<Response<bool>> AddRole(CreateRoleCommand command)
    {
        var role = await _roleManager.FindByNameAsync(command.Name);
        
        if (role is not null) return Response.Fail<bool>("Role already exists");

        role = new Role
        {
            Name = command.Name,
            CreatedAt = DateTime.Now,
            UpdateAt = DateTime.Now
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Response.Fail<bool>(errors.First());
        }

        foreach (var claim in command.Claims)
        {
            var claimResult = await _roleManager.AddClaimAsync(role, new Claim(claim.Model, claim.Value));
            if (claimResult.Succeeded) continue;
            
            var errors = claimResult.Errors.Select(e => e.Description).ToList();
            return Response.Fail<bool>(errors.First());
        }

        return Response.Ok(true, "Role created successfully");
    }

    public async Task<Response<bool>> UpdateRole(Guid id, CreateRoleCommand command)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null) return Response.Fail<bool>("Role not found", System.Net.HttpStatusCode.NotFound);

        role.Name = command.Name;
        role.UpdateAt = DateTime.Now;
        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Response.Fail<bool>(errors.First());
        }

        foreach (var claim in command.Claims)
        {
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            if (existingClaims.Any(c => c.Type == claim.Model && c.Value == claim.Value)) continue;
            var claimResult = await _roleManager.AddClaimAsync(role, new Claim(claim.Model, claim.Value));
            if (claimResult.Succeeded) continue;
            
            var errors = claimResult.Errors.Select(e => e.Description).ToList();
            return Response.Fail<bool>(errors.First());
        }

        return Response.Ok(true, "Role updated successfully");
    }
}