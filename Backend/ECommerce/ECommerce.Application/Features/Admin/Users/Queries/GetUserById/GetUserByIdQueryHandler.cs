using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using ECommerce.Application.Features.Admin.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Admin.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userDetail = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(u => new UserDetailDto(
                u.Id,
                $"{u.FirstName} {u.LastName}",
                u.Email,
                u.PhoneNumber,
                u.UserStatus == UserStatus.Active,
                u.CreatedAt,
                u.Orders.Count,
                u.Orders.Where(o => o.OrderStatus != OrderStatus.Cancelled).Sum(o => o.TotalAmount)
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (userDetail == null)
        {
            return Result<UserDetailDto>.Failure("User not found.");
        }

        return Result<UserDetailDto>.Success(userDetail);
    }
}