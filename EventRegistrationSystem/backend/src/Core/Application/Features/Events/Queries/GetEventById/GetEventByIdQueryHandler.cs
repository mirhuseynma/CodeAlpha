
namespace EventRegistrationSystem.Application.Features.Events.Queries.GetEventById;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IAppDbContext _context;

    public GetEventByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var e = await _context.Events
            .Include(x => x.Organizer)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
        if (e == null)
        {
            throw new EventRegistrationSystem.Application.Exceptions.NotFoundException("Event not found.");
        }

        return new EventDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Location = e.Location,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Capacity = e.Capacity,
            OrganizerId = e.OrganizerId,
            OrganizerName = e.Organizer.FirstName + " " + e.Organizer.LastName,
            Status = e.Status,
            CreatedAt = e.CreatedAt
        };
    }
}
