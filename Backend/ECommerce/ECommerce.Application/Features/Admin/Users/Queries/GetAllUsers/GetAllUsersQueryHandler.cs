using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using ECommerce.Application.Features.Admin.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Admin.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PagedUsersResult>>
{
    private readonly IApplicationDbContext _context;

    public GetAllUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedUsersResult>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            query = query.Where(u => 
                (u.FirstName + " " + u.LastName).ToLower().Contains(search) || 
                (u.Email ?? "").ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                query = query.Where(u => u.UserStatus == UserStatus.Active);
            else
                query = query.Where(u => u.UserStatus != UserStatus.Active);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserListDto(
                u.Id,
                $"{u.FirstName} {u.LastName}",
                u.Email ?? "",
                u.UserStatus == UserStatus.Active,
                u.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var result = new PagedUsersResult(users, totalCount, request.PageNumber, totalPages);

        return Result<PagedUsersResult>.Success(result);
    }
}