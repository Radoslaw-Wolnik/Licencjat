using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.ValueObjects;

public class SocialMediaCollection
{
    private readonly List<SocialMediaLink> _links;
    public IReadOnlyCollection<SocialMediaLink> Links => _links.AsReadOnly();

    public SocialMediaCollection(IEnumerable<SocialMediaLink> socialMediaLinks)
    {
        _links = socialMediaLinks == null
            ? []
            : [.. socialMediaLinks.Distinct()];
    }

    public Result Add(SocialMediaLink link)
    {
        if (_links.Count >= 10)
            return Result.Fail(DomainErrorFactory.LimitReached("SocialMediaLink", "Linmit of 10 links reached"));
        
        if (_links.Contains(link))
            return Result.Fail("Already in social media - duplicate");
        if (_links.Any(sml => sml.Platform == link.Platform))
            return Result.Fail("Platform already used - duplicate");
        if(_links.Any(sml => sml.Url == link.Url))
            return Result.Fail("Link the same as othe already used link - duplicate");
        
        _links.Add(link);
        return Result.Ok();
    }

    public Result Remove(Guid linkId)
    {
        var existing = _links.SingleOrDefault(l => l.Id == linkId);
        if (existing == null)
            return Result.Fail(DomainErrorFactory.NotFound("SocialMediaLink", linkId));
        
        _links.Remove(existing);
        return Result.Ok();
    }

    public Result Update(SocialMediaLink updatedLink){
        var oldLink = _links.SingleOrDefault(sml => sml.Id == updatedLink.Id);
        if (oldLink == null)
            return Result.Fail("Not in the Social Media Links");

        // check duplicates among *the others* before you overwrite
        var others = _links.Where(sm => sm.Id != updatedLink.Id);

        if (others.Any(sml => sml.Platform == updatedLink.Platform))
            return Result.Fail(DomainErrorFactory.AlreadyExists("Platform", "Given platform already taken by different link"));

        if (others.Any(sml => sml.Url == updatedLink.Url))
            return Result.Fail(DomainErrorFactory.AlreadyExists("Url", "Url already taken by different link"));
        
        // replace
        _links.Remove(oldLink);
        _links.Add(updatedLink);
        return Result.Ok();
    }
}
